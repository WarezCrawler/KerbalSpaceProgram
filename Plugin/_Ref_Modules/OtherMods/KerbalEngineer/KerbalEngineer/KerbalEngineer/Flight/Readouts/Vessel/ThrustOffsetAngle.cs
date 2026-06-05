using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class ThrustOffsetAngle : ReadoutModule
{
	public ThrustOffsetAngle()
	{
		base.Name = "Thrust offset angle";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Thrust angle offset due to vessel asymmetries and gimballing";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(Units.ToAngle(SimulationProcessor.LastStage.thrustOffsetAngle, 1), section.IsHud);
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
