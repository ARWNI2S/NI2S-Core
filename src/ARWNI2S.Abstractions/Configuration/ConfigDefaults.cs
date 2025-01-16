namespace ARWNI2S.Configuration
{
    /// <summary>
    /// Represents default values related to configuration services
    /// </summary>
    public static partial class ConfigDefaults
    {
        /// <summary>
        /// Gets the path to file that contains node settings
        /// </summary>
        public static string SettingsFilePath => "NI2S_Data/niissettings.json";

        /// <summary>
        /// Gets the path to file that contains node settings for specific hosting environment
        /// </summary>
        /// <remarks>0 - Environment name</remarks>
        public static string SettingsEnvironmentFilePath => "NI2S_Data/niissettings.{0}.json";
    }
}
