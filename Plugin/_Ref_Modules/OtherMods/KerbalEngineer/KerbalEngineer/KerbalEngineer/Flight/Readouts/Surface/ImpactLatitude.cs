using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class ImpactLatitude : ReadoutModule
{
	public ImpactLatitude()
	{
		base.Name = "Impact Latitude";
		base.Category = ReadoutCategory.GetCategory("Surface");
		base.HelpString = "Latitude of the impact position.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (ImpactProcessor.ShowDetails)
		{
			DrawLine(Units.ToAngleDMS(ImpactProcessor.Latitude) + ((ImpactProcessor.Latitude < 0.0) ? " S" : " N"), section.IsHud);
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
