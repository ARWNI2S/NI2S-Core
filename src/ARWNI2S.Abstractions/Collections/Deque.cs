#region License

/* Copyright (c) 2006 Leslie Sanford
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy 
 * of this software and associated documentation files (the "Software"), to 
 * deal in the Software without restriction, including without limitation the 
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or 
 * sell copies of the Software, and to permit persons to whom the Software is 
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in 
 * all copies or substantial portions of the Software. 
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
 */

#endregion

#region Contact

/*
 * Leslie Sanford
 * Email: jabberdabber@hotmail.com
 */

#endregion

using System.Collections;
using System.Diagnostics;

namespace ARWNI2S.Collections
{
    /// <summary>
    /// Represents a simple double-ended-queue collection of objects.
    /// </summary>
    [Serializable()]
    [GenerateSerializer]
    public class Deque : ICollection, IEnumerable, ICloneable
    {
        #region Deque Members

        #region Fields

        // The node at the front of the deque.
        private Node _front = null;

        // The node at the back of the deque.
        private Node _back = null;

        // The number of elements in the deque.
        private int _count = 0;

        // The version of the deque.
        private long _version = 0;

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the Deque class.
        /// </summary>
        public Deque()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Deque class that contains 
        /// elements copied from the specified collection.
        /// </summary>
        /// <param name="col">
        /// The ICollection to copy elements from.
        /// </param>
        public Deque(ICollection col)
        {
            #region Require

            if (col == null)
            {
                throw new ArgumentNullException("col");
            }

            #endregion

            foreach (object obj in col)
            {
                PushBack(obj);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Removes all objects from the Deque.
        /// </summary>
        public virtual void Clear()
        {
            _count = 0;

            _front = _back = null;

            _version++;

            #region Invariant

            AssertValid();

            #endregion
        }

        /// <summary>
        /// Determines whether or not an element is in the Deque.
        /// </summary>
        /// <param name="obj">
        /// The Object to locate in the Deque.
        /// </param>
        /// <returns>
        /// <b>true</b> if <i>obj</i> if found in the Deque; otherwise, 
        /// <b>false</b>.
        /// </returns>
        public virtual bool Contains(object obj)
        {
            foreach (object o in this)
            {
                if (o == null && obj == null)
                {
                    return true;
                }
                else if (o.Equals(obj))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Inserts an object at the front of the Deque.
        /// </summary>
        /// <param name="obj">
        /// The object to push onto the deque;
        /// </param>
        public virtual void PushFront(object obj)
        {
            // The new node to add to the front of the deque.
            Node newNode = new(obj);

            // Link the new node to the front node. The current front node at 
            // the front of the deque is now the second node in the deque.
            newNode.Next = _front;

            // If the deque isn't empty.
            if (Count > 0)
            {
                // Link the current front to the new node.
                _front.Previous = newNode;
            }

            // Make the new node the front of the deque.
            _front = newNode;

            // Keep track of the number of elements in the deque.
            _count++;

            // If this is the first element in the deque.
            if (Count == 1)
            {
                // The front and back nodes are the same.
                _back = _front;
            }

            _version++;

            #region Invariant

            AssertValid();

            #endregion
        }

        /// <summary>
        /// Inserts an object at the back of the Deque.
        /// </summary>
        /// <param name="obj">
        /// The object to push onto the deque;
        /// </param>
        public virtual void PushBack(object obj)
        {
            // The new node to add to the back of the deque.
            Node newNode = new(obj);

            // Link the new node to the back node. The current back node at 
            // the back of the deque is now the second to the last node in the
            // deque.
            newNode.Previous = _back;

            // If the deque is not empty.
            if (Count > 0)
            {
                // Link the current back node to the new node.
                _back.Next = newNode;
            }

            // Make the new node the back of the deque.
            _back = newNode;

            // Keep track of the number of elements in the deque.
            _count++;

            // If this is the first element in the deque.
            if (Count == 1)
            {
                // The front and back nodes are the same.
                _front = _back;
            }

            _version++;

            #region Invariant

            AssertValid();

            #endregion
        }

        /// <summary>
        /// Removes and returns the object at the front of the Deque.
        /// </summary>
        /// <returns>
        /// The object at the front of the Deque.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The Deque is empty.
        /// </exception>
        public virtual object PopFront()
        {
            #region Require

            if (Count == 0)
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            #endregion

            // Get the object at the front of the deque.
            object obj = _front.Value;

            // Move the front back one node.
            _front = _front.Next;

            // Keep track of the number of nodes in the deque.
            _count--;

            // If the deque is not empty.
            if (Count > 0)
            {
                // Tie off the previous link in the front node.
                _front.Previous = null;
            }
            // Else the deque is empty.
            else
            {
                // Indicate that there is no back node.
                _back = null;
            }

            _version++;

            #region Invariant

            AssertValid();

            #endregion

            return obj;
        }

        /// <summary>
        /// Removes and returns the object at the back of the Deque.
        /// </summary>
        /// <returns>
        /// The object at the back of the Deque.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The Deque is empty.
        /// </exception>
        public virtual object PopBack()
        {
            #region Require

            if (Count == 0)
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            #endregion

            // Get the object at the back of the deque.
            object obj = _back.Value;

            // Move back node forward one node.
            _back = _back.Previous;

            // Keep track of the number of nodes in the deque.
            _count--;

            // If the deque is not empty.
            if (Count > 0)
            {
                // Tie off the next link in the back node.
                _back.Next = null;
            }
            // Else the deque is empty.
            else
            {
                // Indicate that there is no front node.
                _front = null;
            }

            _version++;

            #region Invariant

            AssertValid();

            #endregion

            return obj;
        }

        /// <summary>
        /// Returns the object at the front of the Deque without removing it.
        /// </summary>
        /// <returns>
        /// The object at the front of the Deque.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The Deque is empty.
        /// </exception>
        public virtual object PeekFront()
        {
            #region Require

            if (Count == 0)
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            #endregion

            return _front.Value;
        }

        /// <summary>
        /// Returns the object at the back of the Deque without removing it.
        /// </summary>
        /// <returns>
        /// The object at the back of the Deque.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The Deque is empty.
        /// </exception>
        public virtual object PeekBack()
        {
            #region Require

            if (Count == 0)
            {
                throw new InvalidOperationException("Deque is empty.");
            }

            #endregion

            return _back.Value;
        }

        /// <summary>
        /// Copies the Deque to a new array.
        /// </summary>
        /// <returns>
        /// A new array containing copies of the elements of the Deque.
        /// </returns>
        public virtual object[] ToArray()
        {
            object[] array = new object[Count];
            int index = 0;

            foreach (object obj in this)
            {
                array[index] = obj;
                index++;
            }

            return array;
        }

        /// <summary>
        /// Returns a synchronized (thread-safe) wrapper for the Deque.
        /// </summary>
        /// <param name="deque">
        /// The Deque to synchronize.
        /// </param>
        /// <returns>
        /// A synchronized wrapper around the Deque.
        /// </returns>
        public static Deque Synchronized(Deque deque)
        {
            #region Require

            if (deque == null)
            {
                throw new ArgumentNullException("deque");
            }

            #endregion

            return new SynchronizedDeque(deque);
        }

        [Conditional("DEBUG")]
        private void AssertValid()
        {
            int n = 0;
            Node current = _front;

            while (current != null)
            {
                n++;
                current = current.Next;
            }

            Debug.Assert(n == Count);

            if (Count > 0)
            {
                Debug.Assert(_front != null && _back != null, "Front/Back Null Test - Count > 0");

                Node f = _front;
                Node b = _back;

                while (f.Next != null && b.Previous != null)
                {
                    f = f.Next;
                    b = b.Previous;
                }

                Debug.Assert(f.Next == null && b.Previous == null, "Front/Back Termination Test");
                Debug.Assert(f == _back && b == _front, "Front/Back Equality Test");
            }
            else
            {
                Debug.Assert(_front == null && _back == null, "Front/Back Null Test - Count == 0");
            }
        }

        #endregion

        #region Node Class

        // Represents a node in the deque.
        [Serializable()]
        private class Node
        {
            private object value;

            private Node previous = null;

            private Node next = null;

            public Node(object value)
            {
                this.value = value;
            }

            public object Value
            {
                get
                {
                    return value;
                }
            }

            public Node Previous
            {
                get
                {
                    return previous;
                }
                set
                {
                    previous = value;
                }
            }

            public Node Next
            {
                get
                {
                    return next;
                }
                set
                {
                    next = value;
                }
            }
        }

        #endregion

        #region DequeEnumerator Class

        [Serializable()]
        private class DequeEnumerator : IEnumerator
        {
            private Deque owner;

            private Node currentNode;

            private object current = null;

            private bool moveResult = false;

            private long version;

            public DequeEnumerator(Deque owner)
            {
                this.owner = owner;
                currentNode = owner._front;
                version = owner._version;
            }

            #region IEnumerator Members

            public void Reset()
            {
                #region Require

                if (version != owner._version)
                {
                    throw new InvalidOperationException(
                        "The Deque was modified after the enumerator was created.");
                }

                #endregion

                currentNode = owner._front;
                moveResult = false;
            }

            public object Current
            {
                get
                {
                    #region Require

                    if (!moveResult)
                    {
                        throw new InvalidOperationException(
                            "The enumerator is positioned before the first " +
                            "element of the Deque or after the last element.");
                    }

                    #endregion

                    return current;
                }
            }

            public bool MoveNext()
            {
                #region Require

                if (version != owner._version)
                {
                    throw new InvalidOperationException(
                        "The Deque was modified after the enumerator was created.");
                }

                #endregion

                if (currentNode != null)
                {
                    current = currentNode.Value;
                    currentNode = currentNode.Next;

                    moveResult = true;
                }
                else
                {
                    moveResult = false;
                }

                return moveResult;
            }

            #endregion
        }

        #endregion

        #region SynchronizedDeque Class

        // Implements a synchronization wrapper around a deque.
        [Serializable()]
        private class SynchronizedDeque : Deque
        {
            #region SynchronziedDeque Members

            #region Fields

            // The wrapped deque.
            private Deque deque;

            // The object to lock on.
            private object root;

            #endregion

            #region Construction

            public SynchronizedDeque(Deque deque)
            {
                #region Require

                if (deque == null)
                {
                    throw new ArgumentNullException("deque");
                }

                #endregion

                this.deque = deque;
                root = deque.SyncRoot;
            }

            #endregion

            #region Methods

            public override void Clear()
            {
                lock (root)
                {
                    deque.Clear();
                }
            }

            public override bool Contains(object obj)
            {
                bool result;

                lock (root)
                {
                    result = deque.Contains(obj);
                }

                return result;
            }

            public override void PushFront(object obj)
            {
                lock (root)
                {
                    deque.PushFront(obj);
                }
            }

            public override void PushBack(object obj)
            {
                lock (root)
                {
                    deque.PushBack(obj);
                }
            }

            public override object PopFront()
            {
                object obj;

                lock (root)
                {
                    obj = deque.PopFront();
                }

                return obj;
            }

            public override object PopBack()
            {
                object obj;

                lock (root)
                {
                    obj = deque.PopBack();
                }

                return obj;
            }

            public override object PeekFront()
            {
                object obj;

                lock (root)
                {
                    obj = deque.PeekFront();
                }

                return obj;
            }

            public override object PeekBack()
            {
                object obj;

                lock (root)
                {
                    obj = deque.PeekBack();
                }

                return obj;
            }

            public override object[] ToArray()
            {
                object[] array;

                lock (root)
                {
                    array = deque.ToArray();
                }

                return array;
            }

            public override object Clone()
            {
                object clone;

                lock (root)
                {
                    clone = deque.Clone();
                }

                return clone;
            }

            public override void CopyTo(Array array, int index)
            {
                lock (root)
                {
                    deque.CopyTo(array, index);
                }
            }

            public override IEnumerator GetEnumerator()
            {
                IEnumerator e;

                lock (root)
                {
                    e = deque.GetEnumerator();
                }

                return e;
            }

            #endregion

            #region Properties

            public override int Count
            {
                get
                {
                    lock (root)
                    {
                        return deque.Count;
                    }
                }
            }

            public override bool IsSynchronized
            {
                get
                {
                    return true;
                }
            }

            #endregion

            #endregion
        }

        #endregion

        #endregion

        #region ICollection Members

        /// <summary>
        /// Gets a value indicating whether access to the Deque is synchronized 
        /// (thread-safe).
        /// </summary>
        public virtual bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the Deque.
        /// </summary>
        public virtual int Count
        {
            get
            {
                return _count;
            }
        }

        /// <summary>
        /// Copies the Deque elements to an existing one-dimensional Array, 
        /// starting at the specified array index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional Array that is the destination of the elements 
        /// copied from Deque. The Array must have zero-based indexing. 
        /// </param>
        /// <param name="index">
        /// The zero-based index in array at which copying begins. 
        /// </param>
        public virtual void CopyTo(Array array, int index)
        {
            #region Require

            if (array == null)
            {
                throw new ArgumentNullException("array");
            }
            else if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", index,
                    "Index is less than zero.");
            }
            else if (array.Rank > 1)
            {
                throw new ArgumentException("Array is multidimensional.");
            }
            else if (index >= array.Length)
            {
                throw new ArgumentException("Index is equal to or greater " +
                    "than the length of array.");
            }
            else if (Count > array.Length - index)
            {
                throw new ArgumentException(
                    "The number of elements in the source Deque is greater " +
                    "than the available space from index to the end of the " +
                    "destination array.");
            }

            #endregion

            int i = index;

            foreach (object obj in this)
            {
                array.SetValue(obj, i);
                i++;
            }
        }

        /// <summary>
        /// Gets an object that can be used to synchronize access to the Deque.
        /// </summary>
        public virtual object SyncRoot
        {
            get
            {
                return this;
            }
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an enumerator that can iterate through the Deque.
        /// </summary>
        /// <returns>
        /// An IEnumerator for the Deque.
        /// </returns>
        public virtual IEnumerator GetEnumerator()
        {
            return new DequeEnumerator(this);
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// Creates a shallow copy of the Deque.
        /// </summary>
        /// <returns>
        /// A shallow copy of the Deque.
        /// </returns>
        public virtual object Clone()
        {
            Deque clone = new(this);

            clone._version = _version;

            return clone;
        }

        #endregion
    }
}
