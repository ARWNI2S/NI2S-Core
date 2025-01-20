using ARWNI2S.Engine.Actor.Components;

namespace ARWNI2S.Engine.Actor
{
    public abstract class NI2SActor : ActorBaseObject, INiisActor
    {
        public ComponentTree Components { get; }

        IEnumerable<IActorComponent> INiisActor.Components => Components;

        protected abstract Task OnActivateAsync(CancellationToken cancellationToken);
        protected abstract Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken);

        Task INiisActor.OnActivateAsync(CancellationToken cancellationToken) => OnActivateAsync(cancellationToken);
        Task INiisActor.OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken) => OnDeactivateAsync(reason, cancellationToken);
    }
}
