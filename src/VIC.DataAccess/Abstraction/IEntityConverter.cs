using System.Data.Common;

namespace VIC.DataAccess.Abstraction
{
    public interface IEntityConverter
    {
        dynamic Convert<T>(DbDataReader reader);
    }
}