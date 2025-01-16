using Microsoft.Extensions.DependencyInjection;

namespace ARWNI2S.Serialization
{
    /// <summary>
    /// Builder interface for configuring serialization.
    /// </summary>
    public interface ISerializerBuilder
    {
        /// <summary>
        /// Gets the service collection.
        /// </summary>
        IServiceCollection Services { get; }
    }
}