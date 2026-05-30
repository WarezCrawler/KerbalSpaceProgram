using ns25;
using ns9;

namespace ns26;

public class StringsShowKeysToggle : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(Localizer.ShowKeysOnScreen);
	}

	protected override void OnToggleChanged(bool state)
	{
		Localizer.ShowKeysOnScreen = state;
	}
}
