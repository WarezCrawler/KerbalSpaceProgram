using System;
using KerbalEngineer.Helpers;
using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.Editor;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class BuildOverlay : MonoBehaviour
{
	private static BuildOverlayPartInfo buildOverlayPartInfo;

	private static BuildOverlayResources buildOverlayResources;

	private static BuildOverlayVessel buildOverlayVessel;

	private static BuildOverlay instance;

	private static float minimumWidth = 200f;

	private static GUIStyle nameStyle;

	private static float tabSpeed = 5f;

	private static GUIStyle tabStyle;

	private static GUIStyle titleStyle;

	private static GUIStyle valueStyle;

	private static GUIStyle windowStyle;

	private static bool hasChanged;

	public static BuildOverlayPartInfo BuildOverlayPartInfo => buildOverlayPartInfo;

	public static BuildOverlayResources BuildOverlayResources => buildOverlayResources;

	public static BuildOverlayVessel BuildOverlayVessel => buildOverlayVessel;

	public static float MinimumWidth
	{
		get
		{
			return minimumWidth;
		}
		set
		{
			minimumWidth = value;
		}
	}

	public static GUIStyle NameStyle
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			//IL_0051: Expected O, but got Unknown
			if (nameStyle == null || hasChanged)
			{
				GUIStyle val = new GUIStyle();
				val.normal.textColor = Color.white;
				val.fontSize = (int)(11f * GuiDisplaySize.Offset);
				val.fontStyle = (FontStyle)1;
				val.alignment = (TextAnchor)0;
				val.stretchWidth = true;
				nameStyle = val;
				return val;
			}
			return nameStyle;
		}
	}

	public static float TabSpeed
	{
		get
		{
			return tabSpeed;
		}
		set
		{
			tabSpeed = value;
		}
	}

	public static GUIStyle TabStyle
	{
		get
		{
			//IL_0016: Unknown result type (might be due to invalid IL or missing references)
			//IL_001b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_0044: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0054: Unknown result type (might be due to invalid IL or missing references)
			//IL_006e: Unknown result type (might be due to invalid IL or missing references)
			//IL_007d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0083: Unknown result type (might be due to invalid IL or missing references)
			//IL_008d: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
			//IL_00b6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
			//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
			//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ef: Unknown result type (might be due to invalid IL or missing references)
			//IL_00f5: Unknown result type (might be due to invalid IL or missing references)
			//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
			//IL_0106: Unknown result type (might be due to invalid IL or missing references)
			//IL_0110: Expected O, but got Unknown
			//IL_0110: Unknown result type (might be due to invalid IL or missing references)
			//IL_0122: Unknown result type (might be due to invalid IL or missing references)
			//IL_0129: Unknown result type (might be due to invalid IL or missing references)
			//IL_0130: Unknown result type (might be due to invalid IL or missing references)
			//IL_013b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0142: Unknown result type (might be due to invalid IL or missing references)
			//IL_0148: Expected O, but got Unknown
			//IL_0149: Expected O, but got Unknown
			if (!((tabStyle != null) & !hasChanged))
			{
				GUIStyle val = new GUIStyle();
				val.normal.background = TextureHelper.CreateTextureFromColour(new Color(0f, 0f, 0f, 0.5f));
				val.normal.textColor = Color.yellow;
				val.hover.background = TextureHelper.CreateTextureFromColour(new Color(0f, 0f, 0f, 0.75f));
				val.hover.textColor = Color.yellow;
				val.onNormal.background = TextureHelper.CreateTextureFromColour(new Color(0f, 0f, 0f, 0.5f));
				val.onNormal.textColor = Color.yellow;
				val.onHover.background = TextureHelper.CreateTextureFromColour(new Color(0f, 0f, 0f, 0.75f));
				val.onHover.textColor = Color.yellow;
				val.padding = new RectOffset(20, 20, 0, 0);
				val.fontSize = (int)(11f * GuiDisplaySize.Offset);
				val.fontStyle = (FontStyle)1;
				val.alignment = (TextAnchor)4;
				val.fixedHeight = 15f;
				val.stretchWidth = true;
				tabStyle = val;
				return val;
			}
			return tabStyle;
		}
	}

	public static GUIStyle TitleStyle
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Expected O, but got Unknown
			//IL_004a: Expected O, but got Unknown
			if (titleStyle == null || hasChanged)
			{
				GUIStyle val = new GUIStyle();
				val.normal.textColor = Color.yellow;
				val.fontSize = (int)(11f * GuiDisplaySize.Offset);
				val.fontStyle = (FontStyle)1;
				val.stretchWidth = true;
				titleStyle = val;
				return val;
			}
			return titleStyle;
		}
	}

	public static GUIStyle ValueStyle
	{
		get
		{
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0019: Unknown result type (might be due to invalid IL or missing references)
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0035: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0043: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Expected O, but got Unknown
			//IL_0051: Expected O, but got Unknown
			if (valueStyle == null || hasChanged)
			{
				GUIStyle val = new GUIStyle();
				val.normal.textColor = Color.white;
				val.fontSize = (int)(11f * GuiDisplaySize.Offset);
				val.fontStyle = (FontStyle)0;
				val.alignment = (TextAnchor)2;
				val.stretchWidth = true;
				valueStyle = val;
				return val;
			}
			return valueStyle;
		}
	}

	public static bool Visible
	{
		get
		{
			if (BuildOverlayPartInfo.Visible && BuildOverlayVessel.Visible)
			{
				return BuildOverlayResources.Visible;
			}
			return false;
		}
		set
		{
			bool visible = (BuildOverlayResources.Visible = value);
			BuildOverlayVessel.Visible = visible;
			BuildOverlayPartInfo.Visible = visible;
		}
	}

	public static GUIStyle WindowStyle
	{
		get
		{
			//IL_0009: Unknown result type (might be due to invalid IL or missing references)
			//IL_000e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004c: Expected O, but got Unknown
			object obj = windowStyle;
			if (obj == null)
			{
				GUIStyle val = new GUIStyle();
				val.normal.background = TextureHelper.CreateTextureFromColour(new Color(0f, 0f, 0f, 0.5f));
				val.padding = new RectOffset(5, 5, 3, 3);
				windowStyle = val;
				obj = (object)val;
			}
			return (GUIStyle)obj;
		}
	}

	protected void OnGUI()
	{
	}

	public static void Load()
	{
		SettingHandler settingHandler = SettingHandler.Load("BuildOverlay.xml");
		Visible = settingHandler.GetSet("visible", Visible);
		BuildOverlayPartInfo.NamesOnly = settingHandler.GetSet("namesOnly", BuildOverlayPartInfo.NamesOnly);
		BuildOverlayPartInfo.ClickToOpen = settingHandler.GetSet("clickToOpen", BuildOverlayPartInfo.ClickToOpen);
		buildOverlayVessel.Open = settingHandler.GetSet("vesselOpen", buildOverlayVessel.Open);
		buildOverlayResources.Open = settingHandler.GetSet("resourcesOpen", buildOverlayResources.Open);
		buildOverlayVessel.WindowX = settingHandler.GetSet("vesselWindowX", buildOverlayVessel.WindowX);
		settingHandler.Save("BuildOverlay.xml");
	}

	public static void Save()
	{
		SettingHandler settingHandler = SettingHandler.Load("BuildOverlay.xml");
		settingHandler.Set("visible", Visible);
		settingHandler.Set("namesOnly", BuildOverlayPartInfo.NamesOnly);
		settingHandler.Set("clickToOpen", BuildOverlayPartInfo.ClickToOpen);
		settingHandler.Set("vesselOpen", buildOverlayVessel.Open);
		settingHandler.Set("resourcesOpen", buildOverlayResources.Open);
		settingHandler.Set("vesselWindowX", buildOverlayVessel.WindowX);
		settingHandler.Save("BuildOverlay.xml");
	}

	protected void Awake()
	{
		try
		{
			if ((Object)(object)instance != (Object)null)
			{
				Object.Destroy((Object)(object)this);
				return;
			}
			instance = this;
			buildOverlayPartInfo = ((Component)this).gameObject.AddComponent<BuildOverlayPartInfo>();
			buildOverlayVessel = ((Component)this).gameObject.AddComponent<BuildOverlayVessel>();
			buildOverlayResources = ((Component)this).gameObject.AddComponent<BuildOverlayResources>();
			Load();
			GuiDisplaySize.OnSizeChanged += OnSizeChanged;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnSizeChanged()
	{
		hasChanged = true;
	}

	protected void OnDestroy()
	{
		try
		{
			Save();
			if ((Object)(object)buildOverlayPartInfo != (Object)null)
			{
				Object.Destroy((Object)(object)buildOverlayPartInfo);
			}
			if ((Object)(object)buildOverlayVessel != (Object)null)
			{
				Object.Destroy((Object)(object)buildOverlayVessel);
			}
			if ((Object)(object)buildOverlayResources != (Object)null)
			{
				Object.Destroy((Object)(object)buildOverlayResources);
			}
			GuiDisplaySize.OnSizeChanged -= OnSizeChanged;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
