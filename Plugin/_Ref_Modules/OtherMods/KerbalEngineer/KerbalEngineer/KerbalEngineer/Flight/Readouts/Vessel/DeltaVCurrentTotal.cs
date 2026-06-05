using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class DeltaVCurrentTotal : ReadoutModule
{
	public DeltaVCurrentTotal()
	{
		base.Name = "DeltaV (Current/Total)";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the vessel's current stage delta velocity and total.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(SimulationProcessor.LastStage.deltaV.ToString("N0") + "m/s / " + SimulationProcessor.LastStage.totalDeltaV.ToString("N0") + "m/s", section.IsHud);
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
