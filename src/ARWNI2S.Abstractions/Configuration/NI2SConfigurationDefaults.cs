namespace ARWNI2S.Configuration
{
    internal static class NI2SConfigurationDefaults
    {
        internal static readonly string CommonConfigName = "Commons";
        internal static readonly string PluginConfigName = "Plugins";

        internal static string GetConfigName<TConfig>() where TConfig : class, IConfig
        {
            var name = typeof(TConfig).Name;
            if (name.EndsWith("Config"))
                name = name.Replace("Config", "");
            else if (name.EndsWith("Configuration"))
                name = name.Replace("Configuration", "");

            return name;
        }
    }
}
