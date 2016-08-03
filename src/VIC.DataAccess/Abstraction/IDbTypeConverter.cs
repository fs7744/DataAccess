using System;
using System.Data;

namespace VIC.DataAccess.Abstraction
{
    public interface IDbTypeConverter
    {
        DbType Convert(Type type);
    }
}