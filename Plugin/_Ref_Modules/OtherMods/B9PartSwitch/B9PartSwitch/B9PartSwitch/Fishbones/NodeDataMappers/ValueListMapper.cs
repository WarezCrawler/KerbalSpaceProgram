using System;
using System.Collections;
using System.Collections.Generic;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers;

public class ValueListMapper : INodeDataMapper
{
	public readonly string name;

	public readonly IValueParser parser;

	public readonly Type elementType;

	public readonly Type listType;

	public ValueListMapper(string name, IValueParser parser)
	{
		name.ThrowIfNullArgument("name");
		parser.ThrowIfNullArgument("parser");
		this.name = name;
		this.parser = parser;
		elementType = parser.ParseType;
		listType = typeof(List<>).MakeGenericType(elementType);
	}

	public bool Load(ref object fieldValue, ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		fieldValue.EnsureArgumentType(listType, "fieldValue");
		string[] values = node.GetValues(name);
		if (values.IsNullOrEmpty())
		{
			return false;
		}
		if (fieldValue.IsNull())
		{
			fieldValue = Activator.CreateInstance(listType);
		}
		IList list = (IList)fieldValue;
		if (context.Operation == Operation.Deserialize || context.Operation == Operation.LoadInstance)
		{
			list.Clear();
		}
		string[] array = values;
		foreach (string text in array)
		{
			if (!text.IsNull())
			{
				object obj = parser.Parse(text);
				if (obj.IsNotNull())
				{
					list.Add(obj);
				}
			}
		}
		return true;
	}

	public bool Save(object fieldValue, ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		fieldValue.EnsureArgumentType(listType, "fieldValue");
		IList list = (IList)fieldValue;
		if (list.IsNullOrEmpty())
		{
			return false;
		}
		foreach (object item in list)
		{
			if (!item.IsNull())
			{
				string text = parser.Format(item);
				if (text.IsNotNull())
				{
					node.AddValue(name, text);
				}
			}
		}
		return true;
	}
}
