using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class Eccentricity : ReadoutModule
{
	public Eccentricity()
	{
		base.Name = "Eccentricity";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the vessel's orbital eccentricity.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_orbit.eccentricity.ToString("F5"), section.IsHud);
	}
}
