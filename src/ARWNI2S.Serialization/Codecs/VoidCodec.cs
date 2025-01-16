using ARWNI2S.Serialization.Buffers;
using ARWNI2S.Serialization.Cloning;
using ARWNI2S.Serialization.WireProtocol;
using System.Buffers;

namespace ARWNI2S.Serialization.Codecs
{
    /// <summary>
    /// Serializer for unknown types.
    /// </summary>
    internal sealed class VoidCodec : IFieldCodec
    {
        /// <inheritdoc />
        public void WriteField<TBufferWriter>(ref Writer<TBufferWriter> writer, uint fieldIdDelta, Type expectedType, object value) where TBufferWriter : IBufferWriter<byte>
        {
            if (!ReferenceCodec.TryWriteReferenceField(ref writer, fieldIdDelta, expectedType, value))
            {
                ThrowNotNullException(value);
            }
        }

        /// <inheritdoc />
        public object ReadValue<TInput>(ref Reader<TInput> reader, Field field)
        {
            field.EnsureWireType(WireType.Reference);
            return ReferenceCodec.ReadReference(ref reader, field.FieldType);
        }

        private static void ThrowNotNullException(object value) => throw new InvalidOperationException(
            $"Expected a value of null, but encountered a value of '{value}'.");
    }

    /// <summary>
    /// Copier for unknown types.
    /// </summary>
    internal sealed class VoidCopier : IDeepCopier
    {
        public object DeepCopy(object input, CopyContext context)
        {
            if (context.TryGetCopy<object>(input, out var result))
            {
                return result;
            }

            ThrowNotNullException(input);
            return null;
        }

        private static void ThrowNotNullException(object value) => throw new InvalidOperationException($"Expected a value of null, but encountered a value of type '{value.GetType()}'.");
    }
}