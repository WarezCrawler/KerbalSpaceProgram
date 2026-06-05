using UnityEngine;

namespace B9PartSwitch.PartSwitch.PartModifiers;

public class AttachNodeSizeModifier : PartModifierBase, IPartAspectLock
{
	public readonly AttachNode attachNode;

	private readonly int originalSize;

	private readonly int size;

	private readonly ILinearScaleProvider linearScaleProvider;

	public object PartAspectLock => attachNode.id + "---size";

	public override string Description => "attach node '" + attachNode.id + "' size";

	public AttachNodeSizeModifier(AttachNode attachNode, int size, ILinearScaleProvider linearScaleProvider)
	{
		attachNode.ThrowIfNullArgument("attachNode");
		linearScaleProvider.ThrowIfNullArgument("linearScaleProvider");
		this.attachNode = attachNode;
		this.size = size;
		this.linearScaleProvider = linearScaleProvider;
		originalSize = attachNode.size;
	}

	public override void ActivateOnStartEditor()
	{
		SetAttachNodeSize();
	}

	public override void ActivateOnStartFlight()
	{
		SetAttachNodeSize();
	}

	public override void ActivateOnStartFinishedEditor()
	{
		SetAttachNodeSize();
	}

	public override void ActivateOnStartFinishedFlight()
	{
		SetAttachNodeSize();
	}

	public override void ActivateOnSwitchEditor()
	{
		SetAttachNodeSize();
	}

	public override void ActivateOnSwitchFlight()
	{
		SetAttachNodeSize();
	}

	public override void DeactivateOnSwitchEditor()
	{
		UnsetAttachNodeSize();
	}

	public override void DeactivateOnSwitchFlight()
	{
		UnsetAttachNodeSize();
	}

	public override void OnBeforeReinitializeActiveSubtype()
	{
		UnsetAttachNodeSize();
	}

	public override void OnAfterReinitializeActiveSubtype()
	{
		SetAttachNodeSize();
	}

	private void SetAttachNodeSize()
	{
		attachNode.size = Mathf.RoundToInt((float)size * linearScaleProvider.LinearScale);
	}

	private void UnsetAttachNodeSize()
	{
		attachNode.size = Mathf.RoundToInt((float)originalSize * linearScaleProvider.LinearScale);
	}
}
