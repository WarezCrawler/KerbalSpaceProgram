using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class PostBurnPeriod : ReadoutModule
{
	public PostBurnPeriod()
	{
		base.Name = "Post-burn Period";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "The period of the vessel's orbit after the burn.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine(TimeFormatter.ConvertToString(ManoeuvreProcessor.PostBurnPeriod, "F3"), section.IsHud);
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
