using System;

namespace VIC.DataAccess.Abstraction
{
    public interface IDbFuncNameConverter
    {
        string Convert(Type type);
    }
}