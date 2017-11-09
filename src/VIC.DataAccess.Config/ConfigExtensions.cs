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
        public static IServiceCollection UseDataAccessConfig(this IServiceCollection service, string basePath, bool isWatch = false, DbConfig[] others = null, params string[] xmlFiles)
        {
            return service.UseDataAccessConfigByConnectionStringProvider(basePath, isWatch, others, null, xmlFiles);
        }

        public static IServiceCollection UseDataAccessConfigByConnectionStringProvider(this IServiceCollection service, string basePath, bool isWatch = false, DbConfig[] others = null, IConnectionStringProvider provider = null, params string[] xmlFiles)
        {
            var config = new PhysicalFileConfigBuilder()
                .SetBasePath(basePath)
                .Add(new XmlConfigFileProvider<DbConfig>(DbManager.DbConfigKey, isWatch, i => BuildConfig(i, others, provider), xmlFiles))
                .Build();
            service.AddSingleton<IDbManager>(i => new DbManager(config, i));
            return service;
        }

        public static async Task<DbConfig> BuildConfig(Task<DbConfig>[] vs, DbConfig[] others, IConnectionStringProvider provider)
        {
            var config = new DbConfig()
            {
                Sqls = new Dictionary<string, DbSql>()
            };
            var css = new Dictionary<string, string>();
            if (others != null)
            {
                foreach (var i in others)
                {
                    i?.ConnectionStrings?.ForEach(x => css.Add(x.Name, x.ConnectionString));
                    i?.SqlConfigs?.ForEach(x => config.Sqls.Add(x.CommandName, x));
                }
            }
            foreach (var i in vs)
            {
                var c = await i;
                c?.ConnectionStrings?.ForEach(x => css.Add(x.Name, x.ConnectionString));
                c?.SqlConfigs?.ForEach(x => config.Sqls.Add(x.CommandName, x));
            }
            if (provider != null)
            {
                provider.Update(css);
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