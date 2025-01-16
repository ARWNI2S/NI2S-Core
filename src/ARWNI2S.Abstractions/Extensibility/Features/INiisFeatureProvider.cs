using ARWNI2S.Extensibility.Parts;

namespace ARWNI2S.Extensibility.Features
{
    /// <summary>
    /// Marker interface for <see cref="INiisFeatureProvider"/>
    /// implementations.
    /// </summary>
    public interface INiisFeatureProvider
    {
    }

    /// <summary>
    /// A provider for a given <typeparamref name="TService"/> service.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    public interface INiisFeatureProvider<TService> : INiisFeatureProvider
    {
        /// <summary>
        /// Updates the <paramref name="service"/> instance.
        /// </summary>
        /// <param name="parts">The list of <see cref="NI2SPart"/> instances in the engine.
        /// </param>
        /// <param name="service">The module instance to populate.</param>
        /// <remarks>
        /// <see cref="NI2SPart"/> instances in <paramref name="parts"/> appear in the same ordered sequence they
        /// are stored in <see cref="INiisPartManager.EngineParts"/>. This ordering may be used by the module
        /// provider to make precedence decisions.
        /// </remarks>
        void PopulateFeature(IEnumerable<NI2SPart> parts, TService service);
    }
}