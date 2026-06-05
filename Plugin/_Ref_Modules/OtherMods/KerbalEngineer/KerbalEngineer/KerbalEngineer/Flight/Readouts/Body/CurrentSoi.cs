using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class CurrentSoi : ReadoutModule
{
	public CurrentSoi()
	{
		base.Name = "SOI Alt.";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The altitude of the SOI edge.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (!double.IsInfinity(FlightGlobals.currentMainBody.sphereOfInfluence))
		{
			DrawLine(FlightGlobals.currentMainBody.sphereOfInfluence.ToDistance(), section.IsHud);
		}
		else
		{
			DrawLine("N/A", section.IsHud);
		}
	}
}
