using System;
using System.Collections;
using System.Collections.Generic;

namespace Tool.Observable
{
    internal class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        public event Action<TKey, TValue> OnAdd;
        public event  Action<TKey, TValue> OnRemove;
        private readonly Dictionary<TKey, TValue> _dic = new();

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>) _dic).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>) _dic).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            _dic.Remove(item.Key);
            throw new NotImplementedException();
        }

        public int Count => _dic.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (_dic.TryAdd(key, value))
            {
                OnAdd?.Invoke(key, value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            return _dic.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            if (_dic.TryGetValue(key, out var value))
            {
                OnRemove?.Invoke(key, value);
            }

            return _dic.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dic.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => _dic[key];
            set => _dic[key]=value;
        }

        public ICollection<TKey> Keys => _dic.Keys;
        public ICollection<TValue> Values => _dic.Values;

        public void ClearEvent()
        {
            OnAdd = null;
            OnRemove = null;
        }
    }
}
