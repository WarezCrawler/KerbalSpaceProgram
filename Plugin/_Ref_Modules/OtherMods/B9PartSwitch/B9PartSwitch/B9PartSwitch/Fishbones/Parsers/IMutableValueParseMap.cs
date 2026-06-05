using System;

namespace B9PartSwitch.Fishbones.Parsers;

public interface IMutableValueParseMap : IValueParseMap
{
	void AddParser<T>(Func<string, T> parse, Func<T, string> format);

	void AddParser(IValueParser parser);
}
