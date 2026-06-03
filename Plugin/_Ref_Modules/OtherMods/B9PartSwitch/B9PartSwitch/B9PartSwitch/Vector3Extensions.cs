using UnityEngine;

namespace B9PartSwitch;

public static class Vector3Extensions
{
	public static Vector3 NaN()
	{
		return new Vector3(float.NaN, float.NaN, float.NaN);
	}

	public static bool IsFinite(this Vector3 vector)
	{
		if (vector.x > float.NegativeInfinity && vector.x < float.PositiveInfinity && vector.y > float.NegativeInfinity && vector.y < float.PositiveInfinity && vector.z > float.NegativeInfinity)
		{
			return vector.z < float.PositiveInfinity;
		}
		return false;
	}
}
