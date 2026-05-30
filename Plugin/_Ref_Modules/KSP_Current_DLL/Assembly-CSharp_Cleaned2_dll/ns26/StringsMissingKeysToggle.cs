using ns25;
using ns9;

namespace ns26;

public class StringsMissingKeysToggle : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(Localizer.debugWriteMissingKeysToLog);
	}

	protected override void OnToggleChanged(bool state)
	{
		Localizer.debugWriteMissingKeysToLog = state;
	}
}
