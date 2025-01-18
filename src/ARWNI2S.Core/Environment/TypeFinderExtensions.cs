using ARWNI2S.Extensibility;

namespace ARWNI2S.Environment
{
    internal static class TypeFinderExtensions
    {
        public static IList<T> FindAndInitializeInstancess<T>(this ITypeFinder typeFinder)
        {
            //find, create and sort instances of startup configurations
            var startupConfigurations = typeFinder.FindClassesOfType<T>();

            return startupConfigurations
                .Select(startup => (T)Activator.CreateInstance(startup))
                .Where(startup => startup != null)
                .ToList();
        }

        public static IList<IConfigureStartup> GetStartupConfigurationInstancess(this ITypeFinder typeFinder)
        {
            //create and sort instances of startup configurations
            return typeFinder
                .FindAndInitializeInstancess<IConfigureStartup>()
                .OrderBy(startup => startup.Order).ToList();
        }
    }
}
