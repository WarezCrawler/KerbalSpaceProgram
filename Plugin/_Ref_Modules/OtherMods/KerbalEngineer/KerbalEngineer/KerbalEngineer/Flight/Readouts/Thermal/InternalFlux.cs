using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class InternalFlux : ReadoutModule
{
	public InternalFlux()
	{
		base.Name = "Internal Flux";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = string.Empty;
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(ThermalProcessor.InternalFlux.ToFlux(), section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(ThermalProcessor.Instance);
	}

	public override void Update()
	{
		ThermalProcessor.RequestUpdate();
	}
}
