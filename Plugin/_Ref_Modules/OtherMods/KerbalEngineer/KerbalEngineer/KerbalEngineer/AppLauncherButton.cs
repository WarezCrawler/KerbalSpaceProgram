using KSP.UI.Screens;
using UnityEngine;

namespace KerbalEngineer;

public class AppLauncherButton : MonoBehaviour
{
	private static Texture iconTexture;

	private ApplicationLauncherButton button;

	public ApplicationLauncherButton Button => button;

	public bool IsOn
	{
		get
		{
			//IL_0026: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Invalid comparison between Unknown and I4
			if ((Object)(object)button != (Object)null && button.IsEnabled)
			{
				return (int)button.toggleButton.CurrentState == 0;
			}
			return false;
		}
		set
		{
			if (!((Object)(object)button == (Object)null))
			{
				if (value)
				{
					SetOn();
				}
				else
				{
					SetOff();
				}
			}
		}
	}

	public void Disable()
	{
		if ((Object)(object)button != (Object)null && button.IsEnabled)
		{
			button.Disable(true);
		}
	}

	public void Enable()
	{
		if ((Object)(object)button != (Object)null && !button.IsEnabled)
		{
			button.Enable(true);
		}
	}

	public Vector3 GetAnchor()
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)button == (Object)null)
		{
			return Vector3.zero;
		}
		Vector3 anchor = button.GetAnchor();
		anchor.x -= 3f;
		return anchor;
	}

	public void SetOff()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Invalid comparison between Unknown and I4
		Enable();
		if ((Object)(object)button != (Object)null && (int)button.toggleButton.CurrentState != 1)
		{
			button.SetFalse(true);
		}
	}

	public void SetOn()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		Enable();
		if ((Object)(object)button != (Object)null && (int)button.toggleButton.CurrentState != 0)
		{
			button.SetTrue(true);
		}
	}

	protected virtual void Awake()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		if ((Object)(object)iconTexture == (Object)null && (Object)(object)AssetBundleLoader.Images != (Object)null)
		{
			iconTexture = (Texture)(object)AssetBundleLoader.Images.LoadAsset<Texture2D>("app-launcher-icon");
		}
		GameEvents.onGUIApplicationLauncherReady.Add(new OnEvent(OnGUIApplicationLauncherReady));
		GameEvents.onGUIApplicationLauncherUnreadifying.Add((OnEvent<GameScenes>)OnGUIApplicationLauncherUnreadifying);
	}

	protected virtual void OnDestroy()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		GameEvents.onGUIApplicationLauncherReady.Remove(new OnEvent(OnGUIApplicationLauncherReady));
		GameEvents.onGUIApplicationLauncherUnreadifying.Remove((OnEvent<GameScenes>)OnGUIApplicationLauncherUnreadifying);
	}

	protected virtual void OnDisable()
	{
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnFalse()
	{
	}

	protected virtual void OnHover()
	{
	}

	protected virtual void OnHoverOut()
	{
	}

	protected virtual void OnReady()
	{
	}

	protected virtual void OnTrue()
	{
	}

	protected virtual void OnUnreadifying()
	{
	}

	private void OnGUIApplicationLauncherReady()
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006c: Expected O, but got Unknown
		//IL_006c: Expected O, but got Unknown
		//IL_006c: Expected O, but got Unknown
		//IL_006c: Expected O, but got Unknown
		//IL_006c: Expected O, but got Unknown
		if ((Object)(object)ApplicationLauncher.Instance != (Object)null)
		{
			button = ApplicationLauncher.Instance.AddModApplication(new Callback(OnTrue), new Callback(OnFalse), new Callback(OnHover), new Callback(OnHoverOut), new Callback(OnEnable), new Callback(OnDisable), (AppScenes)(-1), iconTexture);
		}
		OnReady();
	}

	private void OnGUIApplicationLauncherUnreadifying(GameScenes scene)
	{
		if ((Object)(object)ApplicationLauncher.Instance != (Object)null && (Object)(object)button != (Object)null)
		{
			ApplicationLauncher.Instance.RemoveModApplication(button);
		}
		OnUnreadifying();
	}
}
