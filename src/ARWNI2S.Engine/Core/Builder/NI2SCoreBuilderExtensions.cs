using ARWNI2S.Core.Builder;
using ARWNI2S.Extensibility.Plugins;
using Microsoft.Extensions.Configuration;

namespace ARWNI2S.Core.Builder
{
    internal static class NI2SCoreBuilderExtensions
    {
        internal static INiisCoreBuilder InitializePlugins(this INiisCoreBuilder builder, IConfiguration configuration)
        {
            var pluginConfig = new PluginConfig();
            configuration.GetSection(nameof(PluginConfig)).Bind(pluginConfig, options => options.BindNonPublicProperties = true);
            builder.PartManager.InitializePlugins(pluginConfig);
            return builder;
        }

    }
}
