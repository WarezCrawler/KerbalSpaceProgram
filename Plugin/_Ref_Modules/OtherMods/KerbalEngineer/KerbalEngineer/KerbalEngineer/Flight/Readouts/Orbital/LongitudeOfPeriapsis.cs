using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class LongitudeOfPeriapsis : ReadoutModule
{
	public LongitudeOfPeriapsis()
	{
		base.Name = "Longitude of Pe";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the vessel's longitude of periapsis.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine((FlightGlobals.ship_orbit.LAN + FlightGlobals.ship_orbit.argumentOfPeriapsis).ToAngle(), section.IsHud);
	}
}
