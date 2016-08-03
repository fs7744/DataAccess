using System;
using System.Data;

namespace VIC.DataAccess.Abstraction.Converter
{
    public interface IDbTypeConverter
    {
        DbType Convert(Type type);
    }
}