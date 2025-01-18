using System.Runtime.InteropServices;
using System.Security;

namespace ARWNI2S.Platform
{
    /// <summary>
    /// Encapsula las funciones externas, dependientes de Win32.
    /// </summary>
    public static partial class NativeMethods
    {
#if WINDOWS
        [DllImport("Dbghelp.dll")]
        [SuppressUnmanagedCodeSecurity()]
        public static extern bool MiniDumpWriteDump(
            IntPtr hProcess,
            int processId,
            IntPtr hFile,
            MiniDumpType dumpType,
            IntPtr exceptionParam,
            IntPtr userStreamParam,
            IntPtr callbackParam);
#elif LINUX
        public static bool MiniDumpWriteDump(
            IntPtr hProcess,
            int processId,
            IntPtr hFile,
            MiniDumpType dumpType,
            IntPtr exceptionParam,
            IntPtr userStreamParam,
            IntPtr callbackParam)
        {
            try
            {
                // Usa gcore para generar el volcado
                string dumpFilePath = "/tmp/core_" + processId;
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "gcore",
                        Arguments = $"{processId} -o {dumpFilePath}",
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                    }
                };
                process.Start();
                process.WaitForExit();

                // Verifica si se generó el archivo correctamente
                return process.ExitCode == 0;
            }
            catch
            {
                return false;
            }
        }
#else
        public static bool MiniDumpWriteDump(
            IntPtr hProcess,
            int processId,
            IntPtr hFile,
            MiniDumpType dumpType,
            IntPtr exceptionParam,
            IntPtr userStreamParam,
            IntPtr callbackParam)
        {
            throw new PlatformNotSupportedException("MiniDumpWriteDump no está soportado en esta plataforma.");
        }
#endif
    }
}
