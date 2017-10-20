using System.Data.Common;
using VIC.DataAccess.Abstraction.Converter;

namespace VIC.DataAccess.Core.Converter
{
    public class EntityConverter : IEntityConverter
    {
        public dynamic Convert<T>(DbDataReader reader)
        {
            return EmitEntityConverter<T>.Convert(reader);
        }
    }
}