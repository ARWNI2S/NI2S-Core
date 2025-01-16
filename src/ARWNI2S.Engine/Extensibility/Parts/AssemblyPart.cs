using System.Reflection;

namespace ARWNI2S.Extensibility.Parts
{
    /// <summary>
    /// An <see cref="NI2SPart"/> backed by an <see cref="System.Reflection.Assembly"/>.
    /// </summary>
    public class AssemblyPart : NI2SPart, INiisPartTypeProvider
    {
        /// <summary>
        /// Initializes a new <see cref="AssemblyPart"/> instance.
        /// </summary>
        /// <param name="assembly">The backing <see cref="System.Reflection.Assembly"/>.</param>
        public AssemblyPart(Assembly assembly)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
        }

        /// <summary>
        /// Gets the <see cref="Assembly"/> of the <see cref="NI2SPart"/>.
        /// </summary>
        public Assembly Assembly { get; }

        /// <summary>
        /// Gets the name of the <see cref="NI2SPart"/>.
        /// </summary>
        public override string Name => Assembly.GetName().Name!;

        /// <inheritdoc />
        public IEnumerable<TypeInfo> Types => Assembly.DefinedTypes;
    }
}