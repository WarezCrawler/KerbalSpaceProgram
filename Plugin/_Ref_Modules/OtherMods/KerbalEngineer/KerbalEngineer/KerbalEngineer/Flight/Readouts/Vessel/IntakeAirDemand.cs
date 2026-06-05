using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class IntakeAirDemand : ReadoutModule
{
	private double demand;

	public IntakeAirDemand()
	{
		base.Name = "Intake Air (Demand)";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Displays the Amount of Intake Air required.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(demand.ToString("F4"), section.IsHud);
	}

	public override void Update()
	{
		demand = IntakeAirDemandSupply.GetDemand();
	}
}
