using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class SpeedAtPeriapsis : ReadoutModule
{
	public SpeedAtPeriapsis()
	{
		base.Name = "Speed at Periapsis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the orbital speed of the vessel when at periapsis, the lowest point of the orbit.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		Orbit ship_orbit = FlightGlobals.ship_orbit;
		double num = ((ship_orbit.eccentricity == 1.0) ? 0.0 : (1.0 / ship_orbit.semiMajorAxis));
		double num2 = ship_orbit.referenceBody.gravParameter * (2.0 / ship_orbit.PeR - num);
		string value = ((!double.IsNaN(num2) && !(num2 < 0.0)) ? Math.Sqrt(num2).ToSpeed() : "---m/s");
		DrawLine(value, section.IsHud);
	}
}
