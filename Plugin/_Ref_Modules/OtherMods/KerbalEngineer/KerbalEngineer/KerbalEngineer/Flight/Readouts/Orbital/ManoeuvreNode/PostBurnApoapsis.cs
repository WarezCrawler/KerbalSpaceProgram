using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class PostBurnApoapsis : ReadoutModule
{
	public PostBurnApoapsis()
	{
		base.Name = "Post-burn Apoapsis";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Farthest point of the vessel's orbit after the burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Post-burn Apoapsis", ManoeuvreProcessor.PostBurnAp.ToDistance(), section.IsHud);
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
