using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeNormalDeltaV : ReadoutModule
{
	public NodeNormalDeltaV()
	{
		base.Name = "Manoeuvre Node DeltaV (Normal)";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Normal component of the total change in velocity.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Node DeltaV (Normal)", ManoeuvreProcessor.NormalDeltaV.ToSpeed(), section.IsHud);
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
