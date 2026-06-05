using System.Linq;
using KSP.UI.Screens;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class DeltaVStaged : ReadoutModule
{
	public DeltaVStaged()
	{
		base.Name = "DeltaV Staged";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the vessel's delta velocity for each stage.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (!SimulationProcessor.ShowDetails || (Object)(object)StageManager.Instance == (Object)null)
		{
			return;
		}
		foreach (Stage item in SimulationProcessor.Stages.Where((Stage stage) => stage.deltaV > 0.0 || stage.number == StageManager.CurrentStage))
		{
			DrawLine("DeltaV (S" + item.number + ")", item.deltaV.ToString("N0") + "m/s (" + TimeFormatter.ConvertToString(item.time) + ")", section.IsHud);
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
