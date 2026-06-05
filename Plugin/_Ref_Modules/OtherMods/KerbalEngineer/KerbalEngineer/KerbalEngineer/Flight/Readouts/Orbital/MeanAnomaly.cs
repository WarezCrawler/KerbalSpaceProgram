using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class MeanAnomaly : ReadoutModule
{
	public MeanAnomaly()
	{
		base.Name = "Mean Anomaly";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = string.Empty;
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine((FlightGlobals.ship_orbit.meanAnomaly * (180.0 / Math.PI)).ToAngle(), section.IsHud);
	}
}
