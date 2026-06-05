using System;

namespace B9PartSwitch.Fishbones.Parsers;

public static class NodeObjectWrapper
{
	public static INodeObjectWrapper For(Type type)
	{
		type.ThrowIfNullArgument("type");
		if (type.Implements<IConfigNode>())
		{
			return new NodeObjectWrapperIConfigNode(type);
		}
		if (type.Implements<IContextualNode>())
		{
			return new NodeObjectWrapperIContextualNode(type);
		}
		if (type == typeof(ConfigNode))
		{
			return new NodeObjectWrapperConfigNode();
		}
		throw new NotImplementedException($"No way to build node object wrapper for type {type}");
	}

	public static bool IsNodeType(Type type)
	{
		if (!type.Implements<IConfigNode>() && !type.Implements<IContextualNode>())
		{
			return type == typeof(ConfigNode);
		}
		return true;
	}
}
