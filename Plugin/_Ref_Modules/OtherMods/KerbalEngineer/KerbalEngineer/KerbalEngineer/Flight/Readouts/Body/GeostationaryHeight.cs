using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class GeostationaryHeight : ReadoutModule
{
	public GeostationaryHeight()
	{
		base.Name = "Synchronous Alt.";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The altitude where the orbital period equals the body's rotation period.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		double rotationPeriod = FlightGlobals.currentMainBody.rotationPeriod;
		double num = Math.Pow(rotationPeriod * rotationPeriod * FlightGlobals.currentMainBody.gravParameter / 39.47841760435743, 1.0 / 3.0);
		DrawLine((num - FlightGlobals.currentMainBody.Radius).ToDistance(), section.IsHud);
	}
}
