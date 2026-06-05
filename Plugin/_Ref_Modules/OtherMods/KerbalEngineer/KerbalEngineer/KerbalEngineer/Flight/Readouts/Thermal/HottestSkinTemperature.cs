using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class HottestSkinTemperature : ReadoutModule
{
	public HottestSkinTemperature()
	{
		base.Name = "Hottest Skin Temperature";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "Highest external Temperature on the Vessel.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(Units.ToTemperature(ThermalProcessor.HottestSkinTemperature, ThermalProcessor.HottestSkinTemperatureMax), section.IsHud);
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
