using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class HasAtmosphere : ReadoutModule
{
	public HasAtmosphere()
	{
		base.Name = "Has Atmosphere";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "Shows whether the current body has an atmosphere.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ActiveVessel.mainBody.atmosphere ? "Yes" : "No", section.IsHud);
	}
}
