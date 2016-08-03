using System.Data.Common;

namespace VIC.DataAccess.Abstraction
{
    public interface IScalarConverter
    {
        dynamic Convert<T>(DbDataReader reader);
    }
}