using System.Runtime.CompilerServices;

namespace ARWNI2S.Context
{
    /// <summary>
    /// Provides access to the singleton instance of the node engine context.
    /// </summary>
    public sealed class NI2SContext
    {
        #region Methods

        /// <summary>
        /// Create a static instance of the ARWNI2S engine.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static INiisContext Create()
        {
            //create ServerEngine as engine
            return Singleton<INiisContext>.Instance ?? (Singleton<INiisContext>.Instance = new DefaultNI2SContext());
        }

        /// <summary>
        /// Sets the static engine context instance to the supplied engine. Use this method to supply your own engine context implementation.
        /// </summary>
        /// <param name="context">The engine context to use.</param>
        /// <remarks>Only use this method if you know what you're doing.</remarks>
        public static void Replace(INiisContext context)
        {
            Singleton<INiisContext>.Instance = context;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the singleton ARWNI2S engine used to access ARWNI2S services.
        /// </summary>
        public static INiisContext Current
        {
            get
            {
                if (Singleton<INiisContext>.Instance == null)
                {
                    Create();
                }

                return Singleton<INiisContext>.Instance;
            }
        }

        #endregion
    }
}
