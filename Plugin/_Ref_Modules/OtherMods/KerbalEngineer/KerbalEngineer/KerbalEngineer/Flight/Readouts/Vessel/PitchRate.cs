using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class PitchRate : ReadoutModule
{
	public PitchRate()
	{
		base.Name = "Pitch Rate";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current Pitch speed.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToAngle(AttitudeProcessor.PitchRate) + "/sec", section.IsHud);
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
