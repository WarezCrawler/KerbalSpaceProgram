using ns25;

namespace ns26;

public class IgnoreMaxTemperature : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(CheatOptions.IgnoreMaxTemperature);
	}

	protected override void OnToggleChanged(bool state)
	{
		CheatOptions.IgnoreMaxTemperature = state;
	}
}
