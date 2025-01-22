using ARWNI2S.Context;
using System.Reflection;

namespace ARWNI2S.Engine.Object
{
    public abstract class NI2SObject : ObjectBase, INiisObject
    {
        public static bool IsValid(NI2SObject prerequisiteObject, bool v)
        {
            throw new NotImplementedException();
        }

        public static TObject New<TObject>() where TObject : NI2SObject
        {
            // Resolver la factoría específica o usar la predeterminada
            var factory = NI2SContext.Current.Resolve<IObjectFactory<TObject>>();

            // Crear la instancia con el tipo concreto
            return factory is IObjectFactory<TObject> specificFactory
                ? specificFactory.CreateInstance()
                : factory.CreateInstance<TObject>();
        }
    }

    public class NI2SObjectProxy : DispatchProxy
    {
        private NI2SObject_Impl _impl;

        public NI2SObjectProxy()
        {
            _impl = new NI2SObject_Impl();
        }

        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            return _impl.GetType().GetMethod(targetMethod.Name).Invoke(_impl, args);
        }

        private class NI2SObject_Impl // : IGreeting - doesn't implement IGreeting but mimics it
        {
            public string Message => "hello world";
        }
    }

    public class GreetingFactory
    {
        public static object Create()
        {
            var internalType = Assembly.Load("Library").GetType("Library.IGreeting");
            return typeof(DispatchProxy).GetMethod(nameof(DispatchProxy.Create)).MakeGenericMethod(internalType, typeof(NI2SObjectProxy)).Invoke(null, null);
        }
    }
}
