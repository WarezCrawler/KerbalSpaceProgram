using System;
using UnityEngine;

namespace B9PartSwitch.Fishbones.Parsers;

public class ScaleParser : ValueParser<Vector3>
{
	private static readonly char[] splitChars = new char[3] { ',', '\t', ' ' };

	public static Vector3 ParseScale(string value)
	{
		value.ThrowIfNullArgument("value");
		string[] array = value.Split(splitChars, StringSplitOptions.RemoveEmptyEntries);
		if (array.Length == 1)
		{
			float num = float.Parse(array[0]);
			return new Vector3(num, num, num);
		}
		if (array.Length == 3)
		{
			float x = float.Parse(array[0]);
			float y = float.Parse(array[1]);
			float z = float.Parse(array[2]);
			return new Vector3(x, y, z);
		}
		throw new FormatException($"Could not parse value as scale because it split into {array.Length} values: '{value}'");
	}

	public ScaleParser()
		: base((Func<string, Vector3>)ParseScale, (Func<Vector3, string>)ConfigNode.WriteVector)
	{
	}
}
