using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class TimeToRelativeDescendingNode : ReadoutModule
{
	public TimeToRelativeDescendingNode()
	{
		base.Name = "Time to Rel. DN";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Time until the vessel crosses the target's orbit, going south.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			if (RendezvousProcessor.overrideANDN || RendezvousProcessor.overrideANDNRev)
			{
				double seconds = RendezvousProcessor.TimeToPlane[1];
				DrawLine("(L) " + TimeFormatter.ConvertToString(seconds), section.IsHud);
			}
			else
			{
				DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.TimeToDescendingNode), section.IsHud);
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
