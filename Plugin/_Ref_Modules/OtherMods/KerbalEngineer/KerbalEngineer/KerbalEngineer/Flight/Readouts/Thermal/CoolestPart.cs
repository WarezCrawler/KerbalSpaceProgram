using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class CoolestPart : ReadoutModule
{
	public CoolestPart()
	{
		base.Name = "Coolest Part";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "The part of the vessel that is enduring the lowest temperature.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(ThermalProcessor.CoolestPartName, section.IsHud);
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
