using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class LowSpaceHeight : ReadoutModule
{
	public LowSpaceHeight()
	{
		base.Name = "Low Space Alt.";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The altitude where lower space begins.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (FlightGlobals.ActiveVessel.mainBody.atmosphere)
		{
			DrawLine(FlightGlobals.ActiveVessel.mainBody.atmosphereDepth.ToDistance(), section.IsHud);
		}
		else
		{
			DrawLine(0.0.ToDistance(), section.IsHud);
		}
	}
}
