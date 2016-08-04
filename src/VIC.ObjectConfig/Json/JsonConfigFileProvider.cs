using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace VIC.ObjectConfig.Json
{
    public class JsonConfigFileProvider<T> : ConfigFileProvider<T> where T : class, new()
    {
        public JsonConfigFileProvider(string key, bool isWatch, Func<Task<T>[], Task<T>> Aggregate, params string[] fileNames) : base(key, isWatch, Aggregate, fileNames)
        {
        }

        protected override async Task<T> ToObject(Stream stream)
        {
            using (TextReader reader = new StreamReader(stream))
            {
                return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync());
            }
        }
    }
}