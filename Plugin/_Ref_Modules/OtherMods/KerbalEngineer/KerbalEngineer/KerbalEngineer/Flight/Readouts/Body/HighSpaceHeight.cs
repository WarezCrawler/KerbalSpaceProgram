using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class HighSpaceHeight : ReadoutModule
{
	public HighSpaceHeight()
	{
		base.Name = "High Space Alt.";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The altitude where upper space begins.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ActiveVessel.mainBody.scienceValues.spaceAltitudeThreshold.ToDistance(), section.IsHud);
	}
}
