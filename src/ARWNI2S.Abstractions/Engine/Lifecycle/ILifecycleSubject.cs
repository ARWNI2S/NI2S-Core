namespace ARWNI2S.Engine.Lifecycle
{
    /// <summary>
    /// Both a lifecycle observer and observable lifecycle.
    /// </summary>
    public interface ILifecycleSubject : ILifecycleObservable, ILifecycleObserver
    {
    }
}
