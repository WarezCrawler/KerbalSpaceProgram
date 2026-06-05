using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Vessel;

public class RCSTWR : ReadoutModule
{
	private string actual = string.Empty;

	private double gravity;

	private string total = string.Empty;

	public RCSTWR()
	{
		base.Name = "RCS TWR";
		base.Category = ReadoutCategory.GetCategory("Vessel");
		base.HelpString = "Shows the TWR for the RCS system at the current gravity";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		if (SimulationProcessor.ShowDetails)
		{
			if (SimulationProcessor.LastStage.totalMass > 0.0)
			{
				Vector3d geeForceAtPosition = FlightGlobals.getGeeForceAtPosition(FlightGlobals.ship_position);
				gravity = ((Vector3d)(ref geeForceAtPosition)).magnitude;
				total = (SimulationProcessor.LastStage.RCSThrust / (SimulationProcessor.LastStage.totalMass * gravity)).ToString("F2");
				DrawLine(total, section.IsHud);
			}
			else
			{
				DrawLine("N/A", section.IsHud);
			}
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
