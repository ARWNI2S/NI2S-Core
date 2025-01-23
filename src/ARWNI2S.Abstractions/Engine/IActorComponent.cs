namespace ARWNI2S.Engine
{
    internal interface IActorComponent : INiisObject
    {
        INiisActor Owner { get; }
        IActorComponent Parent { get; }
        IEnumerable<IActorComponent> Children { get; }
    }
}
