using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class TimeToTransferAngleTime : ReadoutModule
{
	public TimeToTransferAngleTime()
	{
		base.Name = "Time til Transfer";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "The time until the phase angle equals the transfer angle.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(TimeFormatter.ConvertToString(RendezvousProcessor.TimeToTransferAngle), section.IsHud);
		}
	}

	public override void Reset()
	{
		FlightEngineerCore.Instance.AddUpdatable(RendezvousProcessor.Instance);
	}

	public override void Update()
	{
		RendezvousProcessor.RequestUpdate();
	}
}
