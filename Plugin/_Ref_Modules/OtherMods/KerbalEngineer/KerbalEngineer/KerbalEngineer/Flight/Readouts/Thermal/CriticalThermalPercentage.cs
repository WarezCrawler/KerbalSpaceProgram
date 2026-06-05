using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class CriticalThermalPercentage : ReadoutModule
{
	public CriticalThermalPercentage()
	{
		base.Name = "Critical Thermal Percentage";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "Shows how high a temperature the critical Part is enduring relative to it's maximal temperature.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(ThermalProcessor.CriticalTemperaturePercentage.ToPercent(), section.IsHud);
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
