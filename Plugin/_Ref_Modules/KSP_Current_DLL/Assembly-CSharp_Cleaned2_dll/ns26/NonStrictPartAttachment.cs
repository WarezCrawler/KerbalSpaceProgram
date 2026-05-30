using ns25;

namespace ns26;

public class NonStrictPartAttachment : DebugScreenToggle
{
	protected override void SetupValues()
	{
		SetToggle(CheatOptions.NonStrictAttachmentOrientation);
	}

	protected override void OnToggleChanged(bool state)
	{
		CheatOptions.NonStrictAttachmentOrientation = state;
	}
}
