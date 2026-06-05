using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class ImpactAltitude : ReadoutModule
{
	public ImpactAltitude()
	{
		base.Name = "Impact Altitude";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Altitude at which the Vessel will impact.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ImpactProcessor.ShowDetails)
		{
			DrawLine(ImpactProcessor.Altitude.ToDistance(), section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(ImpactProcessor.Instance);
	}

	public override void Update()
	{
		ImpactProcessor.RequestUpdate();
	}
}
