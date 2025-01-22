using ARWNI2S.Diagnostics;
using ARWNI2S.Serialization.TypeSystem;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

namespace ARWNI2S
{
    /// <summary>
    /// An exception class used by the NI2S runtime for reporting errors.
    /// </summary>
    /// <remarks>
    /// This is also the base class for any more specific exceptions 
    /// raised by the NI2S runtime.
    /// </remarks>
    [Serializable]
    [GenerateSerializer]
    public class NI2SException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NI2SException"/> class.
        /// </summary>
        public NI2SException()
            : base("Unexpected error.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NI2SException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public NI2SException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NI2SException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public NI2SException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NI2SException"/> class.
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
        protected NI2SException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    [GenerateSerializer]
    public class WrappedException : NI2SException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public WrappedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedException"/> class.
        /// </summary>
        /// <param name="info">
        /// The serialization info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        /// <exception cref="SerializationException">The class name is <see langword="null" /> or <see cref="P:System.Exception.HResult" /> is zero (0).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="info" /> is <see langword="null" />.</exception>
        [Obsolete("Legacy")]
        protected WrappedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            OriginalExceptionType = info.GetString(nameof(OriginalExceptionType));
        }

        /// <summary>
        /// Gets or sets the type of the original exception.
        /// </summary>
        //[Id(0)]
        public string OriginalExceptionType { get; set; }

        /// <inheritdoc/>
        [Obsolete]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(OriginalExceptionType), OriginalExceptionType);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="WrappedException"/> class and rethrows it using the provided exception's stack trace.
        /// </summary>
        /// <param name="exception">The exception.</param>
        [DoesNotReturn]
        public static void CreateAndRethrow(Exception exception)
        {
            var error = exception switch
            {
                WrappedException => exception,
                { } => CreateFromException(exception),
                null => throw new ArgumentNullException(nameof(exception))
            };

            ExceptionDispatchInfo.Throw(error);
        }

        private static WrappedException CreateFromException(Exception exception)
        {
            var originalExceptionType = RuntimeTypeNameFormatter.Format(exception.GetType());
            var detailedMessage = LogFormatter.PrintException(exception);
            var result = new WrappedException(detailedMessage)
            {
                OriginalExceptionType = originalExceptionType,
            };

            ExceptionDispatchInfo.SetRemoteStackTrace(result, exception.StackTrace);
            return result;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{nameof(WrappedException)} OriginalType: {OriginalExceptionType}, Message: {Message}";
        }
    }

    internal static class ExceptionExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        public static List<Exception> FlattenAggregate(this Exception exc)
        {
            var result = new List<Exception>();
            if (exc is AggregateException)
                result.AddRange(exc.InnerException.FlattenAggregate());
            else
                result.Add(exc);
            return result;
        }

        /// <summary>
        /// Parse a Uri as an IPEndpoint.
        /// </summary>
        /// <param name="uri">The input Uri</param>
        /// <returns></returns>
        public static IPEndPoint ToIPEndPoint(this Uri uri)
        {
            switch (uri.Scheme)
            {
                case "gwy.tcp":
                    return new IPEndPoint(IPAddress.Parse(uri.Host), uri.Port);
            }
            return null;
        }

        /// <summary>
        /// Represent an IP end point in the gateway URI format..
        /// </summary>
        /// <param name="ep">The input IP end point</param>
        /// <returns></returns>
        public static Uri ToGatewayUri(this IPEndPoint ep)
        {
            return new Uri(string.Format("gwy.tcp://{0}:{1}/0", ep.Address, ep.Port));
        }
    }
}