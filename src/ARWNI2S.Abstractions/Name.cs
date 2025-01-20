using System.Text.RegularExpressions;

namespace ARWNI2S
{
    public sealed partial class Name
    {
        private static readonly HashSet<string> Registry = new(StringComparer.InvariantCultureIgnoreCase);
        private static readonly Regex NameValidationRegex = GetNameValidationRegex();

        private readonly string _value;

        // Static property for the "None" instance
        public static readonly Name None = new();

        // Private constructor for None
        private Name()
        {
            _value = string.Empty;
        }

        // Public constructor for normal usage
        public Name(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name cannot be null, empty, or whitespace.", nameof(value));

            if (!NameValidationRegex.IsMatch(value))
                throw new ArgumentException("Name must be alphanumeric, can contain underscores, and be 1-128 characters long.", nameof(value));

            lock (Registry)
            {
                if (!Registry.Add(value))
                    throw new InvalidOperationException($"The name '{value}' already exists.");
            }

            _value = value;
        }

        // Overload equality operators
        public static bool operator ==(Name left, Name right)
        {
            return ReferenceEquals(left, right);
        }

        public static bool operator !=(Name left, Name right)
        {
            return !(left == right);
        }

        // Override Equals and GetHashCode
        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode(StringComparison.Ordinal);
        }

        // Override ToString to return a safe representation (empty for "None", placeholder otherwise)
        public override string ToString()
        {
            return this == None ? "None" : "Name Instance";
        }

        [GeneratedRegex("^[a-zA-Z0-9_]{1,128}$", RegexOptions.Compiled)]
        private static partial Regex GetNameValidationRegex();
    }
}
