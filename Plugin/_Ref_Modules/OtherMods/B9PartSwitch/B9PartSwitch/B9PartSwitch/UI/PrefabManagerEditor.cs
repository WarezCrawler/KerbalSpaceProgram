using System;
using UnityEngine;

namespace B9PartSwitch.UI;

[KSPAddon(KSPAddon.Startup.EditorAny, false)]
public class PrefabManagerEditor : MonoBehaviour
{
	private void Start()
	{
		if (HighLogic.LoadedSceneIsEditor || HighLogic.LoadedSceneIsFlight)
		{
			try
			{
				UIPartActionSubtypeSelector.EnsurePrefab();
			}
			catch (Exception exception)
			{
				FatalErrorHandler.HandleFatalError(exception);
				Debug.LogException(exception);
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
