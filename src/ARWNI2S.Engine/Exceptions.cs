using System.Runtime.Serialization;

namespace ARWNI2S
{
    /// <summary>
    /// Indicates a lifecycle was canceled, either by request or due to observer error.
    /// </summary>
    [Serializable]
    [GenerateSerializer]
    public sealed class LifecycleCanceledException : NI2SException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LifecycleCanceledException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        internal LifecycleCanceledException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LifecycleCanceledException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        internal LifecycleCanceledException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LifecycleCanceledException"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="info" /> is <see langword="null" />.</exception>
        [Obsolete]
        private LifecycleCanceledException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    internal class ModuleDependencyException : NI2SException
    {
        public string SystemName { get; }
        public string Dependency { get; }

        public ModuleDependencyException(string systemName, string dependency) : base($"Module {systemName} has a dependency on {dependency} which is not registered.")
        {
            SystemName = systemName;
            Dependency = dependency;
        }
    }
}