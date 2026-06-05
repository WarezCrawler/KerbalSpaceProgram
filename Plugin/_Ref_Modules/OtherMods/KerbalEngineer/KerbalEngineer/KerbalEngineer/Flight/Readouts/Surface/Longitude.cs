using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class Longitude : ReadoutModule
{
	public Longitude()
	{
		base.Name = "Longitude";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's longitude around a celestial body. Longitude is the angle from the bodies prime meridian.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		double num = AngleHelper.Clamp180(FlightGlobals.ship_longitude);
		DrawLine(Units.ToAngleDMS(num) + ((num < 0.0) ? " W" : " E"), section.IsHud);
	}
}
