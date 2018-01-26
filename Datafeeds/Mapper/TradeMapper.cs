using CTSTestApplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafeeds.Mapper
{
    public class TradeMapper : Mapper<Trade, TradesListTrade>
    {
        public override Trade map(TradesListTrade xmlTrade)
        {
            Trade trade = new Trade();
            trade.Direction = (Direction)xmlTrade.Direction;
            trade.Isin = xmlTrade.ISIN;
            trade.Quantity = xmlTrade.Quantity;
            trade.Price = xmlTrade.Price;

            return trade;
        }
    }
}
