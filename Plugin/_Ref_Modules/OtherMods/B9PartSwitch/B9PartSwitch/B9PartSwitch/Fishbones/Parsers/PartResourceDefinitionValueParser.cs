using System;

namespace B9PartSwitch.Fishbones.Parsers;

public class PartResourceDefinitionValueParser : ValueParser<PartResourceDefinition>
{
	[Serializable]
	public class PartResourceNotFoundException : Exception
	{
		public PartResourceNotFoundException(string name)
			: base("No resource definition named '" + name + "' could be found")
		{
		}
	}

	public static PartResourceDefinition FindResourceDefinition(string name)
	{
		name.ThrowIfNullArgument("name");
		PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition(name);
		if (definition.IsNull())
		{
			throw new PartResourceNotFoundException(name);
		}
		return definition;
	}

	public PartResourceDefinitionValueParser()
		: base((Func<string, PartResourceDefinition>)FindResourceDefinition, (Func<PartResourceDefinition, string>)((PartResourceDefinition def) => def.name))
	{
	}
}
