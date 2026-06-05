namespace KerbalEngineer.Extensions;

public static class StringExtensions
{
	public static string ToLength(this string value, int length)
	{
		if (value != null && value.Length > length)
		{
			value = value.Substring(0, length) + "...";
		}
		return value;
	}
}
