using System.Collections.Generic;
using UniLinq;
using UnityEngine;

namespace B9PartSwitch;

public static class CustomPartModuleExtensions
{
	public static IEnumerable<T> FindSymmetryCounterparts<T>(this T module) where T : CustomPartModule
	{
		if (module.part == null)
		{
			yield break;
		}
		foreach (Part symmetryCounterpart in module.part.symmetryCounterparts)
		{
			T val = symmetryCounterpart.Modules.OfType<T>().FirstOrDefault((T m) => m.GetType() == module.GetType() && m.moduleID == module.moduleID);
			if (val.IsNotNull())
			{
				yield return val;
			}
			else
			{
				Debug.LogWarning("No symmetry counterpart found on part counterpart");
			}
		}
	}
}
