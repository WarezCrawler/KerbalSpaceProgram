using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class IntakeAirSupply : ReadoutModule
{
	private double supply;

	public IntakeAirSupply()
	{
		base.Name = "Intake Air (Supply)";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Displays the available Intake Air.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(supply.ToString("F4"), section.IsHud);
	}

	public override void Update()
	{
		supply = IntakeAirDemandSupply.GetSupply();
	}
}
