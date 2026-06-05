using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class TimeToRelativeAscendingNode : ReadoutModule
{
	public TimeToRelativeAscendingNode()
	{
		base.Name = "Time to Rel. AN";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Time until the vessel crosses the target's orbit, going north.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			if (RendezvousProcessor.overrideANDN || RendezvousProcessor.overrideANDNRev)
			{
				double seconds = RendezvousProcessor.TimeToPlane[0];
				DrawLine("(L) " + TimeFormatter.ConvertToString(seconds), section.IsHud);
			}
			else
			{
				DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.TimeToAscendingNode), section.IsHud);
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
