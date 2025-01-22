using ARWNI2S.Engine.Object;

namespace ARWNI2S.Engine
{
    internal interface IObjectFactory
    {
        TObject CreateInstance<TObject>()
            where TObject : INiisObject;

        INiisObject CreateInstance(Type type);
    }

    internal interface IObjectFactory<TObject> : IObjectFactory
        where TObject : ObjectBase
    {
        TObject CreateInstance();
    }
}