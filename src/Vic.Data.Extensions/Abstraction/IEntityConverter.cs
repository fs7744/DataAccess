using System.Data;

namespace Vic.Data.Abstraction
{
    public interface IEntityConverter<T>
    {
        T Convert(IDataReader reader);
    }
}