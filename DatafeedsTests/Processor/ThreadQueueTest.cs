using Microsoft.VisualStudio.TestTools.UnitTesting;
using Datafeeds.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Datafeeds.Processor.Tests
{
    [TestClass()]
    public class ThreadQueueTests
    {

        [TestMethod()]
        public void ConsumeTest()
        {
            int num = 100000;
            ThreadQueue<int> threadQueue = new ThreadQueue<int>(100, streamOfIntegers(num) );

            List<int> output = new List<int>();

            foreach(var i in threadQueue.Consume() )
            {
                output.Add(i);
            }

            foreach(var i in streamOfIntegers(num))
            {
              Assert.AreEqual(output[i], i);
            }
            
        }

        IEnumerable<int> streamOfIntegers(int maxInt)
        {
            for (int i = 0; i < maxInt; i++)
                yield return i;
        }
    }
}