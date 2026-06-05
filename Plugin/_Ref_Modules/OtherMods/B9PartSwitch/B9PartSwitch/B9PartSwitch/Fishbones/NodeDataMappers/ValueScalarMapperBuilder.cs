using System;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers;

public class ValueScalarMapperBuilder : INodeDataMapperBuilder
{
	public readonly string nodeDataName;

	public readonly Type fieldType;

	public readonly IValueParseMap parseMap;

	public bool CanBuild => parseMap.CanParse(fieldType);

	public ValueScalarMapperBuilder(string nodeDataName, Type fieldType, IValueParseMap parseMap)
	{
		nodeDataName.ThrowIfNullArgument("nodeDataName");
		fieldType.ThrowIfNullArgument("fieldType");
		parseMap.ThrowIfNullArgument("parseMap");
		this.nodeDataName = nodeDataName;
		this.fieldType = fieldType;
		this.parseMap = parseMap;
	}

	public INodeDataMapper BuildMapper()
	{
		if (!CanBuild)
		{
			throw new InvalidOperationException();
		}
		return new ValueScalarMapper(nodeDataName, parseMap.GetParser(fieldType));
	}
}
