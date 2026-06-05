using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class SpeedAtClosestApproach : ReadoutModule
{
	public SpeedAtClosestApproach()
	{
		base.Name = "Rel. Speed at Approach";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the difference in orbital speed between your vessel and the target object at the next closest approach.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			if (double.IsNaN(RendezvousProcessor.SpeedAtEncounter))
			{
				DrawLine("N/A", section.IsHud);
			}
			else
			{
				DrawLine(RendezvousProcessor.SpeedAtEncounter.ToSpeed(), section.IsHud);
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
