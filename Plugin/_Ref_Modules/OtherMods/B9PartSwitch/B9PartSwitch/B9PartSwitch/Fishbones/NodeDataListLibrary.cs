using System;
using System.Collections.Generic;
using UnityEngine;

namespace B9PartSwitch.Fishbones;

public static class NodeDataListLibrary
{
	private static readonly Dictionary<Type, NodeDataList> dict = new Dictionary<Type, NodeDataList>();

	public static NodeDataList Get<T>()
	{
		return Get(typeof(T));
	}

	public static NodeDataList Get(Type type)
	{
		type.ThrowIfNullArgument("type");
		if (dict.TryGetValue(type, out var value))
		{
			return value;
		}
		try
		{
			Debug.Log($"Generating field configuration for type {type}");
			value = new NodeDataListBuilder(type).CreateList();
		}
		catch (Exception innerException)
		{
			Exception ex = new Exception($"Fatal exception while generating field configuration for type {type}", innerException);
			FatalErrorHandler.HandleFatalError(ex);
			throw ex;
		}
		dict[type] = value;
		return value;
	}
}
