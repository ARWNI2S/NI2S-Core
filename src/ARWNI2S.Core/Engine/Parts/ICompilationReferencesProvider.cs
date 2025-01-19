using ARWNI2S.Engine.EngineParts;

namespace ARWNI2S.Engine.Parts
{
    /// <summary>
    /// Exposes one or more reference paths from an <see cref="EnginePart"/>.
    /// </summary>
    public interface ICompilationReferencesProvider
    {
        /// <summary>
        /// Gets reference paths used to perform runtime compilation.
        /// </summary>
        IEnumerable<string> GetReferencePaths();
    }
}
