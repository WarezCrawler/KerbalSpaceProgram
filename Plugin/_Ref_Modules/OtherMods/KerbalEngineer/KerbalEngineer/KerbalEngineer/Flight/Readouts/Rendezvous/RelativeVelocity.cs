using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class RelativeVelocity : ReadoutModule
{
	public RelativeVelocity()
	{
		base.Name = "Relative Velocity";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the relative velocity between your vessel and the target object.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(RendezvousProcessor.RelativeVelocity.ToSpeed(), section.IsHud);
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
