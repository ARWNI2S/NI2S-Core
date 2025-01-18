using ARWNI2S.Core;
using ARWNI2S.Environment;
using ARWNI2S.Environment.Mapper;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Context
{
    internal static class NI2SContextExtensions
    {
        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration of the application</param>
        internal static void ConfigureServices(this INiisContext context, IServiceCollection services, IConfiguration configuration)
        {
            //register engine context
            services.AddSingleton(context);

            //find startup configurations provided by other assemblies
            var instances = services.GetOrCreateTypeFinder().GetStartupConfigurationInstancess();

            //configure services
            foreach (var instance in instances)
                instance.ConfigureServices(services, configuration);

            //register services
            services.AddSingleton(services);

            //register mapper configurations
            AddAutoMapper();

            ////run startup tasks
            //RunStartupTasks();

            NI2SDomain.InitializeDomain();
        }

        /// <summary>
        /// Register and configure AutoMapper
        /// </summary>
        private static void AddAutoMapper()
        {
            //find mapper configurations provided by other assemblies
            var typeFinder = Singleton<ITypeFinder>.Instance;
            var mapperConfigurations = typeFinder.FindClassesOfType<IOrderedMapperProfile>();

            //create and sort instances of mapper configurations
            var instances = mapperConfigurations
                .Select(mapperConfiguration => (IOrderedMapperProfile)Activator.CreateInstance(mapperConfiguration))
                .OrderBy(mapperConfiguration => mapperConfiguration.Order);

            //create AutoMapper configuration
            var config = new MapperConfiguration(cfg =>
            {
                foreach (var instance in instances)
                {
                    cfg.AddProfile(instance.GetType());
                }
            });

            //register
            AutoMapperConfiguration.Init(config);
        }

        ///// <summary>
        ///// Run startup tasks
        ///// </summary>
        //private void RunStartupTasks()
        //{
        //    //find startup tasks provided by other assemblies
        //    var typeFinder = Singleton<ITypeFinder>.Instance;
        //    var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

        //    //create and sort instances of startup tasks
        //    //we startup this interface even for not installed modules. 
        //    //otherwise, DbContext initializers won't run and a module installation won't work
        //    var instances = startupTasks
        //        .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
        //        .OrderBy(startupTask => startupTask.Order);

        //    //execute tasks
        //    foreach (var task in instances)
        //        task.Execute();
        //}
    }
}
