using System;
using System.Collections.Generic;
using System.Linq;
using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Sections;
using KerbalEngineer.KeyBinding;
using KerbalEngineer.Settings;
using UnityEngine;

namespace KerbalEngineer.Flight;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class DisplayStack : MonoBehaviour
{
	private GUIStyle buttonStyle;

	private int numberOfStackSections;

	private bool resizeRequested;

	private bool showControlBar = true;

	private GUIStyle titleStyle;

	private int windowId;

	private Rect windowPosition;

	private GUIStyle windowStyle;

	public static DisplayStack Instance { get; private set; }

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
				MyLogger.Log("ActionMenu->Awake");
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
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
		MyLogger.Log("ActionMenu->OnDestroy");
	}

	protected void Start()
	{
		try
		{
			windowId = ((object)this).GetHashCode();
			InitialiseStyles();
			Load();
			MyLogger.Log("ActionMenu->Start");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void Update()
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (FlightEngineerCore.IsDisplayable && Input.GetKeyDown(KeyBinder.FlightShowHide))
			{
				Hidden = !Hidden;
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnGUI()
	{
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Expected O, but got Unknown
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!FlightEngineerCore.IsDisplayable)
			{
				return;
			}
			if (resizeRequested || numberOfStackSections != SectionLibrary.NumberOfStackSections)
			{
				numberOfStackSections = SectionLibrary.NumberOfStackSections;
				((Rect)(ref windowPosition)).width = 0f;
				((Rect)(ref windowPosition)).height = 0f;
				resizeRequested = false;
			}
			if (!Hidden && (SectionLibrary.NumberOfStackSections > 0 || ShowControlBar))
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
		List<SectionModule> list = new List<SectionModule>();
		list.AddRange(SectionLibrary.StockSections);
		list.AddRange(SectionLibrary.CustomSections);
		DrawControlBarButtons(list);
	}

	private void DrawControlBarButtons(IEnumerable<SectionModule> sections)
	{
		int num = 0;
		foreach (SectionModule item in sections.Where((SectionModule s) => s.showButton))
		{
			if (num % 4 == 0)
			{
				if (num > 0)
				{
					GUILayout.EndHorizontal();
				}
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
			item.IsVisible = GUILayout.Toggle(item.IsVisible, item.Abbreviation.ToUpper(), buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			num++;
		}
		if (num > 0)
		{
			GUILayout.EndHorizontal();
		}
	}

	private void DrawSections(IEnumerable<SectionModule> sections)
	{
		foreach (SectionModule section in sections)
		{
			if (!section.IsFloating)
			{
				section.Draw();
			}
		}
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
			SettingHandler settingHandler = SettingHandler.Load("DisplayStack.xml");
			Hidden = settingHandler.Get("hidden", Hidden);
			ShowControlBar = settingHandler.Get("showControlBar", ShowControlBar);
			((Rect)(ref windowPosition)).x = settingHandler.Get("windowPositionX", ((Rect)(ref windowPosition)).x);
			((Rect)(ref windowPosition)).y = settingHandler.Get("windowPositionY", ((Rect)(ref windowPosition)).y);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "DisplayStack->Load");
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
			settingHandler.Save("DisplayStack.xml");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "DisplayStack->Save");
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
			if (SectionLibrary.NumberOfStackSections > 0)
			{
				DrawSections(SectionLibrary.StockSections);
				DrawSections(SectionLibrary.CustomSections);
			}
			GUI.DragWindow();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "DisplayStack->Window");
		}
	}
}
