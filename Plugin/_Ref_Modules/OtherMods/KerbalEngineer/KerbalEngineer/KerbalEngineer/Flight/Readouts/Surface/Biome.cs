using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class Biome : ReadoutModule
{
	public Biome()
	{
		base.Name = "Biome";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the biome which the vessel is currently flying over.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		string experimentBiome = ScienceUtil.GetExperimentBiome(FlightGlobals.ActiveVessel.mainBody, FlightGlobals.ActiveVessel.latitude, FlightGlobals.ActiveVessel.longitude);
		experimentBiome = ScienceUtil.GetBiomedisplayName(FlightGlobals.ActiveVessel.mainBody, experimentBiome);
		DrawLine(experimentBiome, section.IsHud);
	}
}
