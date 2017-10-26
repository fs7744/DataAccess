using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace VIC.DataAccess.Abstraction.Converter
{
    public interface IParamConverter
    {
        Tuple<Func<dynamic, List<DbParameter>>, Func<dynamic, IGrouping<string, DbParameter>[]>> GetConverter(Type type);
    }
}