using System.Collections;

namespace ARWNI2S.Extensibility
{
    /// <summary>
    /// Default implementation for <see cref="INiisModuleCollection"/>.
    /// </summary>
    public class NI2SModuleCollection : INiisModuleCollection
    {
        private static readonly KeyComparer ModuleKeyComparer = new();
        private readonly INiisModuleCollection _defaults;
        private readonly int _initialCapacity;
        private IDictionary<Type, INiisModule> _modules;
        private volatile int _containerRevision;

        /// <summary>
        /// Initializes a new instance of <see cref="NI2SModuleCollection"/>.
        /// </summary>
        public NI2SModuleCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NI2SModuleCollection"/> with the specified initial capacity.
        /// </summary>
        /// <param name="initialCapacity">The initial number of elements that the collection can contain.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="initialCapacity"/> is less than 0</exception>
        public NI2SModuleCollection(int initialCapacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(initialCapacity);

            _initialCapacity = initialCapacity;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="NI2SModuleCollection"/> with the specified defaults.
        /// </summary>
        /// <param name="defaults">The module defaults.</param>
        public NI2SModuleCollection(INiisModuleCollection defaults)
        {
            _defaults = defaults;
        }

        /// <inheritdoc />
        public virtual int Revision
        {
            get { return _containerRevision + (_defaults?.Revision ?? 0); }
        }

        /// <inheritdoc />
        public bool IsReadOnly { get { return false; } }

        /// <inheritdoc />
        public INiisModule this[Type key]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(key);

                return _modules != null && _modules.TryGetValue(key, out var result) ? result : _defaults?[key];
            }
            set
            {
                ArgumentNullException.ThrowIfNull(key);

                if (value == null)
                {
                    if (_modules != null && _modules.Remove(key))
                    {
                        _containerRevision++;
                    }
                    return;
                }

                if (_modules == null)
                {
                    _modules = new Dictionary<Type, INiisModule>(_initialCapacity);
                }
                _modules[key] = value;
                _containerRevision++;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Type, INiisModule>> GetEnumerator()
        {
            if (_modules != null)
            {
                foreach (var pair in _modules)
                {
                    yield return pair;
                }
            }

            if (_defaults != null)
            {
                // Don't return modules masked by the wrapper.
                foreach (var pair in _modules == null ? _defaults : _defaults.Except(_modules, ModuleKeyComparer))
                {
                    yield return pair;
                }
            }
        }

        /// <inheritdoc />
        public TModule Get<TModule>() where TModule : class, INiisModule
        {
            if (typeof(TModule).IsValueType)
            {
                var module = this[typeof(TModule)];
                if (module is null && Nullable.GetUnderlyingType(typeof(TModule)) is null)
                {
                    throw new InvalidOperationException(
                        $"{typeof(TModule).FullName} does not exist in the module collection " +
                        $"and because it is a struct the method can't return null. Use 'moduleCollection[typeof({typeof(TModule).FullName})] is not null' to check if the module exists.");
                }
                return (TModule)module;
            }
            return (TModule)this[typeof(TModule)];
        }

        /// <inheritdoc />
        public void Set<TModule>(TModule instance) where TModule : class, INiisModule
        {
            this[typeof(TModule)] = instance;
        }

        // Used by the debugger. Count over enumerable is required to get the correct value.
        private int GetCount() => this.Count();

        private sealed class KeyComparer : IEqualityComparer<KeyValuePair<Type, INiisModule>>
        {
            public bool Equals(KeyValuePair<Type, INiisModule> x, KeyValuePair<Type, INiisModule> y)
            {
                return x.Key.Equals(y.Key);
            }

            public int GetHashCode(KeyValuePair<Type, INiisModule> obj)
            {
                return obj.Key.GetHashCode();
            }
        }
    }
}