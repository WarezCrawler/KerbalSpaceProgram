using System;

namespace B9PartSwitch.Fishbones.Parsers;

[Serializable]
public class ParseTypeAlreadyRegisteredException : Exception
{
	public ParseTypeAlreadyRegisteredException(Type parseType)
		: base($"Attempted to register perser for type '{parseType}', but it has already been registered")
	{
		parseType.ThrowIfNullArgument("parseType");
	}
}
