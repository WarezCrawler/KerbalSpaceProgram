using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class HeadingRate : ReadoutModule
{
	public HeadingRate()
	{
		base.Name = "Heading Rate";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current change in Heading.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToAngle(AttitudeProcessor.HeadingRate) + "/sec", section.IsHud);
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(AttitudeProcessor.Instance);
	}

	public override void Update()
	{
		AttitudeProcessor.RequestUpdate();
	}
}
