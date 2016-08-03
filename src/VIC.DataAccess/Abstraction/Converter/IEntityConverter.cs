using System.Data.Common;

namespace VIC.DataAccess.Abstraction.Converter
{
    public interface IEntityConverter
    {
        dynamic Convert<T>(DbDataReader reader);
    }
}