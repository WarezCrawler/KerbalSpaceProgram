using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class RCSIsp : ReadoutModule
{
	public RCSIsp()
	{
		base.Name = "RCS Isp";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the average specific impulse of the RCS System.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (SimulationProcessor.ShowDetails)
		{
			DrawLine(SimulationProcessor.LastStage.RCSIsp.ToString("F1") + "s", section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(SimulationProcessor.Instance);
	}

	public override void Update()
	{
		SimulationProcessor.RequestUpdate();
	}
}
