#if LINUX
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace ARWNI2S.Platform
{
    public static partial class NativeMethods
    {
        private static long _cpuFrequencyHz = GetCpuFrequencyHz();

        public static bool QueryPerformanceFrequency(out long lpFrequency)
        {
            // Devolvemos la frecuencia base del procesador
            lpFrequency = _cpuFrequencyHz;
            return lpFrequency > 0;
        }

        public static bool QueryPerformanceCounter(out long lpCounter)
        {
            try
            {
                // Usamos rdtsc si la frecuencia es válida
                if (_cpuFrequencyHz > 0)
                {
                    lpCounter = ReadRdtsc() / (_cpuFrequencyHz / 1_000_000_000); // Convertimos a nanosegundos
                    return true;
                }
                else
                {
                    // Alternativa: clock_gettime
                    var timespec = new Timespec();
                    if (clock_gettime(ClockId.CLOCK_MONOTONIC, ref timespec) == 0)
                    {
                        lpCounter = timespec.tv_sec * 1_000_000_000 + timespec.tv_nsec;
                        return true;
                    }
                }
                lpCounter = 0;
                return false;
            }
            catch
            {
                lpCounter = 0;
                return false;
            }
        }

        private static long GetCpuFrequencyHz()
        {
            try
            {
                string cpuInfo = File.ReadAllText("/proc/cpuinfo");
                foreach (string line in cpuInfo.Split('\n'))
                {
                    if (line.StartsWith("cpu MHz"))
                    {
                        // Extraemos frecuencia en MHz y convertimos a Hz
                        string mhzValue = line.Split(':')[1].Trim();
                        return (long)(double.Parse(mhzValue) * 1_000_000);
                    }
                }
            }
            catch
            {
                // Fallback: Frecuencia desconocida
            }
            return 0;
        }

        private static ulong ReadRdtsc()
        {
            // Lectura del contador rdtsc utilizando ensamblador en x86-64
            ulong high, low;
            __asm__ __volatile__("rdtsc" : "=a"(low), "=d"(high));
            return (high << 32) | low;
        }

        [DllImport("libc", EntryPoint = "clock_gettime", SetLastError = true)]
        private static extern int clock_gettime(ClockId clkId, ref Timespec tp);

        private enum ClockId
        {
            CLOCK_MONOTONIC = 1
        }

        private struct Timespec
        {
            public long tv_sec;  // Segundos
            public long tv_nsec; // Nanosegundos
        }
    }
}
#endif