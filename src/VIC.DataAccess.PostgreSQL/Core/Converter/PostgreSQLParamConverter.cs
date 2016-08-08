using Npgsql;
using System;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;

namespace VIC.DataAccess.PostgreSQL.Core.Converter
{
    public class PostgreSQLParamConverter : ParamConverter
    {
        private Type _ParameterType = typeof(NpgsqlParameter);

        public PostgreSQLParamConverter(IDbTypeConverter dc) : base(dc)
        {
        }

        protected override Type GetParameterType()
        {
            return _ParameterType;
        }
    }
}