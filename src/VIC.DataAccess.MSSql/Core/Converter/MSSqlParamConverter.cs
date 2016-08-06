using System;
using System.Data.SqlClient;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;

namespace VIC.DataAccess.MSSql.Core.Converter
{
    public class MSSqlParamConverter : ParamConverter
    {
        private Type _ParameterType = typeof(SqlParameter);

        public MSSqlParamConverter(IDbTypeConverter dc) : base(dc)
        {
        }

        protected override Type GetParameterType()
        {
            return _ParameterType;
        }
    }
}