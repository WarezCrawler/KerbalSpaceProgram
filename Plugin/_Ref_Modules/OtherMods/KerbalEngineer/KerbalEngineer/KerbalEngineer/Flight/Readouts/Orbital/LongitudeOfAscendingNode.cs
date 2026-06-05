using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class LongitudeOfAscendingNode : ReadoutModule
{
	public LongitudeOfAscendingNode()
	{
		base.Name = "Longitude of AN";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the vessel's longitude of the ascending node.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_orbit.LAN.ToAngle(), section.IsHud);
	}
}
