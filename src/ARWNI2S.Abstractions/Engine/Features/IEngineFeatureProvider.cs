using ARWNI2S.Engine.EngineParts;

namespace ARWNI2S.Engine.Features
{
    /// <summary>
    /// Marker interface for <see cref="IEngineFeatureProvider"/>
    /// implementations.
    /// </summary>
    public interface IEngineFeatureProvider
    {
    }

    /// <summary>
    /// A provider for a given <typeparamref name="TService"/> service.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    public interface INiisFeatureProvider<TService> : IEngineFeatureProvider
    {
        /// <summary>
        /// Updates the <paramref name="service"/> instance.
        /// </summary>
        /// <param name="parts">The list of <see cref="EnginePart"/> instances in the engine.
        /// </param>
        /// <param name="service">The module instance to populate.</param>
        /// <remarks>
        /// <see cref="EnginePart"/> instances in <paramref name="parts"/> appear in the same ordered sequence they
        /// are stored in <see cref="IEnginePartManager.EngineParts"/>. This ordering may be used by the module
        /// provider to make precedence decisions.
        /// </remarks>
        void PopulateFeature(IEnumerable<EnginePart> parts, TService service);
    }
}