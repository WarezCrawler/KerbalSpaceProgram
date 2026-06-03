using System;

namespace B9PartSwitch.Fishbones.Parsers;

public class ValueParseMapWrapper : IValueParseMap
{
	private readonly IValueParseMap map;

	public ValueParseMapWrapper(IValueParseMap map)
	{
		map.ThrowIfNullArgument("map");
		this.map = map;
	}

	public IValueParser GetParser(Type parseType)
	{
		parseType.ThrowIfNullArgument("parseType");
		return map.GetParser(parseType);
	}

	public bool CanParse(Type parseType)
	{
		parseType.ThrowIfNullArgument("parseType");
		return map.CanParse(parseType);
	}
}
