using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VIC.ObjectConfig.Abstraction;

namespace VIC.ObjectConfig
{
    public abstract class ConfigFileProvider<T> : IConfigProvider where T : class, new()
    {
        protected string[] _FileNames;

        protected bool _IsWatch;

        protected Func<Task<T>[], Task<T>> _Aggregate;

        protected string _Key;

        public ConfigFileProvider(string key, bool isWatch, Func<Task<T>[], Task<T>> Aggregate, params string[] fileNames)
        {
            _FileNames = fileNames;
            _IsWatch = isWatch;
            _Aggregate = Aggregate;
            _Key = key;
        }

        public void SetConfig(IConfigStore config)
        {
            UpdateConfig(config);
            Watch(config);
        }

        private void Watch(IConfigStore config)
        {
            if (_IsWatch)
            {
                foreach (var item in _FileNames)
                {
                    var watch = config.FileProvider.Watch(item);
                    watch.RegisterChangeCallback(o => UpdateConfig(config), null);
                }
            }
        }

        private async void UpdateConfig(IConfigStore config)
        {
            var data = _FileNames.Select(async i =>
            {
                var file = config.FileProvider.GetFileInfo(i);
                return file.Exists && !file.IsDirectory
                    ? await ToObject(file.CreateReadStream())
                    : null;
            }).Where(i => i != null).ToArray();
            var d = await _Aggregate(data);
            config.Update(new ConfigSource(_Key, () => d));
        }

        protected abstract Task<T> ToObject(Stream stream);
    }
}