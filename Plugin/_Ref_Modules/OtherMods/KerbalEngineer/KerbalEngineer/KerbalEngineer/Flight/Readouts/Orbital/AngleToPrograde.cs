using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class AngleToPrograde : ReadoutModule
{
	public AngleToPrograde()
	{
		base.Name = "Angle to Prograde";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Angular Distance from the vessel to crossing the Orbit of the central body on it's retrograde side.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_orbit.GetAngleToPrograde().ToAngle(), section.IsHud);
	}
}
