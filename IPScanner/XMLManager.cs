using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace IPScanner
{
    public static class XMLManager
    {
        public static string FilePath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "devices.xml");

        public static void Import()
        {

        }

        public static void Export(object obj)
        {

        }

        public static object Deserialize(Type t)
        {
            var serializer = new XmlSerializer(t);
            StreamReader reader = new StreamReader(FilePath);
            var obj = serializer.Deserialize(reader);
            reader.Close();
            return obj;
        }


        public static string Serialize<T>(T value, bool writeToFile = false)
        {
            string output = string.Empty;

            if (value == null)
            {
                return output;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();

                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    output = stringWriter.ToString();
                }

                if (writeToFile)
                {
                    using (var writer = new StreamWriter(FilePath))
                    {
                        xmlserializer.Serialize(writer, value);
                        writer.Flush();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }

            return output;
        }

    }
}
