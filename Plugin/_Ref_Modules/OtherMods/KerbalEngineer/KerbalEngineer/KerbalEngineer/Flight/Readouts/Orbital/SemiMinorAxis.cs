using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class SemiMinorAxis : ReadoutModule
{
	public SemiMinorAxis()
	{
		base.Name = "Semi-minor Axis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the distance from the centre of an orbit to the nearest edge.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToDistance(FlightGlobals.ship_orbit.semiMinorAxis, 3), section.IsHud);
	}
}
