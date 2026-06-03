using System;
using System.Collections.Generic;

namespace B9PartSwitch;

public static class IEnumerableExtensions
{
	public static IEnumerable<T> All<T>(this IEnumerable<T> enumerable)
	{
		foreach (T item in enumerable)
		{
			yield return item;
		}
	}

	public static Tcollection MaxBy<Tcollection, Tcompare>(this IEnumerable<Tcollection> enumerable, Func<Tcollection, Tcompare> mapper) where Tcompare : IComparable<Tcompare>
	{
		enumerable.ThrowIfNullArgument("enumerable");
		mapper.ThrowIfNullArgument("mapper");
		IEnumerator<Tcollection> enumerator = enumerable.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			throw new InvalidOperationException("Enumerable is empty!");
		}
		Tcollection val = enumerator.Current;
		Tcompare other = mapper(val);
		while (enumerator.MoveNext())
		{
			Tcollection current = enumerator.Current;
			Tcompare val2 = mapper(current);
			if (val2.CompareTo(other) > 0)
			{
				val = current;
				other = val2;
			}
		}
		return val;
	}
}
