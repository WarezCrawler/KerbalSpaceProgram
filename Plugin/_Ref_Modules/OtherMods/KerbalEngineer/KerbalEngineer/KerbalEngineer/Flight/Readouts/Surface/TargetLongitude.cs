using KerbalEngineer.Flight.Readouts.Rendezvous;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class TargetLongitude : ReadoutModule
{
	public TargetLongitude()
	{
		base.Name = "Target Longitude";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the target vessel's longitude around a celestial body. Longitude is the angle from the bodies prime meridian.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if (!RendezvousProcessor.ShowDetails)
		{
			return;
		}
		Vessel targetVessel = RendezvousProcessor.targetVessel;
		if ((Object)(object)targetVessel != (Object)null)
		{
			Vessel vessel = targetVessel.GetVessel();
			if ((Object)(object)vessel == (Object)null)
			{
				DrawLine("N/A", section.IsHud);
				return;
			}
			double num = AngleHelper.Clamp180(vessel.longitude);
			DrawLine(Units.ToAngleDMS(num) + ((num < 0.0) ? " W" : " E"), section.IsHud);
		}
	}

	public override void Update()
	{
		RendezvousProcessor.RequestUpdate();
	}
}
