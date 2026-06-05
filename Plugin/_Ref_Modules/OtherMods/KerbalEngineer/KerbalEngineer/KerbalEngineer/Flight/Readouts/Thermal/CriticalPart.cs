using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class CriticalPart : ReadoutModule
{
	public CriticalPart()
	{
		base.Name = "Critical Part";
		base.Category = ReadoutCategory.GetCategory("Thermal");
		base.HelpString = "This part is structually most critical. If it endures too high temperature there is a high chance for major structual failure!";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (ThermalProcessor.ShowDetails)
		{
			DrawLine(ThermalProcessor.CriticalPartName, section.IsHud);
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
