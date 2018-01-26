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
        public TradeProcessor(string directoryPath, string dbConnection) : base(directoryPath, dbConnection)
        {
            base.TRANSACTION_NAME = "TradeProcessor";
        }

        protected override TradesListTrade[] extractList(TradesList container)
        {
            return container.Trade;
        }
    }
}
