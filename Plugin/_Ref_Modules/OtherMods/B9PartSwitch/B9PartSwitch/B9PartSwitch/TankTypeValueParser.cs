using System;
using B9PartSwitch.Fishbones.Parsers;

namespace B9PartSwitch;

public class TankTypeValueParser : IValueParser
{
	public Type ParseType => typeof(TankType);

	public object Parse(string value)
	{
		value.ThrowIfNullArgument("value");
		return B9TankSettings.GetTankType(value);
	}

	public string Format(object value)
	{
		value.ThrowIfNullArgument("value");
		return ((TankType)value).tankName;
	}
}
