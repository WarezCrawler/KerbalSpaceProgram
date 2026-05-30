using ns25;

namespace ns26;

public class NoCrashDamage : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(CheatOptions.NoCrashDamage);
	}

	protected override void OnToggleChanged(bool state)
	{
		CheatOptions.NoCrashDamage = state;
	}
}
