using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class AngleToRelativeAscendingNode : ReadoutModule
{
	public AngleToRelativeAscendingNode()
	{
		base.Name = "Angle to Rel. AN";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Angular Distance from the vessel to crossing the orbit of the target object, going north of it.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			if (RendezvousProcessor.overrideANDN || RendezvousProcessor.overrideANDNRev)
			{
				double value = RendezvousProcessor.AngleToPlane[0];
				DrawLine("(L) " + value.ToAngle(), section.IsHud);
			}
			else
			{
				DrawLine(RendezvousProcessor.AngleToAscendingNode.ToAngle(), section.IsHud);
			}
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
