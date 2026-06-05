using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class HottestPart : ReadoutModule
{
	public HottestPart()
	{
		base.Name = "Hottest Part";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "The part of the vessel that is enduring the highest temperature.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(ThermalProcessor.HottestPartName, section.IsHud);
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
