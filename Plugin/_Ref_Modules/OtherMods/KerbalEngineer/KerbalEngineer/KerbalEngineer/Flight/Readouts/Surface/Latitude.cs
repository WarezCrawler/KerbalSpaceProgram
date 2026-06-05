using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class Latitude : ReadoutModule
{
	public Latitude()
	{
		base.Name = "Latitude";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's latitude position around the celestial body. Latitude is the angle from the equator to poles.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToAngleDMS(FlightGlobals.ship_latitude) + ((FlightGlobals.ship_latitude < 0.0) ? " S" : " N"), section.IsHud);
	}
}
