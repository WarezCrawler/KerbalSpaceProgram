using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class HasOxygen : ReadoutModule
{
	public HasOxygen()
	{
		base.Name = "Has Oxygen";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "Shows whether the current body has an oxygen rich atmosphere.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(FlightGlobals.ActiveVessel.mainBody.atmosphereContainsOxygen ? "Yes" : "No", section.IsHud);
	}
}
