using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class HorizontalSpeed : ReadoutModule
{
	public HorizontalSpeed()
	{
		base.Name = "Horizontal Speed";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's horizontal speed across a celestial body's surface.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ActiveVessel.horizontalSrfSpeed.ToSpeed(), section.IsHud);
	}
}
