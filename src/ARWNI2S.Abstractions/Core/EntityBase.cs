namespace ARWNI2S.Core
{
    public abstract class EntityBase : INiisEntity
    {
        internal virtual EntityId Id { get; set; }

        object INiisEntity.Id => Id;
    }
}
