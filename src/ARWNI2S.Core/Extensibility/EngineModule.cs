using ARWNI2S.Builder;
using ARWNI2S.Lifecycle;
using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Extensibility
{
    /// <summary>
    /// Represents a module that participates in the engine lifecycle
    /// </summary>
    public abstract class EngineModule : ModuleBase, IEngineModule, ILifecycleParticipant<ILifecycleSubject>
    {
        /// <summary>
        /// Gets the order of the module in the lifecycle.
        /// </summary>
        public override int Order => NI2SLifecycleStage.RuntimeInitialize;

        /// <summary>
        /// Gets the module system name.
        /// </summary>
        public override string SystemName { get => GetType().ToSystemName(); set => throw new NotImplementedException(); }
        /// <summary>
        /// Gets the module friendly name.
        /// </summary>
        public override string DisplayName { get => GetType().ToDisplayName(); set => throw new NotImplementedException(); }

        /// <summary>
        /// Configure the engine to use the module
        /// </summary>
        /// <param name="engineBuilder">The engine builder</param>
        public virtual void Configure(INiisBuilder engineBuilder)
        {
            var moduleManager = engineBuilder.EngineServices.GetRequiredService<INiisModuleManager>() ?? 
                throw new InvalidOperationException($"Fatal: Required service {nameof(INiisModuleManager)} not found.");

            moduleManager.RegisterModule(this);
        }

        /// <summary>
        /// Participate in the lifecycle
        /// </summary>
        /// <param name="lifecycle">The lifecycle to participate in</param>
        public void Participate(ILifecycleSubject lifecycle)
        {
            lifecycle.Subscribe(SystemName, Order, OnStart, OnStop);
        }

        /// <summary>
        /// Start the module
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>A task</returns>
        protected virtual Task OnStart(CancellationToken token)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Stop the module
        /// </summary>
        /// <param name="token">Cancellation token</param>
        /// <returns>A task</returns>
        protected virtual Task OnStop(CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}