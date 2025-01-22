using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace ARWNI2S.Diagnostics
{
    public static class LogFormatter
    {
        public const int MAX_LOG_MESSAGE_SIZE = 20000;

        private const string TIME_FORMAT = "HH:mm:ss.fff 'GMT'";

        private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.fff 'GMT'";

        private static readonly ConcurrentDictionary<Type, Func<Exception, string>> exceptionDecoders = new ConcurrentDictionary<Type, Func<Exception, string>>();

        //
        // Resumen:
        //     Utility function to convert a DateTime object into printable data format used
        //     by the Logger subsystem.
        //
        // Parámetros:
        //   date:
        //     The DateTime value to be printed.
        //
        // Devuelve:
        //     Formatted string representation of the input data, in the printable format used
        //     by the Logger subsystem.
        public static string PrintDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss.fff 'GMT'", CultureInfo.InvariantCulture);
        }

        //
        // Resumen:
        //     Parses a date.
        //
        // Parámetros:
        //   dateStr:
        //     The date string.
        //
        // Devuelve:
        //     The parsed date.
        public static DateTime ParseDate(string dateStr)
        {
            return DateTime.ParseExact(dateStr, "yyyy-MM-dd HH:mm:ss.fff 'GMT'", CultureInfo.InvariantCulture);
        }

        //
        // Resumen:
        //     Utility function to convert a DateTime object into printable time format used
        //     by the Logger subsystem.
        //
        // Parámetros:
        //   date:
        //     The DateTime value to be printed.
        //
        // Devuelve:
        //     Formatted string representation of the input data, in the printable format used
        //     by the Logger subsystem.
        public static string PrintTime(DateTime date)
        {
            return date.ToString("HH:mm:ss.fff 'GMT'", CultureInfo.InvariantCulture);
        }

        //
        // Resumen:
        //     Utility function to convert an exception into printable format, including expanding
        //     and formatting any nested sub-expressions.
        //
        // Parámetros:
        //   exception:
        //     The exception to be printed.
        //
        // Devuelve:
        //     Formatted string representation of the exception, including expanding and formatting
        //     any nested sub-expressions.
        public static string PrintException(Exception exception)
        {
            if (exception == null)
            {
                return "";
            }

            StringBuilder stringBuilder = new StringBuilder();
            PrintException_Helper(stringBuilder, exception, 0);
            return stringBuilder.ToString();
        }

        //
        // Resumen:
        //     Configures the exception decoder for the specified exception type.
        //
        // Parámetros:
        //   exceptionType:
        //     The exception type to configure a decoder for.
        //
        //   decoder:
        //     The decoder.
        public static void SetExceptionDecoder(Type exceptionType, Func<Exception, string> decoder)
        {
            exceptionDecoders.TryAdd(exceptionType, decoder);
        }

        private static void PrintException_Helper(StringBuilder sb, Exception exception, int level)
        {
            if (exception == null)
            {
                return;
            }

            Func<Exception, string> value;
            string value2 = exceptionDecoders.TryGetValue(exception.GetType(), out value) ? value(exception) : exception.Message;
            StringBuilder stringBuilder = sb;
            StringBuilder stringBuilder2 = stringBuilder;
            StringBuilder.AppendInterpolatedStringHandler handler = new StringBuilder.AppendInterpolatedStringHandler(14, 4, stringBuilder);
            handler.AppendFormatted(System.Environment.NewLine);
            handler.AppendLiteral("Exc level ");
            handler.AppendFormatted(level);
            handler.AppendLiteral(": ");
            handler.AppendFormatted(exception.GetType());
            handler.AppendLiteral(": ");
            handler.AppendFormatted(value2);
            stringBuilder2.Append(ref handler);
            string stackTrace = exception.StackTrace;
            if (stackTrace != null)
            {
                stringBuilder = sb;
                StringBuilder stringBuilder3 = stringBuilder;
                handler = new StringBuilder.AppendInterpolatedStringHandler(0, 2, stringBuilder);
                handler.AppendFormatted(System.Environment.NewLine);
                handler.AppendFormatted(stackTrace);
                stringBuilder3.Append(ref handler);
            }

            if (exception is ReflectionTypeLoadException ex)
            {
                Exception[] loaderExceptions = ex.LoaderExceptions;
                if (loaderExceptions == null || loaderExceptions.Length == 0)
                {
                    sb.Append("No LoaderExceptions found");
                    return;
                }

                Exception[] array = loaderExceptions;
                foreach (Exception exception2 in array)
                {
                    PrintException_Helper(sb, exception2, level + 1);
                }
            }
            else
            {
                if (exception.InnerException == null)
                {
                    return;
                }

                if (exception is AggregateException ex2)
                {
                    ReadOnlyCollection<Exception> innerExceptions = ex2.InnerExceptions;
                    if (innerExceptions != null && innerExceptions.Count > 1)
                    {
                        foreach (Exception item in innerExceptions)
                        {
                            PrintException_Helper(sb, item, level + 1);
                        }

                        return;
                    }
                }

                PrintException_Helper(sb, exception.InnerException, level + 1);
            }
        }
    }
}