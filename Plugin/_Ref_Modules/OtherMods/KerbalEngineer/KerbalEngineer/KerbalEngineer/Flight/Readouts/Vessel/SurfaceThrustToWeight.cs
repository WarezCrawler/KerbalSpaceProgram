using System;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class SurfaceThrustToWeight : ReadoutModule
{
	private string m_Actual = string.Empty;

	private double m_Gravity;

	private string m_Total = string.Empty;

	public SurfaceThrustToWeight()
	{
		base.Name = "Surface Thrust to Weight Ratio";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the vessel's surface thrust to weight ratio.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (!((Object)(object)FlightGlobals.currentMainBody == (Object)null) && SimulationProcessor.LastStage != null && SimulationProcessor.ShowDetails)
		{
			m_Actual = "N/A";
			m_Total = "N/A";
			if (SimulationProcessor.LastStage.totalMass > 0.0)
			{
				m_Gravity = FlightGlobals.currentMainBody.gravParameter / Math.Pow(FlightGlobals.currentMainBody.Radius, 2.0);
				m_Actual = (SimulationProcessor.LastStage.actualThrust / (SimulationProcessor.LastStage.totalMass * m_Gravity)).ToString("F2");
				m_Total = (SimulationProcessor.LastStage.thrust / (SimulationProcessor.LastStage.totalMass * m_Gravity)).ToString("F2");
			}
			DrawLine("TWR (Surface)", m_Actual + " / " + m_Total, section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
	}

	public override void Update()
	{
		SimulationProcessor.RequestUpdate();
	}
}
