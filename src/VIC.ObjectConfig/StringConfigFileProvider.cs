using System;
using System.IO;
using System.Threading.Tasks;

namespace VIC.ObjectConfig
{
    public class StringConfigFileProvider : ConfigFileProvider<string>
    {
        public StringConfigFileProvider(string key, bool isWatch, Func<Task<string>[], Task<string>> Aggregate, params string[] fileNames) : base(key, isWatch, Aggregate, fileNames)
        {
        }

        protected override Task<string> ToObject(Stream stream)
        {
            using (stream)
            using (TextReader sr = new StreamReader(stream))
            {
                return sr.ReadToEndAsync();
            }
        }
    }
}