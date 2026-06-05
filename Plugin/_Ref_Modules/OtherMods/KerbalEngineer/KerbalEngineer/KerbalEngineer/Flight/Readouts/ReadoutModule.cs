using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts;

public abstract class ReadoutModule
{
	private int lineCountEnd;

	private int lineCountStart;

	public ReadoutCategory Category { get; set; }

	public bool Cloneable { get; set; }

	public float ContentWidth => 230f * GuiDisplaySize.Offset;

	public GUIStyle FlexiLabelStyle { get; set; }

	public string HelpString { get; set; }

	public bool IsDefault { get; set; }

	public int LineCount { get; private set; }

	public string Name { get; set; }

	public bool ResizeRequested { get; set; }

	public bool ShowHelp { get; set; }

	public GUIStyle TextFieldStyle { get; set; }

	public GUIStyle ValueStyle { get; set; }

	public GUIStyle NameStyle { get; set; }

	public GUIStyle MessageStyle { get; set; }

	public GUIStyle ButtonStyle { get; set; }

	public GUIStyle CompactButtonStyle { get; set; }

	protected ReadoutModule()
	{
		InitialiseStyles(force: false);
		GuiDisplaySize.OnSizeChanged += OnSizeChanged;
	}

	public virtual void Draw(ISectionModule section)
	{
	}

	public virtual void FixedUpdate()
	{
	}

	public void LineCountEnd()
	{
		LineCount = lineCountEnd;
		if (lineCountEnd.CompareTo(lineCountStart) < 0)
		{
			ResizeRequested = true;
		}
	}

	public void LineCountStart()
	{
		lineCountStart = lineCountEnd;
		lineCountEnd = 0;
	}

	public virtual void Reset()
	{
	}

	public virtual void Update()
	{
	}

