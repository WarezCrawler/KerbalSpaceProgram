using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class MinOrbitHeight : ReadoutModule
{
	public MinOrbitHeight()
	{
		base.Name = "Min. Safe Alt.";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The minimum safe altitude for orbiting.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		double value = FlightGlobals.ActiveVessel.mainBody.minOrbitalDistance - FlightGlobals.ActiveVessel.mainBody.Radius;
		DrawLine(value.ToDistance(), section.IsHud);
	}
}
