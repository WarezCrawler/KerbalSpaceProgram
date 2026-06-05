using System;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class SimulationProcessor : IUpdatable, IUpdateRequest
{
	private static readonly SimulationProcessor instance;

	public static SimulationProcessor Instance => instance;

	public static Stage LastStage { get; private set; }

	public static bool ShowDetails { get; private set; }

	public static Stage[] Stages { get; private set; }

	public bool UpdateRequested { get; set; }

	static SimulationProcessor()
	{
		instance = new SimulationProcessor();
		SimManager.OnReady += GetStageInfo;
	}

	private static void GetStageInfo()
	{
		Stages = SimManager.Stages;
		LastStage = SimManager.LastStage;
	}

	public static void RequestUpdate()
	{
		instance.UpdateRequested = true;
	}

	public void Update()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		SimManager.RequestSimulation();
		SimManager.TryStartSimulation();
		if (SimManager.ResultsReady())
		{
			if (Stages != null && LastStage != null)
			{
				ShowDetails = true;
			}
			if ((Object)(object)FlightGlobals.ActiveVessel != (Object)null)
			{
				SimManager.Gravity = FlightGlobals.ActiveVessel.mainBody.gravParameter / Math.Pow(FlightGlobals.ActiveVessel.mainBody.Radius + FlightGlobals.ActiveVessel.mainBody.GetAltitude(Vector3d.op_Implicit(FlightGlobals.ActiveVessel.CoM)), 2.0);
				SimManager.Mach = FlightGlobals.ActiveVessel.mach;
			}
		}
	}
}
