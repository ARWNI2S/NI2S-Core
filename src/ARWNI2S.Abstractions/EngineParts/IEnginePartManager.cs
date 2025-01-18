namespace ARWNI2S.EngineParts
{
    public interface IEnginePartManager
    {
        IList<IEngineFeatureProvider> FeatureProviders { get; }

        IList<EnginePart> EngineParts { get; }

        void PopulateFeature<TFeature>(TFeature feature);

        void PopulateDefaultParts(string entryAssemblyName);
    }
}