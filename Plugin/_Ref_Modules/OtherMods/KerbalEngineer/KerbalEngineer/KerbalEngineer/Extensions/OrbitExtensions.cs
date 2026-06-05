using System;
using KerbalEngineer.Helpers;
using UnityEngine;

namespace KerbalEngineer.Extensions;

public static class OrbitExtensions
{
	public const double Tau = Math.PI * 2.0;

	public static double GetAngleToAscendingNode(this Orbit orbit)
	{
		return orbit.GetAngleToTrueAnomaly(orbit.GetTrueAnomalyOfAscendingNode());
	}

	public static double GetAngleToDescendingNode(this Orbit orbit)
	{
		return orbit.GetAngleToTrueAnomaly(orbit.GetTrueAnomalyOfDescendingNode());
	}

	public static double GetAngleToPrograde(this Orbit orbit)
	{
		return orbit.GetAngleToPrograde(Planetarium.GetUniversalTime());
	}

	public static double GetAngleToPrograde(this Orbit orbit, double universalTime)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)orbit.referenceBody == (Object)(object)CelestialBodies.SystemBody.CelestialBody)
		{
			return 0.0;
		}
		Vector3d relativePositionAtUT = orbit.getRelativePositionAtUT(universalTime);
		relativePositionAtUT.z = 0.0;
		Vector3d orbitalVelocityAtUT = orbit.referenceBody.orbit.getOrbitalVelocityAtUT(universalTime);
		orbitalVelocityAtUT.z = 0.0;
		double angleBetweenVectors = AngleHelper.GetAngleBetweenVectors(orbitalVelocityAtUT, relativePositionAtUT);
		return AngleHelper.Clamp360((orbit.inclination < 90.0) ? angleBetweenVectors : (360.0 - angleBetweenVectors));
	}

	public static double GetAngleToRetrograde(this Orbit orbit)
	{
		return orbit.GetAngleToRetrograde(Planetarium.GetUniversalTime());
	}

	public static double GetAngleToRetrograde(this Orbit orbit, double universalTime)
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)orbit.referenceBody == (Object)(object)CelestialBodies.SystemBody.CelestialBody)
		{
			return 0.0;
		}
		Vector3d relativePositionAtUT = orbit.getRelativePositionAtUT(universalTime);
		relativePositionAtUT.z = 0.0;
		Vector3d orbitalVelocityAtUT = orbit.referenceBody.orbit.getOrbitalVelocityAtUT(universalTime);
		orbitalVelocityAtUT.z = 0.0;
		double angleBetweenVectors = AngleHelper.GetAngleBetweenVectors(-orbitalVelocityAtUT, relativePositionAtUT);
		return AngleHelper.Clamp360((orbit.inclination < 90.0) ? angleBetweenVectors : (360.0 - angleBetweenVectors));
	}

	public static double GetAngleToTrueAnomaly(this Orbit orbit, double trueAnomaly)
	{
		return AngleHelper.Clamp360(trueAnomaly - orbit.trueAnomaly * (180.0 / Math.PI));
	}

	public static double GetAngleToVector(this Orbit orbit, Vector3d vector)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		return orbit.GetAngleToTrueAnomaly(orbit.GetTrueAnomalyFromVector(Vector3d.Exclude(orbit.GetOrbitNormal(), vector)));
	}

	public static double GetPhaseAngle(this Orbit orbit, Orbit target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		double angleBetweenVectors = AngleHelper.GetAngleBetweenVectors(Vector3d.Exclude(orbit.GetOrbitNormal(), target.pos), orbit.pos);
		if (!(orbit.semiMajorAxis < target.semiMajorAxis))
		{
			return angleBetweenVectors - 360.0;
		}
		return angleBetweenVectors;
	}

	public static double GetRelativeInclination(this Orbit orbit, Orbit target)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Vector3d.Angle(orbit.GetOrbitNormal(), target.GetOrbitNormal());
	}

	public static double GetTimeToAscendingNode(this Orbit orbit)
	{
		return orbit.GetTimeToTrueAnomaly(orbit.GetTrueAnomalyOfAscendingNode());
	}

	public static double GetTimeToDescendingNode(this Orbit orbit)
	{
		return orbit.GetTimeToTrueAnomaly(orbit.GetTrueAnomalyOfDescendingNode());
	}

	public static double GetTimeToTrueAnomaly(this Orbit orbit, double trueAnomaly)
	{
		double dTforTrueAnomaly = orbit.GetDTforTrueAnomaly(trueAnomaly * 0.01745329238474369, orbit.period);
		if (!(dTforTrueAnomaly < 0.0))
		{
			return dTforTrueAnomaly;
		}
		return dTforTrueAnomaly + orbit.period;
	}

	public static double GetTimeToVector(this Orbit orbit, Vector3d vector)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return orbit.GetTimeToTrueAnomaly(orbit.GetTrueAnomalyFromVector(vector));
	}

	public static double GetTrueAnomalyFromVector(this Orbit orbit, Vector3d vector)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return orbit.GetTrueAnomalyOfZupVector(vector) * 57.295780181884766;
	}

	public static double GetTrueAnomalyOfAscendingNode(this Orbit orbit)
	{
		return 360.0 - orbit.argumentOfPeriapsis;
	}

	public static double GetTrueAnomalyOfDescendingNode(this Orbit orbit)
	{
		return 180.0 - orbit.argumentOfPeriapsis;
	}
}
