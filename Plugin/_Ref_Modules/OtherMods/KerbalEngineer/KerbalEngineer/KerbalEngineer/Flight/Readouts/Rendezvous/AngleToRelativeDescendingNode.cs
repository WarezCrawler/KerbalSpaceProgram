using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class AngleToRelativeDescendingNode : ReadoutModule
{
	public AngleToRelativeDescendingNode()
	{
		base.Name = "Angle to Rel. DN";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Angular Distance from the vessel to crossing the orbit of the target object, going south of it.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			if (RendezvousProcessor.overrideANDN || RendezvousProcessor.overrideANDNRev)
			{
				double value = RendezvousProcessor.AngleToPlane[1];
				DrawLine("(L) " + value.ToAngle(), section.IsHud);
			}
			else
			{
				DrawLine(RendezvousProcessor.AngleToDescendingNode.ToAngle(), section.IsHud);
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
