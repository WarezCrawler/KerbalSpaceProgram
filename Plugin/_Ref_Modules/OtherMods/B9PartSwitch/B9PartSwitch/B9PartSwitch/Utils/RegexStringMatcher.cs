using System;
using System.Text.RegularExpressions;

namespace B9PartSwitch.Utils;

public class RegexStringMatcher : IStringMatcher
{
	private readonly Regex regex;

	public RegexStringMatcher(Regex regex)
	{
		this.regex = regex ?? throw new ArgumentNullException("regex");
	}

	public bool Match(string testMatch)
	{
		testMatch.ThrowIfNullArgument("testMatch");
		return regex.IsMatch(testMatch);
	}

	public override string ToString()
	{
		return $"/{regex}/";
	}
}
