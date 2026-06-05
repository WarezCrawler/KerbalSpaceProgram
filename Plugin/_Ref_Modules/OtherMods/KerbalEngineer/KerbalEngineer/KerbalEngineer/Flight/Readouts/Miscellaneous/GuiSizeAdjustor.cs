using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class GuiSizeAdjustor : ReadoutModule
{
	public GuiSizeAdjustor()
	{
		base.Name = "GUI Size Adjustor";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Shows a control that will allow you to adjust the GUI size.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("GUI Size: " + GuiDisplaySize.Increment, base.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("<", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			GuiDisplaySize.Increment--;
		}
		if (GUILayout.Button(">", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			GuiDisplaySize.Increment++;
		}
		GUILayout.EndHorizontal();
	}
}
