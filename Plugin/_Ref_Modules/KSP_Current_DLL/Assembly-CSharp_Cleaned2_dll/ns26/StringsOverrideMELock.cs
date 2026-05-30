using ns25;
using ns9;

namespace ns26;

public class StringsOverrideMELock : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(Localizer.OverrideMELock);
	}

	protected override void OnToggleChanged(bool state)
	{
		Localizer.OverrideMELock = state;
		GameEvents.Mission.onLocalizationLockOverriden.Fire();
	}
}
