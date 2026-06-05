using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class CriticalSkinTemperature : ReadoutModule
{
	public CriticalSkinTemperature()
	{
		base.Name = "Critical Skin Temperature";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "Highest external Temperature on the part of the Vessel that is structually most critical.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(Units.ToTemperature(ThermalProcessor.CriticalSkinTemperature, ThermalProcessor.CriticalSkinTemperatureMax), section.IsHud);
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
