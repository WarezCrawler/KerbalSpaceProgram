using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class DeltaVCurrent : ReadoutModule
{
	public DeltaVCurrent()
	{
		base.Name = "DeltaV Current";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the vessel's current stage delta velocity.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(SimulationProcessor.LastStage.deltaV.ToString("N0") + "m/s (" + TimeFormatter.ConvertToString(SimulationProcessor.LastStage.time) + ")", section.IsHud);
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
