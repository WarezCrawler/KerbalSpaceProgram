using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class ClearSeparator : ReadoutModule
{
	private GUIStyle boxStyle;

	private GUIStyle boxStyleHud;

	public ClearSeparator()
	{
		base.Name = "Clear Separator";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Creats a space to help seperate subsections in a module.";
		base.IsDefault = false;
		base.Cloneable = true;
		InitialiseStyles();
		GuiDisplaySize.OnSizeChanged += InitialiseStyles;
	}

	public override void Draw(ISectionModule section)
	{
		GUILayout.Box(string.Empty, section.IsHud ? boxStyleHud : boxStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
	}

	private void InitialiseStyles()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Expected O, but got Unknown
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0048: Expected O, but got Unknown
		boxStyle = new GUIStyle
		{
			fixedHeight = 1f,
			stretchWidth = true
		};
		boxStyleHud = new GUIStyle(boxStyle)
		{
			margin = new RectOffset(0, 0, (int)(8f * GuiDisplaySize.Offset), 0)
		};
	}
}
