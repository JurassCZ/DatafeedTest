using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Datafeeds.Mapper
{
    public abstract class Mapper<ORModel, XmlModel>
    {
        public abstract ORModel map(XmlModel model);
    }
}
