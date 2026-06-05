using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class SuicideBurnDistance : ReadoutModule
{
	public SuicideBurnDistance()
	{
		base.Name = "Suicide Burn Dist.";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the distance to the point at which to start a suicide burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails && ImpactProcessor.ShowDetails)
		{
			DrawLine(ImpactProcessor.SuicideDistance.ToDistance(), section.IsHud);
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
