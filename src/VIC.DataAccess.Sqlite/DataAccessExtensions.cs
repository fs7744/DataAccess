﻿using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.Sqlite.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service, DbConfig config)
        {
            return service.AddSingleton<IDbManager>(new SqliteDbManager(config));
        }
    }
}