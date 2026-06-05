using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class RCSThrust : ReadoutModule
{
	public RCSThrust()
	{
		base.Name = "RCS Thrust";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the maximum thrust from RCS thrusters";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(Units.ToForce(SimulationProcessor.LastStage.RCSThrust), section.IsHud);
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
