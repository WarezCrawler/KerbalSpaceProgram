using System;

namespace B9PartSwitch.Fishbones.Parsers;

public interface IValueParser
{
	Type ParseType { get; }

	object Parse(string value);

	string Format(object value);
}
