using ARWNI2S.Context;

namespace ARWNI2S.Engine.Object
{
    public abstract class NI2SObject : ObjectBase, INiisObject
    {
        public static T New<T>() where T : INiisObject
        {
            // Resolver la factoría específica o usar la predeterminada
            var factory = NI2SContext.Current.Resolve<IObjectFactory<T>>();

            // Crear la instancia con el tipo concreto
            return factory is IObjectFactory<T> specificFactory
                ? specificFactory.CreateInstance()
                : factory.CreateInstance<T>();
        }
    }
}
