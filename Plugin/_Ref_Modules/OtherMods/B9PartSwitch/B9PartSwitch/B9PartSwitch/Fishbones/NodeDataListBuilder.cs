using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using B9PartSwitch.Fishbones.FieldWrappers;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones;

public class NodeDataListBuilder
{
	public const BindingFlags FIELD_BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

	private readonly Type type;

	public NodeDataListBuilder(Type type)
	{
		type.ThrowIfNullArgument("type");
		this.type = type;
	}

	public virtual NodeDataList CreateList()
	{
		List<INodeDataField> list = new List<INodeDataField>();
		IValueParseMap instance = DefaultValueParseMap.Instance;
		FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (FieldInfo fieldInfo in fields)
		{
			NodeData nodeData = (NodeData)fieldInfo.GetCustomAttributes(typeof(NodeData), inherit: true).FirstOrDefault();
			if (nodeData != null)
			{
				IFieldWrapper fieldWrapper = new FieldWrapper(fieldInfo);
				INodeDataBuilder nodeDataBuilder = CreateFieldBuilder(nodeData, fieldWrapper, instance);
				list.Add(nodeDataBuilder.CreateNodeDataField());
			}
		}
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo in properties)
		{
			NodeData nodeData2 = (NodeData)propertyInfo.GetCustomAttributes(typeof(NodeData), inherit: true).FirstOrDefault();
			if (nodeData2 != null)
			{
				IFieldWrapper fieldWrapper2 = new PropertyWrapper(propertyInfo);
				INodeDataBuilder nodeDataBuilder2 = CreateFieldBuilder(nodeData2, fieldWrapper2, instance);
				list.Add(nodeDataBuilder2.CreateNodeDataField());
			}
		}
		return new NodeDataList(list.ToArray());
	}

	public virtual INodeDataBuilder CreateFieldBuilder(NodeData nodeData, IFieldWrapper fieldWrapper, IValueParseMap defaultValueParseMap)
	{
		return new NodeDataBuilder(nodeData, fieldWrapper, defaultValueParseMap);
	}
}
