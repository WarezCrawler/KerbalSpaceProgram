using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class Name : ReadoutModule
{
	public Name()
	{
		base.Name = "Name";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Displays the name of the current vessel.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(FlightGlobals.ActiveVessel.vesselName, section.IsHud);
		}
	}
}
