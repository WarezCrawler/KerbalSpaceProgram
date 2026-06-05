using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class CoolestSkinTemperature : ReadoutModule
{
	public CoolestSkinTemperature()
	{
		base.Name = "Coolest Skin Temperature";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "Lowest external Temperature on the Vessel.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(Units.ToTemperature(ThermalProcessor.CoolestSkinTemperature, ThermalProcessor.CoolestSkinTemperatureMax), section.IsHud);
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
