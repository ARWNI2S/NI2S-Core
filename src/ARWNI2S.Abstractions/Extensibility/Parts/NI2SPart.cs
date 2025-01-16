namespace ARWNI2S.Extensibility.Parts
{
    /// <summary>
    /// A part of an NI2S application.
    /// </summary>
    public abstract class NI2SPart
    {
        /// <summary>
        /// Gets the <see cref="NI2SPart"/> name.
        /// </summary>
        public abstract string Name { get; }
    }
}