using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace VIC.ObjectConfig.Xml
{
    public class XmlConfigFileProvider<T> : ConfigFileProvider<T> where T : class
    {
        public XmlConfigFileProvider(string key, bool isWatch, Func<Task<T>[], Task<T>> Aggregate, params string[] fileNames) : base(key, isWatch, Aggregate, fileNames)
        {
        }

        protected override Task<T> ToObject(Stream stream)
        {
            using (stream)
            {
                var serializer = new XmlSerializer(typeof(T));
                return Task.FromResult((T)serializer.Deserialize(stream));
            }
        }
    }
}