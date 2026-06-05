using UnityEngine;

namespace KerbalEngineer.UIControls;

internal class PopOutColorPicker : PopOutElement
{
	private float colorPickerSliderValueR = -1f;

	private string colorPickerStringValueR = "";

	private float colorPickerSliderValueG = -1f;

	private string colorPickerStringValueG = "";

	private float colorPickerSliderValueB = -1f;

	private string colorPickerStringValueB = "";

	public Color DrawColorPicker(Color initial)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_017b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_0217: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_032c: Unknown result type (might be due to invalid IL or missing references)
		//IL_031c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		Color val = initial;
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.BeginVertical(GUIStyle.op_Implicit("Box"), (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("R", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(10f) });
		float num = GUILayout.HorizontalSlider(val.r, 0f, 1f, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (num != colorPickerSliderValueR)
		{
			colorPickerSliderValueR = num;
			val.r = num;
			colorPickerStringValueR = ((int)(num * 255f)).ToString();
		}
		int result = (int)(val.r * 255f);
		string text = GUILayout.TextField(result.ToString(), 3, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) });
		if (text != colorPickerStringValueR && int.TryParse(text, out result))
		{
			val.r = (float)result / 255f;
			colorPickerSliderValueR = val.r;
			colorPickerStringValueR = text;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("G", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(10f) });
		float num2 = GUILayout.HorizontalSlider(val.g, 0f, 1f, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (num2 != colorPickerSliderValueG)
		{
			colorPickerSliderValueG = num2;
			val.g = num2;
			colorPickerStringValueG = ((int)(num2 * 255f)).ToString();
		}
		int result2 = (int)(val.g * 255f);
		string text2 = GUILayout.TextField(result2.ToString(), 3, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) });
		if (text2 != colorPickerStringValueG && int.TryParse(text2, out result2))
		{
			val.g = (float)result2 / 255f;
			colorPickerSliderValueG = val.g;
			colorPickerStringValueG = text2;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("B", (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(10f) });
		float num3 = GUILayout.HorizontalSlider(val.b, 0f, 1f, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (num3 != colorPickerSliderValueB)
		{
			colorPickerSliderValueB = num3;
			val.b = num3;
			colorPickerStringValueB = ((int)(num3 * 255f)).ToString();
		}
		int result3 = (int)(val.b * 255f);
		string text3 = GUILayout.TextField(result3.ToString(), 3, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f) });
		if (text3 != colorPickerStringValueB && int.TryParse(text3, out result3))
		{
			val.b = (float)result3 / 255f;
			colorPickerSliderValueB = val.b;
			colorPickerStringValueB = text3;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		GUILayout.EndHorizontal();
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("RESET", (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			val = HighLogic.Skin.label.normal.textColor;
		}
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
		return val;
	}
}
