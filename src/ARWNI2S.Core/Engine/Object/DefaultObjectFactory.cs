namespace ARWNI2S.Engine.Object
{
    internal class DefaultObjectFactory : ObjectFactoryBase, IObjectFactory<NI2SObject>
    {



        protected override TObject CreateInstance<TObject>()
        {
            return (TObject)CreateInstance(typeof(TObject));
        }

        protected override ObjectBase CreateInstance(Type type)
        {
            return (ObjectBase)Activator.CreateInstance(type);
        }

        NI2SObject IObjectFactory<NI2SObject>.CreateInstance()
        {
            throw new InvalidOperationException($"Direct creation of {nameof(NI2SObject)} is not allowed when using the {nameof(DefaultObjectFactory)}, please use the generic {nameof(Create)} method.");
        }
    }
}
