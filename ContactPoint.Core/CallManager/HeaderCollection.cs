using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ContactPoint.Common.CallManager;

namespace ContactPoint.Core.CallManager
{
    internal class HeaderCollection : IHeaderCollection
    {
        private List<IHeader> _headers = new List<IHeader>();

        public int Count
        {
            get { return _headers.Count; }
        }

        public HeaderCollection()
        { }

        public HeaderCollection(IEnumerable<KeyValuePair<string, string>> values)
        {
            AddRange(values);
        }

        public void AddRange(IEnumerable<KeyValuePair<string, string>> values)
        {
            if (values == null) return;

            foreach (var value in values)
                _headers.Add(new Header(value.Key, value.Value));
        }

        public string GetValueSafe(string key)
        {
            var header = this[key];

            return header == null ? String.Empty : header.Value;
        }

        #region IHeaderCollection Members

        public IHeader this[string key]
        {
            get
            {
                foreach (var header in _headers)
                    if (header.Name == key)
                        return header;

                return null;
            }
        }

        public bool Contains(string key)
        {
            foreach (var header in _headers)
                if (header.Name == key)
                    return true;

            return false;
        }

        #endregion

        #region IEnumerable<IHeader> Members

        public IEnumerator<IHeader> GetEnumerator()
        {
            return _headers.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}
