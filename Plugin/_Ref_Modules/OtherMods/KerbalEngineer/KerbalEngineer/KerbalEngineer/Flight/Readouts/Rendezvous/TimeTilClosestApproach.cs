using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class TimeTilClosestApproach : ReadoutModule
{
	public TimeTilClosestApproach()
	{
		base.Name = "Time til Approach";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Time until the next closest approach to the target.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			if (double.IsNaN(RendezvousProcessor.TimeTilEncounter))
			{
				DrawLine("N/A", section.IsHud);
			}
			else
			{
				DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.TimeTilEncounter), section.IsHud);
			}
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
