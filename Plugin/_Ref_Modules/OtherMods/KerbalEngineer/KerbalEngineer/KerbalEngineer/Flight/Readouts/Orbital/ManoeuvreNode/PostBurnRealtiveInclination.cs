using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts.Rendezvous;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class PostBurnRealtiveInclination : ReadoutModule
{
	public PostBurnRealtiveInclination()
	{
		base.Name = "Post-burn Rel. Inclination";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "The inclination of the vessel's orbit relative to the target after the burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			if (FlightGlobals.ActiveVessel.targetObject == null || FlightGlobals.ActiveVessel.targetObject.GetOrbit() == null)
			{
				DrawLine("N/A", section.IsHud);
			}
			else
			{
				DrawLine(ManoeuvreProcessor.PostBurnRelativeInclination.ToAngle(), section.IsHud);
			}
		}
	}

	public override void Reset()
	{
		ManoeuvreProcessor.Reset();
	}

	public override void Update()
	{
		RendezvousProcessor.RequestUpdate();
	}
}
