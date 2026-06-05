using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class OrbitalPeriod : ReadoutModule
{
	public OrbitalPeriod()
	{
		base.Name = "Orbital Period";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the amount of time it will take the target object to complete a full orbit.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.OrbitalPeriod, "F3"), section.IsHud);
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
