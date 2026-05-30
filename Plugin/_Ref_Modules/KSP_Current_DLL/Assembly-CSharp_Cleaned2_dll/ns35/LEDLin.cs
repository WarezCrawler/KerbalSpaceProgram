using UnityEngine;
using ns10;

namespace ns35;

public class LEDLin : MonoBehaviour
{
	public GClass9 led;

	private void Reset()
	{
		led = GetComponent<GClass9>();
	}

	private void LateUpdate()
	{
		if (FlightGlobals.ready && led != null)
		{
			led.SetOn(!InputBinding.linRotState);
		}
	}
}
