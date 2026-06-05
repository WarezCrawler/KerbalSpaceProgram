using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class VerticalSpeed : ReadoutModule
{
	public VerticalSpeed()
	{
		base.Name = "Vertical Speed";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's vertical speed up and down.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ship_verticalSpeed.ToSpeed(), section.IsHud);
	}
}
