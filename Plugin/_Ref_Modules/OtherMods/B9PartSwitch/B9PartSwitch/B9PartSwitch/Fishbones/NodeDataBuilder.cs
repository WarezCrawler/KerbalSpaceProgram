using System;
using System.Linq;
using B9PartSwitch.Fishbones.FieldWrappers;
using B9PartSwitch.Fishbones.NodeDataMappers;
using B9PartSwitch.Fishbones.Parsers;
using UnityEngine;

namespace B9PartSwitch.Fishbones;

public class NodeDataBuilder : INodeDataBuilder
{
	public readonly NodeData nodeData;

	public readonly IValueParseMap valueParseMap;

	public readonly IFieldWrapper fieldWrapper;

	public virtual string NodeDataName
	{
		get
		{
			if (!nodeData.name.IsNullOrEmpty())
			{
				return nodeData.name;
			}
			return fieldWrapper.MemberInfo.Name;
		}
	}

	public NodeDataBuilder(NodeData nodeData, IFieldWrapper fieldWrapper, IValueParseMap defaultValueParseMap)
	{
		nodeData.ThrowIfNullArgument("nodeData");
		fieldWrapper.ThrowIfNullArgument("fieldWrapper");
		defaultValueParseMap.ThrowIfNullArgument("defaultValueParseMap");
		this.nodeData = nodeData;
		this.fieldWrapper = fieldWrapper;
		object[] customAttributes = fieldWrapper.MemberInfo.GetCustomAttributes(inherit: true);
		if (customAttributes.OfType<IUseParser>().Any())
		{
			IValueParser[] overrides = (from x in customAttributes.OfType<IUseParser>()
				select x.CreateParser()).ToArray();
			valueParseMap = new OverrideValueParseMap(defaultValueParseMap, overrides);
		}
		else
		{
			valueParseMap = defaultValueParseMap;
		}
	}

	public INodeDataField CreateNodeDataField()
	{
		return new NodeDataField(fieldWrapper, CreateOperationManager());
	}

	public virtual IOperaitonManager CreateOperationManager()
	{
		return new OperationManager(CreateParseMapper(), CreateLoadSaveMapper(), CreateSerializeMapper());
	}

	public virtual INodeDataMapper CreateParseMapper()
	{
		return CreateMapperWithParsePriority();
	}

	public virtual INodeDataMapper CreateLoadSaveMapper()
	{
		if (!nodeData.persistent)
		{
			return null;
		}
		return CreateMapperWithParsePriority();
	}

	public virtual INodeDataMapper CreateSerializeMapper()
	{
		if (!nodeData.alwaysSerialize && fieldWrapper.MemberInfo.ReflectedType.Implements<UnityEngine.Object>())
		{
			return null;
		}
		return CreateMapperWithSerializePriority();
	}

	public virtual INodeDataMapper CreateMapperWithParsePriority()
	{
		return BuildFromPrioritizedList(CreateValueScalarMapperBuilder(), CreateNodeScalarMapperBuilder(), CreateValueListMapperBuilder(), CreateNodeListMapperBuilder());
	}

	public virtual INodeDataMapper CreateMapperWithSerializePriority()
	{
		return BuildFromPrioritizedList(CreateNodeListMapperBuilder(), CreateNodeScalarMapperBuilder(), CreateValueListMapperBuilder(), CreateValueScalarMapperBuilder());
	}

	public virtual INodeDataMapper BuildFromPrioritizedList(params INodeDataMapperBuilder[] list)
	{
		INodeDataMapperBuilder nodeDataMapperBuilder = list.FirstOrDefault((INodeDataMapperBuilder x) => x.CanBuild);
		if (nodeDataMapperBuilder.IsNotNull())
		{
			return nodeDataMapperBuilder.BuildMapper();
		}
		throw new NotImplementedException("Cannot find a suitable way to load node data into field " + fieldWrapper.MemberInfo.Name);
	}

	public virtual INodeDataMapperBuilder CreateValueScalarMapperBuilder()
	{
		return new ValueScalarMapperBuilder(NodeDataName, fieldWrapper.FieldType, valueParseMap);
	}

	public virtual INodeDataMapperBuilder CreateValueListMapperBuilder()
	{
		return new ValueListMapperBuilder(NodeDataName, fieldWrapper.FieldType, valueParseMap);
	}

	public virtual INodeDataMapperBuilder CreateNodeScalarMapperBuilder()
	{
		return new NodeScalarMapperBuilder(NodeDataName, fieldWrapper.FieldType);
	}

	public virtual INodeDataMapperBuilder CreateNodeListMapperBuilder()
	{
		return new NodeListMapperBuilder(NodeDataName, fieldWrapper.FieldType);
	}
}
