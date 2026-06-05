using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class AltitudeSeaLevel : ReadoutModule
{
	public AltitudeSeaLevel()
	{
		base.Name = "Altitude (Sea Level)";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the target's altitude above sea level.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(RendezvousProcessor.AltitudeSeaLevel.ToDistance(), section.IsHud);
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
