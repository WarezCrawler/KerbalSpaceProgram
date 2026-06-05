using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class AltitudeTerrain : ReadoutModule
{
	public AltitudeTerrain()
	{
		base.Name = "Altitude (Terrain)";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Shows the vessel's altitude above the terrain and water's surface, or altitude above underwater terrain whilst splashed down.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Invalid comparison between Unknown and I4
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Invalid comparison between Unknown and I4
		if (FlightGlobals.ActiveVessel.terrainAltitude > 0.0 || (int)FlightGlobals.ActiveVessel.situation == 2 || (int)FlightGlobals.ActiveVessel.situation == 1)
		{
			DrawLine((FlightGlobals.ship_altitude - FlightGlobals.ActiveVessel.terrainAltitude).ToDistance(), section.IsHud);
		}
		else
		{
			DrawLine(FlightGlobals.ship_altitude.ToDistance(), section.IsHud);
		}
	}
}
