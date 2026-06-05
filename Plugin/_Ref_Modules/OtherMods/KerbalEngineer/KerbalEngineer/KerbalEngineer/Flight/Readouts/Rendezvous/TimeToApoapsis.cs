using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class TimeToApoapsis : ReadoutModule
{
	public TimeToApoapsis()
	{
		base.Name = "Time to Apoapsis";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the time until the target reaches apoapsis, the highest point of the orbit.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.TimeToApoapsis), section.IsHud);
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
