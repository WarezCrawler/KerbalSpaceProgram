using System.Linq;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class IntakeAirDemandSupply : ReadoutModule
{
	private double demand;

	private double supply;

	public IntakeAirDemandSupply()
	{
		base.Name = "Intake Air (D/S)";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Displays the Ratio between required and available Intake Air.";
		base.IsDefault = false;
	}

	public static double GetDemand()
	{
		double num = 0.0;
		foreach (Part part in FlightGlobals.ActiveVessel.Parts)
		{
			for (int i = 0; i < part.Modules.Count; i++)
			{
				PartModule obj = part.Modules[i];
				ModuleEngines val = (ModuleEngines)(object)((obj is ModuleEngines) ? obj : null);
				if ((Object)(object)val != (Object)null && val.isOperational)
				{
					num += val.propellants.Where((Propellant p) => p.name == "IntakeAir").Sum((Propellant p) => p.currentRequirement);
				}
			}
		}
		return num;
	}

	public static double GetSupply()
	{
		return FlightGlobals.ActiveVessel.Parts.Where((Part p) => p.Resources.Contains("IntakeAir")).Sum((Part p) => p.Resources["IntakeAir"].amount);
	}

	public override void Draw(ISectionModule section)
	{
		DrawLine(demand.ToString("F4") + " / " + supply.ToString("F4"), section.IsHud);
	}

	public override void Update()
	{
		demand = GetDemand();
		supply = GetSupply();
	}
}
