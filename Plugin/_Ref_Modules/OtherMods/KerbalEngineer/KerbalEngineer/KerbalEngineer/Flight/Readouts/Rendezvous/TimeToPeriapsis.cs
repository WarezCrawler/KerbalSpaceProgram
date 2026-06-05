using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class TimeToPeriapsis : ReadoutModule
{
	public TimeToPeriapsis()
	{
		base.Name = "Time to Periapsis";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the time until the target reaches periapsis, the lowest point of the orbit.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.TimeToPeriapsis), section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(RendezvousProcessor.Instance);
	}

	public override void Update()
	{
		RendezvousProcessor.RequestUpdate();
	}
}
