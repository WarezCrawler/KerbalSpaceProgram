using System.Collections;
using System.Collections.Generic;

namespace UniLinq;

internal class Grouping<T, U> : IEnumerable, IEnumerable<U>, IGrouping<T, U>
{
	private T key;

	private IEnumerable<U> group;

	public T Key
	{
		get
		{
			return key;
		}
		set
		{
			key = value;
		}
	}

	public Grouping(T key, IEnumerable<U> group)
	{
		this.group = group;
		this.key = key;
	}

	public IEnumerator<U> GetEnumerator()
	{
		return group.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return group.GetEnumerator();
	}
}
