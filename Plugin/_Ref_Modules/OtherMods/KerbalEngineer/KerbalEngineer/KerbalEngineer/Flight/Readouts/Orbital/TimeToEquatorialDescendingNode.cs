using KerbalEngineer.Extensions;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class TimeToEquatorialDescendingNode : ReadoutModule
{
	public TimeToEquatorialDescendingNode()
	{
		base.Name = "Time to Equ. DN";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the time until the vessel corsses the Equator, going south of it.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(TimeFormatter.ConvertToString(FlightGlobals.ActiveVessel.orbit.GetTimeToDescendingNode()), section.IsHud);
	}
}
