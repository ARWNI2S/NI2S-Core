﻿using Newtonsoft.Json;

namespace ARWNI2S.Configuration
{
    /// <summary>
    /// Represents common configuration parameters
    /// </summary>
    public partial class CommonConfig : IConfig
    {
        /// <summary>
        /// Gets a section name to load configuration
        /// </summary>
        [JsonIgnore]
        public string Name => NI2SConfigurationDefaults.CommonConfigName;

        /// <summary>
        /// Gets or sets a value indicating whether to display the full error in production environment. It's ignored (always enabled) in development environment
        /// </summary>
        public bool DisplayFullErrorStack { get; private set; } = false;

        /// <summary>
        /// The length of time, in milliseconds, before the running schedule task times out. Set null to use default value
        /// </summary>
        public int? ScheduleTaskRunTimeout { get; private set; } = null;

        /// <summary>
        /// Gets or sets a value of "Cache-Control" header value for static content (in seconds)
        /// </summary>
        public string LocalFilesCacheControl { get; private set; } = "public,max-age=31536000";

        /// <summary>
        /// Get or set the blacklist of static file extension for plugin directories
        /// </summary>
        public string PluginLocalFileExtensionsBlacklist { get; private set; } = "";

        /// <summary>
        /// Get or set a value indicating whether to use Autofac IoC container
        ///
        /// If value is set to false then the default .Net IoC container will be use
        /// </summary>
        public bool UseAutofac { get; set; } = true;

        /// <summary>
        /// Gets or sets a value that indicates whether to use MiniProfiler services
        /// </summary>
        public bool MiniProfilerEnabled { get; private set; } = false;
    }
}