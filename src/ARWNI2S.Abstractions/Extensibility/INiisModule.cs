namespace ARWNI2S.Extensibility
{
    public interface INiisModule : IDescriptor
    {
        IList<string> ModuleDependencies { get; }
    }
}