using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Presets;
using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.UIControls;
using UnityEngine;

namespace KerbalEngineer.TrackingStation;

public class SectionEditorTS : MonoBehaviour
{
	public const float Height = 500f;

	public const float Width = 500f;

	private GUIStyle categoryButtonActiveStyle;

	private GUIStyle categoryButtonStyle;

	private PopOutElement categoryList;

	private PopOutColorPicker colorPicker;

	private GUIStyle categoryTitleButtonStyle;

	private GUIStyle helpBoxStyle;

	private GUIStyle helpTextStyle;

	private GUIStyle panelTitleStyle;

	private Rect position;

	private PopOutElement presetList;

	private GUIStyle readoutButtonStyle;

	private GUIStyle readoutNameStyle;

	private Vector2 scrollPositionAvailable;

	private Vector2 scrollPositionInstalled;

	private GUIStyle textStyle;

	private GUIStyle windowStyle;

	private Texture2D swatch = new Texture2D(16, 20);

	private Rect scrollRectInstalled;

	private ReadoutModule editingReadout;

	public SectionModuleTS ParentSection { get; set; }

	public Rect Position
	{
		get
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			return position;
		}
		set
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			position = value;
		}
	}

	protected void Awake()
	{
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Expected O, but got Unknown
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Expected O, but got Unknown
		try
		{
			categoryList = ((Component)this).gameObject.AddComponent<PopOutElement>();
			categoryList.DrawCallback = new Callback(DrawCategories);
			presetList = ((Component)this).gameObject.AddComponent<PopOutElement>();
			presetList.DrawCallback = new Callback(DrawPresets);
			colorPicker = ((Component)this).gameObject.AddComponent<PopOutColorPicker>();
			colorPicker.DrawCallback = new Callback(DrawColorPicker);
			colorPicker.ClosedCallback = new Callback(saveColor);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void OnDestroy()
	{
	}

	protected void Start()
	{
		try
		{
			InitialiseStyles();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void saveColor()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (editingReadout != null)
		{
			if (editingReadout.ValueStyle.normal.textColor == HighLogic.Skin.label.normal.textColor)
			{
				ReadoutLibrary.RemoveReadoutConfig(editingReadout);
			}
			else
			{
				ReadoutLibrary.SaveReadoutConfig(editingReadout);
			}
		}
	}

	private void OnGUI()
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		position = GUILayout.Window(((Object)this).GetInstanceID(), position, new WindowFunction(Window), "EDIT SECTION - " + ParentSection.Name.ToUpper(), windowStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]).ClampToScreen();
		ParentSection.EditorPositionX = ((Rect)(ref position)).x;
		ParentSection.EditorPositionY = ((Rect)(ref position)).y;
	}

	private void DrawAvailableReadouts()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin = HighLogic.Skin;
		scrollPositionAvailable = GUILayout.BeginScrollView(scrollPositionAvailable, false, true, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(200f) });
		GUI.skin = null;
		GUILayout.Label("AVAILABLE", panelTitleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		foreach (ReadoutModule item in ReadoutLibrary.GetCategory(ReadoutCategory.Selected))
		{
			if (!ParentSection.ReadoutModules.Contains(item) || item.Cloneable)
			{
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(30f) });
				GUILayout.Label(item.Name, readoutNameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				item.ShowHelp = GUILayout.Toggle(item.ShowHelp, "?", readoutButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) });
				if (GUILayout.Button("INSTALL", readoutButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(75f) }))
				{
					ParentSection.ReadoutModules.Add(item);
				}
				GUILayout.EndHorizontal();
				ShowHelpMessage(item);
			}
		}
		GUILayout.EndScrollView();
	}

	private void DrawCategories()
	{
		foreach (ReadoutCategory category in ReadoutCategory.Categories)
		{
			if (!(category.Name != "Rendezvous") || !(category.Name != "Miscellaneous"))
			{
				string text = category.Description;
				if (text.Length > 50)
				{
					text = text.Substring(0, 49) + "...";
				}
				if (GUILayout.Button("<b>" + category.Name.ToUpper() + "</b>" + (string.IsNullOrEmpty(category.Description) ? string.Empty : ("\n<i>" + text + "</i>")), (category == ReadoutCategory.Selected) ? categoryButtonActiveStyle : categoryButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
				{
					ReadoutCategory.Selected = category;
					((Behaviour)categoryList).enabled = false;
				}
			}
		}
	}

	private void DrawCategorySelector()
	{
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0050: Invalid comparison between Unknown and I4
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		((Behaviour)categoryList).enabled = GUILayout.Toggle(((Behaviour)categoryList).enabled, "▼ SELECTED CATEGORY: " + ReadoutCategory.Selected.ToString().ToUpper() + " ▼", categoryTitleButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if ((int)Event.current.type == 7)
		{
			categoryList.SetPosition(GUILayoutUtility.GetLastRect().Translate(position), GUILayoutUtility.GetLastRect());
		}
	}

	private void DrawCustomOptions()
	{
	}

	private void DrawInstalledReadouts()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f2: Invalid comparison between Unknown and I4
		//IL_02f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_015e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c9: Invalid comparison between Unknown and I4
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0234: Unknown result type (might be due to invalid IL or missing references)
		GUI.skin = HighLogic.Skin;
		scrollPositionInstalled = GUILayout.BeginScrollView(scrollPositionInstalled, false, true, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUI.skin = null;
		GUILayout.Label("INSTALLED", panelTitleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		bool flag = false;
		int index = 0;
		for (int i = 0; i < ParentSection.ReadoutModules.Count; i++)
		{
			ReadoutModule readoutModule = ParentSection.ReadoutModules[i];
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height(30f) });
			GUILayout.Label(readoutModule.Name, readoutNameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			if (GUILayout.Button("▲", readoutButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) }) && i > 0)
			{
				ParentSection.ReadoutModules[i] = ParentSection.ReadoutModules[i - 1];
				ParentSection.ReadoutModules[i - 1] = readoutModule;
			}
			if (GUILayout.Button("▼", readoutButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) }) && i < ParentSection.ReadoutModules.Count - 1)
			{
				ParentSection.ReadoutModules[i] = ParentSection.ReadoutModules[i + 1];
				ParentSection.ReadoutModules[i + 1] = readoutModule;
			}
			Color color = GUI.color;
			GUI.color = readoutModule.ValueStyle.normal.textColor;
			if (!readoutModule.Cloneable)
			{
				if (GUILayout.Button((Texture)(object)swatch, readoutButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) }))
				{
					editingReadout = readoutModule;
					((Behaviour)colorPicker).enabled = true;
				}
				if ((int)Event.current.type == 7 && editingReadout == readoutModule)
				{
					colorPicker.SetPosition(GUILayoutUtility.GetLastRect().Translate(position).Translate(new Rect(8f, ((Rect)(ref scrollRectInstalled)).y - scrollPositionInstalled.y, 8f, 8f)), new Rect(0f, 0f, 180f, 20f));
				}
			}
			else
			{
				GUILayout.Label("", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(26f) });
			}
			GUI.color = color;
			readoutModule.ShowHelp = GUILayout.Toggle(readoutModule.ShowHelp, "?", readoutButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) });
			if (GUILayout.Button("REMOVE", readoutButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(75f) }))
			{
				flag = true;
				index = i;
			}
			GUILayout.EndHorizontal();
			ShowHelpMessage(readoutModule);
		}
		GUILayout.EndScrollView();
		if ((int)Event.current.type == 7)
		{
			scrollRectInstalled = GUILayoutUtility.GetLastRect();
		}
		if (flag)
		{
			ParentSection.ReadoutModules.RemoveAt(index);
		}
	}

	private void DrawPresetButton(Preset preset)
	{
		if (GUILayout.Button("<b>" + preset.Name.ToUpper() + "</b>", categoryButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			ParentSection.ApplyPreset(preset);
			((Behaviour)presetList).enabled = false;
		}
	}

	private void DrawPresetSaveButton()
	{
		if (GUILayout.Button("<b>SAVE PRESET</b>", categoryButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			SavePreset(PresetLibrary.Presets.Find((Preset p) => string.Equals(p.Name, ParentSection.Name, StringComparison.CurrentCultureIgnoreCase)));
		}
	}

	private void DrawPresetSelector()
	{
	}

	private void DrawPresets()
	{
		Preset preset = null;
		foreach (Preset preset2 in PresetLibrary.Presets)
		{
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			DrawPresetButton(preset2);
			if (GUILayout.Button("<b>X</b>", categoryButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) }))
			{
				preset = preset2;
			}
			GUILayout.EndHorizontal();
		}
		if (preset != null && PresetLibrary.Remove(preset))
		{
			presetList.Resize = true;
		}
		DrawPresetSaveButton();
	}

	private void InitialiseStyles()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Expected O, but got Unknown
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0053: Expected O, but got Unknown
		//IL_0053: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Expected O, but got Unknown
		//IL_00b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d3: Expected O, but got Unknown
		//IL_00d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00de: Expected O, but got Unknown
		//IL_00de: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Expected O, but got Unknown
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Expected O, but got Unknown
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Expected O, but got Unknown
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0165: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Expected O, but got Unknown
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Expected O, but got Unknown
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019c: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Expected O, but got Unknown
		//IL_01b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Expected O, but got Unknown
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e2: Expected O, but got Unknown
		//IL_01e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0204: Expected O, but got Unknown
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0224: Expected O, but got Unknown
		//IL_0224: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Expected O, but got Unknown
		//IL_023c: Expected O, but got Unknown
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_024c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Expected O, but got Unknown
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_0268: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Expected O, but got Unknown
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_0281: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028f: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Expected O, but got Unknown
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02be: Expected O, but got Unknown
		windowStyle = new GUIStyle(HighLogic.Skin.window);
		GUIStyle val = new GUIStyle(HighLogic.Skin.button);
		val.normal.textColor = Color.white;
		val.margin = new RectOffset(0, 0, 2, 0);
		val.padding = new RectOffset(5, 5, 5, 5);
		val.alignment = (TextAnchor)4;
		val.fontSize = 12;
		val.fontStyle = (FontStyle)0;
		val.richText = true;
		categoryButtonStyle = val;
		categoryButtonActiveStyle = new GUIStyle(categoryButtonStyle)
		{
			normal = categoryButtonStyle.onNormal,
			hover = categoryButtonStyle.onHover
		};
		GUIStyle val2 = new GUIStyle(HighLogic.Skin.label);
		val2.normal.textColor = Color.white;
		val2.margin = new RectOffset();
		val2.padding = new RectOffset();
		val2.alignment = (TextAnchor)3;
		val2.fontSize = 12;
		val2.fontStyle = (FontStyle)1;
		val2.fixedHeight = 30f;
		val2.stretchWidth = true;
		panelTitleStyle = val2;
		textStyle = new GUIStyle(HighLogic.Skin.textField)
		{
			margin = new RectOffset(3, 3, 3, 3),
			alignment = (TextAnchor)3,
			stretchWidth = true,
			stretchHeight = true
		};
		GUIStyle val3 = new GUIStyle(HighLogic.Skin.label);
		val3.normal.textColor = Color.white;
		val3.margin = new RectOffset();
		val3.padding = new RectOffset(10, 0, 0, 0);
		val3.alignment = (TextAnchor)3;
		val3.fontSize = 12;
		val3.fontStyle = (FontStyle)1;
		val3.stretchWidth = true;
		val3.stretchHeight = true;
		readoutNameStyle = val3;
		GUIStyle val4 = new GUIStyle(HighLogic.Skin.button);
		val4.normal.textColor = Color.white;
		val4.margin = new RectOffset(2, 2, 2, 2);
		val4.padding = new RectOffset();
		val4.alignment = (TextAnchor)4;
		val4.fontSize = 12;
		val4.fontStyle = (FontStyle)1;
		val4.stretchHeight = true;
		readoutButtonStyle = val4;
		helpBoxStyle = new GUIStyle(HighLogic.Skin.box)
		{
			margin = new RectOffset(2, 2, 2, 10),
			padding = new RectOffset(10, 10, 10, 10)
		};
		GUIStyle val5 = new GUIStyle(HighLogic.Skin.label);
		val5.normal.textColor = Color.yellow;
		val5.margin = new RectOffset();
		val5.padding = new RectOffset();
		val5.alignment = (TextAnchor)3;
		val5.fontSize = 13;
		val5.fontStyle = (FontStyle)0;
		val5.stretchWidth = true;
		val5.richText = true;
		helpTextStyle = val5;
		categoryTitleButtonStyle = new GUIStyle(readoutButtonStyle)
		{
			fixedHeight = 30f,
			stretchHeight = false
		};
	}

	private void SavePreset(Preset preset)
	{
		if (preset == null)
		{
			preset = new Preset();
		}
		preset.Name = ParentSection.Name;
		preset.Abbreviation = ParentSection.Abbreviation;
		preset.ReadoutNames = ParentSection.ReadoutModuleNames;
		preset.IsHud = ParentSection.IsHud;
		preset.IsHudBackground = ParentSection.IsHudBackground;
		PresetLibrary.Save(preset);
	}

	private void ShowHelpMessage(ReadoutModule readout)
	{
		if (readout.ShowHelp)
		{
			GUILayout.BeginVertical(helpBoxStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUILayout.Label((!string.IsNullOrEmpty(readout.HelpString)) ? readout.HelpString : "Sorry, no help information has been provided for this readout module.", helpTextStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUILayout.EndVertical();
		}
	}

	private void Window(int windowId)
	{
		try
		{
			DrawCustomOptions();
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			DrawCategorySelector();
			DrawPresetSelector();
			GUILayout.EndHorizontal();
			DrawAvailableReadouts();
			GUILayout.Space(5f);
			DrawInstalledReadouts();
			if (GUILayout.Button("CLOSE EDITOR", categoryTitleButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
			{
				ParentSection.IsEditorVisible = false;
			}
			GUI.DragWindow();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void DrawColorPicker()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		if (editingReadout != null)
		{
			editingReadout.ValueStyle.normal.textColor = colorPicker.DrawColorPicker(editingReadout.ValueStyle.normal.textColor);
		}
	}
}
