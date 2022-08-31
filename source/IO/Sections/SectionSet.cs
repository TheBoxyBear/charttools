using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChartTools.IO.Sections
{
    /// <summary>
    /// Set of sections contained in a file that cannot be mapped to the model
    /// </summary>
    /// <typeparam name="T">Type of content in the sections depending on the file type</typeparam>
    public abstract class SectionSet<T> : IList<Section<T>>
    {
        private readonly List<Section<T>> _sections = new();
        /// <summary>
        /// Headers used by the model
        /// </summary>
        /// <remarks>Adding a section with a reserved header will cause an <see cref="Exception"/>.</remarks>
        public abstract ReservedSectionHeaderSet ReservedHeaders { get; }

        #region IList
        public int Count => _sections.Count;
        public bool IsReadOnly => false;

        public Section<T> this[int index]
        {
            get => _sections[index];
            set
            {
                CheckHeader(value.Header);
                _sections[index] = value;
            }
        }

        public int IndexOf(Section<T> item) => _sections.IndexOf(item);
        public void Insert(int index, Section<T> item)
        {
            CheckHeader(item.Header);
            _sections.Insert(index, item);
        }
        public void RemoveAt(int index) => _sections.RemoveAt(index);
        public void Add(Section<T> item)
        {
            CheckHeader(item.Header);
            _sections.Add(item);
        }
        public void Clear() => _sections.Clear();
        public bool Contains(Section<T> item) => _sections.Contains(item);
        public void CopyTo(Section<T>[] array, int arrayIndex) => _sections.CopyTo(array, arrayIndex);
        public bool Remove(Section<T> item) => _sections.Remove(item);
        public IEnumerator<Section<T>> GetEnumerator() => _sections.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion

        public Section<T>? Get(string header)
        {
            CheckHeader(header);
            return _sections.FirstOrDefault(s => s.Header == header);
        }

        private void CheckHeader(string header)
        {
            foreach (var reserved in ReservedHeaders)
                if (reserved.Header == header)
                    throw new Exception($"Header {header} is already modeled under {reserved.DataSource}");
        }
    }
}
