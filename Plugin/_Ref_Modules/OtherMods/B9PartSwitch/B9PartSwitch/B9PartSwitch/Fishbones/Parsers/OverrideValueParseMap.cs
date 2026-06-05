using System;
using System.Linq;

namespace B9PartSwitch.Fishbones.Parsers;

public class OverrideValueParseMap : IValueParseMap
{
	private readonly IValueParseMap innerParseMap;

	private readonly IValueParser[] overrides;

	public OverrideValueParseMap(IValueParseMap innerParseMap, params IValueParser[] overrides)
	{
		innerParseMap.ThrowIfNullArgument("innerParseMap");
		overrides.ThrowIfNullArgument("overrides");
		this.overrides = new IValueParser[overrides.Length];
		for (int i = 0; i < overrides.Length; i++)
		{
			IValueParser parser = overrides[i];
			if (parser.IsNull())
			{
				throw new ArgumentNullException($"Encountered null value at index {i}", "overrides");
			}
			if (this.overrides.Any((IValueParser x) => x?.ParseType == parser.ParseType))
			{
				throw new ArgumentException($"Attempted to register override for type {parser.ParseType} more than once", "overrides");
			}
			this.overrides[i] = parser;
		}
		this.innerParseMap = innerParseMap;
	}

	public bool CanParse(Type parseType)
	{
		parseType.ThrowIfNullArgument("parseType");
		if (overrides.Any((IValueParser parser) => parser.ParseType == parseType))
		{
			return true;
		}
		if (parseType.IsNullableValueType())
		{
			Type valueType = parseType.GetGenericArguments()[0];
			if (overrides.Any((IValueParser parser) => parser.ParseType == valueType))
			{
				return true;
			}
		}
		return innerParseMap.CanParse(parseType);
	}

	public IValueParser GetParser(Type parseType)
	{
		parseType.ThrowIfNullArgument("parseType");
		IValueParser valueParser = overrides.FirstOrDefault((IValueParser testParser) => testParser.ParseType == parseType);
		if (valueParser != null)
		{
			return valueParser;
		}
		if (parseType.IsNullableValueType())
		{
			Type type = parseType.GetGenericArguments()[0];
			IValueParser[] array = overrides;
			foreach (IValueParser valueParser2 in array)
			{
				if (valueParser2.ParseType == type)
				{
					return valueParser2;
				}
			}
		}
		return overrides.FirstOrDefault((IValueParser testParser) => testParser.ParseType == parseType) ?? innerParseMap.GetParser(parseType);
	}
}
