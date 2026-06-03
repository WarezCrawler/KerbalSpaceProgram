using System;

namespace B9PartSwitch.Utils;

public class NormalStringMatcher : IStringMatcher
{
	private readonly string str;

	public NormalStringMatcher(string str)
	{
		this.str = str ?? throw new ArgumentNullException("str");
	}

	public bool Match(string testMatch)
	{
		testMatch.ThrowIfNullArgument("testMatch");
		return testMatch == str;
	}

	public override string ToString()
	{
		if (str.Length > 2 && str[0] == '/' && str[str.Length - 1] == '/')
		{
			return "\\" + str;
		}
		return str;
	}
}
