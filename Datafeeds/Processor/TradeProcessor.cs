using CTSTestApplication;
using Datafeeds.Mapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafeeds.Processor
{
    public class TradeProcessor : AbstractProcessor<Trade,TradesListTrade,TradesList,TradeMapper>
    {
        protected string TRANSACTION_NAME = "TradeProcessor";


        public TradeProcessor(string filePath, string dbConnection, int batchSize) : base(filePath, dbConnection, batchSize)
        {

        }

        protected override TradesListTrade[] extractList(TradesList container)
        {
            return container.Trade;
        }
    }
}
