using System;
using UnityEngine;

namespace B9PartSwitch.Fishbones.Parsers;

public class AttachNodeValueParser : ValueParser<AttachNode>
{
	public static AttachNode ParseAttachNode(string value)
	{
		value.ThrowIfNullArgument("value");
		string[] array = value.Split(',');
		int num = array.Length;
		if (num < 6)
		{
			throw new FormatException("Not enough values to parse an AttachNode: '" + value + "'");
		}
		float[] array2 = new float[6];
		for (int i = 0; i < 6; i++)
		{
			array2[i] = float.Parse(array[i]);
		}
		AttachNode attachNode = new AttachNode();
		attachNode.id = "parsed-attach-node";
		attachNode.position = new Vector3(array2[0], array2[1], array2[2]);
		attachNode.orientation = new Vector3(array2[3], array2[4], array2[5]);
		attachNode.originalPosition = attachNode.position;
		attachNode.originalOrientation = attachNode.orientation;
		if (num >= 7)
		{
			attachNode.size = int.Parse(array[6]);
		}
		else
		{
			attachNode.size = 1;
		}
		if (num >= 8)
		{
			attachNode.attachMethod = (AttachNodeMethod)int.Parse(array[7]);
		}
		if (num >= 9)
		{
			attachNode.ResourceXFeed = int.Parse(array[8]) > 0;
		}
		if (num >= 10)
		{
			attachNode.rigid = int.Parse(array[9]) > 0;
		}
		return attachNode;
	}

	public static string FormatAttachNode(AttachNode node)
	{
		node.ThrowIfNullArgument("node");
		return string.Join(", ", node.position.x.ToString(), node.position.y.ToString(), node.position.z.ToString(), node.orientation.x.ToString(), node.orientation.y.ToString(), node.orientation.z.ToString(), node.size.ToString(), Enum.Format(typeof(AttachNodeMethod), node.attachMethod, "d"), node.ResourceXFeed ? "1" : "0", node.rigid ? "1" : "0");
	}

	public AttachNodeValueParser()
		: base((Func<string, AttachNode>)ParseAttachNode, (Func<AttachNode, string>)FormatAttachNode)
	{
	}
}
