namespace ARWNI2S.Extensibility
{
    internal interface INiisModuleManager
    {
        INiisModuleCollection Modules { get; }

        void RegisterModule(INiisModule module);

        INiisModule GetModule(Type type);
        INiisModule FindModule(string name);
    }
}