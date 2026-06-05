using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class ConvectionFlux : ReadoutModule
{
	public ConvectionFlux()
	{
		base.Name = "Convection Flux";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = string.Empty;
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails && FlightGlobals.ActiveVessel.atmDensity > 0.0)
		{
			DrawLine(ThermalProcessor.ConvectionFlux.ToFlux(), section.IsHud);
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
