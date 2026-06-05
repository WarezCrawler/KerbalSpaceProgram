using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class Roll : ReadoutModule
{
	public Roll()
	{
		base.Name = "Roll";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current Roll angle.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToAngle(AttitudeProcessor.Roll), section.IsHud);
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
