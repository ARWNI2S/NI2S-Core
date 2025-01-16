using ARWNI2S.Serialization.Buffers;
using ARWNI2S.Serialization.Codecs;
using ARWNI2S.Serialization.WireProtocol;
using Google.Protobuf;

namespace ARWNI2S.Serialization.Protobuf
{
    /// <summary>
    /// Serializer for <see cref="ByteString"/>.
    /// </summary>
    [RegisterSerializer]
    public sealed class ByteStringCodec : IFieldCodec<ByteString>
    {
        /// <inheritdoc/>
        ByteString IFieldCodec<ByteString>.ReadValue<TInput>(ref Reader<TInput> reader, Field field)
        {
            if (field.WireType == WireType.Reference)
            {
                return ReferenceCodec.ReadReference<ByteString, TInput>(ref reader, field);
            }

            field.EnsureWireType(WireType.LengthPrefixed);
            var length = reader.ReadVarUInt32();
            var result = UnsafeByteOperations.UnsafeWrap(reader.ReadBytes(length));
            ReferenceCodec.RecordObject(reader.Session, result);
            return result;
        }

        /// <inheritdoc/>
        void IFieldCodec<ByteString>.WriteField<TBufferWriter>(ref Writer<TBufferWriter> writer, uint fieldIdDelta, Type expectedType, ByteString value)
        {
            if (ReferenceCodec.TryWriteReferenceField(ref writer, fieldIdDelta, expectedType, value))
            {
                return;
            }

            writer.WriteFieldHeader(fieldIdDelta, expectedType, typeof(ByteString), WireType.LengthPrefixed);
            writer.WriteVarUInt32((uint)value.Length);
            writer.Write(value.Span);
        }
    }
}