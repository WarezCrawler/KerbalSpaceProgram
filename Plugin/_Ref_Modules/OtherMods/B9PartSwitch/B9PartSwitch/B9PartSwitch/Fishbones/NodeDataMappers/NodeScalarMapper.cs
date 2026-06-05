using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers;

public class NodeScalarMapper : INodeDataMapper
{
	public readonly string name;

	public readonly INodeObjectWrapper nodeObjectWrapper;

	public NodeScalarMapper(string name, INodeObjectWrapper nodeObjectWrapper)
	{
		name.ThrowIfNullArgument("name");
		this.name = name;
		nodeObjectWrapper.ThrowIfNullArgument("nodeObjectWrapper");
		this.nodeObjectWrapper = nodeObjectWrapper;
	}

	public bool Load(ref object fieldValue, ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		ConfigNode node2 = node.GetNode(name);
		if (node2.IsNull())
		{
			return false;
		}
		nodeObjectWrapper.Load(ref fieldValue, node2, context);
		return true;
	}

	public bool Save(object fieldValue, ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		if (fieldValue.IsNull())
		{
			return false;
		}
		ConfigNode node2 = nodeObjectWrapper.Save(fieldValue, context);
		node.AddNode(name, node2);
		return true;
	}
}
