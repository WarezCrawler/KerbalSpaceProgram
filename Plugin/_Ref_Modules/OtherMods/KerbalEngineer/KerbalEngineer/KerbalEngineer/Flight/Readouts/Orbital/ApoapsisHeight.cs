using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class ApoapsisHeight : ReadoutModule
{
	public ApoapsisHeight()
	{
		base.Name = "Apoapsis Height";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the vessel's apoapsis height relative to sea level. (Apoapsis is the highest point of an orbit.)";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_orbit.ApA.ToDistance(), section.IsHud);
	}
}
