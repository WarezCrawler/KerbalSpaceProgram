using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class ThrustToWeight : ReadoutModule
{
	private string actual = string.Empty;

	private double gravity;

	private string total = string.Empty;

	public ThrustToWeight()
	{
		base.Name = "Thrust to Weight Ratio";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the vessel's actual and total thrust to weight ratio.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		if (SimulationProcessor.ShowDetails)
		{
			actual = "N/A";
			total = "N/A";
			if (SimulationProcessor.LastStage.totalMass > 0.0)
			{
				Vector3d geeForceAtPosition = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position);
				gravity = ((Vector3d)(ref geeForceAtPosition)).magnitude;
				actual = (SimulationProcessor.LastStage.actualThrust / (SimulationProcessor.LastStage.totalMass * gravity)).ToString("F2");
				total = (SimulationProcessor.LastStage.thrust / (SimulationProcessor.LastStage.totalMass * gravity)).ToString("F2");
			}
			DrawLine("TWR", actual + " / " + total, section.IsHud);
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
