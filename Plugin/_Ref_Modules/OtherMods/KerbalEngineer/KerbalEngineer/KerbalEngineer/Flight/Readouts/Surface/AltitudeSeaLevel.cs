using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class AltitudeSeaLevel : ReadoutModule
{
	public AltitudeSeaLevel()
	{
		base.Name = "Altitude (Sea Level)";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's altitude above sea level.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_altitude.ToDistance(), section.IsHud);
	}
}
