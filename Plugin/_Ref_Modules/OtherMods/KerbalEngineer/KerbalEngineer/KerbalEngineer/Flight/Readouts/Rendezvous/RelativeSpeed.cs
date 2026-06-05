using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class RelativeSpeed : ReadoutModule
{
	public RelativeSpeed()
	{
		base.Name = "Relative Orbital Speed";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the difference in orbital speed between your vessel and the target object.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(RendezvousProcessor.RelativeSpeed.ToSpeed(), section.IsHud);
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
