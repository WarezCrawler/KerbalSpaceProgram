using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class SemiMajorAxis : ReadoutModule
{
	public SemiMajorAxis()
	{
		base.Name = "Semi-major Axis";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the distance from the centre of the target's orbit to the farthest edge.";
		base.IsDefault = true;
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
