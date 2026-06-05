using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class SpecificImpulse : ReadoutModule
{
	public SpecificImpulse()
	{
		base.Name = "Specific Impulse";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the average Specific Impulse of all engines in the current stage.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(SimulationProcessor.LastStage.isp.ToString("F1") + "s", section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
	}

	public override void Update()
	{
		SimulationProcessor.RequestUpdate();
	}
}
