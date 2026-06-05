using System;
using KSP.UI.Screens;
using UnityEngine;

namespace KerbalEngineer.Flight;

public class ActionMenu : MonoBehaviour
{
	private ActionMenuGui actionMenuGui;

	private ApplicationLauncherButton button;

	protected void Awake()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		try
		{
			GameEvents.onGUIApplicationLauncherReady.Add(new OnEvent(OnGuiAppLauncherReady));
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
		MyLogger.Log("ActionMenu was created.");
	}

	protected void Start()
	{
		if ((Object)(object)button == (Object)null)
		{
			OnGuiAppLauncherReady();
		}
	}

	protected void OnDestroy()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Expected O, but got Unknown
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Expected O, but got Unknown
		try
		{
			GameEvents.onGUIApplicationLauncherReady.Remove(new OnEvent(OnGuiAppLauncherReady));
			GameEvents.onHideUI.Remove(new OnEvent(OnHide));
			GameEvents.onShowUI.Remove(new OnEvent(OnShow));
			if ((Object)(object)button != (Object)null)
			{
				ApplicationLauncher.Instance.RemoveModApplication(button);
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
		MyLogger.Log("ActionMenu was destroyed.");
	}

	protected void Update()
	{
		try
		{
			if (!((Object)(object)button == (Object)null))
			{
				if (FlightEngineerCore.IsDisplayable && !button.toggleButton.Interactable)
				{
					button.Enable(true);
				}
				else if (!FlightEngineerCore.IsDisplayable && button.toggleButton.Interactable)
				{
					button.Disable(true);
				}
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnFalse()
	{
		try
		{
			((Behaviour)actionMenuGui).enabled = false;
			actionMenuGui.StayOpen = false;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnGuiAppLauncherReady()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004e: Expected O, but got Unknown
		//IL_004e: Expected O, but got Unknown
		//IL_004e: Expected O, but got Unknown
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected O, but got Unknown
		//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Expected O, but got Unknown
		try
		{
			button = ApplicationLauncher.Instance.AddModApplication(new Callback(OnTrue), new Callback(OnFalse), new Callback(OnHover), new Callback(OnHoverOut), (Callback)null, (Callback)null, (AppScenes)(-1), (Texture)(object)GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/ToolbarIcon", false));
			actionMenuGui = ((Component)button).gameObject.AddComponent<ActionMenuGui>();
			((Component)actionMenuGui).transform.parent = ((Component)button).transform;
			ApplicationLauncher.Instance.EnableMutuallyExclusive(button);
			GameEvents.onHideUI.Add(new OnEvent(OnHide));
			GameEvents.onShowUI.Add(new OnEvent(OnShow));
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnHide()
	{
		try
		{
			actionMenuGui.Hidden = true;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnHover()
	{
		try
		{
			((Behaviour)actionMenuGui).enabled = true;
			actionMenuGui.Hovering = true;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnHoverOut()
	{
		try
		{
			actionMenuGui.Hovering = false;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnShow()
	{
		try
		{
			actionMenuGui.Hidden = false;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnTrue()
	{
		try
		{
			((Behaviour)actionMenuGui).enabled = true;
			actionMenuGui.StayOpen = true;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
