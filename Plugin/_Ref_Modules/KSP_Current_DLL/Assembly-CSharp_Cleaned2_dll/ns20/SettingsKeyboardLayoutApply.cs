using UnityEngine.UI;

namespace ns20;

public class SettingsKeyboardLayoutApply : SettingsControlBase
{
	public Button buttonApply;

	private void Start()
	{
		if (GameSettings.KeyboardLayouts.Count == 0)
		{
			buttonApply.interactable = false;
		}
	}
}
