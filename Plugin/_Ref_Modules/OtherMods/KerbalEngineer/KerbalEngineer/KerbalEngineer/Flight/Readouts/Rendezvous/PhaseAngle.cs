using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class PhaseAngle : ReadoutModule
{
	public PhaseAngle()
	{
		base.Name = "Phase Angle";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Angular distance of the vessel relative to the target object.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(RendezvousProcessor.PhaseAngle.ToAngle(), section.IsHud);
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
