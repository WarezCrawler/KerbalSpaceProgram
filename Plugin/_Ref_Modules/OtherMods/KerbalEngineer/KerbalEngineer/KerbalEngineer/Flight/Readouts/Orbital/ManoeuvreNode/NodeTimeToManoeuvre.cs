using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeTimeToManoeuvre : ReadoutModule
{
	public NodeTimeToManoeuvre()
	{
		base.Name = "Time to Manoeuvre Node";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Time until the vessel reaches the position of the Manoeuvre Node.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Time to Node", TimeFormatter.ConvertToString(ManoeuvreProcessor.UniversalTime - Planetarium.GetUniversalTime()), section.IsHud);
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
