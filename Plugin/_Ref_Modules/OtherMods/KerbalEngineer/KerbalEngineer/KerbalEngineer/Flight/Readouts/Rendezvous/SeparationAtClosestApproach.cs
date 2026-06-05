using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class SeparationAtClosestApproach : ReadoutModule
{
	public SeparationAtClosestApproach()
	{
		base.Name = "Separation at Approach";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Distance to the target at closest approach.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			if (double.IsNaN(RendezvousProcessor.SeparationAtEncounter))
			{
				DrawLine("N/A", section.IsHud);
			}
			else
			{
				DrawLine(Units.ToDistance(RendezvousProcessor.SeparationAtEncounter), section.IsHud);
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
