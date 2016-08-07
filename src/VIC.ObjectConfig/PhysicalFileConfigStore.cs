using Microsoft.Extensions.FileProviders;
using System.Collections.Concurrent;
using VIC.ObjectConfig.Abstraction;

namespace VIC.ObjectConfig
{
    public class PhysicalFileConfigStore : IConfigStore
    {
        public IFileProvider FileProvider { get; protected set; }

        protected ConcurrentDictionary<string, IConfigSource> _Sources = new ConcurrentDictionary<string, IConfigSource>();

        public PhysicalFileConfigStore(string basePath)
        {
            FileProvider = new PhysicalFileProvider(basePath);
        }

        public IConfigSource GetConfigSource(string key)
        {
            IConfigSource result = null;
            _Sources.TryGetValue(key, out result);
            return result;
        }

        public bool Update(IConfigSource source)
        {
            if (_Sources.ContainsKey(source.Key))
            {
                return _Sources.TryUpdate(source.Key, source, GetConfigSource(source.Key));
            }
            else
            {
                return _Sources.TryAdd(source.Key, source);
            }
        }
    }
}