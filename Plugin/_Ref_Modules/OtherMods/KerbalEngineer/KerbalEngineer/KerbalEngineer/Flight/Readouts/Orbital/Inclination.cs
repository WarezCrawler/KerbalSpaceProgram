using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class Inclination : ReadoutModule
{
	public Inclination()
	{
		base.Name = "Inclination";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the vessel's orbital inclination relative to the Equator.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_orbit.inclination.ToAngle(), section.IsHud);
	}
}
