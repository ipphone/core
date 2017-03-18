using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ContactPoint.Contacts
{
    class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private Dictionary<TKey, TValue> _internalDictionary = new Dictionary<TKey, TValue>();

        public int Count { get { return _internalDictionary.Count; } }
        public bool IsReadOnly { get { return false; } }
        public ICollection<TKey> Keys { get { return _internalDictionary.Keys; } }
        public ICollection<TValue> Values { get { return _internalDictionary.Values; } }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _internalDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _internalDictionary.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _internalDictionary.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _internalDictionary.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        { }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _internalDictionary.Remove(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            return _internalDictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _internalDictionary.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return _internalDictionary.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _internalDictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get { return _internalDictionary[key]; }
            set { _internalDictionary[key] = value; }
        }
    }
}
