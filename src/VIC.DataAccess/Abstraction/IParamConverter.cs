using System;
using System.Collections.Generic;
using System.Data.Common;

namespace VIC.DataAccess.Abstraction
{
    public interface IParamConverter
    {
        List<DbParameter> Convert(Type type, dynamic parameters);
    }
}