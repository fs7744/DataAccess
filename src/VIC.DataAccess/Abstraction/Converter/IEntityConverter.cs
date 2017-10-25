using System;
using System.Data;
using System.Data.Common;

namespace VIC.DataAccess.Abstraction.Converter
{
    public interface IEntityConverter
    {
        Func<IDataReader, T> GetConverter<T>(IDataReader reader);
    }
}