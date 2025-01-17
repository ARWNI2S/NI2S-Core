using ARWNI2S.Extensibility.Parts;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Core.Builder
{
    internal class NI2SCoreBuilder : INiisCoreBuilder
    {
        /// <summary>
        /// Initializes a new <see cref="NI2SCoreBuilder"/> instance.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection" /> to add services to.</param>
        /// <param name="manager">The <see cref="NI2SPartManager"/> of the application.</param>
        public NI2SCoreBuilder(
            IServiceCollection services,
            NI2SPartManager manager)
        {
            ArgumentNullException.ThrowIfNull(services);
            ArgumentNullException.ThrowIfNull(manager);

            Services = services;
            PartManager = manager;
        }

        /// <inheritdoc />
        public NI2SPartManager PartManager { get; }

        /// <inheritdoc />
        public IServiceCollection Services { get; }

        INiisPartManager INiisCoreBuilder.PartManager => PartManager;
    }
}