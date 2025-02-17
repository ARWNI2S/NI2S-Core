using System.Collections;
using System.Resources;

namespace ARWNI2S.Collections
{
    /// <summary>
    /// Represents a collection of key-and-value pairs.
    /// </summary>
    /// <remarks>
    /// The SkipList class is an implementation of the IDictionary interface. It 
    /// is based on the data structure created by William Pugh.
    /// </remarks> 
    public class SkipList : IDictionary
    {
        #region SkipList Members

        #region Constants

        // Maximum level any node in a skip list can have
        private const int MaxLevel = 32;

        // Probability factor used to determine the node level
        private const double Probability = 0.5;

        #endregion 

        #region Fields

        // The skip list header. It also serves as the NIL node.
        private Node header = new(MaxLevel);

        // Comparer for comparing keys.
        private IComparer comparer;

        // Random number generator for generating random node levels.
        private Random random = new();

        // Current maximum list level.
        private int listLevel;

        // Current number of elements in the skip list.
        private int count;

        // Version of the skip list. Used for validation checks with 
        // enumerators.
        private long version = 0;

        // Resource manager for retrieving exception messages.
        private ResourceManager resManager;

        #endregion

        /// <summary>
        /// Initializes a new instance of the SkipList class that is empty and 
        /// is sorted according to the IComparable interface implemented by 
        /// each key added to the SkipList.
        /// </summary>
        /// <remarks>
        /// Each key must implement the IComparable interface to be capable of 
        /// comparisons with every other key in the SortedList. The elements 
        /// are sorted according to the IComparable implementation of each key 
        /// added to the SkipList.
        /// </remarks>
        public SkipList()
        {
            // Initialize the skip list.
            Initialize();

            // Load resources.
            resManager =
                new ResourceManager("LSCollections.Resource",
                GetType().Assembly);
        }

        /// <summary>
        /// Initializes a new instance of the SkipList class that is empty and 
        /// is sorted according to the specified IComparer interface.
        /// </summary>
        /// <param name="comparer">
        /// The IComparer implementation to use when comparing keys. 
        /// </param>
        /// <remarks>
        /// The elements are sorted according to the specified IComparer 
        /// implementation. If comparer is a null reference, the IComparable 
        /// implementation of each key is used; therefore, each key must 
        /// implement the IComparable interface to be capable of comparisons 
        /// with every other key in the SkipList.
        /// </remarks>
        public SkipList(IComparer comparer)
        {
            // Initialize comparer with the client provided comparer.
            this.comparer = comparer;

            // Initialize the skip list.
            Initialize();

            // Load resources.
            resManager =
                new ResourceManager("LSCollections.Resource",
                GetType().Assembly);
        }

        ~SkipList()
        {
            Clear();
        }

        #region Private Helper Methods

        /// <summary>
        /// Initializes the SkipList.
        /// </summary>
        private void Initialize()
        {
            listLevel = 1;
            count = 0;

            // When the list is empty, make sure all forward references in the
            // header point back to the header. This is important because the 
            // header is used as the sentinel to mark the end of the skip list.
            for (int i = 0; i < MaxLevel; i++)
            {
                header.forward[i] = header;
            }
        }

        /// <summary>
        /// Returns a level value for a new SkipList node.
        /// </summary>
        /// <returns>
        /// The level value for a new SkipList node.
        /// </returns>
        private int GetNewLevel()
        {
            int level = 1;

            // Determines the next node level.
            while (random.NextDouble() < Probability && level < MaxLevel &&
                level <= listLevel)
            {
                level++;
            }

            return level;
        }

        /// <summary>
        /// Searches for the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to search for.
        /// </param>
        /// <returns>
        /// Returns true if the specified key is in the SkipList.
        /// </returns>
        private bool Search(object key)
        {
            Node curr;
            Node[] dummy = new Node[MaxLevel];

            return Search(key, out curr, dummy);
        }

