using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.MSSql.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service, DbConfig config)
        {
            service.AddSingleton<IDbManager>(new MSSqlDbManager(config));
            return service;
        }

        public static Task ExecuteBulkCopyAsync<T>(this IDataCommand command, List<T> data) where T : class, new()
        {
            var msCommand = command as MSSqlDataCommand;
            if (msCommand == null) throw new ArgumentException("command is not MSSqlDataCommand");
            return msCommand.ExecuteBulkCopyAsync(data);
        }
    }
}