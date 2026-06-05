using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch.Fishbones.Parsers;

public class NodeObjectWrapperConfigNode : INodeObjectWrapper
{
	public void Load(ref object obj, ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		obj = node.CreateCopy();
	}

	public ConfigNode Save(object obj, OperationContext context)
	{
		obj.ThrowIfNullArgument("obj");
		obj.EnsureArgumentType<ConfigNode>("obj");
		return ((ConfigNode)obj).CreateCopy();
	}
}
