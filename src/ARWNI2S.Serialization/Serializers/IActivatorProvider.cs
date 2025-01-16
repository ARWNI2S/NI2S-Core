using ARWNI2S.Serialization.Activators;

namespace ARWNI2S.Serialization.Serializers
{
    /// <summary>
    /// Provides activators.
    /// </summary>
    public interface IActivatorProvider
    {
        /// <summary>
        /// Gets an activator for the specified type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The activator.</returns>
        IActivator<T> GetActivator<T>();
    }
}