using System;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class TimeToAtmosphere : ReadoutModule
{
	public TimeToAtmosphere()
	{
		base.Name = "Time to Atmosphere";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the time until the vessel enters or leaves the atmosphere.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		Orbit ship_orbit = FlightGlobals.ship_orbit;
		string value;
		if (ship_orbit.referenceBody.atmosphere && ship_orbit.PeA < ship_orbit.referenceBody.atmosphereDepth && ship_orbit.ApA > ship_orbit.referenceBody.atmosphereDepth)
		{
			double num = ship_orbit.TrueAnomalyAtRadius(ship_orbit.referenceBody.atmosphereDepth + ship_orbit.referenceBody.Radius);
			double universalTime = Planetarium.GetUniversalTime();
			double num2 = ship_orbit.GetUTforTrueAnomaly(num, ship_orbit.period * 0.5);
			if (num2 < universalTime)
			{
				num2 += ship_orbit.period;
			}
			double num3 = ship_orbit.GetUTforTrueAnomaly(0.0 - num, ship_orbit.period * 0.5);
			if (num3 < universalTime)
			{
				num3 += ship_orbit.period;
			}
			double num4 = Math.Min(num2, num3) - universalTime;
			value = ((!double.IsNaN(num4)) ? TimeFormatter.ConvertToString(num4) : "---s");
		}
		else
		{
			value = "---s";
		}
		DrawLine(value, section.IsHud);
	}
}
