using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;
using VIC.ObjectConfig;
using VIC.ObjectConfig.Xml;

namespace VIC.DataAccess.Config
{
    public static class ConfigExtensions
    {
        public static IServiceCollection UseDataAccessConfig(this IServiceCollection service, string basePath, bool isWatch = false, params string[] xmlFiles)
        {
            var config = new PhysicalFileConfigBuilder()
                .SetBasePath(basePath)
                .Add(new XmlConfigFileProvider<DbConfig>(DbManager.DbConfigKey, isWatch, BuildConfig, xmlFiles))
                .Build();
            service.AddSingleton<IDbManager>(i => new DbManager(config, i));
            return service;
        }

        private static async Task<DbConfig> BuildConfig(Task<DbConfig>[] vs)
        {
            var config = new DbConfig()
            {
                Sqls = new Dictionary<string, DbSql>()
            };
            var css = new Dictionary<string, string>();
            foreach (var i in vs)
            {
                var c = await i;
                c?.ConnectionStrings?.ForEach(x => css.Add(x.Name, x.ConnectionString));
                c?.SqlConfigs?.ForEach(x => config.Sqls.Add(x.CommandName, x));
            }
            foreach (var item in config.Sqls.Values)
            {
                var connectionString = string.Empty;
                css.TryGetValue(item.ConnectionName, out connectionString);
                item.ConnectionString = connectionString;
            }
            return config;
        }
    }
}