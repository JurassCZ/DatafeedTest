using CTSTestApplication;
using Datafeeds.Mapper;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafeeds.Processor
{
    public abstract class AbstractProcessor<ORModel, XmlModel, XmlModelContainer, MappingClass> where MappingClass : Mapper<ORModel, XmlModel>
    {
        public int BATCH_SIZE = 100;

        protected string directoryPath;
        protected string dbConnection;

        protected string TRANSACTION_NAME = "AbstractProcessor";

        //Filters
        protected XmlPartitioner xmlPartitioner;
        protected XmlDeserializator<XmlModelContainer> xmlDeserializer = new XmlDeserializator<XmlModelContainer>();
        protected MappingClass mapper = (MappingClass)Activator.CreateInstance(typeof(MappingClass));
        protected DataAdapter dataAdapter;



        public AbstractProcessor(string directoryPath, string dbConnection)
        {
            this.directoryPath = directoryPath;
            this.dbConnection = dbConnection;
       
            this.xmlPartitioner = new XmlPartitioner(directoryPath, BATCH_SIZE);
            this.dataAdapter = new DataAdapter(dbConnection);
        }
        public AbstractProcessor(string directoryPath, string dbConnection, int batchSize) : this(directoryPath, dbConnection)
        {
            this.BATCH_SIZE = batchSize;
        }

        protected abstract XmlModel[] extractList(XmlModelContainer container);
        /// <summary>
        /// Transofrm xml file in given location into database
        /// </summary>
        public void Process()
        {
            ORModel[] oRModels = new ORModel[BATCH_SIZE];
            try
            {
                dataAdapter.BeginTransaction(TRANSACTION_NAME);

                // Obtain XML piece by piece
                foreach (var xmlPart in xmlPartitioner.GetPartializedXML())
                {
                    // Deserialize XML part into real objects, classes are pre-generated from XSD file
                    XmlModelContainer xmlModelsContainer = xmlDeserializer.deserialize(xmlPart.ToString());

                    // Extract objects jailed in root container
                    XmlModel[] xmlModels = extractList(xmlModelsContainer);

                    // Map objects into ORM objects
                    if (xmlModels.Length < oRModels.Length) // make buffer smaller (in case of the last xml part)
                        Array.Resize(ref oRModels, xmlModels.Length);
                    for (int i = 0; i<xmlModels.Length; i++)
                    {
                        oRModels[i] = mapper.map(xmlModels[i]);
                    }
              
                    // Write ORM objects to database
                    dataAdapter.Process(Operation.Insert, "someCommand", oRModels);
                }

                dataAdapter.CommitTransaction(TRANSACTION_NAME);
            }
            catch (Exception e)
            {
                dataAdapter.RollbackTransaction(TRANSACTION_NAME);
                Trace.WriteLine(e);
            }
        }
    }
}
