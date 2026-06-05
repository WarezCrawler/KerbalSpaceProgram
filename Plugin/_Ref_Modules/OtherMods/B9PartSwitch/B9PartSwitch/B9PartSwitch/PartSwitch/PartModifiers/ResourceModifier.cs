namespace B9PartSwitch.PartSwitch.PartModifiers;

public class ResourceModifier : PartModifierBase, IPartAspectLock
{
	private readonly TankResource tankResource;

	private readonly GetVolumeDelegate getVolumeDelegate;

	private readonly Part part;

	private readonly float filledProportion;

	private readonly bool? tweakable;

	public object PartAspectLock => tankResource.ResourceName;

	public override string Description => "resource '" + tankResource.ResourceName + "'";

	public ResourceModifier(TankResource tankResource, GetVolumeDelegate getVolumeDelegate, Part part, float filledProportion, bool? tweakable)
	{
		tankResource.ThrowIfNullArgument("tankResource");
		getVolumeDelegate.ThrowIfNullArgument("getVolumeDelegate");
		part.ThrowIfNullArgument("part");
		this.tankResource = tankResource;
		this.getVolumeDelegate = getVolumeDelegate;
		this.part = part;
		this.filledProportion = filledProportion;
		this.tweakable = tweakable;
	}

	public override void ActivateOnStartEditor()
	{
		UpsertResource(fillTanks: false, zeroAmount: false);
	}

	public override void ActivateOnStartFlight()
	{
		UpsertResource(fillTanks: false, zeroAmount: false);
	}

	public override void DeactivateOnSwitchEditor()
	{
		RemoveResource();
	}

	public override void DeactivateOnSwitchFlight()
	{
		RemoveResource();
	}

	public override void ActivateOnSwitchEditor()
	{
		UpsertResource(fillTanks: true, zeroAmount: false);
	}

	public override void ActivateOnSwitchFlight()
	{
		UpsertResource(fillTanks: true, zeroAmount: true);
	}

	public override void UpdateVolumeEditor()
	{
		UpsertResource(fillTanks: true, zeroAmount: false);
	}

	public override void UpdateVolumeFlight()
	{
		UpsertResource(fillTanks: true, zeroAmount: true);
	}

	private void UpsertResource(bool fillTanks, bool zeroAmount)
	{
		float num = getVolumeDelegate() * tankResource.unitsPerVolume;
		PartResource partResource = PartExtensions.AddOrCreateResource(amount: (!(zeroAmount && fillTanks)) ? (num * filledProportion) : 0f, part: part, info: tankResource.resourceDefinition, maxAmount: num, modifyAmountIfPresent: fillTanks);
		if (tweakable.HasValue)
		{
			partResource.isTweakable = tweakable.Value;
		}
	}

	private void RemoveResource()
	{
		part.RemoveResource(tankResource.ResourceName);
	}
}