        /// <summary>
        /// Searches for the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to search for.
        /// </param>
        /// <param name="curr">
        /// A SkipList node to hold the results of the search.
        /// </param>
        /// <returns>
        /// Returns true if the specified key is in the SkipList.
        /// </returns>
        private bool Search(object key, out Node curr)
        {
            Node[] dummy = new Node[MaxLevel];

            return Search(key, out curr, dummy);
        }

        /// <summary>
        /// Searches for the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to search for.
        /// </param>
        /// <param name="update">
        /// An array of nodes holding references to the places in the SkipList
        /// search in which the search dropped down one level.
        /// </param>
        /// <returns>
        /// Returns true if the specified key is in the SkipList.
        /// </returns>
        private bool Search(object key, Node[] update)
        {
            Node curr;

            return Search(key, out curr, update);
        }

        /// <summary>
        /// Searches for the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to search for.
        /// </param>
        /// <param name="curr">
        /// A SkipList node to hold the results of the search.
        /// </param>
        /// <param name="update">
        /// An array of nodes holding references to the places in the SkipList
        /// search in which the search dropped down one level.
        /// </param>
        /// <returns>
        /// Returns true if the specified key is in the SkipList.
        /// </returns>
        private bool Search(object key, out Node curr, Node[] update)
        {
            // Make sure key isn't null.
            if (key == null)
            {
                string msg = resManager.GetString("NullKey");
                throw new ArgumentNullException(msg);
            }

            bool result;

            // Checks to see if we will search with a comparer.
            if (comparer != null)
            {
                result = SearchWithComparer(key, out curr, update);
            }
            // Else we're using the IComparable interface.
            else
            {
                result = SearchWithComparable(key, out curr, update);
            }

            return result;
        }

        /// <summary>
        /// Search for the specified key using a comparer.
        /// </summary>
        /// <param name="key">
        /// The key to search for.
        /// </param>
        /// <param name="curr">
        /// A SkipList node to hold the results of the search.
        /// </param>
        /// <param name="update">
        /// An array of nodes holding references to the places in the SkipList
        /// search in which the search dropped down one level.
        /// </param>
        /// <returns>
        /// Returns true if the specified key is in the SkipList.
        /// </returns>
        private bool SearchWithComparer(object key, out Node curr,
            Node[] update)
        {
            bool found = false;

            // Start from the beginning of the skip list.
            curr = header;

            // Work our way down from the top of the skip list to the bottom.
            for (int i = listLevel - 1; i >= 0; i--)
            {
                // While we haven't reached the end of the skip list and the 
                // current key is less than the new key.
                while (curr.forward[i] != header &&
                    comparer.Compare(curr.forward[i].entry.Key, key) < 0)
                {
                    // Move forward in the skip list.
                    curr = curr.forward[i];
                }

                // Keep track of each node where we move down a level. This 
                // will be used later to rearrange node references when 
                // inserting a new element.
                update[i] = curr;
            }

            // Move ahead in the skip list. If the new key doesn't already 
            // exist in the skip list, this should put us at either the end of
            // the skip list or at a node with a key greater than the sarch key.
            // If the new key already exists in the skip list, this should put 
            // us at a node with a key equal to the search key.
            curr = curr.forward[0];

            // If we haven't reached the end of the skip list and the 
            // current key is equal to the search key.
            if (curr != header && comparer.Compare(key, curr.entry.Key) == 0)
            {
                // Indicate that we've found the search key.
                found = true;
            }

            return found;
        }

