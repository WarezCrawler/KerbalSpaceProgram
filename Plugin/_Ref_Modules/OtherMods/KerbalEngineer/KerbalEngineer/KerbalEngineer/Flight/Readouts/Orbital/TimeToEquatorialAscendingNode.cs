using KerbalEngineer.Extensions;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class TimeToEquatorialAscendingNode : ReadoutModule
{
	public TimeToEquatorialAscendingNode()
	{
		base.Name = "Time to Equ. AN";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the time until the vessel corsses the Equator, going north of it.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(TimeFormatter.ConvertToString(FlightGlobals.ActiveVessel.orbit.GetTimeToAscendingNode()), section.IsHud);
	}
}
