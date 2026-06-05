namespace B9PartSwitch.PartSwitch.PartModifiers;

public class EffectDeactivator : PartModifierBase
{
	private readonly Part part;

	private readonly string originalEffectName;

	private readonly string newEffectName;

	public override string Description => "switch between effects " + originalEffectName + " and " + newEffectName;

	public EffectDeactivator(Part part, string originalEffectName, string newEffectName)
	{
		part.ThrowIfNullArgument("part");
		this.part = part;
		this.originalEffectName = originalEffectName;
		this.newEffectName = newEffectName;
	}

	public override void ActivateOnStartFinishedFlight()
	{
		Activate();
	}

	public override void DeactivateOnStartFinishedFlight()
	{
		Deactivate();
	}

	public override void DeactivateOnSwitchFlight()
	{
		Deactivate();
	}

	public override void ActivateOnSwitchFlight()
	{
		Activate();
	}

	private void Activate()
	{
		part.Effect(originalEffectName, 0f);
	}

	private void Deactivate()
	{
		part.Effect(newEffectName, 0f);
	}
}
