using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Orbital.ManoeuvreNode;

public class TripTotalDeltaV : ReadoutModule
{
	public TripTotalDeltaV()
	{
		base.Name = "Trip Total DeltaV (Normal)";
		base.Category = ReadoutCategory.GetCategory("Orbital");
		base.HelpString = "Total DeltaV of all maneuver nodes.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ManoeuvreProcessor.ShowDetails)
		{
			DrawLine("Trip Total DeltaV ", ManoeuvreProcessor.TripDeltaV.ToSpeed(), section.IsHud);
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
