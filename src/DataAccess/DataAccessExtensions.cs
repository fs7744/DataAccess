using System;
using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(IServiceCollection service)
        {    

            return service;
        }
    }
}
