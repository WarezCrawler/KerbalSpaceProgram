namespace B9PartSwitch.PartSwitch.PartModifiers;

public class AttachNodeToggler : PartModifierBase
{
	private readonly AttachNode node;

	public override string Description => "stack node '" + node.id + "' enabled status";

	public AttachNodeToggler(AttachNode node)
	{
		node.ThrowIfNullArgument("node");
		this.node = node;
	}

	public override void DeactivateOnStartEditor()
	{
		Deactivate();
	}

	public override void ActivateOnStartEditor()
	{
		MaybeActivate();
	}

	public override void DeactivateOnSwitchEditor()
	{
		Deactivate();
	}

	public override void ActivateOnSwitchEditor()
	{
		MaybeActivate();
	}

	public override void OnWillBeCopiedInactiveSubtype()
	{
		Activate();
	}

	public override void OnWasCopiedInactiveSubtype()
	{
		Deactivate();
	}

	public override void OnWasCopiedActiveSubtype()
	{
		MaybeActivate();
	}

	public override void OnBeforeReinitializeInactiveSubtype()
	{
		Activate();
	}

	private void MaybeActivate()
	{
		node.owner.UpdateNodeEnabled(node);
	}

	private void Activate()
	{
		node.Unhide();
	}

	private void Deactivate()
	{
		node.Hide();
	}
}
