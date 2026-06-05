using System;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class EscapeVelocity : ReadoutModule
{
	public EscapeVelocity()
	{
		base.Name = "Surface Escape Velocity";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The velocity needed to escape the SOI, starting from sea level.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		double value = Math.Sqrt(2.0 * FlightGlobals.currentMainBody.gravParameter / FlightGlobals.currentMainBody.Radius);
		DrawLine(Units.ToSpeed(value), section.IsHud);
	}
}
