using System;

namespace VIC.DataAccess.Abstraction.Converter
{
    public interface IDbFuncNameConverter
    {
        string Convert(Type type);
    }
}