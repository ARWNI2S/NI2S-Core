using System.Runtime.InteropServices;

namespace ARWNI2S.Core
{
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct EntityId
    {
        [FieldOffset(0)]
        private readonly int _identity;

        public static readonly EntityId Null = new(0);

        private EntityId(int value)
        {
            _identity = value;
        }

        //public readonly int Value => _identity;

        public readonly bool IsNull => _identity == 0;

        // Implicit conversion
        public static implicit operator int(EntityId id) => id._identity;
        public static implicit operator uint(EntityId id) => unchecked((uint)id._identity);
        public static implicit operator long(EntityId id) => (long)id._identity << 32;
        public static implicit operator ulong(EntityId id) => (ulong)(uint)id._identity << 32;
        public static implicit operator byte[](EntityId id) => BitConverter.GetBytes(id._identity);
        public static implicit operator bool[](EntityId id)
        {
            var bits = new bool[32];
            for (int i = 0; i < 32; i++)
                bits[i] = (id._identity & 1 << i) != 0;
            return bits;
        }
        public static implicit operator string(EntityId id) => $"0x{id._identity:X8}";

        // Explicit conversion
        public static explicit operator EntityId(int value)
        {
            if (value == 0)
                throw new ArgumentException("EntityId value cannot be zero. Use EntityId.Null instead.");
            return new EntityId(value);
        }
        public static explicit operator EntityId(uint value)
        {
            if (value == 0)
                throw new ArgumentException("EntityId value cannot be zero. Use EntityId.Null instead.");
            return new EntityId((int)value);
        }
        public static explicit operator EntityId(long value)
        {
            int intValue = (int)(value >> 32);
            if (intValue == 0)
                throw new ArgumentException("EntityId value cannot be zero. Use EntityId.Null instead.");
            return new EntityId(intValue);
        }
        public static explicit operator EntityId(ulong value)
        {
            int intValue = (int)(value >> 32);
            if (intValue == 0)
                throw new ArgumentException("EntityId value cannot be zero. Use EntityId.Null instead.");
            return new EntityId(intValue);
        }
        public static explicit operator EntityId(byte[] value)
        {
            if (value == null || value.Length != 4)
                throw new ArgumentException("Invalid byte array length for EntityId. Expected 4 bytes.");
            int intValue = BitConverter.ToInt32(value, 0);
            if (intValue == 0)
                throw new ArgumentException("EntityId value cannot be zero. Use EntityId.Null instead.");
            return new EntityId(intValue);
        }
        public static explicit operator EntityId(bool[] value)
        {
            if (value == null || value.Length != 32)
                throw new ArgumentException("Invalid bool array length for EntityId. Expected 32 booleans.");
            int result = 0;
            for (int i = 0; i < 32; i++)
                if (value[i])
                    result |= 1 << i;
            if (result == 0)
                throw new ArgumentException("EntityId value cannot be zero. Use EntityId.Null instead.");
            return new EntityId(result);
        }
        public static explicit operator EntityId(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("0x"))
                throw new ArgumentException("Invalid string format for EntityId. Expected hexadecimal starting with 0x.");
            if (!int.TryParse(value[2..], System.Globalization.NumberStyles.HexNumber, null, out int parsedValue) || parsedValue == 0)
                throw new ArgumentException("Invalid or zero hexadecimal value for EntityId. Use EntityId.Null instead.");
            return new EntityId(parsedValue);
        }

        // Operators
        public static bool operator ==(EntityId left, EntityId right) => left._identity == right._identity;
        public static bool operator !=(EntityId left, EntityId right) => !(left == right);
        public static bool operator ==(EntityId id, int value) => id._identity == value;
        public static bool operator !=(EntityId id, int value) => !(id == value);
        public static bool operator ==(EntityId id, uint value) => id._identity == unchecked((int)value);
        public static bool operator !=(EntityId id, uint value) => !(id == value);
        public static bool operator ==(EntityId id, long value) => id._identity == (int)(value >> 32);
        public static bool operator !=(EntityId id, long value) => !(id == value);
        public static bool operator ==(EntityId id, ulong value) => id._identity == (int)(value >> 32);
        public static bool operator !=(EntityId id, ulong value) => !(id == value);
        public static bool operator ==(EntityId id, byte[] value)
        {
            if (value == null || value.Length != 4)
                return false;
            return id._identity == BitConverter.ToInt32(value, 0);
        }
        public static bool operator !=(EntityId id, byte[] value) => !(id == value);
        public static bool operator ==(EntityId id, bool[] value)
        {
            if (value == null || value.Length != 32)
                return false;
            int result = 0;
            for (int i = 0; i < 32; i++)
                if (value[i])
                    result |= 1 << i;
            return id._identity == result;
        }
        public static bool operator !=(EntityId id, bool[] value) => !(id == value);
        public static bool operator ==(EntityId id, string value)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.StartsWith("0x"))
                return false;
            if (!int.TryParse(value[2..], System.Globalization.NumberStyles.HexNumber, null, out int parsedValue))
                return false;
            return id._identity == parsedValue;
        }
        public static bool operator !=(EntityId id, string value) => !(id == value);

        // Overrides
        public override bool Equals(object obj) => obj is EntityId other && _identity == other._identity;
        public override int GetHashCode() => _identity.GetHashCode();
        public override string ToString() => $"EntityId(Raw: 0x{_identity:X8})";
    }
}
