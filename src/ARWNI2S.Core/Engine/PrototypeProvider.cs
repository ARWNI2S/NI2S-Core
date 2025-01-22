using ARWNI2S.Engine.Object;
using ARWNI2S.Environment;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;

namespace ARWNI2S.Engine
{
    internal class PrototypeProvider
    {
        private readonly ConcurrentDictionary<Name, NI2SObject> _prototypeInstances;
        private readonly ITypeFinder _typeFinder;

        public PrototypeProvider(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;

            _prototypeInstances = new ConcurrentDictionary<Name, NI2SObject>();
        }

        internal protected void Initialize()
        {
            try
            {
                // Obtén los tipos válidos directamente con LINQ
                var instances = _typeFinder.FindClassesOfType<NI2SObject>()
                    .Where(type => type.IsClass && !type.IsAbstract)
                    .Where(type => type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Length != 0)
                    .Select(type =>
                    {
                        try
                        {
                            // Usa el primer constructor público
                            var constructor = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).First();
                            var parameters = constructor.GetParameters();

                            // Crea valores predeterminados para los parámetros del constructor
                            var args = parameters.Select(p => GetDefaultValue(p.ParameterType)).ToArray();
                            //object[] args = null;
                            return (type, instance: constructor.Invoke(args) as NI2SObject);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Error creating instance of {type.FullName}: {ex.Message}");
                            return (type, instance: null);
                        }
                    })
                    .Where(result => result.instance != null); // Filtra las instancias válidas

                // Agrega las instancias al diccionario
                foreach (var (type, instance) in instances)
                {
                    var typeName = new Name(type.FullName ?? type.Name);
                    _prototypeInstances.TryAdd(typeName, instance);
                }
            }
            catch (Exception e)
            {
                // Maneja excepciones generales de inicialización
                throw new EngineInitializationException(e);
            }

            object GetDefaultValue(Type parameterType)
            {
                return parameterType.IsClass ? null : default;
            }

        }

    }
}
