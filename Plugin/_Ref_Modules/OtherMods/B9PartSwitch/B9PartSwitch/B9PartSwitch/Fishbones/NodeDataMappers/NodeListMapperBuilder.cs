using System;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers;

public class NodeListMapperBuilder : INodeDataMapperBuilder
{
	public readonly string nodeDataName;

	public readonly Type elementType;

	public bool CanBuild
	{
		get
		{
			if (elementType.IsNotNull())
			{
				return NodeObjectWrapper.IsNodeType(elementType);
			}
			return false;
		}
	}

	public NodeListMapperBuilder(string nodeDataName, Type fieldType)
	{
		nodeDataName.ThrowIfNullArgument("nodeDataName");
		fieldType.ThrowIfNullArgument("fieldType");
		this.nodeDataName = nodeDataName;
		if (fieldType.IsListType())
		{
			elementType = fieldType.GetGenericArguments()[0];
		}
	}

	public INodeDataMapper BuildMapper()
	{
		if (!CanBuild)
		{
			throw new InvalidOperationException();
		}
		return new NodeListMapper(nodeDataName, elementType, NodeObjectWrapper.For(elementType));
	}
}
