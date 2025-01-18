using ARWNI2S.Caching;
using ARWNI2S.Configuration;
using ARWNI2S.Context;
using ARWNI2S.Core.Builder;
using ARWNI2S.Environment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ARWNI2S.Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Configure base application settings
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureNI2SSettings(this IServiceCollection services,
            IHostApplicationBuilder builder)
        {
            //create default file provider
            services.ConfigureFileProvider(builder.Environment);

            //register type finder
            services.GetOrCreateTypeFinder();

            //bind general configuration
            services.BindNI2SSettings(builder.Configuration);
        }

        /// <summary>
        /// Add services to the application and configure service provider
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="builder">A builder for web applications and services</param>
        public static void ConfigureNI2SServices(this IServiceCollection services,
            IHostApplicationBuilder builder)
        {
            //add accessor to HttpContext
            services.AddContextAccessor();

            //initialize plugins
            services.AddNI2SCore().InitializePlugins(builder.Configuration);

            //bind plugins configurations
            services.BindNI2SSettings(builder.Configuration);

            //create engine and configure service provider
            var context = NI2SContext.Create();

            context.ConfigureServices(services, builder.Configuration);
        }

        internal static void ConfigureFileProvider(this IServiceCollection services, IHostEnvironment hostingEnvironment)
        {
            NI2SFileProvider.Default = new NI2SFileProvider(hostingEnvironment);
            services.AddScoped<INiisFileProvider, NI2SFileProvider>();
        }

        internal static void ConfigureFileProvider(this IServiceCollection services, INodeHostEnvironment hostingEnvironment)
        {
            NI2SFileProvider.Default = new NI2SFileProvider(hostingEnvironment);
            services.AddScoped<INiisFileProvider, NI2SFileProvider>();
        }

        public static ITypeFinder GetOrCreateTypeFinder(this IServiceCollection services)
        {
            if (Singleton<ITypeFinder>.Instance == null)
            {
                //register type finder
                Singleton<ITypeFinder>.Instance = new NI2STypeFinder();
                services.AddSingleton(sp => Singleton<ITypeFinder>.Instance);
            }
            return Singleton<ITypeFinder>.Instance;
        }

        internal static NI2SSettings BindNI2SSettings(this IServiceCollection services, IConfiguration configuration)
        {
            //add configuration parameters
            var configurations = services.GetOrCreateTypeFinder()
                .FindClassesOfType<IConfig>()
                .Select(configType => (IConfig)Activator.CreateInstance(configType))
                .ToList();

            foreach (var config in configurations)
                configuration.GetSection(config.Name).Bind(config, options => options.BindNonPublicProperties = true);

            var nodeSettings = Singleton<NI2SSettings>.Instance;

            if (nodeSettings == null)
            {
                nodeSettings = NI2SSettingsHelper.SaveNodeSettings(configurations, NI2SFileProvider.Default, false);
                services.AddSingleton(nodeSettings);
            }
            else
            {
                var needToUpdate = configurations.Any(conf => !nodeSettings.Configuration.ContainsKey(conf.Name));
                NI2SSettingsHelper.SaveNodeSettings(configurations, NI2SFileProvider.Default, needToUpdate);
            }

            return nodeSettings;
        }

        internal static void AddContextAccessor(this IServiceCollection services)
        {
            services.AddSingleton<IWorkingContextAccessor, WorkingContextAccessor>();
        }

        /// <summary>
        /// Adds services required for distributed cache
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        internal static void AddDistributedCache(this IServiceCollection services)
        {
            var nodeSettings = Singleton<NI2SSettings>.Instance;
            var distributedCacheConfig = nodeSettings.Get<DistributedCacheConfig>();

            if (!distributedCacheConfig.Enabled)
                return;

            switch (distributedCacheConfig.DistributedCacheType)
            {
                case DistributedCacheType.Memory:
                    services.AddDistributedMemoryCache();
                    break;

                case DistributedCacheType.SqlServer:
                    services.AddDistributedSqlServerCache(options =>
                    {
                        options.ConnectionString = distributedCacheConfig.ConnectionString;
                        options.SchemaName = distributedCacheConfig.SchemaName;
                        options.TableName = distributedCacheConfig.TableName;
                    });
                    break;

                case DistributedCacheType.Redis:
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = distributedCacheConfig.ConnectionString;
                        options.InstanceName = distributedCacheConfig.InstanceName ?? string.Empty;
                    });
                    break;

                case DistributedCacheType.RedisSynchronizedMemory:
                    services.AddStackExchangeRedisCache(options =>
                    {
                        options.Configuration = distributedCacheConfig.ConnectionString;
                        options.InstanceName = distributedCacheConfig.InstanceName ?? string.Empty;
                    });
                    break;
            }
        }
    }
}
