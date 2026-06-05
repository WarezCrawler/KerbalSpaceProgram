using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class ApoapsisHeight : ReadoutModule
{
	public ApoapsisHeight()
	{
		base.Name = "Apoapsis Height";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the targets's apoapsis height relative to sea level. (Apoapsis is the highest point of an orbit.)";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(RendezvousProcessor.ApoapsisHeight.ToDistance(), section.IsHud);
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
