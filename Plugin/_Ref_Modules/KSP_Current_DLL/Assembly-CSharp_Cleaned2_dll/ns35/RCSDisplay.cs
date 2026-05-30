using UnityEngine;
using ns2;

namespace ns35;

public class RCSDisplay : MonoBehaviour
{
	public UIStateImage stateImage;

	public UIStateText stateText;

	private void Reset()
	{
		stateImage = GetComponent<UIStateImage>();
		stateText = GetComponent<UIStateText>();
	}

	private void Start()
	{
		LateUpdate();
	}

	private void SetNewState(string newState)
	{
		stateImage.SetState(newState);
		stateText.SetState(newState);
	}

	private void LateUpdate()
	{
		if (FlightGlobals.ready)
		{
			KerbalEVA kerbalEVA = (FlightGlobals.ActiveVessel.isEVA ? FlightGlobals.ActiveVessel.evaController : null);
			if (kerbalEVA == null)
			{
				SetNewState(FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.flag_5] ? "On" : "Off");
			}
			else
			{
				SetNewState(kerbalEVA.JetpackDeployed ? "On" : "Off");
			}
		}
	}
}
