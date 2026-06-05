using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class PostBurnInclination : ReadoutModule
{
	public PostBurnInclination()
	{
		base.Name = "Post-burn Inclination";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "The inclination of the vessel's orbit after the burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine(ManoeuvreProcessor.PostBurnInclination.ToAngle(), section.IsHud);
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
