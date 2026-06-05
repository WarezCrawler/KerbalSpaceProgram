using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Body;

public class BodyName : ReadoutModule
{
	public BodyName()
	{
		base.Name = "Current Body Name";
		base.Category = ReadoutCategory.GetCategory("Body");
		base.HelpString = "Shows the name of the current body.";
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
			DrawLine(FlightGlobals.ActiveVessel.mainBody.bodyName, section.IsHud);
		}
	}
}
