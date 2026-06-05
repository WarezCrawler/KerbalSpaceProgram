using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class SemiMinorAxis : ReadoutModule
{
	public SemiMinorAxis()
	{
		base.Name = "Semi-minor Axis";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the distance from the centre of the target's orbit to the nearest edge.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(Units.ToDistance(RendezvousProcessor.SemiMajorAxis, 3), section.IsHud);
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
