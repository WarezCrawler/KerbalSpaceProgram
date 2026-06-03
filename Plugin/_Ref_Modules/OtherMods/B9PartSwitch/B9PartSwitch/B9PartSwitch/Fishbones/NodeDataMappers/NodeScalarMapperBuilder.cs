using System;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers;

public class NodeScalarMapperBuilder : INodeDataMapperBuilder
{
	public readonly string nodeDataName;

	public readonly Type fieldType;

	public bool CanBuild => NodeObjectWrapper.IsNodeType(fieldType);

	public NodeScalarMapperBuilder(string nodeDataName, Type fieldType)
	{
		nodeDataName.ThrowIfNullArgument("nodeDataName");
		fieldType.ThrowIfNullArgument("fieldType");
		this.nodeDataName = nodeDataName;
		this.fieldType = fieldType;
	}

	public INodeDataMapper BuildMapper()
	{
		if (!CanBuild)
		{
			throw new InvalidOperationException();
		}
		return new NodeScalarMapper(nodeDataName, NodeObjectWrapper.For(fieldType));
	}
}
