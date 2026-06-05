using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeTimeToHalfBurn : ReadoutModule
{
	public NodeTimeToHalfBurn()
	{
		base.Name = "Time to Manoeuvre Burn";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Time until the Manoeuvre should be started.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Time to Node Burn", TimeFormatter.ConvertToString(ManoeuvreProcessor.UniversalTime - ManoeuvreProcessor.HalfBurnTime - Planetarium.GetUniversalTime()), section.IsHud);
		}
	}

	public override void Reset()
	{
		ManoeuvreProcessor.Reset();
	}

	public override void Update()
	{
		ManoeuvreProcessor.RequestUpdate();
	}
}
