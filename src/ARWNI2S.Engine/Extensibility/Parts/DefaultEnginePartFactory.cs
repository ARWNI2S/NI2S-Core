using System.Reflection;

namespace ARWNI2S.Extensibility.Parts
{
    /// <summary>
    /// Default <see cref="EnginePartFactory"/>.
    /// </summary>
    public class DefaultEnginePartFactory : EnginePartFactory
    {
        /// <summary>
        /// Gets an instance of <see cref="DefaultEnginePartFactory"/>.
        /// </summary>
        public static DefaultEnginePartFactory Instance { get; } = new DefaultEnginePartFactory();

        /// <summary>
        /// Gets the sequence of <see cref="NI2SPart"/> instances that are created by this instance of <see cref="DefaultEnginePartFactory"/>.
        /// <para>
        /// Engines may use this method to get the same behavior as this factory produces during MVC's default part discovery.
        /// </para>
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <returns>The sequence of <see cref="NI2SPart"/> instances.</returns>
        public static IEnumerable<NI2SPart> GetDefaultEngineParts(Assembly assembly)
        {
            ArgumentNullException.ThrowIfNull(assembly);

            yield return new AssemblyPart(assembly);
        }

        /// <inheritdoc />
        public override IEnumerable<NI2SPart> GetEngineParts(Assembly assembly)
        {
            return GetDefaultEngineParts(assembly);
        }
    }
}