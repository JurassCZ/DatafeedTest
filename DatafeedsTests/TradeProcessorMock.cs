using Datafeeds.Processor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatafeedsTests
{
    public class TradeProcessorMock : IProcessor
    {
        public int ProcessRunCount { get; set; }
        public void Process()
        {
            ProcessRunCount++;
        }
    }
}
