namespace ARWNI2S.Engine
{
    public interface IActorComponent : INiisObject
    {
        INiisActor Owner { get; }
        IActorComponent Parent { get; }
        IEnumerable<IActorComponent> Children { get; }
    }
}
