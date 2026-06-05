using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeProgradeDeltaV : ReadoutModule
{
	public NodeProgradeDeltaV()
	{
		base.Name = "Manoeuvre Node DeltaV (Prograde)";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Prograde/Retrograde component of the total change in velocity.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Node DeltaV (Prograde)", ManoeuvreProcessor.ProgradeDeltaV.ToSpeed(), section.IsHud);
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
