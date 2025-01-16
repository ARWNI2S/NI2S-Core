using ARWNI2S.Extensibility.Parts;

namespace ARWNI2S.Core.Builder
{
    public interface INiisCoreBuilder
    {
        INiisPartManager PartManager { get; }
    }
}