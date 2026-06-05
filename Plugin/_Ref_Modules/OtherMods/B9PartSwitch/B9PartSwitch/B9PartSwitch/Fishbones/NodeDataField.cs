using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.Fishbones.FieldWrappers;
using B9PartSwitch.Fishbones.NodeDataMappers;

namespace B9PartSwitch.Fishbones;

public class NodeDataField : INodeDataField
{
	public readonly IFieldWrapper field;

	public readonly IOperaitonManager operationManager;

	public string Name => field.Name;

	public NodeDataField(IFieldWrapper field, IOperaitonManager operationManager)
	{
		field.ThrowIfNullArgument("field");
		operationManager.ThrowIfNullArgument("operationManager");
		this.field = field;
		this.operationManager = operationManager;
	}

	public void Load(ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		INodeDataMapper nodeDataMapper = operationManager.MapperFor(context.Operation);
		if (!nodeDataMapper.IsNull())
		{
			object fieldValue = field.GetValue(context.Subject);
			if (nodeDataMapper.Load(ref fieldValue, node, context))
			{
				field.SetValue(context.Subject, fieldValue);
			}
		}
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		context.ThrowIfNullArgument("context");
		INodeDataMapper nodeDataMapper = operationManager.MapperFor(context.Operation);
		if (!nodeDataMapper.IsNull())
		{
			object value = field.GetValue(context.Subject);
			nodeDataMapper.Save(value, node, context);
		}
	}
}
