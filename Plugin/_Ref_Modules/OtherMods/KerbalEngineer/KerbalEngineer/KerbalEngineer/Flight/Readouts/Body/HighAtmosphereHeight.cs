using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class HighAtmosphereHeight : ReadoutModule
{
	public HighAtmosphereHeight()
	{
		base.Name = "High Atmosphere Alt.";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The altitude where the upper atmosphere begins.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (FlightGlobals.ActiveVessel.mainBody.atmosphere)
		{
			DrawLine(FlightGlobals.ActiveVessel.mainBody.scienceValues.flyingAltitudeThreshold.ToDistance(), section.IsHud);
		}
		else
		{
			DrawLine("N/A", section.IsHud);
		}
	}
}
