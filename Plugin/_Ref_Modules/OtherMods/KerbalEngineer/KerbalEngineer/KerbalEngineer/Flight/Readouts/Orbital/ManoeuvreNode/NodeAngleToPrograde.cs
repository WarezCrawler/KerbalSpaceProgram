using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeAngleToPrograde : ReadoutModule
{
	public NodeAngleToPrograde()
	{
		base.Name = "Manoeuvre Node Angle to Prograde";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Angular Distance from the Node to crossing the Orbit of the central body on it's prograde side.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Node Angle to Prograde", ManoeuvreProcessor.AngleToPrograde.ToAngle(), section.IsHud);
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
