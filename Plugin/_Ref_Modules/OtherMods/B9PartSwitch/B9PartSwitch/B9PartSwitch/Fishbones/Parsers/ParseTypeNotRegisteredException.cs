using System;

namespace B9PartSwitch.Fishbones.Parsers;

[Serializable]
public class ParseTypeNotRegisteredException : Exception
{
	public ParseTypeNotRegisteredException(Type parseType)
		: base($"Attempted to get the parser for type '{parseType}', but it has not been registered")
	{
		parseType.ThrowIfNullArgument("parseType");
	}
}
