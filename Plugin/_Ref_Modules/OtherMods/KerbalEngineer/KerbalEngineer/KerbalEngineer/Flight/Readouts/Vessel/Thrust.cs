using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class Thrust : ReadoutModule
{
	public Thrust()
	{
		base.Name = "Thrust";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current and maximum thrust the vessel can put out.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(Units.ToForce(SimulationProcessor.LastStage.actualThrust, SimulationProcessor.LastStage.thrust), section.IsHud);
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
