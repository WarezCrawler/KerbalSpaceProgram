using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using UnityEngine;

namespace InterstellarFuelSwitch;

public static class ParseTools
{
	public static double ParseDouble(string data)
	{
		string text = data.Trim();
		if (text == string.Empty)
		{
			return 0.0;
		}
		return double.Parse(data);
	}

	public static List<double> ParseDoubles<T>(string stringOfDoubles, Expression<Func<T>> expr)
	{
		if (string.IsNullOrEmpty(stringOfDoubles))
		{
			return new List<double>();
		}
		MemberExpression memberExpression = (MemberExpression)expr.Body;
		return ParseDoubles(stringOfDoubles, memberExpression.Member.Name);
	}

	public static List<double> ParseDoubles(string stringOfDoubles, string errorDisplayName = "")
	{
		List<double> list = new List<double>();
		string[] array = stringOfDoubles.Trim().Split(';');
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (double.TryParse(text.Trim(), out var result))
			{
				list.Add(result);
				continue;
			}
			Debug.Log("InsterstellarFuelSwitch: parseDoubles error in '" + stringOfDoubles + "', invalid float: " + errorDisplayName + " [len:" + text.Length + "] '" + text + "']");
		}
		return list;
	}

	public static List<int> ParseIntegers(string stringOfInts)
	{
		List<int> list = new List<int>();
		string[] array = stringOfInts.Split(';');
		for (int i = 0; i < array.Length; i++)
		{
			int result = 0;
			if (int.TryParse(array[i], out result))
			{
				list.Add(result);
			}
			else
			{
				Debug.Log("InsterstellarFuelSwitch: error in '" + stringOfInts + "',  invalid integer: " + array[i]);
			}
		}
		return list;
	}

	public static List<string> ParseNames(string names)
	{
		return ParseNames(names, replaceBackslashErrors: false, trimWhiteSpace: true, string.Empty);
	}

	public static List<string> ParseNames(string names, bool replaceBackslashErrors)
	{
		return ParseNames(names, replaceBackslashErrors, trimWhiteSpace: true, string.Empty);
	}

	public static List<string> ParseNames(string names, bool replaceBackslashErrors, bool trimWhiteSpace, string prefix)
	{
		List<string> list = names.Split(';', ',').ToList();
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (list[num] == string.Empty)
			{
				list.RemoveAt(num);
			}
		}
		if (trimWhiteSpace)
		{
			for (int i = 0; i < list.Count; i++)
			{
				list[i] = list[i].Trim(' ');
			}
		}
		if (prefix != string.Empty)
		{
			for (int j = 0; j < list.Count; j++)
			{
				list[j] = prefix + list[j];
			}
		}
		if (replaceBackslashErrors)
		{
			for (int k = 0; k < list.Count; k++)
			{
				list[k] = list[k].Replace('\\', '/');
			}
		}
		return list.ToList();
	}

	public static string Print(IEnumerable<string> list)
	{
		string text = "";
		foreach (string item in list)
		{
			text = text + item + ";";
		}
		return text;
	}

	public static bool ListEquals<T>(IList<T> list1, IList<T> list2)
	{
		if (list1.Count != list2.Count)
		{
			return false;
		}
		for (int i = 0; i < list1.Count; i++)
		{
			if (!list1[i].Equals(list2[i]))
			{
				return false;
			}
		}
		return true;
	}
}
