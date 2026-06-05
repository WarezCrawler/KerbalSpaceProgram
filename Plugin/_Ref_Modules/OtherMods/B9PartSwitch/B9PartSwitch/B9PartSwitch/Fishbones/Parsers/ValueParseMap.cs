using System;
using System.Collections.Generic;

namespace B9PartSwitch.Fishbones.Parsers;

public class ValueParseMap : IMutableValueParseMap, IValueParseMap
{
	protected Dictionary<Type, IValueParser> parsers = new Dictionary<Type, IValueParser>();

	public virtual IValueParser GetParser(Type parseType)
	{
		parseType.ThrowIfNullArgument("parseType");
		if (parsers.TryGetValue(parseType, out var value))
		{
			return value;
		}
		throw new ParseTypeNotRegisteredException(parseType);
	}

	public virtual void AddParser<T>(Func<string, T> parse, Func<T, string> format)
	{
		parse.ThrowIfNullArgument("parse");
		format.ThrowIfNullArgument("format");
		AddParser(new ValueParser<T>(parse, format));
	}

	public virtual void AddParser(IValueParser parser)
	{
		parser.ThrowIfNullArgument("parser");
		if (!CanAdd(parser.ParseType))
		{
			throw new ParseTypeAlreadyRegisteredException(parser.ParseType);
		}
		parsers[parser.ParseType] = parser;
	}

	public virtual bool CanParse(Type parseType)
	{
		parseType.ThrowIfNullArgument("parseType");
		return parsers.ContainsKey(parseType);
	}

	public virtual bool CanAdd(Type parseType)
	{
		parseType.ThrowIfNullArgument("parseType");
		return !parsers.ContainsKey(parseType);
	}

	public ValueParseMap Clone()
	{
		ValueParseMap valueParseMap = new ValueParseMap();
		foreach (IValueParser value in parsers.Values)
		{
			valueParseMap.AddParser(value);
		}
		return valueParseMap;
	}
}
