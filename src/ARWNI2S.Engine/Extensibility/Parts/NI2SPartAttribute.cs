namespace ARWNI2S.Extensibility.Parts
{
    /// <summary>
    /// Specifies an assembly to be added as an <see cref="NI2SPart" />.
    /// <para>
    /// In the ordinary case, NI2S will generate <see cref="NI2SPartAttribute" />
    /// instances on the entry assembly for each dependency that references MVC.
    /// Each of these assemblies is treated as an <see cref="NI2SPart" />.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public sealed class NI2SPartAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NI2SPartAttribute" />.
        /// </summary>
        /// <param name="assemblyName">The assembly name.</param>
        public NI2SPartAttribute(string assemblyName)
        {
            AssemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
        }

        /// <summary>
        /// Gets the assembly name.
        /// </summary>
        public string AssemblyName { get; }
    }
}