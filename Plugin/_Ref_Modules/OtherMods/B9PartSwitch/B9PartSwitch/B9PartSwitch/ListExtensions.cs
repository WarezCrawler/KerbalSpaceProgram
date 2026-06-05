using System;
using System.Collections;
using System.Collections.Generic;
using UniLinq;

namespace B9PartSwitch;

public static class ListExtensions
{
	public static bool IsList(this object o)
	{
		return o?.GetType().IsListType() ?? false;
	}

	public static bool IsListType(this Type t)
	{
		if (t == null)
		{
			return false;
		}
		if (t.GetInterfaces().Contains(typeof(IList)) && t.IsGenericType)
		{
			return t.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
		}
		return false;
	}

	public static bool IsNullOrEmpty(this IList list)
	{
		if (!list.IsNull())
		{
			return list.Count == 0;
		}
		return true;
	}

	public static bool ValidIndex(this IList list, int index)
	{
		if (index >= 0)
		{
			return index < list.Count;
		}
		return false;
	}

	public static bool SameElementsAs<T>(this IEnumerable<T> set1, IEnumerable<T> set2)
	{
		if (set1.Count() == set2.Count() && !set1.Except(set1).Any())
		{
			return !set2.Except(set1).Any();
		}
		return false;
	}
}
