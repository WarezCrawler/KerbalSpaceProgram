using KerbalEngineer.Flight.Readouts.Rendezvous;
using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Surface;

public class TargetLatitude : ReadoutModule
{
	public TargetLatitude()
	{
		base.Name = "Target Latitude";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "Shows the target vessel's latitude position around the celestial body. Latitude is the angle from the equator to poles.";
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
			double num = AngleHelper.Clamp180(vessel.latitude);
			DrawLine(Units.ToAngleDMS(num) + ((num < 0.0) ? " S" : " N"), section.IsHud);
		}
	}

	public override void Update()
	{
		RendezvousProcessor.RequestUpdate();
	}
}
