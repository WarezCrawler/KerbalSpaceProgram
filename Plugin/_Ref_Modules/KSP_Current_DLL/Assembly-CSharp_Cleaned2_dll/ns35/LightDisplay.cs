using UnityEngine;
using ns2;

namespace ns35;

public class LightDisplay : MonoBehaviour
{
	public UIButtonToggle toggle;

	private void LateUpdate()
	{
		if (FlightGlobals.ready)
		{
			if (FlightGlobals.ActiveVessel.isEVA)
			{
				toggle.SetState(FlightGlobals.ActiveVessel.evaController.lampOn);
			}
			else
			{
				toggle.SetState(FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.Light]);
			}
		}
	}
}
