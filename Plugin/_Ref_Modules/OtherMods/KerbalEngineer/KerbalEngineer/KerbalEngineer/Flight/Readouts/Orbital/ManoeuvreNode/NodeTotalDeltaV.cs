using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeTotalDeltaV : ReadoutModule
{
	public NodeTotalDeltaV()
	{
		base.Name = "Manoeuvre Node DeltaV (Total)";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Total change in velocity during the burn.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Node DeltaV (Total)", ManoeuvreProcessor.TotalDeltaV.ToSpeed() + " (" + (ManoeuvreProcessor.HasDeltaV ? ("S" + ManoeuvreProcessor.FinalStage) : "X") + ")", section.IsHud);
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