        /// <summary>
        /// Search for the specified key using the IComparable interface 
        /// implemented by each key.
        /// </summary>
        /// <param name="key">
        /// The key to search for.
        /// </param>
        /// <param name="curr">
        /// A SkipList node to hold the results of the search.
        /// </param>
        /// <param name="update">
        /// An array of nodes holding references to the places in the SkipList
        /// search in which the search dropped down one level.
        /// </param>
        /// <returns>
        /// Returns true if the specified key is in the SkipList.
        /// </returns>
        /// <remarks>
        /// Assumes each key inserted into the SkipList implements the 
        /// IComparable interface.
        /// 
        /// If the specified key is in the SkipList, the curr parameter will
        /// reference the node with the key. If the specified key is not in the
        /// SkipList, the curr paramater will either hold the node with the 
        /// first key value greater than the specified key or null indicating 
        /// that the search reached the end of the SkipList.
        /// </remarks>
        private bool SearchWithComparable(object key, out Node curr,
            Node[] update)
        {
            // Make sure key is comparable.
            if (!(key is IComparable))
            {
                string msg = resManager.GetString("ComparableError");
                throw new ArgumentException(msg);
            }

            bool found = false;
            IComparable comp;

            // Begin at the start of the skip list.
            curr = header;

            // Work our way down from the top of the skip list to the bottom.
            for (int i = listLevel - 1; i >= 0; i--)
            {
                // Get the comparable interface for the current key.
                comp = (IComparable)curr.forward[i].entry.Key;

                // While we haven't reached the end of the skip list and the 
                // current key is less than the search key.
                while (curr.forward[i] != header && comp.CompareTo(key) < 0)
                {
                    // Move forward in the skip list.
                    curr = curr.forward[i];
                    // Get the comparable interface for the current key.
                    comp = (IComparable)curr.forward[i].entry.Key;
                }

                // Keep track of each node where we move down a level. This 
                // will be used later to rearrange node references when 
                // inserting a new element.
                update[i] = curr;
            }

            // Move ahead in the skip list. If the new key doesn't already 
            // exist in the skip list, this should put us at either the end of
            // the skip list or at a node with a key greater than the search key.
            // If the new key already exists in the skip list, this should put 
            // us at a node with a key equal to the search key.
            curr = curr.forward[0];

            // Get the comparable interface for the current key.
            comp = (IComparable)curr.entry.Key;

            // If we haven't reached the end of the skip list and the 
            // current key is equal to the search key.
            if (curr != header && comp.CompareTo(key) == 0)
            {
                // Indicate that we've found the search key.
                found = true;
            }

            return found;
        }

        /// <summary>
        /// Inserts a key/value pair into the SkipList.
        /// </summary>
        /// <param name="key">
        /// The key to insert into the SkipList.
        /// </param>
        /// <param name="val">
        /// The value to insert into the SkipList.
        /// </param>
        /// <param name="update">
        /// An array of nodes holding references to places in the SkipList in 
        /// which the search for the place to insert the new key/value pair 
        /// dropped down one level.
        /// </param>
        private void Insert(object key, object val, Node[] update)
        {
            // Get the level for the new node.
            int newLevel = GetNewLevel();

            // If the level for the new node is greater than the skip list 
            // level.
            if (newLevel > listLevel)
            {
                // Make sure our update references above the current skip list
                // level point to the header. 
                for (int i = listLevel; i < newLevel; i++)
                {
                    update[i] = header;
                }

                // The current skip list level is now the new node level.
                listLevel = newLevel;
            }

            // Create the new node.
            Node newNode = new(newLevel, key, val);

            // Insert the new node into the skip list.
            for (int i = 0; i < newLevel; i++)
            {
                // The new node forward references are initialized to point to
                // our update forward references which point to nodes further 
                // along in the skip list.
                newNode.forward[i] = update[i].forward[i];

                // Take our update forward references and point them towards 
                // the new node. 
                update[i].forward[i] = newNode;
            }

            // Keep track of the number of nodes in the skip list.
            count++;
            // Indicate that the skip list has changed.
            version++;
        }

        #endregion

        #endregion

        #region Node Class

        /// <summary>
        /// Represents a node in the SkipList.
        /// </summary>
        private class Node : IDisposable
        {
            #region Fields

