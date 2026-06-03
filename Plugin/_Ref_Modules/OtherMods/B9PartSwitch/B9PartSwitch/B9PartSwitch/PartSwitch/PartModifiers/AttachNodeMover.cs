using UnityEngine;

namespace B9PartSwitch.PartSwitch.PartModifiers;

public class AttachNodeMover : PartModifierBase, IPartAspectLock
{
	public readonly AttachNode attachNode;

	private readonly Vector3 position;

	private readonly ILinearScaleProvider linearScaleProvider;

	public object PartAspectLock => attachNode.id + "---position";

	public override string Description => "attach node '" + attachNode.id + "' position";

	public AttachNodeMover(AttachNode attachNode, Vector3 position, ILinearScaleProvider linearScaleProvider)
	{
		attachNode.ThrowIfNullArgument("attachNode");
		linearScaleProvider.ThrowIfNullArgument("linearScaleProvider");
		this.attachNode = attachNode;
		this.position = position;
		this.linearScaleProvider = linearScaleProvider;
	}

	public override void ActivateOnStartEditor()
	{
		SetAttachNodePosition();
	}

	public override void ActivateOnStartFlight()
	{
		SetAttachNodePosition();
	}

	public override void ActivateOnStartFinishedEditor()
	{
		SetAttachNodePosition();
	}

	public override void ActivateOnStartFinishedFlight()
	{
		SetAttachNodePosition();
	}

	public override void ActivateOnSwitchEditor()
	{
		SetAttachNodePositionAndMoveParts();
	}

	public override void ActivateOnSwitchFlight()
	{
		SetAttachNodePosition();
	}

	public override void DeactivateOnSwitchEditor()
	{
		UnsetAttachNodePositionAndMoveParts();
	}

	public override void DeactivateOnSwitchFlight()
	{
		UnsetAttachNodePosition();
	}

	private void SetAttachNodePositionAndMoveParts()
	{
		Vector3 vector = position * linearScaleProvider.LinearScale - attachNode.position;
		SetAttachNodePosition();
		if (HighLogic.LoadedSceneIsEditor)
		{
			if (attachNode.owner.parent != null && attachNode.owner.parent == attachNode.attachedPart)
			{
				vector = attachNode.owner.transform.localRotation * vector;
				attachNode.owner.transform.localPosition -= vector;
			}
			else if (attachNode.attachedPart != null)
			{
				attachNode.attachedPart.transform.localPosition += vector;
			}
		}
	}

	private void UnsetAttachNodePositionAndMoveParts()
	{
		Vector3 vector = attachNode.originalPosition * linearScaleProvider.LinearScale - attachNode.position;
		UnsetAttachNodePosition();
		if (HighLogic.LoadedSceneIsEditor)
		{
			if (attachNode.owner.parent != null && attachNode.owner.parent == attachNode.attachedPart)
			{
				vector = attachNode.owner.transform.localRotation * vector;
				attachNode.owner.transform.localPosition -= vector;
			}
			else if (attachNode.attachedPart != null)
			{
				attachNode.attachedPart.transform.localPosition += vector;
			}
		}
	}

	private void SetAttachNodePosition()
	{
		attachNode.position = position * linearScaleProvider.LinearScale;
	}

	private void UnsetAttachNodePosition()
	{
		attachNode.position = attachNode.originalPosition * linearScaleProvider.LinearScale;
	}
}
