using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class SemiMajorAxis : ReadoutModule
{
	public SemiMajorAxis()
	{
		base.Name = "Semi-major Axis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the distance from the centre of an orbit to the farthest edge.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToDistance(FlightGlobals.ship_orbit.semiMajorAxis, 3), section.IsHud);
	}
}
