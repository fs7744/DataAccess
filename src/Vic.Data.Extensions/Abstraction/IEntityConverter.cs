using System.Data.Common;

namespace Vic.Data.Abstraction
{
    public interface IEntityConverter<T> where T : class
    {
        T Convert(DbDataReader reader);
    }
}