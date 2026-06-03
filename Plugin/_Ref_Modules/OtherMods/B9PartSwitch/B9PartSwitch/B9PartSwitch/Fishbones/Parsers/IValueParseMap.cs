using System;

namespace B9PartSwitch.Fishbones.Parsers;

public interface IValueParseMap
{
	IValueParser GetParser(Type parseType);

	bool CanParse(Type parseType);
}
