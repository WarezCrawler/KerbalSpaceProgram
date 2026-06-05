using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class DeltaVTotal : ReadoutModule
{
	public DeltaVTotal()
	{
		base.Name = "DeltaV Total";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the vessel's total delta velocity.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(SimulationProcessor.LastStage.totalDeltaV.ToString("N0") + "m/s (" + TimeFormatter.ConvertToString(SimulationProcessor.LastStage.totalTime) + ")", section.IsHud);
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
