using UnityEngine;

namespace KerbalEngineer.Editor;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class BuildAppLauncher : AppLauncherButton
{
	protected override void OnFalse()
	{
		if ((Object)(object)BuildAdvanced.Instance != (Object)null)
		{
			BuildAdvanced.Instance.Visible = false;
		}
	}

	protected override void OnTrue()
	{
		if ((Object)(object)BuildAdvanced.Instance != (Object)null)
		{
			BuildAdvanced.Instance.Visible = true;
		}
	}

	protected virtual void Update()
	{
		if (!((Object)(object)BuildAdvanced.Instance == (Object)null))
		{
			if ((Object)(object)EditorLogic.RootPart != (Object)null)
			{
				base.IsOn = BuildAdvanced.Instance.Visible;
			}
			else
			{
				Disable();
			}
		}
	}
}
