using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class PeriapsisHeight : ReadoutModule
{
	public PeriapsisHeight()
	{
		base.Name = "Periapsis Height";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the vessel's periapsis height relative to sea level. (Periapsis is the lowest point of an orbit.)";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_orbit.PeA.ToDistance(), section.IsHud);
	}
}
