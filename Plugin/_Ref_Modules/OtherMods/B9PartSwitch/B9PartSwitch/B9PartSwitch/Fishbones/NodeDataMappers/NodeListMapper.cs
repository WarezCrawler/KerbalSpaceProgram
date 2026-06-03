using System;
using System.Collections;
using System.Collections.Generic;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers;

public class NodeListMapper : INodeDataMapper
{
	public readonly string name;

	public readonly Type listType;

	public readonly INodeObjectWrapper nodeObjectWrapper;

	public NodeListMapper(string name, Type elementType, INodeObjectWrapper nodeObjectWrapper)
	{
		name.ThrowIfNullArgument("name");
		elementType.ThrowIfNullArgument("elementType");
		nodeObjectWrapper.ThrowIfNullArgument("nodeObjectWrapper");
		this.name = name;
		listType = typeof(List<>).MakeGenericType(elementType);
		this.nodeObjectWrapper = nodeObjectWrapper;
	}

	public bool Load(ref object fieldValue, ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		fieldValue.EnsureArgumentType(listType, "fieldValue");
		context.ThrowIfNullArgument("context");
		ConfigNode[] nodes = node.GetNodes(name);
		if (nodes.IsNullOrEmpty())
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
		ConfigNode[] array = nodes;
		foreach (ConfigNode configNode in array)
		{
			if (!configNode.IsNull())
			{
				object obj = null;
				nodeObjectWrapper.Load(ref obj, configNode, context);
				list.Add(obj);
			}
		}
		return true;
	}

	public bool Save(object fieldValue, ConfigNode node, OperationContext context)
	{
		fieldValue.EnsureArgumentType(listType, "fieldValue");
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		IList list = (IList)fieldValue;
		if (list.IsNullOrEmpty())
		{
			return false;
		}
		foreach (object item in list)
		{
			if (!item.IsNull())
			{
				node.AddNode(name, nodeObjectWrapper.Save(item, context));
			}
		}
		return true;
	}
}
