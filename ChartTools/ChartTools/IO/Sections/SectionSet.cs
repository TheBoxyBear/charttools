using System.Collections;

namespace ChartTools.IO.Sections;

/// <summary>
/// Set of sections contained in a file that cannot be mapped to the model
/// </summary>
/// <typeparam name="T">Type of content in the sections depending on the file type</typeparam>
public abstract class SectionSet<T> : IList<Section<T>>
{
	private readonly List<Section<T>> m_sections = [];

	public abstract ReservedSectionHeaderSet ReservedHeaders { get; }

	#region IList
	public int Count => m_sections.Count;

	public bool IsReadOnly => false;

	public Section<T> this[int index]
	{
		get => m_sections[index];
		set
		{
			CheckHeader(value.Header);
			m_sections[index] = value;
		}
	}

	public int IndexOf(Section<T> item)
		=> m_sections.IndexOf(item);

	public void Insert(int index, Section<T> item)
	{
		CheckHeader(item.Header);
		m_sections.Insert(index, item);
	}

	public void RemoveAt(int index)
		=> m_sections.RemoveAt(index);

	public void Add(Section<T> item)
	{
		CheckHeader(item.Header);
		m_sections.Add(item);
	}

	public void Clear()
		=> m_sections.Clear();

	public bool Contains(Section<T> item)
		=> m_sections.Contains(item);

	public void CopyTo(Section<T>[] array, int arrayIndex)
		=> m_sections.CopyTo(array, arrayIndex);

	public bool Remove(Section<T> item)
		=> m_sections.Remove(item);

	public IEnumerator<Section<T>> GetEnumerator()
		=> m_sections.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator()
		=> GetEnumerator();
	#endregion

	public Section<T>? Get(string header)
	{
		CheckHeader(header);
		return m_sections.FirstOrDefault(s => s.Header == header);
	}

	private void CheckHeader(string header)
	{
		foreach (ReservedSectionHeader reserved in ReservedHeaders)
			if (reserved.Header == header)
				throw new Exception($"Header {header} is already modeled under {reserved.DataSource}");
	}
}
