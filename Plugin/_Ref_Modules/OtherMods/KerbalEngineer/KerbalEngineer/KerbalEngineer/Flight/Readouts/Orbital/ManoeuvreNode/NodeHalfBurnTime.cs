using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeHalfBurnTime : ReadoutModule
{
	public NodeHalfBurnTime()
	{
		base.Name = "Manoeuvre Node Half Burn Time";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Half of the burn's total duration.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Node Burn Time (½)", TimeFormatter.ConvertToString(ManoeuvreProcessor.HalfBurnTime), section.IsHud);
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
