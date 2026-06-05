using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class ThrustTorque : ReadoutModule
{
	public ThrustTorque()
	{
		base.Name = "Thrust torque";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Thrust torque due to vessel asymmetries and gimballing";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(Units.ToTorque(SimulationProcessor.LastStage.maxThrustTorque), section.IsHud);
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
