using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.Helpers;

public static class AngleHelper
{
	public static double Clamp180(double angle)
	{
		if (angle.IsValid())
		{
			if (angle < -180.0)
			{
				do
				{
					angle += 360.0;
				}
				while (angle < -180.0);
			}
			else if (angle > 180.0)
			{
				do
				{
					angle -= 360.0;
				}
				while (angle > 180.0);
			}
		}
		return angle;
	}

	public static double Clamp360(double angle)
	{
		if (angle.IsValid())
		{
			if (angle < 0.0)
			{
				do
				{
					angle += 360.0;
				}
				while (angle < 0.0);
			}
			else if (angle >= 360.0)
			{
				do
				{
					angle -= 360.0;
				}
				while (angle >= 360.0);
			}
		}
		return angle;
	}

	public static double ClampBetween(double value, double minimum, double maximum)
	{
		if (value.IsValid() && minimum.IsValid() && maximum.IsValid())
		{
			while (value < minimum)
			{
				value += maximum;
			}
			while (value > maximum)
			{
				value -= maximum;
			}
		}
		return value;
	}

	public static double GetAngleBetweenVectors(Vector3d left, Vector3d right)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		double num = Vector3d.Angle(left, right);
		if (Vector3d.Angle(QuaternionD.AngleAxis(90.0, Vector3d.forward) * right, left) > 90.0)
		{
			return 360.0 - num;
		}
		return num;
	}
}
