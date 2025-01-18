using Microsoft.Extensions.Logging;

namespace ARWNI2S.Extensibility
{
    internal sealed class NI2SModuleManager : INiisModuleManager
    {
        private readonly ILogger _logger;

        public NI2SModuleCollection Modules { get; }

        public NI2SModuleManager(ILoggerFactory logger)
        {
            _logger = logger.CreateLogger(nameof(NI2SModuleManager));
            Modules = new NI2SModuleCollection();
        }

        public void RegisterModule(INiisModule module)
        {
            _logger.LogTrace("Registering module: {SystemName}", module.SystemName);

            if (CheckModuleDependencies(module))
            {
                Modules[module.GetType()] = module;
                _logger.LogDebug("Module {SystemName} registered.", module.SystemName);
            }
        }

        INiisModuleCollection INiisModuleManager.Modules => Modules;

        public INiisModule GetModule(Type type) => Modules[type];

        public INiisModule FindModule(string name)
        {
            ArgumentNullException.ThrowIfNullOrEmpty(name);

            if (Modules != null)
            {
                var found = Modules.FirstOrDefault(kvp => kvp.Value.SystemName.Equals(name, StringComparison.InvariantCultureIgnoreCase)).Value;
                if (found != null)
                    return found;
            }

            return null;
        }

        private bool CheckModuleDependencies(INiisModule module)
        {
            if (module.ModuleDependencies.Count > 0)
            {
                _logger.LogDebug("Checking dependencies...");
                foreach (var dependency in module.ModuleDependencies)
                {
                    var found = Modules.Where(m => m.Value.SystemName == dependency).ToArray();

                    if (found == null || found.Length == 0)
                    {
                        _logger.LogError("Module {SystemName} has a dependency on {dependency} which is not registered.", module.SystemName, dependency);
                        throw new ModuleDependencyException(module.SystemName, dependency);
                    }
                }
            }
            return true;
        }
    }
}
