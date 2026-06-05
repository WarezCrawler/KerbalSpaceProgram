using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.Flight.Readouts.Rendezvous;
using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.TrackingStation;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class DisplayStackTS : MonoBehaviour
{
	private GUIStyle buttonStyle;

	private int numberOfStackSections;

	private bool resizeRequested;

	private bool showControlBar = true;

	private GUIStyle titleStyle;

	private int windowId;

	private Rect windowPosition;

	private GUIStyle windowStyle;

	public static SectionEditorTS editor;

	private ITargetable lastSource;

	private ITargetable lastTarget;

	public static DisplayStackTS Instance { get; private set; }

	public bool Hidden { get; set; }

	public bool ShowControlBar
	{
		get
		{
			return showControlBar;
		}
		set
		{
			if (showControlBar != value)
			{
				showControlBar = value;
				RequestResize();
			}
		}
	}

	public void RequestResize()
	{
		resizeRequested = true;
	}

	protected void Awake()
	{
		try
		{
			if ((Object)(object)Instance == (Object)null)
			{
				Instance = this;
				GuiDisplaySize.OnSizeChanged += OnSizeChanged;
				Debug.Log((object)"DisplayStackTS->Awake");
			}
			else
			{
				Object.Destroy((Object)(object)this);
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void OnDestroy()
	{
		try
		{
			Save();
			SectionLibrary.SaveTS();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
		MyLogger.Log("DisplayStackTS->OnDestroy");
	}

	internal SectionEditorTS MakeEditor()
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		editor = ((Component)this).gameObject.AddComponent<SectionEditorTS>();
		editor.ParentSection = SectionLibrary.TrackingStationSection;
		editor.Position = new Rect(SectionLibrary.TrackingStationSection.EditorPositionX, SectionLibrary.TrackingStationSection.EditorPositionY, 500f, 500f);
		ReadoutCategory.Selected = ReadoutCategory.GetCategory("Rendezvous");
		return editor;
	}

	protected void Start()
	{
		try
		{
			SectionLibrary.LoadTS();
			windowId = ((object)this).GetHashCode();
			InitialiseStyles();
			Load();
			Debug.Log((object)"DisplayStackTS->Start");
		}
		catch (Exception ex)
		{
			Debug.Log((object)((ex.ToString() + ex.InnerException == null) ? "" : ex.InnerException.ToString()));
		}
	}

	protected void Update()
	{
		try
		{
			SectionLibrary.TrackingStationSection.Update();
			RendezvousProcessor.Instance.Update();
			if (RendezvousProcessor.TrackingStationSource != lastSource)
			{
				RequestResize();
			}
			if (RendezvousProcessor.activeTarget != lastTarget)
			{
				RequestResize();
			}
			lastSource = RendezvousProcessor.TrackingStationSource;
			lastTarget = RendezvousProcessor.activeTarget;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnGUI()
	{
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Expected O, but got Unknown
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c7: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (resizeRequested)
			{
				numberOfStackSections = 1;
				((Rect)(ref windowPosition)).width = 0f;
				((Rect)(ref windowPosition)).height = 0f;
				resizeRequested = false;
			}
			if (!Hidden && ShowControlBar)
			{
				bool flag = ((Rect)(ref windowPosition)).min == Vector2.zero;
				GUI.skin = null;
				windowPosition = GUILayout.Window(windowId, windowPosition, new WindowFunction(Window), string.Empty, windowStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]).ClampToScreen();
				if (flag)
				{
					((Rect)(ref windowPosition)).center = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
				}
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void DrawControlBar()
	{
		GUILayout.Label("FLIGHT ENGINEER 1.1.7.1", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
	}

	private void InitialiseStyles()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Expected O, but got Unknown
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Expected O, but got Unknown
		//IL_002f: Expected O, but got Unknown
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Expected O, but got Unknown
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Expected O, but got Unknown
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bb: Expected O, but got Unknown
		//IL_00bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Expected O, but got Unknown
		windowStyle = new GUIStyle(HighLogic.Skin.window)
		{
			margin = new RectOffset(),
			padding = new RectOffset(5, 5, 0, 5)
		};
		titleStyle = new GUIStyle(HighLogic.Skin.label)
		{
			margin = new RectOffset(0, 0, 5, 3),
			padding = new RectOffset(),
			alignment = (TextAnchor)4,
			fontSize = (int)(13f * GuiDisplaySize.Offset),
			fontStyle = (FontStyle)1,
			stretchWidth = true
		};
		GUIStyle val = new GUIStyle(HighLogic.Skin.button);
		val.normal.textColor = Color.white;
		val.margin = new RectOffset();
		val.padding = new RectOffset();
		val.alignment = (TextAnchor)4;
		val.fontSize = (int)(11f * GuiDisplaySize.Offset);
		val.fontStyle = (FontStyle)1;
		val.fixedWidth = 60f * GuiDisplaySize.Offset;
		val.fixedHeight = 25f * GuiDisplaySize.Offset;
		buttonStyle = val;
	}

	private void Load()
	{
		try
		{
			SettingHandler settingHandler = SettingHandler.Load("DisplayStackTS.xml");
			Hidden = settingHandler.Get("hidden", Hidden);
			ShowControlBar = settingHandler.Get("showControlBar", ShowControlBar);
			((Rect)(ref windowPosition)).x = settingHandler.Get("windowPositionX", ((Rect)(ref windowPosition)).x);
			((Rect)(ref windowPosition)).y = settingHandler.Get("windowPositionY", ((Rect)(ref windowPosition)).y);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "DisplayStackTS->Load");
		}
	}

	private void OnSizeChanged()
	{
		InitialiseStyles();
		RequestResize();
	}

	private void Save()
	{
		try
		{
			SettingHandler settingHandler = new SettingHandler();
			settingHandler.Set("hidden", Hidden);
			settingHandler.Set("showControlBar", ShowControlBar);
			settingHandler.Set("windowPositionX", ((Rect)(ref windowPosition)).x);
			settingHandler.Set("windowPositionY", ((Rect)(ref windowPosition)).y);
			settingHandler.Save("DisplayStackTS.xml");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "DisplayStackTS->Save");
		}
	}

	private void Window(int windowId)
	{
		try
		{
			if (ShowControlBar)
			{
				DrawControlBar();
			}
			SectionLibrary.TrackingStationSection.Name = "TRACKING";
			ITargetable trackingStationSource = RendezvousProcessor.TrackingStationSource;
			if (trackingStationSource != null)
			{
				SectionLibrary.TrackingStationSection.Name = "TRACKING (REF: " + RendezvousProcessor.nameForTargetable(trackingStationSource) + ")";
			}
			SectionLibrary.TrackingStationSection.Draw();
			GUI.DragWindow();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "DisplayStackTS->Window");
		}
	}
}