            // References to nodes further along in the skip list.
            public Node[] forward;

            // The key/value pair.
            public DictionaryEntry entry;

            #endregion

            /// <summary>
            /// Initializes an instant of a Node with its node level.
            /// </summary>
            /// <param name="level">
            /// The node level.
            /// </param>
            public Node(int level)
            {
                forward = new Node[level];
            }

            /// <summary>
            /// Initializes an instant of a Node with its node level and 
            /// key/value pair.
            /// </summary>
            /// <param name="level">
            /// The node level.
            /// </param>
            /// <param name="key">
            /// The key for the node.
            /// </param>
            /// <param name="val">
            /// The value for the node.
            /// </param>
            public Node(int level, object key, object val)
            {
                forward = new Node[level];
                entry.Key = key;
                entry.Value = val;
            }

            #region IDisposable Members

            /// <summary>
            /// Disposes the Node.
            /// </summary>
            public void Dispose()
            {
                for (int i = 0; i < forward.Length; i++)
                {
                    forward[i] = null;
                }
            }

            #endregion
        }

        #endregion

        #region SkipListEnumerator Class

        /// <summary>
        /// Enumerates the elements of a skip list.
        /// </summary>
        private class SkipListEnumerator : IDictionaryEnumerator
        {
            #region SkipListEnumerator Members

            #region Fields

            // The skip list to enumerate.
            private SkipList list;

            // The current node.
            private Node current;

            // The version of the skip list we are enumerating.
            private long version;

            // Keeps track of previous move result so that we can know 
            // whether or not we are at the end of the skip list.
            private bool moveResult = true;

            #endregion

            /// <summary>
            /// Initializes an instance of a SkipListEnumerator.
            /// </summary>
            /// <param name="list"></param>
            public SkipListEnumerator(SkipList list)
            {
                this.list = list;
                version = list.version;
                current = list.header;
            }

            #endregion

            #region IDictionaryEnumerator Members

            /// <summary>
            /// Gets both the key and the value of the current dictionary 
            /// entry.
            /// </summary>
            public DictionaryEntry Entry
            {
                get
                {
                    DictionaryEntry entry;

                    // Make sure the skip list hasn't been modified since the
                    // enumerator was created.
                    if (version != list.version)
                    {
                        string msg = list.resManager.GetString("InvalidEnum");
                        throw new InvalidOperationException(msg);
                    }
                    // Make sure we are not before the beginning or beyond the 
                    // end of the skip list.
                    else if (current == list.header)
                    {
                        string msg = list.resManager.GetString("BadEnumAccess");
                        throw new InvalidOperationException();
                    }
                    // Finally, all checks have passed. Get the current entry.
                    else
                    {
                        entry = current.entry;
                    }

                    return entry;
                }
            }

            /// <summary>
            /// Gets the key of the current dictionary entry.
            /// </summary>
            public object Key
            {
                get
                {
                    object key = Entry.Key;

                    return key;
                }
            }

            /// <summary>
            /// Gets the value of the current dictionary entry.
            /// </summary>
            public object Value
            {
                get
                {
                    object val = Entry.Value;

                    return val;
                }
            }

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Advances the enumerator to the next element of the skip list.
            /// </summary>
            /// <returns>
            /// true if the enumerator was successfully advanced to the next 
            /// element; false if the enumerator has passed the end of the 
            /// skip list.
            /// </returns>
            public bool MoveNext()
            {
                // Make sure the skip list hasn't been modified since the
                // enumerator was created.
                if (version == list.version)
                {
                    // If the result of the previous move operation was true
                    // we can still move forward in the skip list.
                    if (moveResult)
                    {
                        // Move forward in the skip list.
                        current = current.forward[0];

                        // If we are at the end of the skip list.
                        if (current == list.header)
                        {
                            // Indicate that we've reached the end of the skip 
                            // list.
                            moveResult = false;
                        }
                    }
                }
                // Else this version of the enumerator doesn't match that of 
                // the skip list. The skip list has been modified since the 
                // creation of the enumerator.
                else
                {
                    string msg = list.resManager.GetString("InvalidEnum");
                    throw new InvalidOperationException(msg);
                }

                return moveResult;
            }

