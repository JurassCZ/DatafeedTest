using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datafeeds.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafeeds.Util.Tests
{
    [TestClass()]
    public class XMLSaverTests
    {


        [TestMethod()]
        public void saveTest()
        {
            XMLSaver s = new XMLSaver(AppDomain.CurrentDomain.BaseDirectory, "testFile");

            string path = s.save("Hello world!");


        }
    }
}