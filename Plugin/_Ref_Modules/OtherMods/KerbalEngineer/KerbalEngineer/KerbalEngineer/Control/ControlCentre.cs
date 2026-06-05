using System;
using System.Collections.Generic;
using KerbalEngineer.Control.Panels;
using UnityEngine;

namespace KerbalEngineer.Control;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class ControlCentre : MonoBehaviour
{
	private static readonly List<IControlPanel> panels = new List<IControlPanel>();

	private static GUIStyle button;

	private static ControlCentre instance;

	private static GUIStyle label;

	private static GUIStyle title;

	private Vector2 contentsScrollPosition;

	private GUIStyle panelSelectorStyle;

	private Rect position = new Rect((float)Screen.width, (float)Screen.height, 900f, 500f);

	private IControlPanel selectedPanel;

	private bool shouldCentre = true;

	public static GUIStyle Button
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0039: Expected O, but got Unknown
			object obj = button;
			if (obj == null)
			{
				GUIStyle val = new GUIStyle(HighLogic.Skin.button);
				val.normal.textColor = Color.white;
				val.fixedHeight = 30f;
				button = val;
				obj = (object)val;
			}
			return (GUIStyle)obj;
		}
	}

	public static bool Enabled
	{
		get
		{
			return ((Behaviour)instance).enabled;
		}
		set
		{
			((Behaviour)instance).enabled = value;
		}
	}

	public static GUIStyle Label
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_002f: Unknown result type (might be due to invalid IL or missing references)
			//IL_003a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0041: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Expected O, but got Unknown
			object obj = label;
			if (obj == null)
			{
				GUIStyle val = new GUIStyle(HighLogic.Skin.label);
				val.normal.textColor = Color.white;
				val.fontStyle = (FontStyle)1;
				val.fixedHeight = 30f;
				val.alignment = (TextAnchor)3;
				val.stretchWidth = true;
				label = val;
				obj = (object)val;
			}
			return (GUIStyle)obj;
		}
	}

	public static List<IControlPanel> Panels => panels;

	public static GUIStyle Title
	{
		get
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_001e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0028: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Unknown result type (might be due to invalid IL or missing references)
			//IL_0037: Unknown result type (might be due to invalid IL or missing references)
			//IL_003e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0045: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Expected O, but got Unknown
			object obj = title;
			if (obj == null)
			{
				GUIStyle val = new GUIStyle(HighLogic.Skin.label);
				val.normal.textColor = Color.white;
				val.fontSize = 26;
				val.fontStyle = (FontStyle)1;
				val.alignment = (TextAnchor)1;
				val.stretchWidth = true;
				title = val;
				obj = (object)val;
			}
			return (GUIStyle)obj;
		}
	}

	protected void Awake()
	{
		try
		{
			if ((Object)(object)instance == (Object)null)
			{
				Object.DontDestroyOnLoad((Object)(object)this);
				instance = this;
				((Behaviour)this).enabled = false;
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

	protected void OnGUI()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Expected O, but got Unknown
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			GUI.skin = null;
			position = GUILayout.Window(((Object)this).GetInstanceID(), position, new WindowFunction(Window), "KERBAL ENGINEER REDUX 1.1.7.1p - CONTROL CENTRE", HighLogic.Skin.window, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			CentreWindow();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void Start()
	{
		try
		{
			InitialiseStyles();
			LoadPanels();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private static void LoadPanels()
	{
		panels.Add(new BuildEngineerPanel());
		panels.Add(new BuildOverlayPanel());
	}

	private void CentreWindow()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		if (shouldCentre && ((Rect)(ref position)).width > 0f && ((Rect)(ref position)).height > 0f)
		{
			((Rect)(ref position)).center = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
			shouldCentre = false;
		}
	}

	private void DrawContents()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin = HighLogic.Skin;
		contentsScrollPosition = GUILayout.BeginScrollView(contentsScrollPosition, false, true, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUI.skin = null;
		if (selectedPanel != null)
		{
			selectedPanel.Draw();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndScrollView();
	}

	private void DrawSelectors()
	{
		GUILayout.BeginVertical(HighLogic.Skin.box, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(225f) });
		foreach (IControlPanel panel in panels)
		{
			if (GUILayout.Toggle(selectedPanel == panel, panel.Name, panelSelectorStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
			{
				selectedPanel = panel;
			}
		}
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("CLOSE", Button, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			((Behaviour)this).enabled = false;
		}
		GUILayout.EndVertical();
	}

	private void InitialiseStyles()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Expected O, but got Unknown
		panelSelectorStyle = new GUIStyle(Button)
		{
			fontSize = 16,
			fixedHeight = 40f
		};
	}

	private void Window(int windowId)
	{
		try
		{
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			DrawSelectors();
			DrawContents();
			GUILayout.EndHorizontal();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
