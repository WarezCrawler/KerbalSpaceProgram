using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class AtmosphericEfficiency : ReadoutModule
{
	public AtmosphericEfficiency()
	{
		base.Name = "Atmos. Efficiency";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows you vessel's efficiency as a ratio of the current velocity and terminal velocity.  Less than 100% means that you are losing efficiency due to gravity and greater than 100% is due to drag.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (AtmosphericProcessor.ShowDetails)
		{
			DrawLine(AtmosphericProcessor.Efficiency.ToPercent(), section.IsHud);
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
