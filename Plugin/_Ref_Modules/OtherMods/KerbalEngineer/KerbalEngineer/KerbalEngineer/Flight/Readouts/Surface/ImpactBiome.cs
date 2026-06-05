using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class ImpactBiome : ReadoutModule
{
	public ImpactBiome()
	{
		base.Name = "Impact Biome";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Biome the Vessel will impact in.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ImpactProcessor.ShowDetails)
		{
			DrawLine(ImpactProcessor.Biome, section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(ImpactProcessor.Instance);
	}

	public override void Update()
	{
		ImpactProcessor.RequestUpdate();
	}
}
