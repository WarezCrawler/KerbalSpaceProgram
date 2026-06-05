using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class ArgumentOfPeriapsis : ReadoutModule
{
	public ArgumentOfPeriapsis()
	{
		base.Name = "Arg. Of Periapsis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = string.Empty;
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_orbit.argumentOfPeriapsis.ToAngle(), section.IsHud);
	}
}
