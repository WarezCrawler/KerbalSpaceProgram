using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class RCSDeltaV : ReadoutModule
{
	public RCSDeltaV()
	{
		base.Name = "RCS DeltaV";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current possible DeltaV from RCS";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(SimulationProcessor.LastStage.RCSdeltaVStart.ToString("N0") + "m/s (" + TimeFormatter.ConvertToString(SimulationProcessor.LastStage.RCSBurnTime) + ")", section.IsHud);
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
