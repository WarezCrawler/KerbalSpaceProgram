using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class PostBurnEccentricity : ReadoutModule
{
	public PostBurnEccentricity()
	{
		base.Name = "Post-burn Eccentricity";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "The eccentricity of the vessel's orbit after the burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine(ManoeuvreProcessor.PostBurnEcc.ToString("F5"), section.IsHud);
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
