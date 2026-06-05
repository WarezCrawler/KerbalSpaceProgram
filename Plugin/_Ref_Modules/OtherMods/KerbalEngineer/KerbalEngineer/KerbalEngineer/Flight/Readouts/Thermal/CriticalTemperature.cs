using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class CriticalTemperature : ReadoutModule
{
	public CriticalTemperature()
	{
		base.Name = "Critical Temperature";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "Internal Temperature on the part of the Vessel that is structually most critical.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(Units.ToTemperature(ThermalProcessor.CriticalTemperature, ThermalProcessor.CriticalTemperatureMax), section.IsHud);
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
