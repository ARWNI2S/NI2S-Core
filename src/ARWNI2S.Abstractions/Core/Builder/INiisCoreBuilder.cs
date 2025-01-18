using ARWNI2S.EngineParts;

namespace ARWNI2S.Core.Builder
{
    public interface INiisCoreBuilder
    {
        IEnginePartManager PartManager { get; }
    }
}