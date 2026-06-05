using System;
using UnityEngine;

namespace B9PartSwitch.UI;

[KSPAddon(KSPAddon.Startup.Instantly, true)]
public class PrefabManagerInstant : MonoBehaviour
{
	private void Awake()
	{
		try
		{
			TooltipHelper.EnsurePrefabs();
		}
		catch (Exception exception)
		{
			FatalErrorHandler.HandleFatalError(exception);
			Debug.LogException(exception);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
