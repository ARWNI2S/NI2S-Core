using System.Reflection;

namespace ARWNI2S.Extensibility.Parts
{
    /// <summary>
    /// Exposes a set of types from an <see cref="NI2SPart"/>.
    /// </summary>
    public interface INiisPartTypeProvider
    {
        /// <summary>
        /// Gets the list of available types in the <see cref="NI2SPart"/>.
        /// </summary>
        IEnumerable<TypeInfo> Types { get; }
    }
}