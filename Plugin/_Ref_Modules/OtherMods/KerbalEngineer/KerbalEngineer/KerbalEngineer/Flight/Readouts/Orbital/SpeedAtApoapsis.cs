using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class SpeedAtApoapsis : ReadoutModule
{
	public SpeedAtApoapsis()
	{
		base.Name = "Speed at Apoapsis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the orbital speed of the vessel when at apoapsis, the highest point of the orbit.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		Orbit ship_orbit = FlightGlobals.ship_orbit;
		string value;
		if (ship_orbit.eccentricity > 1.0)
		{
			value = "---m/s";
		}
		else
		{
			double num = ship_orbit.referenceBody.gravParameter * (2.0 / ship_orbit.ApR - 1.0 / ship_orbit.semiMajorAxis);
			value = ((!double.IsNaN(num) && !(num < 0.0)) ? Math.Sqrt(num).ToSpeed() : "---m/s");
		}
		DrawLine(value, section.IsHud);
	}
}
