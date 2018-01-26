using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Datafeeds
{
    public class XmlDeserializator<T>
    {
        XmlSerializer serializer;
        StreamReader reader;

        public XmlDeserializator()
        {
            serializer = new XmlSerializer(typeof(T));
        }

        public T deserialize(string xml)
        {
            TextReader reader = new StringReader(xml);
            T result = (T)serializer.Deserialize(reader);
            reader.Close();
            return result;
        }
    }
}
