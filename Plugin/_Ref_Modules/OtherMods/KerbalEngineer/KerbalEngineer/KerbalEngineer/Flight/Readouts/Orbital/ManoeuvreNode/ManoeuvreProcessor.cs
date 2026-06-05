using System;
using System.Linq;
using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts.Vessel;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class ManoeuvreProcessor : IUpdatable, IUpdateRequest
{
	private static readonly ManoeuvreProcessor instance = new ManoeuvreProcessor();

	public static double AngleToPrograde { get; private set; }

	public static double AngleToRetrograde { get; private set; }

	public static double AvailableDeltaV { get; private set; }

	public static double BurnTime { get; private set; }

	public static int FinalStage { get; private set; }

	public static double HalfBurnTime { get; private set; }

	public static bool HasDeltaV { get; private set; }

	public static ManoeuvreProcessor Instance => instance;

	public static double NormalDeltaV { get; private set; }

	public static double PostBurnAp { get; private set; }

	public static double PostBurnEcc { get; private set; }

	public static double PostBurnPe { get; private set; }

	public static double PostBurnInclination { get; private set; }

	public static double PostBurnRelativeInclination { get; set; }

	public static double PostBurnPeriod { get; private set; }

	public static double ProgradeDeltaV { get; private set; }

	public static double RadialDeltaV { get; private set; }

	public static double TripDeltaV { get; private set; }

	public static bool ShowDetails { get; set; }

	public static double TotalDeltaV { get; private set; }

	public static double UniversalTime { get; private set; }

	public bool UpdateRequested { get; set; }

	public static void RequestUpdate()
	{
		instance.UpdateRequested = true;
		SimulationProcessor.RequestUpdate();
	}

	public static void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
		FlightEngineerCore.Instance.AddUpdatable(instance);
	}

	public void Update()
	{
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)FlightGlobals.ActiveVessel.patchedConicSolver == (Object)null || FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes == null || FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Count == 0 || !SimulationProcessor.ShowDetails)
		{
			ShowDetails = false;
			return;
		}
		ManeuverNode val = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0];
		Vector3d deltaV = val.DeltaV;
		ProgradeDeltaV = deltaV.z;
		NormalDeltaV = deltaV.y;
		RadialDeltaV = deltaV.x;
		Vector3d burnVector = val.GetBurnVector(FlightGlobals.ship_orbit);
		TotalDeltaV = ((Vector3d)(ref burnVector)).magnitude;
		PostBurnAp = ((val.nextPatch != null) ? val.nextPatch.ApA : 0.0);
		PostBurnEcc = ((val.nextPatch != null) ? val.nextPatch.eccentricity : 0.0);
		PostBurnPe = ((val.nextPatch != null) ? val.nextPatch.PeA : 0.0);
		PostBurnInclination = ((val.nextPatch != null) ? val.nextPatch.inclination : 0.0);
		PostBurnPeriod = ((val.nextPatch != null) ? val.nextPatch.period : 0.0);
		UniversalTime = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].UT;
		AngleToPrograde = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].patch.GetAngleToPrograde(UniversalTime);
		AngleToRetrograde = FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes[0].patch.GetAngleToRetrograde(UniversalTime);
		double burnTime = 0.0;
		double midPointTime = 0.0;
		HasDeltaV = GetBurnTime(TotalDeltaV, ref burnTime, ref midPointTime);
		AvailableDeltaV = SimulationProcessor.LastStage.totalDeltaV;
		BurnTime = burnTime;
		HalfBurnTime = midPointTime;
		double num = TotalDeltaV;
		foreach (ManeuverNode item in FlightGlobals.ActiveVessel.patchedConicSolver.maneuverNodes.Skip(1))
		{
			num += ((Vector3d)(ref item.DeltaV)).magnitude;
		}
		TripDeltaV = num;
		ShowDetails = true;
	}

	private static bool GetBurnTime(double deltaV, ref double burnTime, ref double midPointTime)
	{
		bool flag = false;
		double num = deltaV * 0.5;
		for (int num2 = SimulationProcessor.Stages.Length - 1; num2 > -1; num2--)
		{
			Stage stage = SimulationProcessor.Stages[num2];
			double num3 = stage.deltaV;
			double num4 = stage.totalMass;
			while (!(deltaV <= double.Epsilon))
			{
				if (!(num3 <= double.Epsilon))
				{
					FinalStage = num2;
					double num5;
					if (num > 0.0)
					{
						num5 = deltaV.Clamp(0.0, num3.Clamp(0.0, num));
						num -= num5;
						flag = num <= double.Epsilon;
					}
					else
					{
						num5 = deltaV.Clamp(0.0, num3);
					}
					double num6 = stage.isp * 9.80665;
					double num7 = stage.thrust / num6;
					double num8 = Math.Exp(Math.Log(num4) - num5 / num6);
					double num9 = (num4 - num8) * Math.Exp((0.0 - num5 * 0.001) / num6);
					burnTime += num9 / num7;
					deltaV -= num5;
					num3 -= num5;
					num4 -= num9;
					if (flag)
					{
						midPointTime = burnTime;
						flag = false;
						continue;
					}
				}
				goto IL_012c;
			}
			break;
			IL_012c:;
		}
		return deltaV <= double.Epsilon;
	}
}
