using KerbalEngineer.Extensions;
using KerbalEngineer.Unity.Flight;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class InterceptAngle : ReadoutModule
{
	public InterceptAngle()
	{
		base.Name = "Transfer Angle";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "The phase angle for starting a Hohmann transfer.";
		base.HelpString = string.Empty;
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		if (RendezvousProcessor.ShowDetails)
		{
			DrawLine(RendezvousProcessor.InterceptAngle.ToAngle(), section.IsHud);
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
