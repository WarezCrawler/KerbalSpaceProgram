using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch.Fishbones;

public interface IUseParser
{
	IValueParser CreateParser();
}
