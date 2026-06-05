using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class SuicideBurnDeltaV : ReadoutModule
{
	public SuicideBurnDeltaV()
	{
		base.Name = "Suicide Burn dV";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the DeltaV of a suicide burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails && ImpactProcessor.ShowDetails)
		{
			DrawLine(ImpactProcessor.SuicideDeltaV.ToString("N1") + "m/s", section.IsHud);
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
