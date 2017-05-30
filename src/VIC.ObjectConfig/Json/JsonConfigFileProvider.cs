using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace VIC.ObjectConfig.Json
{
    public class JsonConfigFileProvider<T> : ConfigFileProvider<T> where T : class
    {
        public JsonConfigFileProvider(string key, bool isWatch, Func<Task<T>[], Task<T>> Aggregate, params string[] fileNames) : base(key, isWatch, Aggregate, fileNames)
        {
        }

        protected override Task<T> ToObject(Stream stream)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (stream)
            using (var sr = new StreamReader(stream))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                return Task.FromResult(serializer.Deserialize<T>(reader));
            }
        }
    }
}