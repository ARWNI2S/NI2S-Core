namespace ARWNI2S.Extensibility
{

    //public interface IBuildModuleSupported
    //{
    //    IModuleDataSource DataSource { get; }
    //}

    public interface IEngineModule : INiisModule, IConfigureEngine//, ILifecycleParticipant<IEngineLifecycle>
    {
    }
}