using System;
using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch.Fishbones.Parsers;

public class NodeObjectWrapperIConfigNode : INodeObjectWrapper
{
	public readonly Type type;

	public NodeObjectWrapperIConfigNode(Type type)
	{
		type.ThrowIfNullArgument("type");
		if (!type.Implements<IConfigNode>())
		{
			throw new ArgumentException(string.Format("Type {0} does not implement {1}", type, "IConfigNode"), "type");
		}
		this.type = type;
	}

	public void Load(ref object obj, ConfigNode node, OperationContext context)
	{
		obj.EnsureArgumentType<IConfigNode>("obj");
		node.ThrowIfNullArgument("node");
		if (obj.IsNull())
		{
			obj = Activator.CreateInstance(type);
		}
		((IConfigNode)obj).Load(node);
	}

	public ConfigNode Save(object obj, OperationContext context)
	{
		obj.ThrowIfNullArgument("obj");
		obj.EnsureArgumentType<IConfigNode>("obj");
		ConfigNode configNode = new ConfigNode();
		((IConfigNode)obj).Save(configNode);
		return configNode;
	}
}
