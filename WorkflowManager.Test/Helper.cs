using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml.Serialization;

namespace WorkflowManager.Test
{
    public static class Helper
    {
        public static string GetContentsFromEmbeddedFile(string fileName)
        {
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(fileName))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;
        }

        public static T Deserialize<T>(string fileName) where T : class
        {
            var asm = Assembly.GetExecutingAssembly();
            var formatter = new BinaryFormatter();
            using (var stream = asm.GetManifestResourceStream(fileName))
            {
                return (T)formatter.Deserialize(stream);
            }
        }

        public static T DeserializeXml<T>(string fileName) where T : class
        {
            var asm = Assembly.GetExecutingAssembly();
            var formatter = new XmlSerializer(typeof(T));
            using (var stream = asm.GetManifestResourceStream(fileName))
            {
                return (T)formatter.Deserialize(stream);
            }
        }

        public static string SerializeXml<T>(T obj)
        {
            var asm = Assembly.GetExecutingAssembly();
            var formatter = new XmlSerializer(typeof(T));
            using (var stream = new StringWriter())
            {
                formatter.Serialize(stream, obj);
                return stream.ToString();
            }
        }


    }
}
