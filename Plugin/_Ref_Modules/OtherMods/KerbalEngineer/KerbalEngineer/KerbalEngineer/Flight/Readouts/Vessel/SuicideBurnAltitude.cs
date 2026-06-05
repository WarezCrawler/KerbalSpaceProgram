using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class SuicideBurnAltitude : ReadoutModule
{
	public SuicideBurnAltitude()
	{
		base.Name = "Suicide Burn Alt.";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the sea level altitude when to start a suicide burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails && ImpactProcessor.ShowDetails)
		{
			DrawLine(ImpactProcessor.SuicideAltitude.ToDistance(), section.IsHud);
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
