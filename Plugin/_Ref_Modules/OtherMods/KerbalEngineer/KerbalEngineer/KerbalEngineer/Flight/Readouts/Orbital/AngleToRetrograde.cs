using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class AngleToRetrograde : ReadoutModule
{
	public AngleToRetrograde()
	{
		base.Name = "Angle to Retrograde";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Angular Distance from the vessel to crossing the Orbit of the central body on it's retrograde side.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_orbit.GetAngleToRetrograde().ToAngle(), section.IsHud);
	}
}
