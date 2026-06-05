using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital;

public class OrbitalSpeed : ReadoutModule
{
	public OrbitalSpeed()
	{
		base.Name = "Orbital Speed";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Shows the vessel's orbital speed.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_obtSpeed.ToSpeed(), section.IsHud);
	}
}
