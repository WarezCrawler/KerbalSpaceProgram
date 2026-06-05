using System;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class BodyGravity : ReadoutModule
{
	public BodyGravity()
	{
		base.Name = "Surface Gravity";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The surface gravity of the body.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToSpeed(FlightGlobals.ActiveVessel.mainBody.gravParameter / Math.Pow(FlightGlobals.currentMainBody.Radius, 2.0)), section.IsHud);
	}
}
