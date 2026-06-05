using System;
using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch.Fishbones;

public class NodeDataList
{
	private readonly INodeDataField[] fields;

	public NodeDataList(params INodeDataField[] fields)
	{
		fields.ThrowIfNullArgument("fields");
		this.fields = new INodeDataField[fields.Length];
		for (int i = 0; i < fields.Length; i++)
		{
			INodeDataField nodeDataField = fields[i];
			if (nodeDataField.IsNull())
			{
				throw new ArgumentNullException($"Encountered null in list at position {i}", "fields");
			}
			this.fields[i] = nodeDataField;
		}
	}

	public void Load(ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		INodeDataField[] array = fields;
		foreach (INodeDataField nodeDataField in array)
		{
			try
			{
				nodeDataField.Load(node, context);
			}
			catch (Exception innerException)
			{
				throw new Exception($"Exception while loading field {nodeDataField.Name} on type {context.Subject?.GetType()}", innerException);
			}
		}
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		INodeDataField[] array = fields;
		foreach (INodeDataField nodeDataField in array)
		{
			try
			{
				nodeDataField.Save(node, context);
			}
			catch (Exception innerException)
			{
				throw new Exception($"Exception while saving field {nodeDataField.Name} on type {context.Subject?.GetType()}", innerException);
			}
		}
	}
}
