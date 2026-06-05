using System;

namespace B9PartSwitch.Fishbones.Parsers;

public class ValueParser<T> : IValueParser
{
	private readonly Func<string, T> parseFunction;

	private readonly Func<T, string> formatFunction;

	public Type ParseType => typeof(T);

	public ValueParser(Func<string, T> parseFunction, Func<T, string> formatFunction)
	{
		parseFunction.ThrowIfNullArgument("parseFunction");
		formatFunction.ThrowIfNullArgument("formatFunction");
		this.parseFunction = parseFunction;
		this.formatFunction = formatFunction;
	}

	public object Parse(string value)
	{
		value.ThrowIfNullArgument("value");
		return parseFunction(value);
	}

	public string Format(object value)
	{
		value.ThrowIfNullArgument("value");
		value.EnsureArgumentType<T>("value");
		return formatFunction((T)value);
	}
}
