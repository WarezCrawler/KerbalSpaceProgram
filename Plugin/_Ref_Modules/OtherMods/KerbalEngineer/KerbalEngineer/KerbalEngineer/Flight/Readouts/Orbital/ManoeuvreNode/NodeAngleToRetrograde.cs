using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class NodeAngleToRetrograde : ReadoutModule
{
	public NodeAngleToRetrograde()
	{
		base.Name = "Manoeuvre Node Angle to Retrograde";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Angular Distance from the Node to crossing the Orbit of the central body on it's retrograde side.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Node Angle to Retrograde", ManoeuvreProcessor.AngleToRetrograde.ToAngle(), section.IsHud);
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
