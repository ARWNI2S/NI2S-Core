using ARWNI2S.Engine.Features;

namespace ARWNI2S.Engine.EngineParts
{
    public interface IEnginePartManager
    {
        IList<IEngineFeatureProvider> FeatureProviders { get; }

        IList<EnginePart> EngineParts { get; }

        void PopulateFeature<TFeature>(TFeature feature);

        void PopulateDefaultParts(string entryAssemblyName);
    }
}