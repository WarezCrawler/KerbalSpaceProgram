using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class ImpactLongitude : ReadoutModule
{
	public ImpactLongitude()
	{
		base.Name = "Impact Longitude";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Longditude of the impact position.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ImpactProcessor.ShowDetails)
		{
			DrawLine(Units.ToAngleDMS(ImpactProcessor.Longitude) + ((ImpactProcessor.Longitude < 0.0) ? " W" : " E"), section.IsHud);
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
