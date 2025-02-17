﻿using ARWNI2S.Environment;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace ARWNI2S.Configuration
{
    /// <summary>
    /// Represents the niis settings helper
    /// </summary>
    public partial class NI2SSettingsHelper
    {
        #region Fields

        private static Dictionary<string, int> _configurationOrder;

        #endregion

        #region Methods

        /// <summary>
        /// Create niis settings with the passed configurations and save it to the file
        /// </summary>
        /// <param name="configurations">Configurations to save</param>
        /// <param name="fileProvider">File provider</param>
        /// <param name="overwrite">Whether to overwrite niissettings file</param>
        /// <returns>Node settings</returns>
        public static NI2SSettings SaveNodeSettings(IList<IConfig> configurations, INiisFileProvider fileProvider, bool overwrite = true)
        {
            ArgumentNullException.ThrowIfNull(configurations);

            _configurationOrder ??= configurations.ToDictionary(config => config.Name, config => config.GetOrder());

            //create node settings
            var nodeSettings = Singleton<NI2SSettings>.Instance ?? new NI2SSettings();
            nodeSettings.Update(configurations);
            Singleton<NI2SSettings>.Instance = nodeSettings;

            //create file if not exists
            var filePath = fileProvider.MapPath(ConfigDefaults.SettingsFilePath);
            var fileExists = fileProvider.FileExists(filePath);
            fileProvider.CreateFile(filePath);

            //get raw configuration parameters
            var configuration = JsonConvert.DeserializeObject<NI2SSettings>(fileProvider.ReadAllText(filePath, Encoding.UTF8))
                ?.Configuration
                ?? [];
            foreach (var config in configurations)
            {
                configuration[config.Name] = JToken.FromObject(config);
            }

            //sort configurations for display by order (e.g. data configuration with 0 will be the first)
            nodeSettings.Configuration = configuration
                .SelectMany(outConfig => _configurationOrder.Where(inConfig => inConfig.Key == outConfig.Key).DefaultIfEmpty(),
                    (outConfig, inConfig) => new { OutConfig = outConfig, InConfig = inConfig })
                .OrderBy(config => config.InConfig.Value)
                .Select(config => config.OutConfig)
                .ToDictionary(config => config.Key, config => config.Value);

            //save niis settings to the file
            if (!fileExists || overwrite)
            {
                var text = JsonConvert.SerializeObject(nodeSettings, Formatting.Indented);//, new JsonSerializerSettings { ContractResolver = new SecretContractResolver() });
                fileProvider.WriteAllText(filePath, text, Encoding.UTF8);
            }

            return nodeSettings;
        }

        #endregion
    }
}