using TMPro;
using UnityEngine;
using ns9;

namespace ns20;

public class SettingsKeyboardLayoutOs : SettingsControlBase
{
	public TextMeshProUGUI keyboardLabel;

	private void Start()
	{
		KeyboardLayout keyboardLayout = KeyboardLayout.GetKeyboardLayout();
		Debug.Log("Locale is: " + keyboardLayout.Locale.DisplayName);
		keyboardLabel.text = Localizer.Format("#autoLOC_6001209", keyboardLayout.Type, keyboardLayout.Locale.NativeName);
	}
}
