using System.Data.Common;

namespace VIC.DataAccess.Abstraction.Converter
{
    public interface IScalarConverter
    {
        dynamic Convert<T>(DbDataReader reader);
    }
}