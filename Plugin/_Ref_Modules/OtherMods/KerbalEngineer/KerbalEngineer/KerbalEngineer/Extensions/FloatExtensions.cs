using KerbalEngineer.Helpers;

namespace KerbalEngineer.Extensions;

public static class FloatExtensions
{
	public static string ToAcceleration(this float value)
	{
		return Units.ToAcceleration(value);
	}

	public static string ToAngle(this float value)
	{
		return Units.ToAngle(value);
	}

	public static string ToDistance(this float value)
	{
		return Units.ToDistance(value);
	}

	public static string ToFlux(this float value)
	{
		return Units.ToFlux(value);
	}

	public static string ToForce(this float value)
	{
		return Units.ToForce(value);
	}

	public static string ToMach(this float value)
	{
		return Units.ToMach(value);
	}

	public static string ToMass(this float value)
	{
		return Units.ToMass(value);
	}

	public static string ToPercent(this float value)
	{
		return Units.ToPercent(value);
	}

	public static string ToRate(this float value)
	{
		return Units.ToRate(value);
	}

	public static string ToSpeed(this float value)
	{
		return Units.ToSpeed(value);
	}

	public static string ToTemperature(this float value)
	{
		return Units.ToTemperature(value);
	}

	public static string ToTorque(this float value)
	{
		return Units.ToTorque(value);
	}
}
