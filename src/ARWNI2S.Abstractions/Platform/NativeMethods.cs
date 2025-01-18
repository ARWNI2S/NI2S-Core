using System.Runtime.InteropServices;
using System.Security;

namespace ARWNI2S.Platform
{
    public static partial class NativeMethods
    {
        // Fallback para otras plataformas
#if !WINDOWS && !LINUX
        public static bool QueryPerformanceFrequency(out long lpFrequency)
        {
            throw new PlatformNotSupportedException("QueryPerformanceFrequency no está soportado en esta plataforma.");
        }

        public static bool QueryPerformanceCounter(out long lpCounter)
        {
            throw new PlatformNotSupportedException("QueryPerformanceCounter no está soportado en esta plataforma.");
        }
#endif
    }

    public enum MiniDumpType
    {
        // ReSharper disable UnusedMember.Global
        MiniDumpNormal = 0x00000000,
        MiniDumpWithDataSegs = 0x00000001,
        MiniDumpWithFullMemory = 0x00000002,
        MiniDumpWithHandleData = 0x00000004,
        MiniDumpFilterMemory = 0x00000008,
        MiniDumpScanMemory = 0x00000010,
        MiniDumpWithUnloadedModules = 0x00000020,
        MiniDumpWithIndirectlyReferencedMemory = 0x00000040,
        MiniDumpFilterModulePaths = 0x00000080,
        MiniDumpWithProcessThreadData = 0x00000100,
        MiniDumpWithPrivateReadWriteMemory = 0x00000200,
        MiniDumpWithoutOptionalData = 0x00000400,
        MiniDumpWithFullMemoryInfo = 0x00000800,
        MiniDumpWithThreadInfo = 0x00001000,
        MiniDumpWithCodeSegs = 0x00002000,
        MiniDumpWithoutManagedState = 0x00004000,
        // ReSharper restore UnusedMember.Global
    }
}
