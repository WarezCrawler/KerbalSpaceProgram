using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class MachNumber : ReadoutModule
{
	public MachNumber()
	{
		base.Name = "Mach Number";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's mach number.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (FlightGlobals.ActiveVessel.atmDensity > 0.0)
		{
			DrawLine(FlightGlobals.ActiveVessel.mach.ToMach(), section.IsHud);
		}
	}
}