            /// <summary>
            /// Sets the enumerator to its initial position, which is before 
            /// the first element in the skip list.
            /// </summary>
            public void Reset()
            {
                // Make sure the skip list hasn't been modified since the
                // enumerator was created.
                if (version == list.version)
                {
                    current = list.header;
                    moveResult = true;
                }
                // Else this version of the enumerator doesn't match that of 
                // the skip list. The skip list has been modified since the 
                // creation of the enumerator.
                else
                {
                    string msg = list.resManager.GetString("InvalidEnum");
                    throw new InvalidOperationException(msg);
                }
            }

            /// <summary>
            /// Gets the current element in the skip list.
            /// </summary>
            public object Current
            {
                get
                {
                    return Value;
                }
            }

            #endregion
        }

        #endregion

        #region IDictionary Members

        /// <summary>
        /// Adds an element with the provided key and value to the SkipList.
        /// </summary>
        /// <param name="key">
        /// The Object to use as the key of the element to add. 
        /// </param>
        /// <param name="value">
        /// The Object to use as the value of the element to add. 
        /// </param>
        public void Add(object key, object value)
        {
            Node[] update = new Node[MaxLevel];

            // If key does not already exist in the skip list.
            if (!Search(key, update))
            {
                // Inseart key/value pair into the skip list.
                Insert(key, value, update);
            }
            // Else throw an exception. The IDictionary Add method throws an
            // exception if an attempt is made to add a key that already 
            // exists in the skip list.
            else
            {
                string msg = resManager.GetString("KeyExistsAdd");
                throw new ArgumentException(msg);
            }
        }

        /// <summary>
        /// Removes all elements from the SkipList.
        /// </summary>
        public void Clear()
        {
            // Start at the beginning of the skip list.
            Node curr = header.forward[0];
            Node prev;

            // While we haven't reached the end of the skip list.
            while (curr != header)
            {
                // Keep track of the previous node.
                prev = curr;
                // Move forward in the skip list.
                curr = curr.forward[0];
                // Dispose of the previous node.
                prev.Dispose();
            }

            // Initialize skip list and indicate that it has been changed.
            Initialize();
            version++;
        }

        /// <summary>
        /// Determines whether the SkipList contains an element with the 
        /// specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the SkipList.
        /// </param>
        /// <returns>
        /// true if the SkipList contains an element with the key; otherwise, 
        /// false.
        /// </returns>
        public bool Contains(object key)
        {
            return Search(key);
        }

        /// <summary>
        /// Returns an IDictionaryEnumerator for the SkipList.
        /// </summary>
        /// <returns>
        /// An IDictionaryEnumerator for the SkipList.
        /// </returns>
        public IDictionaryEnumerator GetEnumerator()
        {
            return new SkipListEnumerator(this);
        }

