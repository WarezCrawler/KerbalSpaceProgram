using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class TimeToApoapsis : ReadoutModule
{
	public TimeToApoapsis()
	{
		base.Name = "Time to Apoapsis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the time until the vessel reaches apoapsis, the highest point of the orbit.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(TimeFormatter.ConvertToString(FlightGlobals.ship_orbit.timeToAp), section.IsHud);
	}
}
