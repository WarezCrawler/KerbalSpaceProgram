using KerbalEngineer.Helpers;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Body;

public class BodyRadius : ReadoutModule
{
	public BodyRadius()
	{
		base.Name = "Body Radius";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "The radius of the body at sea level.";
		base.IsDefault = false;
	}

	public override void Draw(ISectionModule section)
	{
		if ((Object)(object)FlightGlobals.ActiveVessel.mainBody == (Object)null)
		{
			DrawLine("N/A", section.IsHud);
		}
		else
		{
			DrawLine(Units.ToDistance(FlightGlobals.ActiveVessel.mainBody.Radius), section.IsHud);
		}
	}
}
