using System.Text.RegularExpressions;

namespace B9PartSwitch.Utils;

public static class StringMatcher
{
	public static IStringMatcher Parse(string str)
	{
		str.ThrowIfNullArgument("str");
		if (str.Length > 2 && str[0] == '/' && str[str.Length - 1] == '/')
		{
			return new RegexStringMatcher(new Regex(str.Substring(1, str.Length - 2)));
		}
		if (str.Length > 2 && str[0] == '\\' && str[1] == '/')
		{
			str = str.Substring(1);
		}
		if (str.IndexOf('*') != -1 || str.IndexOf('?') != -1)
		{
			str = Regex.Escape(str).Replace("\\*", ".*").Replace("\\?", ".");
			return new RegexStringMatcher(new Regex("^" + str + "$"));
		}
		return new NormalStringMatcher(str);
	}
}
