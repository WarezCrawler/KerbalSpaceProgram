using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace B9PartSwitch.Utils;

public static class ColorParser
{
	private static readonly Dictionary<string, Color> namedColors;

	private static readonly char[] commaSeparator;

	private static readonly char[] spaceSeparator;

	static ColorParser()
	{
		namedColors = new Dictionary<string, Color>();
		commaSeparator = new char[1] { ',' };
		spaceSeparator = new char[2] { ' ', '\t' };
		PropertyInfo[] properties = typeof(Color).GetProperties(BindingFlags.Static | BindingFlags.Public);
		foreach (PropertyInfo propertyInfo in properties)
		{
			if (propertyInfo.CanRead && !(propertyInfo.PropertyType != typeof(Color)))
			{
				namedColors.Add(propertyInfo.Name, (Color)propertyInfo.GetValue(null, null));
			}
		}
		properties = typeof(XKCDColors).GetProperties(BindingFlags.Static | BindingFlags.Public);
		foreach (PropertyInfo propertyInfo2 in properties)
		{
			if (propertyInfo2.CanRead && !(propertyInfo2.PropertyType != typeof(Color)))
			{
				if (namedColors.ContainsKey(propertyInfo2.Name))
				{
					throw new Exception("duplicate key " + propertyInfo2.Name);
				}
				namedColors.Add(propertyInfo2.Name, (Color)propertyInfo2.GetValue(null, null));
			}
		}
		namedColors.Add("ResourceColorLiquidFuel", ResourceColors.LiquidFuel);
		namedColors.Add("ResourceColorLqdHydrogen", ResourceColors.LqdHydrogen);
		namedColors.Add("ResourceColorLqdMethane", ResourceColors.LqdMethane);
		namedColors.Add("ResourceColorOxidizer", ResourceColors.Oxidizer);
		namedColors.Add("ResourceColorMonoPropellant", ResourceColors.MonoPropellant);
		namedColors.Add("ResourceColorXenonGas", ResourceColors.XenonGas);
		namedColors.Add("ResourceColorElectricChargePrimary", ResourceColors.ElectricChargePrimary);
		namedColors.Add("ResourceColorElectricChargeSecondary", ResourceColors.ElectricChargeSecondary);
		namedColors.Add("ResourceColorOre", ResourceColors.Ore);
	}

	public static Color Parse(string colorStr)
	{
		colorStr.ThrowIfNullArgument("colorStr");
		if (colorStr == string.Empty)
		{
			throw new FormatException("Cannot parse empty color");
		}
		if (colorStr[0] == '#')
		{
			switch (colorStr.Length)
			{
			case 4:
			{
				byte num7 = ParseHex(colorStr[1]);
				byte b4 = ParseHex(colorStr[2]);
				return new Color(b: (float)(int)ParseHex(colorStr[3]) / 15f, r: (float)(int)num7 / 15f, g: (float)(int)b4 / 15f);
			}
			case 5:
			{
				byte num6 = ParseHex(colorStr[1]);
				byte b2 = ParseHex(colorStr[2]);
				byte b3 = ParseHex(colorStr[3]);
				return new Color(a: (float)(int)ParseHex(colorStr[4]) / 15f, r: (float)(int)num6 / 15f, g: (float)(int)b2 / 15f, b: (float)(int)b3 / 15f);
			}
			case 7:
			{
				int num4 = (ParseHex(colorStr[1]) << 4) | ParseHex(colorStr[2]);
				int num5 = (ParseHex(colorStr[3]) << 4) | ParseHex(colorStr[4]);
				return new Color(b: (float)((ParseHex(colorStr[5]) << 4) | ParseHex(colorStr[6])) / 255f, r: (float)num4 / 255f, g: (float)num5 / 255f);
			}
			case 9:
			{
				int num = (ParseHex(colorStr[1]) << 4) | ParseHex(colorStr[2]);
				int num2 = (ParseHex(colorStr[3]) << 4) | ParseHex(colorStr[4]);
				int num3 = (ParseHex(colorStr[5]) << 4) | ParseHex(colorStr[6]);
				return new Color(a: (float)((ParseHex(colorStr[7]) << 4) | ParseHex(colorStr[8])) / 255f, r: (float)num / 255f, g: (float)num2 / 255f, b: (float)num3 / 255f);
			}
			default:
				throw new FormatException("Value looks like HTML color (begins with #) but has wrong number of digits (must be 3, 4, 6, or 8): " + colorStr);
			}
		}
		if (namedColors.TryGetValue(colorStr, out var value))
		{
			return value;
		}
		string[] array;
		if (colorStr.IndexOf(',') != -1)
		{
			array = colorStr.Split(commaSeparator, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = array[i].Trim();
			}
		}
		else
		{
			array = colorStr.Split(spaceSeparator, StringSplitOptions.RemoveEmptyEntries);
		}
		if (array.Length == 3)
		{
			float r = ParseFloatValue(array[0]);
			float g = ParseFloatValue(array[1]);
			float b6 = ParseFloatValue(array[2]);
			return new Color(r, g, b6);
		}
		if (array.Length == 4)
		{
			float r2 = ParseFloatValue(array[0]);
			float g2 = ParseFloatValue(array[1]);
			float b7 = ParseFloatValue(array[2]);
			float a3 = ParseFloatValue(array[3]);
			return new Color(r2, g2, b7, a3);
		}
		throw new FormatException("Could not value parse as color: " + colorStr);
	}

	private static byte ParseHex(char value)
	{
		switch (value)
		{
		case '0':
			return 0;
		case '1':
			return 1;
		case '2':
			return 2;
		case '3':
			return 3;
		case '4':
			return 4;
		case '5':
			return 5;
		case '6':
			return 6;
		case '7':
			return 7;
		case '8':
			return 8;
		case '9':
			return 9;
		case 'A':
		case 'a':
			return 10;
		case 'B':
		case 'b':
			return 11;
		case 'C':
		case 'c':
			return 12;
		case 'D':
		case 'd':
			return 13;
		case 'E':
		case 'e':
			return 14;
		case 'F':
		case 'f':
			return 15;
		default:
			throw new FormatException("Invalid hexadecimal character: " + value);
		}
	}

	private static float ParseFloatValue(string valueStr)
	{
		if (!float.TryParse(valueStr, out var result) || float.IsNaN(result) || result < 0f || result > 1f)
		{
			throw new FormatException("Invalid float value when parsing color (should be 0-1): " + valueStr);
		}
		return result;
	}
}
