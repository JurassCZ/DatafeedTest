using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Datafeeds;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using Datafeeds.Processor;
using Datafeeds.Properties;

namespace Datafeeds
{
    class Program
    {
        string currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public void generateXml()
        {
            CTSTestApplication.Tester tester = new CTSTestApplication.Tester();
            tester.CreateTestFile(currentDirectory, 10);
        }
        
        public void process()
        {
            TradeProcessor tradeProcessor = new TradeProcessor(currentDirectory+"/TradesList.xml", currentDirectory);
            tradeProcessor.Process();
        }

        static void Main(string[] args)
        {
            LoggerSetting.init();
            Program prog = new Program();
    //          prog.generateXml();
            prog.process();

        }
    }
}
