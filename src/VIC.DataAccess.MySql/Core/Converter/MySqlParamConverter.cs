using Pomelo.Data.MySql;
using System;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;

namespace VIC.DataAccess.MySql.Core.Converter
{
    public class MySqlParamConverter : ParamConverter
    {
        private Type _ParameterType = typeof(MySqlParameter);

        public MySqlParamConverter(IDbTypeConverter dc) : base(dc)
        {
        }

        protected override Type GetParameterType()
        {
            return _ParameterType;
        }
    }
}