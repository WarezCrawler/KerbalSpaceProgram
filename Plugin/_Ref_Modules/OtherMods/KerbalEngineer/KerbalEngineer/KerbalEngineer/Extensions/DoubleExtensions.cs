using KerbalEngineer.Helpers;

namespace KerbalEngineer.Extensions;

public static class DoubleExtensions
{
	public static double Clamp(this double value, double lower, double higher)
	{
		if (!(value < lower))
		{
			if (!(value > higher))
			{
				return value;
			}
			return higher;
		}
		return lower;
	}

	public static bool IsValid(this double value)
	{
		if (!double.IsNaN(value))
		{
			return !double.IsInfinity(value);
		}
		return false;
	}

	public static string ToAcceleration(this double value)
	{
		return Units.ToAcceleration(value);
	}

	public static string ToAngle(this double value)
	{
		return Units.ToAngle(value);
	}

	public static string ToDistance(this double value)
	{
		return Units.ToDistance(value);
	}

	public static string ToFlux(this double value)
	{
		return Units.ToFlux(value);
	}

	public static string ToForce(this double value)
	{
		return Units.ToForce(value);
	}

	public static string ToMach(this double value)
	{
		return Units.ToMach(value);
	}

	public static string ToMass(this double value)
	{
		return Units.ToMass(value);
	}

	public static string ToPercent(this double value)
	{
		return Units.ToPercent(value);
	}

	public static string ToPressure(this double value)
	{
		return Units.ToPressure(value);
	}

	public static string ToRate(this double value)
	{
		return Units.ToRate(value);
	}

	public static string ToSpeed(this double value)
	{
		return Units.ToSpeed(value);
	}

	public static string ToTemperature(this double value)
	{
		return Units.ToTemperature(value);
	}

	public static string ToTorque(this double value)
	{
		return Units.ToTorque(value);
	}
}
