using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class PostBurnPeriapsis : ReadoutModule
{
	public PostBurnPeriapsis()
	{
		base.Name = "Post-burn Periapsis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Closest point of the vessel's orbit after the burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Post-burn Periapsis", ManoeuvreProcessor.PostBurnPe.ToDistance(), section.IsHud);
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
