using UnityEngine;
using ns19;
using ns20;

namespace ns21;

public class SettingsValueBoolTitleChange : MonoBehaviour
{
	[SettingsValue("True")]
	public string titleEnabled = "True";

	[SettingsValue("False")]
	public string titleDisabled = "False";

	private SettingsControlReflection control;

	private void Awake()
	{
		control = GetComponent<SettingsControlReflection>();
	}

	public void UpdateValue(object value)
	{
		if (value is bool && control.titleText != null)
		{
			control.titleText.text = (((bool)value) ? titleEnabled : titleDisabled);
		}
	}
}
