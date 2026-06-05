using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class CoolestTemperature : ReadoutModule
{
	public CoolestTemperature()
	{
		base.Name = "Coolest Temperature";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "Lowest internal Temperature on the Vessel.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(Units.ToTemperature(ThermalProcessor.CoolestTemperature, ThermalProcessor.CoolestTemperatureMax), section.IsHud);
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
