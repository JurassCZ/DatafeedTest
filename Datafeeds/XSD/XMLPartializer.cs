using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Datafeeds
{
    /// <summary>
    /// Lazy splits some very large XML into smaller (valid) XMLs. Split of original XML is perfromed on the Root Node level. Smaller XML are therefore suitable for XMLSerialize.deserialize(xml)
    /// </summary>
    public class XmlPartitioner
    {
        string path;
        int batchSize;

        enum BufferState { FirstBuffer, LastBuffer, MiddleBuffer }

        StringBuilder startingXmlPart;
        StringBuilder endingXmlPart;

        public XmlPartitioner(string path, int batchSize)
        {
            this.path = path;
            this.batchSize = batchSize;
        }

        public IEnumerable<StringBuilder> GetPartializedXML()
        {
            var settings = new XmlWriterSettings() { Indent = true, IndentChars = "    " };
            StringBuilder buffer = new StringBuilder();

            using (var reader = XmlReader.Create(new StreamReader(this.path)))
            using (var writer = XmlWriter.Create(buffer, settings))
            {
                int prevDepth = 0;
                int mainElementCounter = 0;

                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            // if this is root element, then save it into ending part of XML
                            if (reader.Depth == 0 && this.endingXmlPart == null)
                            {
                                endingXmlPart = createEndingXmlPart(reader.Name);
                            }

                            writer.WriteStartElement(reader.Prefix, reader.Name, reader.NamespaceURI);
                            writer.WriteAttributes(reader, true);
                            if (reader.IsEmptyElement)
                            {
                                writer.WriteEndElement();
                            }
                            break;

                        case XmlNodeType.Text:
                            writer.WriteString(reader.Value);
                            break;

                        case XmlNodeType.EndElement:
                            if (reader.Depth != 0)
                                writer.WriteFullEndElement();
                            break;

                        case XmlNodeType.XmlDeclaration:
                            writer.WriteProcessingInstruction(reader.Name, reader.Value);
                            break;
                        case XmlNodeType.ProcessingInstruction:
                            writer.WriteProcessingInstruction(reader.Name, reader.Value);
                            break;

                        case XmlNodeType.SignificantWhitespace:
                            writer.WriteWhitespace(reader.Value);
                            break;
                    }

                    // If first jump into inner node from root node happen, then save buffer as starting part of XML
                    if (reader.Depth == 1 && startingXmlPart == null)
                    {
                        writer.Flush();
                        startingXmlPart = new StringBuilder(buffer.ToString());
                        buffer.Clear();
                    }

                    // Each time jump to outer Depth=1 node happen
                    if (prevDepth == 2 && reader.Depth == 1 && ++mainElementCounter % batchSize == 0)
                    {
                        writer.Flush();
                        if (mainElementCounter == batchSize)
                        {
                            yield return createXmlPart(buffer, BufferState.FirstBuffer);
                        }
                        else
                        {
                            yield return createXmlPart(buffer, BufferState.MiddleBuffer);
                        }
                    }
                    prevDepth = reader.Depth;
                }

                // flush resting buffer
                writer.Flush();
                if (buffer.Length != 0)
                { 
                    yield return createXmlPart(buffer, BufferState.LastBuffer);
                }
            }

        }

        private StringBuilder createXmlPart(StringBuilder currentBuffer, BufferState bufferState)
        {
            StringBuilder result = new StringBuilder();
            result.Append(startingXmlPart);

            if(bufferState == BufferState.MiddleBuffer || bufferState == BufferState.LastBuffer)
            {
                result.Append(">");
            }

            result.Append(currentBuffer);
            result.Append("\n");
            result.Append(endingXmlPart);
            currentBuffer.Clear();
            return result;
        }
        private StringBuilder createEndingXmlPart(string name)
        {
            StringBuilder result = new StringBuilder();
            result.Append("</");
            result.Append(name);
            result.Append(">");
            return result;
        }
    }
}
