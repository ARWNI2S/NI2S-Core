using Microsoft.Extensions.Logging;

namespace ARWNI2S.Lifecycle
{
    /// <summary>
    /// Implementation of <see cref="INiisLifecycle"/>.
    /// </summary>
    internal class NI2SLifecycle : LifecycleSubject, INiisLifecycle
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NI2SLifecycle"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public NI2SLifecycle(ILogger logger) : base(logger)
        {
        }
    }
}