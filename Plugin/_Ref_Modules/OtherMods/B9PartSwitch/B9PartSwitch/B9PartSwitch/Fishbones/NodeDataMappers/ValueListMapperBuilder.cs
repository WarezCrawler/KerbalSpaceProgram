using System;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers;

public class ValueListMapperBuilder : INodeDataMapperBuilder
{
	public readonly string nodeDataName;

	public readonly Type elementType;

	public readonly IValueParseMap parseMap;

	public bool CanBuild
	{
		get
		{
			if (elementType.IsNotNull())
			{
				return parseMap.CanParse(elementType);
			}
			return false;
		}
	}

	public ValueListMapperBuilder(string nodeDataName, Type fieldType, IValueParseMap parseMap)
	{
		nodeDataName.ThrowIfNullArgument("nodeDataName");
		fieldType.ThrowIfNullArgument("fieldType");
		parseMap.ThrowIfNullArgument("parseMap");
		this.nodeDataName = nodeDataName;
		this.parseMap = parseMap;
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
		return new ValueListMapper(nodeDataName, parseMap.GetParser(elementType));
	}
}
