using UnityEngine;
using UnityEngine.EventSystems;
using ns9;

namespace ns26;

public class MaxProgression : MonoBehaviour, IEventSystemHandler, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (!(ProgressTracking.Instance == null))
		{
			PointerEventData.InputButton button = eventData.button;
			if (button == PointerEventData.InputButton.Right)
			{
				ProgressTracking.Instance.CheatEarlyProgression();
				ScreenMessages.PostScreenMessage(Localizer.Format("#autoLOC_6001914", FlightGlobals.GetHomeBodyDisplayName()), 5f, ScreenMessageStyle.UPPER_CENTER);
			}
			else
			{
				ProgressTracking.Instance.CheatProgression();
				ScreenMessages.PostScreenMessage(Localizer.Format("#autoLOC_6001915"), 5f, ScreenMessageStyle.UPPER_CENTER);
			}
		}
	}
}
