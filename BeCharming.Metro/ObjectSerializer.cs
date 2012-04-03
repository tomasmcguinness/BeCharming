using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace BeCharming.Metro
{
    internal static class ObjectSerializer<T>
    {
        /// <summary>
        /// Serialize the given value to XML using
        /// XmlSerializer
        /// </summary>
        /// <param name="value">value to serialize</param>
        /// <returns>xml serialized string</returns>
        public static string ToXml(T value)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            StringBuilder stringBuilder = new StringBuilder();
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Indent = true,
                OmitXmlDeclaration = true,
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(stringBuilder, settings))
            {
                serializer.Serialize(xmlWriter, value);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Deserialize the xml string to an object
        /// </summary>
        /// <param name="xml">xml string to deserialize</param>
        /// <returns>deserialized object</returns>
        public static T FromXml(string xml)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            T value;
            using (StringReader stringReader = new StringReader(xml))
            {
                object deserialized = serializer.Deserialize(stringReader);
                value = (T)deserialized;
            }

            return value;
        }
    }

}
