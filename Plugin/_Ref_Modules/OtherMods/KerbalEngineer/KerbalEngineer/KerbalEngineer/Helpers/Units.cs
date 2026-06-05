using System;

namespace KerbalEngineer.Helpers;

public static class Units
{
	public const double GRAVITY = 9.80665;

	public const double RAD_TO_DEG = 180.0 / Math.PI;

	public const double DEG_TO_RAD = Math.PI / 180.0;

	public static string Concat(int value1, int value2)
	{
		return value1 + " / " + value2;
	}

	public static string ConcatF(double value1, double value2, int decimals = 1)
	{
		return value1.ToString("F" + decimals) + " / " + value2.ToString("F" + decimals);
	}

	public static string ConcatF(double value1, double value2, double value3, int decimals = 1)
	{
		return value1.ToString("F" + decimals) + " / " + value2.ToString("F" + decimals) + " / " + value3.ToString("F" + decimals);
	}

	public static string ConcatN(double value1, double value2, int decimals = 1)
	{
		return value1.ToString("N" + decimals) + " / " + value2.ToString("N" + decimals);
	}

	public static string ConcatN(double value1, double value2, double value3, int decimals = 1)
	{
		return value1.ToString("N" + decimals) + " / " + value2.ToString("N" + decimals) + " / " + value3.ToString("N" + decimals);
	}

	public static string Cost(double value, int decimals = 1)
	{
		if (value >= 1000000.0)
		{
			return (value / 1000.0).ToString("N" + decimals) + "K";
		}
		return value.ToString("N" + decimals);
	}

	public static string Cost(double value1, double value2, int decimals = 1)
	{
		if (value1 >= 1000000.0 || value2 >= 1000000.0)
		{
			return (value1 / 1000.0).ToString("N" + decimals) + " / " + (value2 / 1000.0).ToString("N" + decimals) + "K";
		}
		return value1.ToString("N" + decimals) + " / " + value2.ToString("N" + decimals);
	}

	public static string ToAcceleration(double value, int decimals = 2)
	{
		return value.ToString("N" + decimals) + "m/s²";
	}

	public static string ToAcceleration(double value1, double value2, int decimals = 2)
	{
		return value1.ToString("N" + decimals) + " / " + value2.ToString("N" + decimals) + "m/s²";
	}

	public static string ToAngle(double value, int decimals = 5)
	{
		return value.ToString("F" + decimals) + "°";
	}

	public static string ToAngleDMS(double value)
	{
		double num = Math.Abs(value);
		int num2 = (int)Math.Floor(num);
		double num3 = num - (double)num2;
		int num4 = (int)Math.Floor(num3 * 60.0);
		int num5 = (int)Math.Floor((num3 - (double)num4 / 60.0) * 3600.0);
		return $"{num2:0}° {num4:00}' {num5:00}\"";
	}

	public static string ToDistance(double value, int decimals = 1)
	{
		if (Math.Abs(value) < 1000000.0)
		{
			if (Math.Abs(value) >= 10.0)
			{
				return value.ToString("N" + decimals) + "m";
			}
			value *= 100.0;
			if (Math.Abs(value) >= 100.0)
			{
				return value.ToString("N" + decimals) + "cm";
			}
			value *= 10.0;
			return value.ToString("N" + decimals) + "mm";
		}
		value /= 1000.0;
		if (Math.Abs(value) < 1000000.0)
		{
			return value.ToString("N" + decimals) + "km";
		}
		value /= 1000.0;
		return value.ToString("N" + decimals) + "Mm";
	}

	public static string ToFlux(double value)
	{
		return value.ToString("#,0.00") + "kW";
	}

	public static string ToForce(double value)
	{
		return value.ToString((!(value < 100000.0)) ? "N0" : ((!(value < 10000.0)) ? "N1" : ((!(value < 100.0)) ? "N2" : ((Math.Abs(value) < double.Epsilon) ? "N0" : "N3")))) + "kN";
	}

	public static string ToForce(double value1, double value2)
	{
		string text = ((!(value1 < 100000.0)) ? "N0" : ((!(value1 < 10000.0)) ? "N1" : ((!(value1 < 100.0)) ? "N2" : ((Math.Abs(value1) < double.Epsilon) ? "N0" : "N3"))));
		string text2 = ((!(value2 < 100000.0)) ? "N0" : ((!(value2 < 10000.0)) ? "N1" : ((!(value2 < 100.0)) ? "N2" : ((Math.Abs(value2) < double.Epsilon) ? "N0" : "N3"))));
		return value1.ToString(text) + " / " + value2.ToString(text2) + "kN";
	}

	public static string ToMach(double value)
	{
		return value.ToString("0.00") + "Ma";
	}

	public static string ToMass(double value, int decimals = 0)
	{
		if (value > 10000000000000.0)
		{
			return value.ToString("e" + decimals + 8) + "t";
		}
		if (value >= 1000.0)
		{
			return value.ToString("N" + decimals + 2) + "t";
		}
		value *= 1000.0;
		return value.ToString("N" + decimals) + "kg";
	}

	public static string ToMass(double value1, double value2, int decimals = 0)
	{
		if (value1 >= 1000.0 || value2 >= 1000.0)
		{
			return value1.ToString("N" + decimals + 2) + " / " + value2.ToString("N" + decimals + 2) + "t";
		}
		value1 *= 1000.0;
		value2 *= 1000.0;
		return value1.ToString("N" + decimals) + " / " + value2.ToString("N" + decimals) + "kg";
	}

	public static string ToPercent(double value, int decimals = 2)
	{
		value *= 100.0;
		return value.ToString("F" + decimals) + "%";
	}

	public static string ToPressure(double value)
	{
		return value.ToString((!(value < 100000.0)) ? "N0" : ((!(value < 10000.0)) ? "N1" : ((!(value < 100.0)) ? "N2" : ((Math.Abs(value) < double.Epsilon) ? "N0" : "N3")))) + "kN/m²";
	}

	public static string ToRate(double value, int decimals = 1)
	{
		if (!(value < 1.0))
		{
			return value.ToString("F" + decimals) + "/sec";
		}
		return (value * 60.0).ToString("F" + decimals) + "/min";
	}

	public static string ToSpeed(double value, int decimals = 2)
	{
		if (Math.Abs(value) < 1.0)
		{
			return (value * 1000.0).ToString("N" + decimals) + "mm/s";
		}
		return value.ToString("N" + decimals) + "m/s";
	}

	public static string ToTemperature(double value)
	{
		return value.ToString("#,0") + "K";
	}

	public static string ToTemperature(double value1, double value2)
	{
		return value1.ToString("#,0") + " / " + value2.ToString("#,0") + "K";
	}

	public static string ToTime(double value)
	{
		return TimeFormatter.ConvertToString(value);
	}

	public static string ToTorque(double value)
	{
		return value.ToString((!(value < 100.0)) ? "N0" : ((Math.Abs(value) < double.Epsilon) ? "N0" : "N2")) + "kNm";
	}
}
