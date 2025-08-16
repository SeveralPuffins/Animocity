using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Animocity.Utilities
{
    public class MultiMap<T, U> : IDictionary<T, U>
    {
        private Dictionary<T, U> _forward = new();
        private Dictionary<U, HashSet<T>> _reverse = new();

        public U this[T key] { get => _forward[key]; set => _forward[key] = value; }
        public HashSet<T> this[U key] { get => _reverse[key]; set => _reverse[key] = value; }


        public ICollection<T> Keys => _forward.Keys;
        public ICollection<U> Values => _forward.Values;

        public int Count => _forward.Count;

        public bool IsReadOnly => false;

        public MultiMap() { }

        public void Add(T key, U value)
        {
            _forward.Add(key, value);
            if (_reverse.TryGetValue(value, out var set))
            {
                set.Add(key);
            }
            else
            {
                HashSet<T> revSet = new HashSet<T>();
                revSet.Add(key);
                _reverse.Add(value, revSet);
            }
        }
        public void Add(KeyValuePair<T, U> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _forward.Clear();
            _reverse.Clear();   
        }

        public bool Contains(KeyValuePair<T, U> item)
        {
            return _forward.Contains(item);
        }

        public bool ContainsKey(T key)
        {
            return ((IDictionary<T, U>)_forward).ContainsKey(key);
        }

     

        IEnumerator<KeyValuePair<T, U>> IEnumerable<KeyValuePair<T, U>>.GetEnumerator()
        {
            return ((IDictionary<T, U>)_forward).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<T, U>)_forward).GetEnumerator();
        }

        public bool Remove(T key)
        {
            if(_forward.Remove(key, out var u))
            {
                _reverse[u].Remove(key);
                return true;
            }
            return false;
        }
        public bool DeepRemove(T key)
        {
            if (_forward.Remove(key, out var u))
            {
                foreach(T t in _reverse[u])
                {
                    _forward.Remove(t);
                }
                _reverse.Remove(u);
                return true;
            }
            return false;
        }

        public bool Remove(KeyValuePair<T, U> item)
        {
            return Remove(item.Key);
        }

        public bool TryGetValue(T key, out U value)
        {
            return _forward.TryGetValue(key, out value);
        }
        public bool TryGetValues(U key, out HashSet<T> value)
        {
            return _reverse.TryGetValue(key, out value);
        }

        public void CopyTo(KeyValuePair<T, U>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<T, U>>)_forward).CopyTo(array, arrayIndex);
        }
    }
}
