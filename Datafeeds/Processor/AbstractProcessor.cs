using CTSTestApplication;
using Datafeeds.Mapper;
using Datafeeds.Util;
using System;
using System.Diagnostics;
using System.IO;

namespace Datafeeds.Processor
{
    /// <summary>
    /// Lazy deserialize some XML on the disc into the Database.
    /// </summary>
    /// <typeparam name="ORModel">ORM model class</typeparam>
    /// <typeparam name="XmlModel">XML model class of the output objects</typeparam>
    /// <typeparam name="XmlModelContainer">XML model class which acts as container for deserialized objects</typeparam>
    /// <typeparam name="MappingClass">Class mapping XML model into ORM model</typeparam>
    public abstract class AbstractProcessor<ORModel, XmlModel, XmlModelContainer, MappingClass> : IProcessor 
        where MappingClass : Mapper<ORModel, XmlModel>
    {
        private const int DEFAULT_BATCH_SIZE = 100000;

        protected string filePath;
        protected string dbConnection;
        protected int batchSize;

        protected string TRANSACTION_NAME = "AbstractProcessor";

        //Filters
        protected XmlPartitioner xmlPartitioner;
        protected XmlDeserializator<XmlModelContainer> xmlDeserializer = new XmlDeserializator<XmlModelContainer>();
        protected MappingClass mapper = (MappingClass)Activator.CreateInstance(typeof(MappingClass));
        protected DataAdapter dataAdapter;
        protected XMLSaver xMLSaver;

        public AbstractProcessor(string filePath, string dbConnection) : this(filePath, dbConnection, DEFAULT_BATCH_SIZE)
        {

        }
  
        public AbstractProcessor(string filePath, string dbConnection, int batchSize)
        {
            this.filePath = filePath;
            this.dbConnection = dbConnection;
            this.batchSize = batchSize;

            this.xmlPartitioner = new XmlPartitioner(filePath, batchSize);
            this.dataAdapter = new DataAdapter(dbConnection);
            this.xMLSaver = new XMLSaver(AppDomain.CurrentDomain.BaseDirectory, TRANSACTION_NAME + ".xml");
        }

        /// <summary>
        /// Transofrm xml file into database
        /// </summary>
        public void Process()
        {
            try
            {
                // Split potencionally large XML file into smaller XMLs
                foreach (var xmlPart in xmlPartitioner.GetPartializedXML())
                {
                   Trace.TraceInformation("Input XML splited:" + xMLSaver.save(xmlPart.ToString()) );
                }


                ORModel[] oRModels = new ORModel[batchSize];

                // For each created XML file
                int count = 0;
                foreach (var xmlFilePath in xMLSaver.FilePaths)
                {
                    // 1. Deserialize XML file into real objects
                    XmlModelContainer xmlModelsContainer = xmlDeserializer.deserializeFile(xmlFilePath);

                    // 2. Extract objects jailed in root container
                    XmlModel[] xmlModels = extractList(xmlModelsContainer);

                    // 3. Map objects into ORM objects
                    if (xmlModels.Length != oRModels.Length) // resize if neccessary
                        Array.Resize(ref oRModels, xmlModels.Length);
                    for (int i = 0; i<xmlModels.Length; i++)
                    {
                        oRModels[i] = mapper.map(xmlModels[i]);
                    }

                    // 4. Write ORM objects to database
                    insertToDatabase(oRModels);

                    // 5. Delete processed XML 
                    File.Delete(xmlFilePath);

                    Trace.TraceInformation("Processed batch num.:" + count++ + "\t Batch size:" + oRModels.LongLength);
                }

            }
            catch (Exception e)
            {
                Trace.WriteLine(e);
            }
        }

        protected abstract XmlModel[] extractList(XmlModelContainer container);

        private void insertToDatabase(ORModel[] oRModels)
        {
            const int MAX_ATTEMPTS = 10;
            for(int attempt = 1; attempt <= MAX_ATTEMPTS; attempt++)
            {
                try
                {
                    Trace.TraceInformation("Inserting batch into DB. Attemt num.:" + attempt);
                    dataAdapter.BeginTransaction(TRANSACTION_NAME);

                    dataAdapter.Process(Operation.Insert, "someCommand", oRModels);
                    
                    dataAdapter.CommitTransaction(TRANSACTION_NAME);

                    return;
                }
                catch (Exception e)
                {
                    if (attempt < MAX_ATTEMPTS)
                    {
                        Trace.TraceInformation("Inserting failed.");
                        Trace.TraceError(e.ToString());
                        dataAdapter.RollbackTransaction(TRANSACTION_NAME);
                    }
                    else
                    {
                        throw new Exception("Insert to DB not succesful. Total attempts: " + attempt);
                    }
                }
            }
        }
            
    }
}
