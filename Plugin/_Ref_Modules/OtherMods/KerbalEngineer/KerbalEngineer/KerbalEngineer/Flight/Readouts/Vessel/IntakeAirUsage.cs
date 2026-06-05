using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class IntakeAirUsage : ReadoutModule
{
	private double percentage;

	public IntakeAirUsage()
	{
		base.Name = "Intake Air (Usage)";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Displays the consumption of Intake Air.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(Units.ToPercent(percentage), section.IsHud);
	}

	public override void Update()
	{
		percentage = IntakeAirDemandSupply.GetDemand() / IntakeAirDemandSupply.GetSupply();
		if (double.IsNaN(percentage) || double.IsInfinity(percentage))
		{
			percentage = 0.0;
		}
	}
}
