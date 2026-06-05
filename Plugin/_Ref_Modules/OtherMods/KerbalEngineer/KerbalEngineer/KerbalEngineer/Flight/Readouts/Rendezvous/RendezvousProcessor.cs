using System;
using KSP.Localization;
using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class RendezvousProcessor : IUpdatable, IUpdateRequest
{
	private static readonly RendezvousProcessor instance = new RendezvousProcessor();

	public static bool overrideANDN = false;

	public static bool overrideANDNRev = false;

	public static Vessel targetVessel = null;

	public static Vessel activeVessel = null;

	public static ITargetable TrackingStationSource = null;

	public static ITargetable activeTarget = null;

	public static double AltitudeSeaLevel { get; private set; }

	public static double AngleToAscendingNode { get; private set; }

	public static double AngleToDescendingNode { get; private set; }

	public static double ApoapsisHeight { get; private set; }

	public static double Distance { get; private set; }

	public static RendezvousProcessor Instance => instance;

	public static double InterceptAngle { get; private set; }

	public static double OrbitalPeriod { get; private set; }

	public static double PeriapsisHeight { get; private set; }

	public static double PhaseAngle { get; private set; }

	public static double TransferAngle { get; private set; }

	public static double TimeToTransferAngle { get; private set; }

	public static double RelativeInclination { get; private set; }

	public static double RelativeSpeed { get; private set; }

	public static double RelativeVelocity { get; private set; }

	public static double SemiMajorAxis { get; private set; }

	public static double SemiMinorAxis { get; private set; }

	public static bool ShowDetails { get; private set; }

	public static double TimeToApoapsis { get; private set; }

	public static double TimeToAscendingNode { get; private set; }

	public static double TimeToDescendingNode { get; private set; }

	public static double TimeToPeriapsis { get; private set; }

	public static double[] TimeToPlane { get; private set; } = new double[2];


	public static double[] AngleToPlane { get; private set; } = new double[2];


	public static bool TimeToPlaneisAsc { get; private set; }

	public static bool isLanded { get; private set; }

	public static bool landedSamePlanet { get; private set; }

	public static double bodyRotationPeriod { get; private set; }

	public static string targetDisplay { get; private set; }

	public static string sourceDisplay { get; private set; }

	public static double TimeTilEncounter { get; private set; }

	public static double SeparationAtEncounter { get; private set; }

	public static double SpeedAtEncounter { get; private set; }

	public bool UpdateRequested { get; set; }

	public static void RequestUpdate()
	{
		instance.UpdateRequested = true;
	}

	public void Update()
	{
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a5: Invalid comparison between Unknown and I4
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Invalid comparison between Unknown and I4
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Invalid comparison between Unknown and I4
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Expected O, but got Unknown
		//IL_019b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a28: Unknown result type (might be due to invalid IL or missing references)
		//IL_0a2e: Invalid comparison between Unknown and I4
		//IL_0481: Unknown result type (might be due to invalid IL or missing references)
		//IL_0487: Unknown result type (might be due to invalid IL or missing references)
		//IL_06bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_06c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_06cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_06e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_06ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0834: Unknown result type (might be due to invalid IL or missing references)
		//IL_084a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0860: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0878: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_08bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_09bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_09c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_09ce: Unknown result type (might be due to invalid IL or missing references)
		ITargetable val = null;
		Vessel val2 = null;
		landedSamePlanet = false;
		isLanded = false;
		overrideANDN = false;
		overrideANDNRev = false;
		if (HighLogic.LoadedSceneIsFlight)
		{
			if ((Object)(object)FlightGlobals.fetch == (Object)null || FlightGlobals.fetch.VesselTarget == null || (Object)(object)FlightGlobals.ActiveVessel == (Object)null || FlightGlobals.ActiveVessel.targetObject == null || FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null || FlightGlobals.ship_orbit == null || (Object)(object)FlightGlobals.ship_orbit.referenceBody == (Object)null)
			{
				ShowDetails = false;
				return;
			}
			val = FlightGlobals.ActiveVessel.targetObject;
			val2 = FlightGlobals.ActiveVessel;
			TrackingStationSource = null;
		}
		else if ((int)HighLogic.LoadedScene == 8)
		{
			if ((int)PlanetariumCamera.fetch.target.type == 2)
			{
				val = (ITargetable)(object)PlanetariumCamera.fetch.target.celestialBody;
			}
			else if ((int)PlanetariumCamera.fetch.target.type == 3)
			{
				val = (ITargetable)(object)PlanetariumCamera.fetch.target.vessel;
			}
			if (TrackingStationSource != null)
			{
				val2 = TrackingStationSource.GetVessel();
			}
			else
			{
				TrackingStationSource = val;
			}
		}
		activeTarget = val;
		if (val == null)
		{
			ShowDetails = false;
			return;
		}
		ShowDetails = true;
		Orbit val3 = (((Object)(object)val2 != (Object)null) ? val2.orbit : TrackingStationSource.GetOrbit());
		Orbit orbit = val.GetOrbit();
		if (val is Vessel)
		{
			targetVessel = (Vessel)val;
		}
		else
		{
			targetVessel = null;
		}
		activeVessel = val2;
		if (val3 == null)
		{
			TrackingStationSource = null;
			ShowDetails = false;
			return;
		}
		if (orbit == null)
		{
			ShowDetails = false;
			return;
		}
		isLanded = (Object)(object)val2 != (Object)null && val2.LandedOrSplashed;
		bool flag = val is Vessel && ((Vessel)val).LandedOrSplashed;
		landedSamePlanet = isLanded && flag && (Object)(object)val3.referenceBody == (Object)(object)orbit.referenceBody;
		Orbit source = (isLanded ? val3.referenceBody.orbit : val3);
		Orbit target = (flag ? orbit.referenceBody.orbit : orbit);
		if ((!isLanded && !flag) || (Object)(object)val3.referenceBody != (Object)(object)orbit.referenceBody)
		{
			findConcentricParents(ref source, ref target);
		}
		if (source == target && !landedSamePlanet)
		{
			target = orbit;
			source = null;
		}
		AltitudeSeaLevel = target.altitude;
		ApoapsisHeight = target.ApA;
		PeriapsisHeight = target.PeA;
		TimeToApoapsis = target.timeToAp;
		TimeToPeriapsis = target.timeToPe;
		SemiMajorAxis = target.semiMajorAxis;
		SemiMinorAxis = target.semiMinorAxis;
		OrbitalPeriod = target.period;
		RelativeInclination = target.inclination;
		TimeToAscendingNode = 0.0;
		TimeToDescendingNode = 0.0;
		AngleToAscendingNode = 0.0;
		AngleToDescendingNode = 0.0;
		RelativeVelocity = 0.0;
		RelativeSpeed = 0.0;
		PhaseAngle = 0.0;
		InterceptAngle = 0.0;
		TimeToTransferAngle = 0.0;
		AngleToPlane[0] = 0.0;
		TimeToPlane[0] = 0.0;
		AngleToPlane[1] = 0.0;
		TimeToPlane[1] = 0.0;
		Distance = 0.0;
		TimeTilEncounter = double.NaN;
		SeparationAtEncounter = double.NaN;
		SpeedAtEncounter = double.NaN;
		ManoeuvreProcessor.PostBurnRelativeInclination = 0.0;
		if (source != null)
		{
			overrideANDN = isLanded && (Object)(object)val3.referenceBody == (Object)(object)target.referenceBody;
			overrideANDNRev = flag && !isLanded && (Object)(object)orbit.referenceBody == (Object)(object)val3.referenceBody && (Object)(object)val.GetVessel() != (Object)null;
			bodyRotationPeriod = val3.referenceBody.rotationPeriod;
			if (landedSamePlanet)
			{
				AltitudeSeaLevel = val.GetVessel().altitude;
				ApoapsisHeight = 0.0;
				PeriapsisHeight = 0.0;
				TimeToApoapsis = 0.0;
				TimeToPeriapsis = 0.0;
				SemiMajorAxis = 0.0;
				SemiMinorAxis = 0.0;
				Distance = Vector3d.Distance(val.GetVessel().GetWorldPos3D(), val2.GetWorldPos3D());
				OrbitalPeriod = bodyRotationPeriod;
			}
			else if (overrideANDN)
			{
				if ((Object)(object)val2 != (Object)null)
				{
					AngleToPlane = CalcAngleToPlane(val2.GetOrbit().referenceBody, val2.latitude, val2.longitude, target);
					TimeToPlane[0] = AngleToPlane[0] / 360.0 * val2.GetOrbit().referenceBody.rotationPeriod;
					TimeToPlane[1] = AngleToPlane[1] / 360.0 * val2.GetOrbit().referenceBody.rotationPeriod;
				}
				RelativeInclination = target.inclination;
				PhaseAngle = val3.GetPhaseAngle(orbit);
			}
			else if (overrideANDNRev)
			{
				Vessel vessel = val.GetVessel();
				if ((Object)(object)val2 != (Object)null)
				{
					AngleToPlane = CalcAngleToPlane(val2.GetOrbit().referenceBody, vessel.latitude, vessel.longitude, source);
					TimeToPlane[0] = AngleToPlane[0] / 360.0 * val2.GetOrbit().referenceBody.rotationPeriod;
					TimeToPlane[1] = AngleToPlane[1] / 360.0 * val2.GetOrbit().referenceBody.rotationPeriod;
				}
				RelativeInclination = source.inclination;
				Distance = Vector3d.Distance(val.GetVessel().GetWorldPos3D(), val2.GetWorldPos3D());
				AltitudeSeaLevel = vessel.altitude;
				PhaseAngle = val3.GetPhaseAngle(orbit);
			}
			else
			{
				RelativeInclination = source.GetRelativeInclination(target);
				if (!((Object)(object)FlightGlobals.ActiveVessel == (Object)null) && !((Object)(object)FlightGlobals.ActiveVessel.patchedConicSolver == (Object)null) && FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes != null && FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count != 0)
				{
					ManeuverNode val4 = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0];
					if (val4 != null && val4.nextPatch != null)
					{
						ManoeuvreProcessor.PostBurnRelativeInclination = val4.nextPatch.GetRelativeInclination(FlightGlobals.ActiveVessel.targetObject.GetOrbit());
					}
				}
				Vector3d val5 = source.GetRelativeVel();
				double magnitude = ((Vector3d)(ref val5)).magnitude;
				Vector3 obtVelocity = val.GetObtVelocity();
				RelativeSpeed = magnitude - (double)((Vector3)(ref obtVelocity)).magnitude;
				val5 = source.GetRelativeVel() - val.GetObtVelocity();
				RelativeVelocity = ((Vector3d)(ref val5)).magnitude;
				PhaseAngle = source.GetPhaseAngle(target);
				InterceptAngle = CalcInterceptAngle(target, source);
				double num = 360.0 / target.period;
				double num2 = 360.0 / source.period;
				if (PhaseAngle < 0.0)
				{
					double num3 = InterceptAngle - PhaseAngle;
					if (num3 < 0.0)
					{
						num3 += 360.0;
					}
					if (num3 > 340.0)
					{
						num3 -= 360.0;
					}
					TimeToTransferAngle = ((num2 == num) ? 0.0 : (num3 / (num - num2)));
				}
				else
				{
					double num4 = PhaseAngle - InterceptAngle;
					if (num4 < 0.0)
					{
						num4 += 360.0;
					}
					if (num4 > 340.0)
					{
						num4 -= 360.0;
					}
					TimeToTransferAngle = ((num2 == num) ? 0.0 : (num4 / (num2 - num)));
				}
				TimeToAscendingNode = source.GetTimeToVector(GetAscendingNode(target, source));
				TimeToDescendingNode = source.GetTimeToVector(GetDescendingNode(target, source));
				AngleToAscendingNode = source.GetAngleToVector(GetAscendingNode(target, source));
				AngleToDescendingNode = source.GetAngleToVector(GetDescendingNode(target, source));
				Distance = Vector3d.Distance(target.pos, source.pos);
				double num5 = 0.0;
				if (val is CelestialBody)
				{
					Vector3d relativePositionAtUT = source.getRelativePositionAtUT(source.closestTgtApprUT);
					Vector3d relativePositionAtUT2 = target.getRelativePositionAtUT(source.closestTgtApprUT);
					val5 = relativePositionAtUT - relativePositionAtUT2;
					_ = ((Vector3d)(ref val5)).magnitude;
					num5 = source.closestTgtApprUT;
				}
				else
				{
					double num6 = 0.0;
					double num7 = 0.0;
					double num8 = 0.0;
					double num9 = 0.0;
					double num10 = 0.0;
					double num11 = 0.0;
					int num12 = 0;
					Orbit.FindClosestPoints.Invoke(source, target, ref num6, ref num7, ref num8, ref num9, ref num10, ref num11, 0.0001, 20, ref num12);
					num5 = source.StartUT + source.GetDTforTrueAnomaly(num8, 0.0);
					double num13 = source.StartUT + source.GetDTforTrueAnomaly(num10, 0.0);
					if (num5 > num13)
					{
						UtilMath.SwapValues(ref num5, ref num13);
						UtilMath.SwapValues(ref num8, ref num10);
						UtilMath.SwapValues(ref num9, ref num11);
					}
				}
				if (num5 > source.StartUT)
				{
					TimeTilEncounter = num5 - source.StartUT;
					val5 = source.getPositionAtUT(num5) - target.getPositionAtUT(num5);
					SeparationAtEncounter = ((Vector3d)(ref val5)).magnitude;
					SpeedAtEncounter = Math.Abs(source.getOrbitalSpeedAt(num5) - target.getOrbitalSpeedAt(num5));
				}
			}
		}
		if (orbit != target)
		{
			targetDisplay = findNameForOrbit(target, val);
		}
		else
		{
			targetDisplay = null;
		}
		if (source == null)
		{
			sourceDisplay = "N/A";
		}
		else if (val3 != source || (int)HighLogic.LoadedScene == 8)
		{
			Orbit orbit2 = source;
			ITargetable start;
			if (!((Object)(object)val2 != (Object)null))
			{
				start = TrackingStationSource;
			}
			else
			{
				ITargetable val6 = (ITargetable)(object)val2;
				start = val6;
			}
			sourceDisplay = findNameForOrbit(orbit2, start);
		}
		else
		{
			sourceDisplay = null;
		}
	}

	public string findNameForOrbit(Orbit orbit, ITargetable start)
	{
		if (start.GetOrbit() == orbit || start.GetOrbit() == null)
		{
			return nameForTargetable(start);
		}
		return findNameForOrbit(orbit, (ITargetable)(object)start.GetOrbit().referenceBody);
	}

	public void findConcentricParents(ref Orbit source, ref Orbit target)
	{
		Orbit val = target;
		while (!((Object)(object)source.referenceBody == (Object)(object)target.referenceBody))
		{
			if (target.referenceBody.orbit != null)
			{
				target = target.referenceBody.orbit;
				continue;
			}
			if (source.referenceBody.orbit == null)
			{
				break;
			}
			source = source.referenceBody.orbit;
			target = val;
		}
	}

	public static string nameForTargetable(ITargetable tgt)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		MapObject val = null;
		if (tgt is CelestialBody)
		{
			val = ((CelestialBody)tgt).MapObject;
		}
		else if (tgt is Vessel)
		{
			val = ((Vessel)tgt).mapObject;
		}
		if ((Object)(object)val == (Object)null || val.Discoverable == null)
		{
			return Localizer.Format("<<1>>", new string[1] { tgt.GetDisplayName() });
		}
		return Localizer.Format("<<1>>", new string[1] { val.GetDisplayName() });
	}

	private double CalcInterceptAngle(Orbit targetOrbit, Orbit originOrbit)
	{
		double num = (originOrbit.semiMinorAxis + originOrbit.semiMajorAxis) * 0.5;
		double num2 = (targetOrbit.semiMinorAxis + targetOrbit.semiMajorAxis) * 0.5;
		double num3 = 180.0 * (1.0 - Math.Pow((num + num2) / (2.0 * num2), 1.5));
		if (!(RelativeInclination < 90.0))
		{
			return 360.0 - (180.0 - num3);
		}
		return num3;
	}

	private Vector3d GetAscendingNode(Orbit targetOrbit, Orbit originOrbit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector3d.Cross(targetOrbit.GetOrbitNormal(), originOrbit.GetOrbitNormal());
	}

	private Vector3d GetDescendingNode(Orbit targetOrbit, Orbit originOrbit)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return Vector3d.Cross(originOrbit.GetOrbitNormal(), targetOrbit.GetOrbitNormal());
	}

	private double[] CalcAngleToPlane(CelestialBody launchBody, double launchLatitude, double launchLongitude, Orbit target)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0108: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0134: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0144: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		double[] array = new double[2];
		double num = Math.Abs(Vector3d.Angle(SwappedOrbitNormal(target), launchBody.angularVelocity));
		Vector3d val = Vector3d.Exclude(launchBody.angularVelocity, SwappedOrbitNormal(target));
		Vector3d val2 = ((Vector3d)(ref val)).normalized;
		val2 *= launchBody.Radius * Math.Sin(Math.PI / 180.0 * launchLatitude) / Math.Tan(Math.PI / 180.0 * num);
		val = Vector3d.Cross(SwappedOrbitNormal(target), launchBody.angularVelocity);
		Vector3d normalized = ((Vector3d)(ref val)).normalized;
		double num2 = Math.Pow(launchBody.Radius * Math.Cos(Math.PI / 180.0 * launchLatitude), 2.0) - ((Vector3d)(ref val2)).sqrMagnitude;
		if (num2 < 0.0)
		{
			num2 = 0.0;
		}
		normalized *= Math.Sqrt(num2);
		Vector3d val3 = val2 + normalized;
		Vector3d val4 = val2 - normalized;
		Vector3d surfaceNVector = launchBody.GetSurfaceNVector(0.0, launchLongitude);
		double num3 = Math.Abs(Vector3d.Angle(surfaceNVector, val3));
		if (Vector3d.Dot(Vector3d.Cross(surfaceNVector, val3), launchBody.angularVelocity) < 0.0)
		{
			num3 = 360.0 - num3;
		}
		double num4 = Math.Abs(Vector3d.Angle(surfaceNVector, val4));
		if (Vector3d.Dot(Vector3d.Cross(surfaceNVector, val4), launchBody.angularVelocity) < 0.0)
		{
			num4 = 360.0 - num4;
		}
		array[0] = Math.Min(num3, num4);
		array[1] = Math.Max(num3, num4) - 360.0;
		return array;
	}

	public static Vector3d SwappedOrbitNormal(Orbit o)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		Vector3d val = SwapYZ(o.GetOrbitNormal());
		return -((Vector3d)(ref val)).normalized;
	}

	public static Vector3d SwapYZ(Vector3d v)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		return ((Vector3d)(ref v)).xzy;
	}
}
