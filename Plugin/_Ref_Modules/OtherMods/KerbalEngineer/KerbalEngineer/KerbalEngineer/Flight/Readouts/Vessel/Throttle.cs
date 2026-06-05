using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class Throttle : ReadoutModule
{
	public Throttle()
	{
		base.Name = "Throttle";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current requested throttle %. ";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(FlightInputHandler.state.mainThrottle.ToString("0%"), section.IsHud);
		}
	}

	public override void Reset()
	{
	}

	public override void Update()
	{
		SimulationProcessor.RequestUpdate();
	}
}
