using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class TimeToPeriapsis : ReadoutModule
{
	public TimeToPeriapsis()
	{
		base.Name = "Time to Periapsis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the time until the vessel reaches periapsis, the lowest point of the orbit.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(TimeFormatter.ConvertToString(FlightGlobals.ship_orbit.timeToPe), section.IsHud);
	}
}
