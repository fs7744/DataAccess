using Microsoft.Extensions.FileProviders;

namespace VIC.ObjectConfig.Abstraction
{
    public interface IConfigStore : IConfig
    {
        IFileProvider FileProvider { get; }

        bool Update(IConfigSource source);
    }
}