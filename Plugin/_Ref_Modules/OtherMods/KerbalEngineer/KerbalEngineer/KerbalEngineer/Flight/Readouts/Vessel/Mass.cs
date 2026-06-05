using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class Mass : ReadoutModule
{
	public Mass()
	{
		base.Name = "Mass";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Displays the total Mass of the Vessel.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(Units.ToMass(SimulationProcessor.LastStage.mass, SimulationProcessor.LastStage.totalMass), section.IsHud);
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
