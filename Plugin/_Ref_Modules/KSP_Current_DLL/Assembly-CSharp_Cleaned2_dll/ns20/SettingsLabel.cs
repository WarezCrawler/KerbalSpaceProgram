using UnityEngine;
using ns19;

namespace ns20;

public class SettingsLabel : SettingsControlReflection
{
	[SettingsValue("#FFE805FF")]
	public Color textColor;

	[SettingsValue(14)]
	public int fontSize;

	protected override void OnStart()
	{
		base.IgnoreEmptySetting = true;
		titleText.color = textColor;
		titleText.fontSize = fontSize;
	}
}
