using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class RelativeInclination : ReadoutModule
{
	public RelativeInclination()
	{
		base.Name = "Relative Inclination";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the relative inclination between your vessel and the target object.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(RendezvousProcessor.RelativeInclination.ToAngle(), section.IsHud);
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
