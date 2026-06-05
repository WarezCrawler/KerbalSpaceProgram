using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeRadialDeltaV : ReadoutModule
{
	public NodeRadialDeltaV()
	{
		base.Name = "Manoeuvre Node DeltaV (Radial)";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Radial component of the total change in velocity.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Node DeltaV (Radial)", ManoeuvreProcessor.RadialDeltaV.ToSpeed(), section.IsHud);
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
