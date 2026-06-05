using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class BodyOrbitalPeriod : ReadoutModule
{
	public BodyOrbitalPeriod()
	{
		base.Name = "Body Orbital Period";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The time to complete one orbit about the body's parent.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (FlightGlobals.ActiveVessel.mainBody.orbit == null)
		{
			DrawLine("N/A", section.IsHud);
		}
		else
		{
			DrawLine(Units.ToTime(FlightGlobals.ActiveVessel.mainBody.orbit.period), section.IsHud);
		}
	}
}
