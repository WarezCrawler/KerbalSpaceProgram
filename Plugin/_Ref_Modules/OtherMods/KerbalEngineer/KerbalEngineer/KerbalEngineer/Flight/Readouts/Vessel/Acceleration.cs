using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class Acceleration : ReadoutModule
{
	public Acceleration()
	{
		base.Name = "Acceleration";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current and maximum acceleration of the craft.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			double value = ((SimulationProcessor.LastStage.totalMass > 0.0) ? (SimulationProcessor.LastStage.actualThrust / SimulationProcessor.LastStage.totalMass) : 0.0);
			double value2 = ((SimulationProcessor.LastStage.totalMass > 0.0) ? (SimulationProcessor.LastStage.thrust / SimulationProcessor.LastStage.totalMass) : 0.0);
			DrawLine(Units.ToAcceleration(value, value2), section.IsHud);
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
