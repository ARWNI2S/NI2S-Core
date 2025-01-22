using ARWNI2S.Engine.Object;

namespace ARWNI2S.Engine
{
    internal interface IObjectDirectory
    {
        INiisObject GetPrototype(Name name);
        TObject GetObject<TObject>(ObjectId objectId);
    }
}
