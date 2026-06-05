using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class AngleToEquatorialDescendingNode : ReadoutModule
{
	public AngleToEquatorialDescendingNode()
	{
		base.Name = "Angle to Equ. DN";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Angular Distance from the vessel to crossing the Equator of the central body, going south of it.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ActiveVessel.orbit.GetAngleToDescendingNode().ToAngle(), section.IsHud);
	}
}
