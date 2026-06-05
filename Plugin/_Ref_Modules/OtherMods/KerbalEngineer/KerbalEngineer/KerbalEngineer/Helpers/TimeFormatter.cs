using System;

namespace KerbalEngineer.Helpers;

public static class TimeFormatter
{
	public static string ConvertToString(double seconds, string format = "F1")
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		bool flag = seconds < 0.0;
		seconds = Math.Abs(seconds);
		if (seconds > 0.0)
		{
			num = (int)(seconds / (double)KSPUtil.dateTimeFormatter.Year);
			seconds -= (double)(num * KSPUtil.dateTimeFormatter.Year);
			num2 = (int)(seconds / (double)KSPUtil.dateTimeFormatter.Day);
			seconds -= (double)(num2 * KSPUtil.dateTimeFormatter.Day);
			num3 = (int)(seconds / 3600.0);
			seconds -= (double)num3 * 3600.0;
			num4 = (int)(seconds / 60.0);
			seconds -= (double)num4 * 60.0;
		}
		if (num > 0)
		{
			return (flag ? "-" : "") + $"{num}y {num2}d {num3}h {num4}m {seconds.ToString(format)}s";
		}
		if (num2 > 0)
		{
			return (flag ? "-" : "") + $"{num2}d {num3}h {num4}m {seconds.ToString(format)}s";
		}
		if (num3 > 0)
		{
			return (flag ? "-" : "") + $"{num3}h {num4}m {seconds.ToString(format)}s";
		}
		return (flag ? "-" : "") + ((num4 > 0) ? $"{num4}m {seconds.ToString(format)}s" : $"{seconds.ToString(format)}s");
	}
}
