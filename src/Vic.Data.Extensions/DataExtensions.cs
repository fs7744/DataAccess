using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vic.Data;
using Vic.Data.Abstraction;
using Vic.ServiceLocation;

namespace System.Data
{
    public static class DataExtensions
    {
        private static IEnumerable<T> ToEntities<T>(this IDataReader reader)
        {
            var converter = ServiceLocator.Current.GetRequiredService<IEntityConverter<T>>();
            if (reader.Read() && reader.FieldCount > 0)
            {
                yield return converter.Convert(reader);
            }
            else
            {
                yield return default(T);
            }
        }

        public static IServiceCollection UseVicData(this IServiceCollection services)
        {
            return services.AddSingleton(typeof(IEntityConverter<>), typeof(EmitEntityConverter<>))
                .UseServiceLocator();
        }

        public static IEnumerable<T> ExecuteEntities<T>(this DbCommand command, CommandBehavior behavior = CommandBehavior.Default)
        {
            var reader = command.ExecuteReader(behavior);
            return ToEntities<T>(reader);
        }

        public static async Task<IEnumerable<T>> ExecuteEntitiesAsync<T>(this DbCommand command, CommandBehavior behavior = CommandBehavior.Default, CancellationToken cancellation = default(CancellationToken))
        {
            var reader = await command.ExecuteReaderAsync(behavior, cancellation);
            return ToEntities<T>(reader);
        }

        public static T ExecuteEntitiy<T>(this DbCommand command, CommandBehavior behavior = CommandBehavior.Default)
        {
            return command.ExecuteEntities<T>(behavior).First();
        }

        public static async Task<T> ExecuteEntitiyAsync<T>(this DbCommand command, CommandBehavior behavior = CommandBehavior.Default, CancellationToken cancellation = default(CancellationToken))
        {
            return (await command.ExecuteEntitiesAsync<T>(behavior, cancellation)).First();
        }
    }
}