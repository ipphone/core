using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Contacts
{
    internal class ObservableDictionaryValuesMapperConverter<TValue, TInterface> : ICollection<TInterface> 
        where TValue : IEntity, TInterface
    {
        private readonly IDictionary<long, TValue> _targetDictionary;

        public ObservableDictionaryValuesMapperConverter(IDictionary<long, TValue> targetDictionary)
        {
            _targetDictionary = targetDictionary;
        }

        public IEnumerator<TInterface> GetEnumerator()
        {
            return _targetDictionary.Values.Cast<TInterface>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TInterface item)
        {
            if (item is TValue)
                _targetDictionary.Add(((TValue)item).Id, (TValue)item);
        }

        public void Clear()
        {
            _targetDictionary.Clear();
        }

        public bool Contains(TInterface item)
        {
            if (item is TValue)
                return _targetDictionary.ContainsKey(((TValue)item).Id);

            return false;
        }

        public void CopyTo(TInterface[] array, int arrayIndex)
        { }

        public bool Remove(TInterface item)
        {
            if (item is TValue)
                return _targetDictionary.Remove(((TValue)item).Id);

            return false;
        }

        public int Count { get { return _targetDictionary.Count; } }
        public bool IsReadOnly { get { return false; } }
    }
}
