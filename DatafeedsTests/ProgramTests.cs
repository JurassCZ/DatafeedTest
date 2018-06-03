using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datafeeds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Datafeeds.Processor;
using DatafeedsTests;
using CTSTestApplication;
using System.Threading;

namespace Datafeeds.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        TradeProcessorMock tradeProcessorMock = new TradeProcessorMock();

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }

        [TestMethod()]
        public void ProgramTest()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            string s = "-filePath=" + path;
            string s2 = "-targetPath=" + path;
            string s3 = "-testFile=150";
            string s4 = "-batchSize=999";

            Program p = new Program(s, s2, s3, s4);

            Assert.AreEqual(p.FeedFilePath, path);
            Assert.AreEqual(p.TargetLocation, path);
            Assert.AreEqual(p.TestFileCount, 150);
            Assert.AreEqual(p.BatchSize, 999);

            p.TradeProcessor = tradeProcessorMock;
            p.TestFileCount = null; // Can't mock Tester

            p.run();
            Assert.AreEqual(tradeProcessorMock.ProcessRunCount, 1);

            Program p2 = new Program();
            p2.run();
        }
    }
}