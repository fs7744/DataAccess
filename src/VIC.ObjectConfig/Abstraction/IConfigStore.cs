using Microsoft.Extensions.FileProviders;

namespace VIC.ObjectConfig.Abstraction
{
    public interface IConfigStore : IConfig
    {
        IFileProvider FileProvider { get; }

        void Update(IConfigSource source);
    }
}