using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ContactPoint.Common.Contacts;

namespace ContactPoint.Contacts
{
    internal class ObservableCollectionMapperConverter<TValue, TInterface> : ICollection<TInterface>
        where TValue : IEntity, TInterface
    {
        private readonly ICollection<TValue> _targetCollection;

        public ObservableCollectionMapperConverter(ICollection<TValue> targetCollection)
        {
            _targetCollection = targetCollection;
        }

        public IEnumerator<TInterface> GetEnumerator()
        {
            return _targetCollection.Cast<TInterface>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TInterface item)
        {
            if (item is TValue)
                _targetCollection.Add((TValue)item);
        }

        public void Clear()
        {
            _targetCollection.Clear();
        }

        public bool Contains(TInterface item)
        {
            if (item is TValue)
                return _targetCollection.Contains((TValue)item);

            return false;
        }

        public void CopyTo(TInterface[] array, int arrayIndex)
        { }

        public bool Remove(TInterface item)
        {
            if (item is TValue)
                return _targetCollection.Remove((TValue)item);

            return false;
        }

        public int Count { get { return _targetCollection.Count; } }
        public bool IsReadOnly { get { return false; } }
    }
}
