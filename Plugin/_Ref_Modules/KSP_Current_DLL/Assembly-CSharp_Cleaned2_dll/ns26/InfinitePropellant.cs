using ns25;

namespace ns26;

public class InfinitePropellant : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(CheatOptions.InfinitePropellant);
	}

	protected override void OnToggleChanged(bool state)
	{
		CheatOptions.InfinitePropellant = state;
	}
}
