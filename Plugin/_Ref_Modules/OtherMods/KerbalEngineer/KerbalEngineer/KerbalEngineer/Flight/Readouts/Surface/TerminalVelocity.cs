using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class TerminalVelocity : ReadoutModule
{
	public TerminalVelocity()
	{
		base.Name = "Terminal Velocity";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the velocity where the efforts of thrust and drag are equalled out.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (AtmosphericProcessor.ShowDetails)
		{
			DrawLine(AtmosphericProcessor.TerminalVelocity.ToSpeed(), section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(AtmosphericProcessor.Instance);
	}

	public override void Update()
	{
		AtmosphericProcessor.RequestUpdate();
	}
}
