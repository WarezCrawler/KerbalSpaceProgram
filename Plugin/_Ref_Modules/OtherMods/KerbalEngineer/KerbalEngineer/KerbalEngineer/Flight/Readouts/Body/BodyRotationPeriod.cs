using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Body;

public class BodyRotationPeriod : ReadoutModule
{
	public BodyRotationPeriod()
	{
		base.Name = "Body Rotation Period";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The time to complete one revolution about the body's axis.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToTime(FlightGlobals.ActiveVessel.mainBody.rotationPeriod), section.IsHud);
	}
}
