using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class SuicideBurnCountdown : ReadoutModule
{
	public SuicideBurnCountdown()
	{
		base.Name = "Suicide Burn Countdown";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Time until suicide burn should start.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails && ImpactProcessor.ShowDetails)
		{
			DrawLine(TimeFormatter.ConvertToString(ImpactProcessor.SuicideCountdown), section.IsHud);
		}
	}

	public override void Reset()
	{
	}

	public override void Update()
	{
		ImpactProcessor.RequestUpdate();
	}
}
