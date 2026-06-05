using System;

namespace B9PartSwitch.Fishbones;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class NodeData : Attribute
{
	public string name;

	public bool persistent;

	public bool alwaysSerialize;
}
