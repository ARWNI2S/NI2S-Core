namespace ARWNI2S.Engine.Object
{
    public abstract class ObjectFactoryBase : IObjectFactory
    {
        protected ObjectFactoryBase() { }

        public virtual TObject Create<TObject>() where TObject : ObjectBase
        {
            var newObj = CreateInstance<TObject>();

            return newObj;
        }

        protected abstract TObject CreateInstance<TObject>() where TObject : ObjectBase;

        protected abstract ObjectBase CreateInstance(Type type);

        TObject IObjectFactory.CreateInstance<TObject>() => (TObject)((IObjectFactory)this).CreateInstance(typeof(TObject));
        INiisObject IObjectFactory.CreateInstance(Type type) => (INiisObject)CreateInstance(type);
    }

}