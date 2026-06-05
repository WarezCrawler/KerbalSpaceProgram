using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class AngleToEquatorialAscendingNode : ReadoutModule
{
	public AngleToEquatorialAscendingNode()
	{
		base.Name = "Angle to Equ. AN";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Angular Distance from the vessel to crossing the Equator of the central body, going north of it.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ActiveVessel.orbit.GetAngleToAscendingNode().ToAngle(), section.IsHud);
	}
}
