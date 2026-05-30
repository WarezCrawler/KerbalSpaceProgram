using ns25;

namespace ns26;

public class AllowPartClipping : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(CheatOptions.AllowPartClipping);
	}

	protected override void OnToggleChanged(bool state)
	{
		CheatOptions.AllowPartClipping = state;
	}
}
