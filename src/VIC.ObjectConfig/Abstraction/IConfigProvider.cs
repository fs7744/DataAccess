using System.Collections.Generic;

namespace VIC.ObjectConfig.Abstraction
{
    public interface IConfigProvider
    {
        IEnumerable<IConfigSource> GetSources();
    }
}