using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class SuicideBurnLength : ReadoutModule
{
	public SuicideBurnLength()
	{
		base.Name = "Suicide Burn Length";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the duration of the suicide burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails && ImpactProcessor.ShowDetails)
		{
			DrawLine(TimeFormatter.ConvertToString(ImpactProcessor.SuicideLength), section.IsHud);
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
