#if WINDOWS
using System.Runtime.InteropServices;
using System.Security;

namespace ARWNI2S.Platform
{
    /// <summary>
    /// Encapsula las funciones externas, dependientes de Win32.
    /// </summary>
    public static partial class NativeMethods
    {
        /// <summary>
        /// Query processor performance frequency.
        /// </summary>
        /// <param name="lpFrequency">referenced performance frequency as long.</param>
        /// <returns>true if operation was successful.</returns>
        [LibraryImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity]
        public static partial bool QueryPerformanceFrequency(out long lpFrequency);
        /// <summary>
        /// Query processor performance frequency.
        /// </summary>
        /// <param name="lpFrequency">referenced performance frequency as ulong.</param>
        /// <returns>true if operation was successful.</returns>
        [LibraryImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity]
        public static partial bool QueryPerformanceFrequency(out ulong lpFrequency);

        /// <summary>
        /// Query processor performance counter.
        /// </summary>
        /// <param name="lpCounter">referenced performance counter as long</param>
        /// <returns>true if operation was successful.</returns>
        [LibraryImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity]
        public static partial bool QueryPerformanceCounter(out long lpCounter);
        /// <summary>
        /// Query processor performance counter.
        /// </summary>
        /// <param name="lpCounter">referenced performance counter as ulong</param>
        /// <returns>true if operation was successful.</returns>
        [LibraryImport("kernel32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity]
        public static partial bool QueryPerformanceCounter(out ulong lpCounter);

        /// <summary>
        /// Retrieves the cycle time for the specified thread.
        /// </summary>
        /// <param name="threadHandle">A handle to the thread. The handle must have the PROCESS_QUERY_INFORMATION or PROCESS_QUERY_LIMITED_INFORMATION access right.</param>
        /// <param name="cycleTime">The number of CPU clock cycles used by the thread. This value includes cycles spent in both user mode and kernel mode.</param>
        /// <returns></returns>
        [LibraryImport("Kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SuppressUnmanagedCodeSecurity]
        public static partial bool QueryThreadCycleTime(IntPtr threadHandle, out ulong cycleTime);

        /// <summary>
        /// Retrieves a pseudo handle for the calling thread.
        /// </summary>
        /// <returns>A pseudo handle for the current thread.</returns>
        [LibraryImport("Kernel32.dll")]
        [SuppressUnmanagedCodeSecurity]
        public static partial IntPtr GetCurrentThread();
    }
}
#endif