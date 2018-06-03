using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CTSTestApplication;
using Datafeeds.Processor;
using NDesk.Options;

namespace Datafeeds
{
    public class Program
    {
        OptionSet o;

        // Console params
        public String FeedFilePath { get; set; }
        public String TargetLocation { get; set; }
        public int? TestFileCount { get; set; }
        public bool Help { get; set; } = false;
        public int? BatchSize { get; set; }

        public IProcessor TradeProcessor { get; set; }
        public Tester Tester { get; set; }

        public Program(params string[] args)
        {
            LoggerSetting.init();

            o = new OptionSet()
            {
                 { "f|filePath=",   "the input {FILE} path of Trade datafeed. Defaul is current dir + '/TradesList.xml'.", v => FeedFilePath = v },
                 { "t|targetPath=", "the {DESTINATION} path. Default is current dir.", v => TargetLocation = v },
                 { "g|testFile=",   "generate TradesList.xml of {COUNT} elements in current dir.", v => TestFileCount = Int32.Parse(v) },
                 { "b|batchSize=",  "Batch {SIZE}. Default is 1000.", v => BatchSize = Int32.Parse(v) },
                 { "h|help",        "Show this help.",  v => Help = v != null}
            };

            List<string> extra;
            try
            {
                extra = o.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Write("datafeeds: ");
                Console.WriteLine(e.Message);
                Console.WriteLine("Try `--help' for more information.");
                return;
            }

            if (Help)
            {
                printHelp();
                return;
            }
            if (FeedFilePath == null)
            {
                FeedFilePath = AppDomain.CurrentDomain.BaseDirectory + "/TradesList.xml";
            }
            if (TargetLocation == null)
            {
                TargetLocation = AppDomain.CurrentDomain.BaseDirectory;
            }
            if (BatchSize == null)
            {
                BatchSize = 1000;
            }

            TradeProcessor = new TradeProcessor(this.FeedFilePath, this.TargetLocation, BatchSize.Value);
            Tester = new CTSTestApplication.Tester();
        }

        public void run()
        {
            if(TestFileCount.HasValue)
            {
                Tester.CreateTestFile(AppDomain.CurrentDomain.BaseDirectory, TestFileCount.Value);
            }
            if (TradeProcessor != null)
            {
                TradeProcessor.Process();
            }
        }

        private void printHelp()
        {
            Console.WriteLine("help");
            o.WriteOptionDescriptions(Console.Out);
        }
        
        public static void Main(string[] args)
        {
            Program program = new Program(args);
            program.run();
        }
    }
}
