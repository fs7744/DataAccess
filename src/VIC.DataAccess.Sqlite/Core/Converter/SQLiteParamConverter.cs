using Microsoft.Data.Sqlite;
using System;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;

namespace VIC.DataAccess.SQLite.Core.Converter
{
    public class SQLiteParamConverter : ParamConverter
    {
        private Type _ParameterType = typeof(SqliteParameter);

        public SQLiteParamConverter(IDbTypeConverter dc) : base(dc)
        {
        }

        protected override Type GetParameterType()
        {
            return _ParameterType;
        }
    }
}