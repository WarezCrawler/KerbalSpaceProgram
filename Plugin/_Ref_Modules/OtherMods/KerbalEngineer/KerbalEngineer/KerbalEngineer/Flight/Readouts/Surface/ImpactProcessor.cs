using System;
using KerbalEngineer.Drawing;
using KerbalEngineer.Flight.Readouts.Vessel;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class ImpactProcessor : IUpdatable, IUpdateRequest
{
	private static readonly ImpactProcessor instance = new ImpactProcessor();

	public static bool ShowMarker = true;

	public static ImpactProcessor Instance => instance;

	public static double Altitude { get; private set; }

	public static string Biome { get; private set; }

	public static double Latitude { get; private set; }

	public static double Longitude { get; private set; }

	public static bool ShowDetails { get; private set; }

	public static double Time { get; private set; }

	public static double SuicideDeltaV { get; private set; }

	public static double SuicideDistance { get; private set; }

	public static double SuicideAltitude { get; private set; }

	public static double SuicideLength { get; private set; }

	public static double SuicideCountdown { get; private set; }

	public bool UpdateRequested { get; set; }

	public void Update()
	{
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_083a: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a24: Unknown result type (might be due to invalid IL or missing references)
		//IL_09e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_093c: Unknown result type (might be due to invalid IL or missing references)
		//IL_093e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b32: Unknown result type (might be due to invalid IL or missing references)
		//IL_0b4f: Unknown result type (might be due to invalid IL or missing references)
		//IL_097f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0390: Unknown result type (might be due to invalid IL or missing references)
		//IL_0395: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c03: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c10: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c15: Unknown result type (might be due to invalid IL or missing references)
		//IL_0c1a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_045c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0461: Unknown result type (might be due to invalid IL or missing references)
		//IL_0466: Unknown result type (might be due to invalid IL or missing references)
		//IL_046b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0470: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_047d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		if (FlightEngineerCore.gamePaused)
		{
			return;
		}
		Vector3d val = default(Vector3d);
		CelestialBody mainBody = FlightGlobals.ActiveVessel.mainBody;
		Vessel activeVessel = FlightGlobals.ActiveVessel;
		double num = 0.0;
		bool flag = false;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		double num5 = 0.0;
		SuicideAltitude = 0.0;
		SuicideCountdown = 0.0;
		SuicideDeltaV = 0.0;
		SuicideDistance = 0.0;
		SuicideLength = 0.0;
		ShowDetails = false;
		bool flag2 = false;
		if (activeVessel.Landed || (Object)(object)mainBody.pqsController == (Object)null)
		{
			return;
		}
		if (activeVessel.orbit.PeA >= mainBody.minOrbitalDistance)
		{
			if (flag2)
			{
				Debug.Log((object)("no impact: periapse > min alt " + activeVessel.orbit.PeA + mainBody.minOrbitalDistance));
			}
			return;
		}
		if (activeVessel.orbit.eccentricity >= 1.0 && activeVessel.orbit.timeToPe <= 0.0)
		{
			if (flag2)
			{
				Debug.Log((object)"no impact: escaping and passed periapse");
			}
			return;
		}
		flag = false;
		num4 = 0.0;
		num3 = 0.0;
		num2 = 0.0;
		num = 0.0;
		double eccentricity = activeVessel.orbit.eccentricity;
		double semiMajorAxis = activeVessel.orbit.semiMajorAxis;
		double num6 = mainBody.Radius * 0.9999;
		Vector3d relativePositionFromTrueAnomaly = activeVessel.orbit.getRelativePositionFromTrueAnomaly(activeVessel.orbit.trueAnomaly);
		double num7 = 180.0 * Math.Atan2(relativePositionFromTrueAnomaly.x, relativePositionFromTrueAnomaly.y) / Math.PI;
		int num8 = 1;
		double num9 = activeVessel.GetOrbit().trueAnomaly * 180.0 / Math.PI;
		if (num9 > 0.0)
		{
			num9 = -360.0 + num9;
		}
		double num10 = num9 + 360.0;
		if (activeVessel.orbit.PeR <= mainBody.Radius)
		{
			double num11 = semiMajorAxis / num6 / eccentricity - semiMajorAxis * eccentricity / num6 - 1.0 / eccentricity;
			if (num11 < -1.0)
			{
				num11 = -1.0;
			}
			else if (num11 > 1.0)
			{
				num11 = 1.0;
			}
			num10 = -180.0 * Math.Acos(num11) / Math.PI;
			num10 += Math.Abs(num10 - num9) / 10.0;
		}
		double num12 = Math.Abs(num10 - num9) / 36.0;
		int num13 = 0;
		int num14 = 0;
		if (flag2)
		{
			Debug.Log((object)("Impact pre " + num8 + " " + num9 + ">" + num10 + " " + num12 + " " + num));
		}
		bool flag3 = false;
		do
		{
			flag3 = false;
			num13++;
			for (num5 = num9 + num12 * (double)num8; (num8 == 1) ? (num5 <= num10) : (num5 >= num10); num5 += num12 * (double)num8)
			{
				num14++;
				double num15 = Math.PI * num5 / 180.0;
				val = activeVessel.orbit.getRelativePositionFromTrueAnomaly(num15);
				if (((Vector3d)(ref val)).magnitude > mainBody.minOrbitalDistance)
				{
					continue;
				}
				num4 = activeVessel.orbit.GetDTforTrueAnomaly(num15, 0.0);
				val = activeVessel.orbit.getRelativePositionFromTrueAnomaly(num15);
				double num16 = 180.0 * Math.Atan2(val.x, val.y) / Math.PI - num7;
				double num17 = 360.0 * num4 / mainBody.rotationPeriod;
				double longitude = activeVessel.longitude;
				num3 = NormAngle(longitude - num16 - num17);
				num2 = 180.0 * Math.Asin(val.z / ((Vector3d)(ref val)).magnitude) / Math.PI;
				Vector3d val2 = QuaternionD.AngleAxis(num3, Vector3d.down) * QuaternionD.AngleAxis(num2, Vector3d.forward) * Vector3d.right;
				num = mainBody.pqsController.GetSurfaceHeight(val2) - mainBody.pqsController.radius;
				double num18 = ((Vector3d)(ref val)).magnitude - mainBody.Radius;
				if (num < 0.0 && mainBody.ocean)
				{
					num = 0.0;
				}
				double num19 = num18 - num;
				if (flag2)
				{
					Debug.Log((object)string.Concat("Impact iteration ", relativePositionFromTrueAnomaly, " ", val, " ", num8, " ", num9, ">", num10, " ", num5, " ", num12, " ", num18, " ", num, " ", num19));
				}
				if ((double)num8 * num19 < 0.0)
				{
					flag = true;
					num8 *= -1;
					num9 = num5;
					num10 = num5 + (double)num8 * num12;
					num12 = Math.Abs(num10 - num9) / 20.0;
					num10 += num12 * (double)num8;
					if (flag2)
					{
						Debug.Log((object)("Impact Switch! " + num9 + " > " + num10 + " " + num12));
					}
					flag3 = true;
					break;
				}
				if (num19 == 0.0)
				{
					if (flag2)
					{
						Debug.Log((object)("Impact Zero! " + num9 + " > " + num10 + " " + num12));
					}
					flag = true;
					num12 = 0.0;
					flag3 = true;
					break;
				}
			}
			if (!flag3)
			{
				if (flag2)
				{
					Debug.Log((object)"bad loop");
				}
				break;
			}
		}
		while (num12 > 1E-05 && flag && num13 < 36);
		if (flag2)
		{
			Debug.Log((object)("Impact calc! iterations " + flag.ToString() + " " + num13 + " " + num14));
		}
		if (flag)
		{
			ShowDetails = true;
			Time = num4;
			Longitude = num3;
			Latitude = num2;
			Altitude = num;
			try
			{
				Biome = ScienceUtil.GetExperimentBiome(mainBody, num2, num3);
				Biome = ScienceUtil.GetBiomedisplayName(mainBody, Biome);
			}
			catch (Exception)
			{
				Biome = "<failed>";
			}
			double num20 = mainBody.gravParameter / Math.Pow(mainBody.Radius + num, 2.0);
			double num21 = ((SimulationProcessor.LastStage.totalMass > 0.0) ? (SimulationProcessor.LastStage.thrust / SimulationProcessor.LastStage.totalMass) : 0.0);
			bool flag4 = false;
			if (num21 == 0.0 || num21 < num20)
			{
				if (flag4)
				{
					Debug.Log((object)("insuffucuent thrust " + num21));
				}
				return;
			}
			double num22 = 0.0;
			double burntime = 0.0;
			Vector3d srf = default(Vector3d);
			double shipalt = 0.0;
			num22 = getBrakingDistanceForDT(activeVessel, 0.0, mainBody, out shipalt, out burntime, out srf, debug: false);
			int num23 = 0;
			if (num22 < shipalt)
			{
				double num24 = 0.0;
				double num25 = Time;
				double num26 = 0.0;
				double num27 = double.MaxValue;
				double burntime2 = 0.0;
				Vector3d srf2 = default(Vector3d);
				double num28 = 0.0;
				while (true)
				{
					num23++;
					double num29 = num24 + (num25 - num24) / 2.0;
					if (Math.Abs(num26 - num29) < 0.05)
					{
						break;
					}
					num26 = num29;
					num28 = getBrakingDistanceForDT(activeVessel, num29, mainBody, out shipalt, out burntime2, out srf2, debug: false);
					if (num28 <= shipalt)
					{
						if (shipalt - num28 < num27)
						{
							num27 = shipalt - num28;
							num22 = num28;
							burntime = burntime2;
							srf = srf2;
						}
						num24 = num29;
						if (flag4)
						{
							Debug.Log((object)string.Concat("forward: t ", num29, " shipalt ", shipalt, " srf ", srf2, " bdist ", num28));
						}
					}
					else
					{
						num25 = num29;
						if (flag4)
						{
							Debug.Log((object)string.Concat("backward: t ", num29, " shipalt ", shipalt, " srf ", srf2, " bdist ", num28));
						}
					}
				}
				if (flag4)
				{
					getBrakingDistanceForDT(activeVessel, num26, mainBody, out shipalt, out burntime2, out srf2, debug: true);
				}
			}
			else if (flag4)
			{
				Debug.Log((object)string.Concat("Can't stop, won't stop. srf:", srf, " ", shipalt));
			}
			if (flag4)
			{
				Debug.Log((object)("breaking dist " + num22 + " " + num23));
			}
			SuicideDeltaV = ((Vector3d)(ref srf)).magnitude;
			SuicideAltitude = num + num22;
			SuicideLength = burntime;
			num6 = SuicideAltitude + mainBody.Radius;
			double num30 = semiMajorAxis / num6 / eccentricity - semiMajorAxis * eccentricity / num6 - 1.0 / eccentricity;
			if (num30 < -1.0)
			{
				num30 = -1.0;
			}
			else if (num30 > 1.0)
			{
				num30 = 1.0;
			}
			double num31 = 0.0 - Math.Acos(num30);
			if (flag4)
			{
				Debug.Log((object)string.Concat("burnpos ", num30, " ", activeVessel.orbit.pos, " ", activeVessel.orbit.getRelativePositionFromTrueAnomaly(num31)));
			}
			SuicideCountdown = activeVessel.orbit.GetDTforTrueAnomaly(num31, double.MaxValue);
			if (SuicideCountdown < 0.0 && activeVessel.orbit.trueAnomaly > 0.0)
			{
				SuicideCountdown = activeVessel.orbit.period + SuicideCountdown;
			}
			double eccentricAnomaly = activeVessel.orbit.GetEccentricAnomaly(num31);
			double num32 = activeVessel.orbit.GetEccentricAnomaly(activeVessel.orbit.trueAnomaly);
			if (num32 > 0.0)
			{
				num32 = Math.PI * -2.0 + num32;
			}
			Vector3d val3 = activeVessel.orbit.pos - activeVessel.orbit.getRelativePositionFromTrueAnomaly(num31);
			double magnitude = ((Vector3d)(ref val3)).magnitude;
			double num33 = eccentricAnomaly - num32;
			double num34 = Math.Abs(magnitude / 2.0 / Math.Sin(num33 / 2.0));
			SuicideDistance = num34 * num33;
			if (flag4)
			{
				Debug.Log((object)("dist debug eburn " + eccentricAnomaly + " eship " + num32 + " ch " + magnitude + " dt " + num33 + " rc " + num34));
			}
		}
		else
		{
			ShowDetails = false;
		}
	}

	private double getBrakingDistanceForDT(Vessel vessel, double t, CelestialBody body, out double shipalt, out double burntime, out Vector3d srf, bool debug)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		Vector3d relativePositionAtUT = vessel.GetOrbit().getRelativePositionAtUT(Planetarium.GetUniversalTime() + t);
		double magnitude = ((Vector3d)(ref relativePositionAtUT)).magnitude;
		shipalt = magnitude - body.Radius;
		double num = body.gravParameter / Math.Pow(magnitude - shipalt / 2.0, 2.0);
		srf = GetSrfAtDT(vessel, t, body);
		Vector3d normalized = ((Vector3d)(ref relativePositionAtUT)).normalized;
		Vector3d xzy = ((Vector3d)(ref normalized)).xzy;
		double num2 = 0.0 - Vector3d.Dot(srf, xzy);
		if (num2 <= 0.0)
		{
			if (debug)
			{
				Debug.Log((object)string.Concat("calc brake: going up (land) ", srf, " ", xzy, " ", relativePositionAtUT, " ", num2));
			}
			burntime = 0.0;
			return 0.0;
		}
		double num3 = Math.Sqrt(((Vector3d)(ref srf)).sqrMagnitude - num2 * num2);
		double num4 = ((num2 == 0.0) ? 0.0 : Math.Cos(Math.Atan(num3 / num2)));
		double num5 = ((SimulationProcessor.LastStage.totalMass > 0.0) ? (SimulationProcessor.LastStage.thrust / SimulationProcessor.LastStage.totalMass) : 0.0);
		double num6 = num5 * num4 - num;
		double num7 = num5 * (1.0 - num4);
		double num8 = Math.Sqrt(num6 * num6 + num7 * num7);
		burntime = ((Vector3d)(ref srf)).magnitude / num8;
		double num9 = num2 / num6;
		double result = num2 * num9 - 0.5 * num6 * num9 * num9;
		if (debug)
		{
			Debug.Log((object)("calc brake  g " + num + " a " + num5 + " loss " + num4 + " vi " + num2 + " hi " + num3 + " b " + burntime + " vb " + num9));
		}
		return result;
	}

	public static void drawImpact(Color color)
	{
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		if (ShowDetails && ShowMarker && (Object)(object)FlightGlobals.fetch != (Object)null && (Object)(object)FlightGlobals.ActiveVessel != (Object)null && (Object)(object)FlightGlobals.ActiveVessel.mainBody != (Object)null)
		{
			DebugDrawing.DrawGroundMarker(FlightGlobals.ActiveVessel.mainBody, Latitude, Longitude, color, MapView.MapIsEnabled);
		}
	}

	public static void RequestUpdate()
	{
		SimulationProcessor.RequestUpdate();
		instance.UpdateRequested = true;
	}

	public static double ACosh(double x)
	{
		return Math.Log(x + Math.Sqrt(x * x - 1.0));
	}

	private Vector3d GetSrfAtDT(Vessel vessel, double deltaTime, CelestialBody body)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		Vector3d val = vessel.GetOrbit().getOrbitalVelocityAtUT(Planetarium.GetUniversalTime() + deltaTime);
		Vector3d xzy = ((Vector3d)(ref val)).xzy;
		val = vessel.GetOrbit().getRelativePositionAtUT(Planetarium.GetUniversalTime() + deltaTime);
		Vector3d xzy2 = ((Vector3d)(ref val)).xzy;
		Vector3d val2 = Vector3d.Cross(body.angularVelocity, xzy2);
		return xzy - val2;
	}

	private double NormAngle(double ang)
	{
		if (ang > 180.0)
		{
			ang -= 360.0 * Math.Ceiling((ang - 180.0) / 360.0);
		}
		if (ang <= -180.0)
		{
			ang -= 360.0 * Math.Floor((ang + 180.0) / 360.0);
		}
		return ang;
	}

	private Vector3d RadiusDirection(double theta)
	{
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		theta = Math.PI * theta / 180.0;
		double num = Math.PI * FlightGlobals.ActiveVessel.orbit.argumentOfPeriapsis / 180.0;
		double num2 = Math.PI * FlightGlobals.ActiveVessel.orbit.inclination / 180.0;
		double num3 = Math.Cos(theta);
		double num4 = Math.Sin(theta);
		double num5 = Math.Cos(num);
		double num6 = Math.Sin(num);
		double num7 = Math.Cos(num2);
		double num8 = Math.Sin(num2);
		Vector3d result = default(Vector3d);
		result.x = num5 * num3 - num6 * num4;
		result.y = num7 * (num6 * num3 + num5 * num4);
		result.z = num8 * (num6 * num3 + num5 * num4);
		return result;
	}

	private double TimeToPeriapsis(double theta)
	{
		double eccentricity = FlightGlobals.ActiveVessel.orbit.eccentricity;
		double semiMajorAxis = FlightGlobals.ActiveVessel.orbit.semiMajorAxis;
		double peR = FlightGlobals.ActiveVessel.orbit.PeR;
		double gravParameter = FlightGlobals.ActiveVessel.mainBody.gravParameter;
		if (eccentricity == 1.0)
		{
			double num = Math.Tan(Math.PI * theta / 360.0);
			double num2 = num + num * num * num / 3.0;
			return Math.Sqrt(2.0 * peR * peR * peR / gravParameter) * num2;
		}
		if (semiMajorAxis > 0.0)
		{
			double num3 = Math.Cos(Math.PI * theta / 180.0);
			double num4 = Math.Acos((eccentricity + num3) / (1.0 + eccentricity * num3));
			double num5 = num4 - eccentricity * Math.Sin(num4);
			return Math.Sqrt(semiMajorAxis * semiMajorAxis * semiMajorAxis / gravParameter) * num5;
		}
		if (semiMajorAxis < 0.0)
		{
			double num6 = Math.Cos(Math.PI * theta / 180.0);
			double num7 = ACosh((eccentricity + num6) / (1.0 + eccentricity * num6));
			double num8 = eccentricity * Math.Sinh(num7) - num7;
			return Math.Sqrt((0.0 - semiMajorAxis) * semiMajorAxis * semiMajorAxis / gravParameter) * num8;
		}
		return 0.0;
	}
}
