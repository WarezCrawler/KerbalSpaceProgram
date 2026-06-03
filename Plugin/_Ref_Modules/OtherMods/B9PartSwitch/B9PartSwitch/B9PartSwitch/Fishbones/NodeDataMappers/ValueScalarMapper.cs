using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones.NodeDataMappers;

public class ValueScalarMapper : INodeDataMapper
{
	public readonly string name;

	public readonly IValueParser parser;

	public ValueScalarMapper(string name, IValueParser parser)
	{
		name.ThrowIfNullArgument("name");
		parser.ThrowIfNullArgument("parser");
		this.name = name;
		this.parser = parser;
	}

	public bool Load(ref object fieldValue, ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		string value = node.GetValue(name);
		if (value.IsNull())
		{
			return false;
		}
		fieldValue = parser.Parse(value);
		return true;
	}

	public bool Save(object fieldValue, ConfigNode node, OperationContext context)
	{
		node.ThrowIfNullArgument("node");
		if (fieldValue.IsNull())
		{
			return false;
		}
		string text = parser.Format(fieldValue);
		if (text.IsNull())
		{
			return false;
		}
		node.SetValue(name, text, createIfNotFound: true);
		return true;
	}
}
