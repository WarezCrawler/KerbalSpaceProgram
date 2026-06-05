using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;
using KerbalEngineer.VesselSimulator;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class Gravity : ReadoutModule
{
	public Gravity()
	{
		base.Name = "Gravity";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "The current gravity experienced by the vessel.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(Units.ToSpeed(SimManager.Gravity), section.IsHud);
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
