using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeBurnTime : ReadoutModule
{
	public NodeBurnTime()
	{
		base.Name = "Manoeuvre Node Burn Time";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "The burn's total duration.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Node Burn Time", TimeFormatter.ConvertToString(ManoeuvreProcessor.BurnTime), section.IsHud);
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
