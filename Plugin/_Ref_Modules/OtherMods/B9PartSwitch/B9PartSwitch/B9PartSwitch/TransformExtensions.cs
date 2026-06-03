using System.Collections.Generic;
using UnityEngine;

namespace B9PartSwitch;

public static class TransformExtensions
{
	public static void Enable(this Transform trans)
	{
		trans.gameObject.SetActive(value: true);
	}

	public static void Disable(this Transform trans)
	{
		trans.gameObject.SetActive(value: false);
	}

	public static IEnumerable<Transform> TraverseHierarchy(this Transform transform)
	{
		transform.ThrowIfNullArgument("transform");
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			yield return child;
			foreach (Transform item in child.TraverseHierarchy())
			{
				yield return item;
			}
		}
	}
}