	protected void DrawLine(string value, bool compact)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(ContentWidth) });
		if (!compact)
		{
			GUILayout.Label(Name, NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(value.ToLength(20), ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		}
		else
		{
			GUILayout.Label(Name, NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height((float)NameStyle.fontSize * 1.2f) });
			GUILayout.FlexibleSpace();
			GUILayout.Label(value.ToLength(20), ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height((float)ValueStyle.fontSize * 1.2f) });
		}
		GUILayout.EndHorizontal();
		lineCountEnd++;
	}

	protected void DrawLine(string name, string value, bool compact = false)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(ContentWidth) });
		if (!compact)
		{
			GUILayout.Label(name, NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUILayout.FlexibleSpace();
			GUILayout.Label(value.ToLength(20), ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		}
		else
		{
			GUILayout.Label(name, NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height((float)NameStyle.fontSize * 1.2f) });
			GUILayout.FlexibleSpace();
			GUILayout.Label(value.ToLength(20), ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height((float)ValueStyle.fontSize * 1.2f) });
		}
		GUILayout.EndHorizontal();
		lineCountEnd++;
	}

	protected void DrawLine(Action drawAction, bool showName = true, bool compact = false)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(ContentWidth) });
		if (showName)
		{
			if (!compact)
			{
				GUILayout.Label(Name, NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
			else
			{
				GUILayout.Label(Name, NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height((float)NameStyle.fontSize * 1.2f) });
			}
			GUILayout.FlexibleSpace();
		}
		drawAction();
		GUILayout.EndHorizontal();
		lineCountEnd++;
	}

	protected void DrawMessageLine(string value, bool compact = false)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(ContentWidth) });
		if (!compact)
		{
			GUILayout.Label(value, MessageStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		}
		else
		{
			GUILayout.Label(value, MessageStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Height((float)MessageStyle.fontSize * 1.2f) });
		}
		GUILayout.EndHorizontal();
		lineCountEnd++;
	}

	private void InitialiseStyles(bool force)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Expected O, but got Unknown
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Expected O, but got Unknown
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Expected O, but got Unknown
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cc: Expected O, but got Unknown
		//IL_00cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Expected O, but got Unknown
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Expected O, but got Unknown
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0121: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Expected O, but got Unknown
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Expected O, but got Unknown
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_014e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0167: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Expected O, but got Unknown
		//IL_018b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a7: Expected O, but got Unknown
		//IL_01b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Expected O, but got Unknown
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e1: Expected O, but got Unknown
		//IL_01e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0210: Expected O, but got Unknown
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_023d: Expected O, but got Unknown
		//IL_023d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0254: Expected O, but got Unknown
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0264: Unknown result type (might be due to invalid IL or missing references)
		//IL_0269: Unknown result type (might be due to invalid IL or missing references)
		//IL_0273: Expected O, but got Unknown
		//IL_0273: Unknown result type (might be due to invalid IL or missing references)
		//IL_0278: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Expected O, but got Unknown
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0289: Unknown result type (might be due to invalid IL or missing references)
		//IL_029b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Expected O, but got Unknown
		//IL_02bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		if (NameStyle == null || force)
		{
			ReadoutModule readout = ReadoutLibrary.GetReadout(Name);
			Color textColor = HighLogic.Skin.label.normal.textColor;
			if (readout != null)
			{
				textColor = readout.ValueStyle.normal.textColor;
			}
			GUIStyle val = new GUIStyle(HighLogic.Skin.label);
			val.normal.textColor = Color.white;
			val.margin = new RectOffset();
			val.padding = new RectOffset(5, 0, 0, 0);
			val.alignment = (TextAnchor)3;
			val.fontSize = (int)(11f * GuiDisplaySize.Offset);
			val.fontStyle = (FontStyle)1;
			val.fixedHeight = 20f * GuiDisplaySize.Offset;
			NameStyle = val;
			ValueStyle = new GUIStyle(HighLogic.Skin.label)
			{
				margin = new RectOffset(),
				padding = new RectOffset(0, 5, 0, 0),
				alignment = (TextAnchor)5,
				fontSize = (int)(11f * GuiDisplaySize.Offset),
				fontStyle = (FontStyle)0,
				fixedHeight = 20f * GuiDisplaySize.Offset
			};
			GUIStyle val2 = new GUIStyle(HighLogic.Skin.label);
			val2.normal.textColor = Color.white;
			val2.margin = new RectOffset();
			val2.padding = new RectOffset();
			val2.alignment = (TextAnchor)4;
			val2.fontSize = (int)(11f * GuiDisplaySize.Offset);
			val2.fontStyle = (FontStyle)0;
			val2.fixedHeight = 20f * GuiDisplaySize.Offset;
			val2.stretchWidth = true;
			MessageStyle = val2;
			FlexiLabelStyle = new GUIStyle(NameStyle)
			{
				fixedWidth = 0f,
				stretchWidth = true
			};
			GUIStyle val3 = new GUIStyle(HighLogic.Skin.button);
			val3.normal.textColor = Color.white;
			val3.margin = new RectOffset(0, 0, 1, 1);
			val3.padding = new RectOffset();
			val3.alignment = (TextAnchor)4;
			val3.fontSize = (int)(11f * GuiDisplaySize.Offset);
			val3.fixedHeight = 18f * GuiDisplaySize.Offset;
			ButtonStyle = val3;
			CompactButtonStyle = new GUIStyle(ButtonStyle)
			{
				fontSize = (int)(10f * GuiDisplaySize.Offset),
				margin = new RectOffset(0, 0, 5, 5),
				fixedHeight = ButtonStyle.fontSize
			};
			TextFieldStyle = new GUIStyle(HighLogic.Skin.textField)
			{
				margin = new RectOffset(0, 0, 1, 1),
				padding = new RectOffset(5, 5, 0, 0),
				alignment = (TextAnchor)3,
				fontSize = (int)(11f * GuiDisplaySize.Offset),
				fixedHeight = 18f * GuiDisplaySize.Offset
			};
			ValueStyle.normal.textColor = textColor;
		}
	}

	private void OnSizeChanged()
	{
		InitialiseStyles(force: true);
		ResizeRequested = true;
	}
}
