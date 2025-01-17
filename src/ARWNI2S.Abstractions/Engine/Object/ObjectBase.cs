using ARWNI2S.Core;

namespace ARWNI2S.Engine.Object
{
    public abstract class ObjectBase : EntityBase, INiisEntity
    {
        internal virtual ObjectId ObjectId { get; }


        internal override EntityId Id => ObjectId.EntityId;

        object INiisEntity.Id => ObjectId;
    }
}
