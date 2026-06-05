using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class PartCount : ReadoutModule
{
	public PartCount()
	{
		base.Name = "Part Count";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the total number of Parts the current and next stage.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(Units.ConcatF(SimulationProcessor.LastStage.partCount, SimulationProcessor.LastStage.totalPartCount, 0), section.IsHud);
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
