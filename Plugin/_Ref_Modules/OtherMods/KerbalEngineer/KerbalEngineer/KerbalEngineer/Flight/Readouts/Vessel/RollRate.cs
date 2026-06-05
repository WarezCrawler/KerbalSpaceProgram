using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class RollRate : ReadoutModule
{
	public RollRate()
	{
		base.Name = "Roll Rate";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the current Roll speed.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToAngle(AttitudeProcessor.RollRate) + "/sec", section.IsHud);
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
