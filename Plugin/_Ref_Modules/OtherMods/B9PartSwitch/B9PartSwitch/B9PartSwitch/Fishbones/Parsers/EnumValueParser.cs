using System;

namespace B9PartSwitch.Fishbones.Parsers;

public class EnumValueParser : IValueParser
{
	private readonly Type enumType;

	public Type ParseType => enumType;

	public EnumValueParser(Type enumType)
	{
		enumType.ThrowIfNullArgument("enumType");
		if (!enumType.IsEnum)
		{
			throw new ArgumentException($"Expecting enum type but got '{enumType}'", "enumType");
		}
		this.enumType = enumType;
	}

	public object Parse(string value)
	{
		value.ThrowIfNullArgument("value");
		return Enum.Parse(enumType, value);
	}

	public string Format(object value)
	{
		value.ThrowIfNullArgument("value");
		value.EnsureArgumentType(enumType, "value");
		return Enum.Format(enumType, value, "g");
	}
}
