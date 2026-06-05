using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class PeriapsisHeight : ReadoutModule
{
	public PeriapsisHeight()
	{
		base.Name = "Periapsis Height";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the targets's periapsis height relative to sea level. (Periapsis is the lowest point of an orbit.)";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(RendezvousProcessor.PeriapsisHeight.ToDistance(), section.IsHud);
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
