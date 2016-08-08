using System.Collections.Generic;
using VIC.ObjectConfig.Abstraction;

namespace VIC.ObjectConfig
{
    public class PhysicalFileConfigBuilder : IConfigBuilder
    {
        protected IConfigStore _Store;
        protected List<IConfigProvider> _Providers = new List<IConfigProvider>();

        public IConfigBuilder SetBasePath(string basePath)
        {
            _Store = new PhysicalFileConfigStore(basePath);
            return this;
        }

        public IConfigBuilder Add(IConfigProvider provider)
        {
            _Providers.Add(provider);
            return this;
        }

        public IConfig Build()
        {
            _Providers.ForEach(i => i.SetConfig(_Store));
            return _Store;
        }
    }
}