        /// <summary>
        /// Removes the element with the specified key from the SkipList.
        /// </summary>
        /// <param name="key">
        /// The key of the element to remove.
        /// </param>
        public void Remove(object key)
        {
            Node[] update = new Node[MaxLevel];
            Node curr;

            if (Search(key, out curr, update))
            {
                // Take the forward references that point to the node to be 
                // removed and reassign them to the nodes that come after it.
                for (int i = 0; i < listLevel &&
                    update[i].forward[i] == curr; i++)
                {
                    update[i].forward[i] = curr.forward[i];
                }

                curr.Dispose();

                // After removing the node, we may need to lower the current 
                // skip list level if the node had the highest level of all of
                // the nodes.
                while (listLevel > 1 && header.forward[listLevel - 1] == header)
                {
                    listLevel--;
                }

                // Keep track of the number of nodes.
                count--;
                // Indicate that the skip list has changed.
                version++;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the SkipList has a fixed size.
        /// </summary>
        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the IDictionary is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key. This is the 
        /// indexer for the SkipList. 
        /// </summary>
        public object this[object key]
        {
            get
            {
                object val = null;
                Node curr;

                if (Search(key, out curr))
                {
                    val = curr.entry.Value;
                }

                return val;
            }
            set
            {
                Node[] update = new Node[MaxLevel];
                Node curr;

                // If the search key already exists in the skip list.
                if (Search(key, out curr, update))
                {
                    // Replace the current value with the new value.
                    curr.entry.Value = value;
                    // Indicate that the skip list has changed.
                    version++;
                }
                // Else the key doesn't exist in the skip list.
                else
                {
                    // Insert the key and value into the skip list.
                    Insert(key, value, update);
                }
            }
        }

        /// <summary>
        /// Gets an ICollection containing the keys of the SkipList.
        /// </summary>
        public ICollection Keys
        {
            get
            {
                // Start at the beginning of the skip list.
                Node curr = header.forward[0];
                // Create a collection to hold the keys.
                ArrayList collection = new();

                // While we haven't reached the end of the skip list.
                while (curr != header)
                {
                    // Add the key to the collection.
                    collection.Add(curr.entry.Key);
                    // Move forward in the skip list.
                    curr = curr.forward[0];
                }

                return collection;
            }
        }

        /// <summary>
        /// Gets an ICollection containing the values of the SkipList.
        /// </summary>
        public ICollection Values
        {
            get
            {
                // Start at the beginning of the skip list.
                Node curr = header.forward[0];
                // Create a collection to hold the values.
                ArrayList collection = new();

                // While we haven't reached the end of the skip list.
                while (curr != header)
                {
                    // Add the value to the collection.
                    collection.Add(curr.entry.Value);
                    // Move forward in the skip list.
                    curr = curr.forward[0];
                }

                return collection;
            }
        }

        #endregion

        #region ICollection Members

        /// <summary>
        /// Copies the elements of the SkipList to an Array, starting at a 
        /// particular Array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements 
        /// copied from SkipList.
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins.
        /// </param>
        public void CopyTo(Array array, int index)
        {
            // Make sure array isn't null.
            if (array == null)
            {
                string msg = resManager.GetString("NullArrayCopyTo");
                throw new ArgumentNullException(msg);
            }
            // Make sure index is not negative.
            else if (index < 0)
            {
                string msg = resManager.GetString("BadIndexCopyTo");
                throw new ArgumentOutOfRangeException(msg);
            }
            // Array bounds checking.
            else if (index >= array.Length)
            {
                string msg = resManager.GetString("BadIndexCopyTo");
                throw new ArgumentException(msg);
            }
            // Make sure that the number of elements in the skip list is not 
            // greater than the available space from index to the end of the 
            // array.
            else if (array.Length - index < Count)
            {
                string msg = resManager.GetString("BadIndexCopyTo");
                throw new ArgumentException(msg);
            }
            // Else copy elements from skip list into array.
            else
            {
                // Start at the beginning of the skip list.
                Node curr = header.forward[0];

                // While we haven't reached the end of the skip list.
                while (curr != header)
                {
                    // Copy current value into array.
                    array.SetValue(curr.entry.Value, index);

                    // Move forward in the skip list and array.
                    curr = curr.forward[0];
                    index++;
                }
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the SkipList.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether access to the SkipList is 
        /// synchronized (thread-safe).
        /// </summary>
        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the 
        /// SkipList.
        /// </summary>
        public object SyncRoot
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that can iterate through the SkipList.
        /// </summary>
        /// <returns>
        /// An IEnumerator that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SkipListEnumerator(this);
        }

        #endregion
    }
}
