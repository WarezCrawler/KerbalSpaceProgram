using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class Pitch : ReadoutModule
{
	public Pitch()
	{
		base.Name = "Pitch";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current Pitch angle.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToAngle(AttitudeProcessor.Pitch), section.IsHud);
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
