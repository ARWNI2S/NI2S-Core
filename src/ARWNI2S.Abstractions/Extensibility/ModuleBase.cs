using ARWNI2S.Configuration;
using ARWNI2S.Environment;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Extensibility
{
    public abstract class ModuleBase : INiisModule, IConfigureStartup
    {
        /// <summary>
        /// Gets or sets the module system name.
        /// </summary>
        public abstract string SystemName { get; set; }

        /// <summary>
        /// Gets or sets the module friendly name.
        /// </summary>
        public abstract string DisplayName { get; set; }

        /// <summary>
        /// Gets the initialization order of the module.
        /// </summary>
        public abstract int Order { get; }

        /// <summary>
        /// Gets the module dependencies
        /// </summary>
        public virtual IList<string> ModuleDependencies { get; } = [];

        /// <summary>
        /// Gets the global type finder.
        /// </summary>
        protected readonly ITypeFinder TypeFinder = Singleton<ITypeFinder>.Instance;

        /// <summary>
        /// Gets the current local configuration.
        /// </summary>
        protected readonly NI2SSettings Settings = Singleton<NI2SSettings>.Instance;

        /// <summary>
        /// Add and configure any of the module services
        /// </summary>
        /// <param name="services">Service collection</param>
        /// <param name="configuration">Host configuration</param>
        public abstract void ConfigureServices(IServiceCollection services, IConfiguration configuration);
    }
}
