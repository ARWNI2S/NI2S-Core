using ARWNI2S.Engine.EngineParts;

namespace ARWNI2S.Core.Builder
{
    public interface INiisCoreBuilder
    {
        IEnginePartManager PartManager { get; }
    }
}