using System.Collections.Generic;

namespace VIC.DataAccess.Abstraction
{
    public interface IConnectionStringProvider
    {
        void Update(Dictionary<string, string> connectionStrings);
    }
}