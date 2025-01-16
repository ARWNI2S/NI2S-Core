using ARWNI2S.Extensibility.Features;

namespace ARWNI2S.Extensibility.Parts
{
    public interface INiisPartManager
    {
        IList<INiisFeatureProvider> ServiceProviders { get; }

        IList<NI2SPart> EngineParts { get; }

        void PopulateFeature<TFeature>(TFeature feature);

        void PopulateDefaultParts(string entryAssemblyName);
    }
}