using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Miscellaneous;

public class Separator : ReadoutModule
{
	private GUIStyle boxStyle;

	private GUIStyle boxStyleHud;

	private static readonly Texture2D tex = TextureHelper.CreateTextureFromColour(new Color(1f, 1f, 1f, 0.5f));

	public Separator()
	{
		base.Name = "Separator";
		base.Category = ReadoutCategory.GetCategory("Miscellaneous");
		base.HelpString = "Creats a line to help seperate subsections in a module.";
		base.IsDefault = false;
		base.Cloneable = true;
		InitialiseStyles();
		GuiDisplaySize.OnSizeChanged += InitialiseStyles;
	}

	public override void Draw(ISectionModule section)
	{
		GUILayout.Box("", section.IsHud ? boxStyleHud : boxStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
	}

	private void InitialiseStyles()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Expected O, but got Unknown
		//IL_007e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_009d: Expected O, but got Unknown
		//IL_00a2: Expected O, but got Unknown
		GUIStyle val = new GUIStyle();
		val.normal.background = tex;
		val.normal.textColor = new Color(1f, 1f, 1f, 0.5f);
		val.active.background = tex;
		val.border = new RectOffset(0, 0, 0, 1);
		val.fixedHeight = 0f;
		val.stretchWidth = true;
		val.imagePosition = (ImagePosition)2;
		boxStyle = val;
		boxStyleHud = new GUIStyle(boxStyle)
		{
			margin = new RectOffset(0, 0, (int)(8f * GuiDisplaySize.Offset), 0)
		};
	}
}
