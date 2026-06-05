using System;
using System.Collections.Generic;
using System.Linq;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;
using B9PartSwitch.Utils;
using UnityEngine;

namespace B9PartSwitch;

public class AttachNodeModifierInfo : IContextualNode
{
	[NodeData(name = "name")]
	public IStringMatcher nodeID;

	[NodeData]
	public Vector3? position;

	[NodeData]
	public int? size;

	public void Load(ConfigNode node, OperationContext context)
	{
		this.LoadFields(node, context);
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		this.SaveFields(node, context);
	}

	public IEnumerable<IPartModifier> CreatePartModifiers(Part part, ILinearScaleProvider linearScaleProvider, Action<string> onError)
	{
		AttachNode node = part.attachNodes.FirstOrDefault((AttachNode n) => (n.nodeType == AttachNode.NodeType.Stack || n.nodeType == AttachNode.NodeType.Dock) && nodeID.Match(n.id));
		if (node == null)
		{
			onError($"Attach node with id matching '{nodeID}' not found for attach node modifier");
			yield break;
		}
		Part part2 = part.partInfo?.partPrefab ?? part;
		float num = part2.scaleFactor * part2.rescaleFactor * part2.rescaleFactor;
		if (position.HasValue)
		{
			yield return new AttachNodeMover(node, position.Value * num, linearScaleProvider);
		}
		if (size.HasValue)
		{
			yield return new AttachNodeSizeModifier(node, size.Value, linearScaleProvider);
		}
	}
}
