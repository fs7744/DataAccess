using System;
using System.Data;
using VIC.DataAccess.Abstraction.Converter;

namespace VIC.DataAccess.Core.Converter
{
    public class EntityConverter : IEntityConverter
    {
        public Func<IDataReader, T> GetConverter<T>(IDataReader reader)
        {
            return EmitEntityConverter<T>.GetConverter(reader);
        }
    }
}