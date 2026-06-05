using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class HottestTemperature : ReadoutModule
{
	public HottestTemperature()
	{
		base.Name = "Hottest Temperature";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "Highest internal Temperature on the Vessel.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(Units.ToTemperature(ThermalProcessor.HottestTemperature, ThermalProcessor.HottestTemperatureMax), section.IsHud);
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
