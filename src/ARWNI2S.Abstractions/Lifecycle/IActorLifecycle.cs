#nullable enable
using ARWNI2S;
using System.Buffers;

namespace ARWNI2S.Lifecycle
{
    /// <summary>
    /// The observable actor lifecycle.
    /// </summary>
    /// <remarks>
    /// This type is usually used as the generic parameter in <see cref="ILifecycleParticipant{IActorLifecycle}"/> as
    /// a means of participating in the lifecycle stages of a actor activation.
    /// </remarks>
    public interface IActorLifecycle : ILifecycleObservable
    {
        /// <summary>
        /// Registers a actor migration participant.
        /// </summary>
        /// <param name="participant">The participant.</param>
        void AddMigrationParticipant(IActorMigrationParticipant participant);

        /// <summary>
        /// Unregisters a actor migration participant.
        /// </summary>
        /// <param name="participant">The participant.</param>
        void RemoveMigrationParticipant(IActorMigrationParticipant participant);
    }

    public interface IActorMigrationParticipant
    {
        /// <summary>
        /// Called on the original activation when migration is initiated.
        /// The participant can access and update the deflation context.
        /// </summary>
        /// <param name="deflationContext">The deflation context.</param>
        void OnDeflate(IDeflationContext deflationContext);

        /// <summary>
        /// Called on the new activation after a migration.
        /// The participant can restore state from the migration context.
        /// </summary>
        /// <param name="inflationContext">The inflation context.</param>
        void OnInflate(IInflationContext inflationContext);
    }

    /// <summary>
    /// Records the state of a actor activation which is in the process of being deflated for migration to another location.
    /// </summary>
    public interface IDeflationContext
    {
        /// <summary>
        /// Gets the keys in the context.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Adds a sequence of bytes to the deflation context, associating the sequence with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        void AddBytes(string key, ReadOnlySpan<byte> value);

        /// <summary>
        /// Adds a sequence of bytes to the deflation context, associating the sequence with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="valueWriter">A delegate used to write the provided value to the context.</param>
        /// <param name="value">The value to provide to <paramref name="valueWriter"/>.</param>
        void AddBytes<T>(string key, Action<T, IBufferWriter<byte>> valueWriter, T value);

        /// <summary>
        /// Attempts to a value to the deflation context, associated with the provided key, serializing it using <see cref="Serialization.Serializer"/>.
        /// If a serializer is found for the value, and the key has not already been added, then the value is added and the method returns <see langword="true"/>.
        /// If no serializer exists or the key has already been added, then the value is not added and the method returns <see langword="false"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value to add.</param>
        bool TryAddValue<T>(string key, T? value);
    }

    /// <summary>
    /// Contains the state of a actor activation which is in the process of being inflated after moving from another location.
    /// </summary>
    public interface IInflationContext
    {
        /// <summary>
        /// Gets the keys in the context.
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// Tries to get a sequence of bytes from the inflation context, associated with the provided key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value, if present.</param>
        /// <returns><see langword="true"/> if the key exists in the context, otherwise <see langword="false"/>.</returns>
        bool TryGetBytes(string key, out ReadOnlySequence<byte> value);

        /// <summary>
        /// Tries to get a value from the inflation context, associated with the provided key, deserializing it using <see cref="Serialization.Serializer"/>.
        /// If a serializer is found for the value, and the key is present, then the value is deserialized and the method returns <see langword="true"/>.
        /// If no serializer exists or the key has already been added, then the value is not added and the method returns <see langword="false"/>.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value, if present.</param>
        /// <returns><see langword="true"/> if the key exists in the context, otherwise <see langword="false"/>.</returns>
        bool TryGetValue<T>(string key, out T? value);
    }
}
