using ns25;

namespace ns26;

public class PauseOnVesselUnpack : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(CheatOptions.PauseOnVesselUnpack);
	}

	protected override void OnToggleChanged(bool state)
	{
		CheatOptions.PauseOnVesselUnpack = state;
	}
}
