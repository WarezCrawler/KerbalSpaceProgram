using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class OrbitalPeriod : ReadoutModule
{
	public OrbitalPeriod()
	{
		base.Name = "Orbital Period";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the amount of time it will take to complete a full orbit.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(TimeFormatter.ConvertToString(FlightGlobals.ship_orbit.period, "F3"), section.IsHud);
	}
}
