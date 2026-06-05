using System.Collections.Generic;
using KSP.UI;
using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Settings;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class FlightAppLauncher : AppLauncherButton, IFlightAppLauncher
{
	private static FlightAppLauncher instance;

	private FlightMenu flightMenu;

	private GameObject menuObject;

	private GameObject menuPrefab;

	public static FlightAppLauncher Instance => instance;

	public bool IsControlBarVisible
	{
		get
		{
			if ((Object)(object)DisplayStack.Instance != (Object)null)
			{
				return DisplayStack.Instance.ShowControlBar;
			}
			return false;
		}
		set
		{
			if ((Object)(object)DisplayStack.Instance != (Object)null)
			{
				DisplayStack.Instance.ShowControlBar = value;
			}
		}
	}

	public bool IsDisplayStackVisible
	{
		get
		{
			if ((Object)(object)DisplayStack.Instance != (Object)null)
			{
				return !DisplayStack.Instance.Hidden;
			}
			return false;
		}
		set
		{
			if ((Object)(object)DisplayStack.Instance != (Object)null)
			{
				DisplayStack.Instance.Hidden = !value;
			}
		}
	}

	public static bool IsHoverActivated
	{
		get
		{
			try
			{
				return GeneralSettings.Handler.Get("FlightAppLauncher_IsHoverActivated", defaultObject: true);
			}
			catch
			{
				return true;
			}
		}
		set
		{
			GeneralSettings.Handler.Set("FlightAppLauncher_IsHoverActivated", value);
		}
	}

	public void ApplyTheme(GameObject gameObject)
	{
		StyleManager.Process(gameObject);
	}

	public void ClampToScreen(RectTransform rectTransform)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		UIMasterController.ClampToScreen(rectTransform, Vector2.zero);
	}

	IList<ISectionModule> IFlightAppLauncher.GetCustomSections()
	{
		return new List<ISectionModule>(SectionLibrary.CustomSections.ToArray());
	}

	IList<ISectionModule> IFlightAppLauncher.GetStockSections()
	{
		return new List<ISectionModule>(SectionLibrary.StockSections.ToArray());
	}

	public ISectionModule NewCustomSection()
	{
		SectionModule sectionModule = new SectionModule
		{
			Name = "Custom " + (SectionLibrary.CustomSections.Count + 1),
			Abbreviation = "CUST " + (SectionLibrary.CustomSections.Count + 1),
			IsVisible = true,
			IsEditorVisible = true
		};
		SectionLibrary.CustomSections.Add(sectionModule);
		return sectionModule;
	}

	protected override void Awake()
	{
		base.Awake();
		instance = this;
		if ((Object)(object)menuPrefab == (Object)null && (Object)(object)AssetBundleLoader.Prefabs != (Object)null)
		{
			menuPrefab = AssetBundleLoader.Prefabs.LoadAsset<GameObject>("FlightMenu");
		}
	}

	protected override void OnFalse()
	{
		Close();
	}

	protected override void OnHover()
	{
		if (IsHoverActivated)
		{
			Open();
		}
	}

	protected override void OnHoverOut()
	{
		if (!base.IsOn && IsHoverActivated)
		{
			Close();
		}
	}

	protected override void OnTrue()
	{
		Open();
	}

	protected virtual void Update()
	{
		if (FlightEngineerCore.IsDisplayable)
		{
			Enable();
		}
		else if (!FlightEngineerCore.IsDisplayable)
		{
			Disable();
		}
	}

	private void Close()
	{
		if ((Object)(object)flightMenu != (Object)null)
		{
			flightMenu.Close();
		}
		else if ((Object)(object)menuObject != (Object)null)
		{
			Object.Destroy((Object)(object)menuObject);
		}
	}

	private void Open()
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)flightMenu != (Object)null)
		{
			flightMenu.FadeIn();
		}
		else
		{
			if ((Object)(object)menuPrefab == (Object)null || (Object)(object)menuObject != (Object)null)
			{
				return;
			}
			menuObject = Object.Instantiate<GameObject>(menuPrefab, GetAnchor(), Quaternion.identity);
			if (!((Object)(object)menuObject == (Object)null))
			{
				StyleManager.Process(menuObject);
				menuObject.transform.SetParent(((Component)MainCanvasUtil.MainCanvas).transform);
				flightMenu = menuObject.GetComponent<FlightMenu>();
				if ((Object)(object)flightMenu != (Object)null)
				{
					flightMenu.SetFlightAppLauncher(this);
				}
			}
		}
	}
}
