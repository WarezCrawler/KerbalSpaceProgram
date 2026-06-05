using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class BodyMass : ReadoutModule
{
	public BodyMass()
	{
		base.Name = "Body Mass";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The mass of the body.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToMass(FlightGlobals.ActiveVessel.mainBody.Mass), section.IsHud);
	}
}
