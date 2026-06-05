using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using KerbalEngineer.Flight.Presets;
using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Sections;

public class SectionModule : ISectionModule
{
	private SectionEditor editor;

	private bool isHud;

	private int numberOfReadouts;

	public bool showButton = true;

	private GUIStyle boxStyle;

	private GUIStyle buttonStyle;

	private GUIStyle messageStyle;

	private GUIStyle titleStyle;

	public string Abbreviation { get; set; }

	public float EditorPositionX { get; set; }

	public float EditorPositionY { get; set; }

	public float FloatingPositionX { get; set; }

	public float FloatingPositionY { get; set; }

	public bool showEditButton { get; set; } = true;


	public bool showFloatButton { get; set; } = true;


	public bool IsEditorVisible
	{
		get
		{
			return (Object)(object)editor != (Object)null;
		}
		set
		{
			if (value && (Object)(object)editor == (Object)null)
			{
				editor = FlightEngineerCore.Instance.AddSectionEditor(this);
			}
			else if (!value && (Object)(object)editor != (Object)null)
			{
				Object.Destroy((Object)(object)editor);
			}
		}
	}

	public bool IsFloating
	{
		get
		{
			return (Object)(object)Window != (Object)null;
		}
		set
		{
			if (value && (Object)(object)Window == (Object)null)
			{
				Window = FlightEngineerCore.Instance.AddSectionWindow(this);
			}
			else if (!value && (Object)(object)Window != (Object)null)
			{
				Object.Destroy((Object)(object)Window);
			}
		}
	}

	public bool IsHud
	{
		get
		{
			return isHud;
		}
		set
		{
			if (isHud != value)
			{
				isHud = value;
				if (isHud)
				{
					IsFloating = true;
				}
				if ((Object)(object)Window != (Object)null)
				{
					Window.RequestResize();
				}
			}
		}
	}

	public bool IsDeleted { get; set; }

	public bool IsHudBackground { get; set; }

	public bool IsVisible { get; set; }

	public int LineCount { get; set; }

	public string Name { get; set; }

	public string[] ReadoutModuleNames
	{
		get
		{
			return ReadoutModules.Select((ReadoutModule r) => string.Concat(r.Category, ".", r.GetType().Name)).ToArray();
		}
		set
		{
			ReadoutModules = value.Select(ReadoutLibrary.GetReadout).ToList();
		}
	}

	[XmlIgnore]
	public List<ReadoutModule> ReadoutModules { get; set; }

	[XmlIgnore]
	public SectionWindow Window { get; set; }

	public SectionModule()
	{
		FloatingPositionX = (float)Screen.width * 0.5f - 125f;
		FloatingPositionY = 100f;
		EditorPositionX = (float)Screen.width * 0.5f - 250f;
		EditorPositionY = (float)Screen.height * 0.5f - 250f;
		ReadoutModules = new List<ReadoutModule>();
		InitialiseStyles();
		GuiDisplaySize.OnSizeChanged += OnSizeChanged;
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
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Expected O, but got Unknown
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Expected O, but got Unknown
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Expected O, but got Unknown
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Expected O, but got Unknown
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Expected O, but got Unknown
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Expected O, but got Unknown
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Expected O, but got Unknown
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Expected O, but got Unknown
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_014d: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Expected O, but got Unknown
		boxStyle = new GUIStyle(HighLogic.Skin.box)
		{
			margin = new RectOffset(),
			padding = new RectOffset(5, 5, 5, 5)
		};
		GUIStyle val = new GUIStyle(HighLogic.Skin.label);
		val.normal.textColor = Color.white;
		val.margin = new RectOffset();
		val.padding = new RectOffset(2, 0, 5, 2);
		val.fontSize = (int)(13f * GuiDisplaySize.Offset);
		val.fontStyle = (FontStyle)1;
		val.stretchWidth = true;
		titleStyle = val;
		GUIStyle val2 = new GUIStyle(HighLogic.Skin.button);
		val2.normal.textColor = Color.white;
		val2.margin = new RectOffset(0, 0, 5, 3);
		val2.padding = new RectOffset();
		val2.fontSize = (int)(10f * GuiDisplaySize.Offset);
		val2.stretchHeight = true;
		val2.fixedWidth = 60f * GuiDisplaySize.Offset;
		buttonStyle = val2;
		GUIStyle val3 = new GUIStyle(HighLogic.Skin.label);
		val3.normal.textColor = Color.white;
		val3.margin = new RectOffset();
		val3.padding = new RectOffset();
		val3.alignment = (TextAnchor)4;
		val3.fontSize = (int)(12f * GuiDisplaySize.Offset);
		val3.fontStyle = (FontStyle)1;
		val3.fixedWidth = 220f * GuiDisplaySize.Offset;
		val3.fixedHeight = 20f * GuiDisplaySize.Offset;
		messageStyle = val3;
	}

	private void OnSizeChanged()
	{
		InitialiseStyles();
	}

	public void FixedUpdate()
	{
		if (!IsVisible)
		{
			return;
		}
		foreach (ReadoutModule readoutModule in ReadoutModules)
		{
			readoutModule.FixedUpdate();
		}
	}

	public void Update()
	{
		if (!IsVisible)
		{
			return;
		}
		foreach (ReadoutModule readoutModule in ReadoutModules)
		{
			readoutModule.Update();
		}
		if (numberOfReadouts != ReadoutModules.Count)
		{
			numberOfReadouts = ReadoutModules.Count;
			if (!IsFloating)
			{
				DisplayStack.Instance.RequestResize();
			}
			else
			{
				Window.RequestResize();
			}
		}
	}

	public void Draw()
	{
		if (IsVisible)
		{
			if (!IsHud)
			{
				DrawSectionTitleBar();
			}
			DrawReadoutModules();
		}
	}

	private void DrawReadoutModules()
	{
		if (!IsHud)
		{
			GUILayout.BeginVertical(boxStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		}
		LineCount = 0;
		if (ReadoutModules.Count > 0)
		{
			foreach (ReadoutModule readoutModule in ReadoutModules)
			{
				readoutModule.LineCountStart();
				readoutModule.Draw(this);
				readoutModule.LineCountEnd();
				LineCount += readoutModule.LineCount;
			}
		}
		else
		{
			GUILayout.Label("No readouts are installed.", messageStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			LineCount = 1;
		}
		if (!IsHud)
		{
			GUILayout.EndVertical();
		}
	}

	public void ApplyPreset(Preset preset)
	{
		if (preset != null)
		{
			Name = preset.Name;
			Abbreviation = preset.Abbreviation;
			ReadoutModuleNames = preset.ReadoutNames;
			IsHud = preset.IsHud;
			IsHudBackground = preset.IsHudBackground;
		}
	}

	private void DrawSectionTitleBar()
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label(Name.ToUpper(), titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (showEditButton)
		{
			IsEditorVisible = GUILayout.Toggle(IsEditorVisible, "EDIT", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		}
		if (showFloatButton)
		{
			IsFloating = GUILayout.Toggle(IsFloating, "FLOAT", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		}
		GUILayout.EndHorizontal();
	}

	public void ClearNullReadouts()
	{
		ReadoutModules.RemoveAll((ReadoutModule r) => r == null);
	}
}
