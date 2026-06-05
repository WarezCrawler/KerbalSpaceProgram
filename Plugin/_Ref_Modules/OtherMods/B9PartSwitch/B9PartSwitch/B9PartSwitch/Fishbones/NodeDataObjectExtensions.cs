using System;
using B9PartSwitch.Fishbones.Context;
using UnityEngine;

namespace B9PartSwitch.Fishbones;

public static class NodeDataObjectExtensions
{
	public const string SERIALIZED_NODE = "SERIALIZED_NODE";

	public static OperationContext LoadFields(this object obj, ConfigNode node, OperationContext context)
	{
		obj.ThrowIfNullArgument("obj");
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		NodeDataList nodeDataList = NodeDataListLibrary.Get(obj.GetType());
		OperationContext operationContext = new OperationContext(context, obj);
		nodeDataList.Load(node, operationContext);
		return operationContext;
	}

	public static OperationContext SaveFields(this object obj, ConfigNode node, OperationContext context)
	{
		obj.ThrowIfNullArgument("obj");
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		NodeDataList nodeDataList = NodeDataListLibrary.Get(obj.GetType());
		OperationContext operationContext = new OperationContext(context, obj);
		nodeDataList.Save(node, operationContext);
		return operationContext;
	}

	public static SerializedDataContainer SerializeToContainer(this object obj)
	{
		obj.ThrowIfNullArgument("obj");
		SerializedDataContainer serializedDataContainer = ScriptableObject.CreateInstance<SerializedDataContainer>();
		serializedDataContainer.data = obj.SerializeToString();
		return serializedDataContainer;
	}

	public static string SerializeToString(this object obj)
	{
		obj.ThrowIfNullArgument("obj");
		ConfigNode configNode = obj.SerializeToNode();
		EscapeValuesRecursive(configNode);
		return configNode.ToString();
		static void EscapeValuesRecursive(ConfigNode theNode)
		{
			foreach (ConfigNode node in theNode.nodes)
			{
				EscapeValuesRecursive(node);
			}
			foreach (ConfigNode.Value value in theNode.values)
			{
				value.value = value.value.Replace("\n", "\\n");
				value.value = value.value.Replace("\t", "\\t");
			}
		}
	}

	public static ConfigNode SerializeToNode(this object obj)
	{
		obj.ThrowIfNullArgument("obj");
		ConfigNode configNode = new ConfigNode("SERIALIZED_NODE");
		NodeDataList nodeDataList = NodeDataListLibrary.Get(obj.GetType());
		OperationContext context = new OperationContext(Operation.Serialize, obj);
		nodeDataList.Save(configNode, context);
		return configNode;
	}

	public static void DeserializeFromContainer(this object obj, SerializedDataContainer container)
	{
		obj.ThrowIfNullArgument("obj");
		container.ThrowIfNullArgument("container");
		if (container.data.IsNullOrEmpty())
		{
			throw new ArgumentException("Container must have data", "container");
		}
		obj.DeserializeFromString(container.data);
	}

	public static void DeserializeFromString(this object obj, string serializedData)
	{
		obj.ThrowIfNullArgument("obj");
		serializedData.ThrowIfNullArgument("serializedData");
		ConfigNode configNode;
		try
		{
			configNode = ConfigNode.Parse(serializedData);
		}
		catch (Exception innerException)
		{
			throw new FormatException("Failed to parse a ConfigNode from serialized data", innerException);
		}
		ConfigNode node = configNode.GetNode("SERIALIZED_NODE");
		if (node.IsNull())
		{
			throw new FormatException("No serialized data node found");
		}
		UnescapeValuesRecursive(node);
		obj.DeserializeFromNode(node);
		static void UnescapeValuesRecursive(ConfigNode theNode)
		{
			foreach (ConfigNode node2 in theNode.nodes)
			{
				UnescapeValuesRecursive(node2);
			}
			foreach (ConfigNode.Value value in theNode.values)
			{
				value.value = value.value.Replace("\\n", "\n");
				value.value = value.value.Replace("\\t", "\t");
			}
		}
	}

	public static void DeserializeFromNode(this object obj, ConfigNode node)
	{
		obj.ThrowIfNullArgument("obj");
		node.ThrowIfNullArgument("node");
		NodeDataList nodeDataList = NodeDataListLibrary.Get(obj.GetType());
		OperationContext context = new OperationContext(Operation.Deserialize, obj);
		nodeDataList.Load(node, context);
	}

	public static T CloneUsingFields<T>(this T obj) where T : new()
	{
		T val = new T();
		DeserializeFromNode(node: obj.SerializeToNode(), obj: val);
		return val;
	}
}
