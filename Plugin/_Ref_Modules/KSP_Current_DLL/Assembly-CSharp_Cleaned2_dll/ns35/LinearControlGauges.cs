using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ns10;

namespace ns35;

public class LinearControlGauges : MonoBehaviour
{
	public List<Image> inputGaugeImages;

	public ns10.LinearGauge pitch;

	public ns10.LinearGauge yaw;

	public ns10.LinearGauge roll;

	public ns10.LinearGauge linX;

	public ns10.LinearGauge linY;

	public ns10.LinearGauge linZ;

	public bool linXInverted;

	public bool linYInverted;

	public bool linZInverted;

	public void Start()
	{
		GameEvents.Input.OnPrecisionModeToggle.Add(onPrecisionModeToggle);
	}

	public void OnDestroy()
	{
		GameEvents.Input.OnPrecisionModeToggle.Remove(onPrecisionModeToggle);
	}

	protected void onPrecisionModeToggle(bool precisionMode)
	{
		int count = inputGaugeImages.Count;
		while (count-- > 0)
		{
			inputGaugeImages[count].color = (precisionMode ? XKCDColors.BrightCyan : XKCDColors.Orange);
		}
	}

	private void LateUpdate()
	{
		if (!FlightGlobals.ready)
		{
			return;
		}
		if (pitch != null)
		{
			pitch.SetValue(0f - FlightGlobals.ActiveVessel.ctrlState.pitch);
		}
		if (yaw != null)
		{
			yaw.SetValue(FlightGlobals.ActiveVessel.ctrlState.yaw);
		}
		if (roll != null)
		{
			roll.SetValue(FlightGlobals.ActiveVessel.ctrlState.roll);
		}
		if (linX != null)
		{
			if (!linXInverted)
			{
				linX.SetValue(FlightGlobals.ActiveVessel.ctrlState.float_0);
			}
			else
			{
				linX.SetValue(0f - FlightGlobals.ActiveVessel.ctrlState.float_0);
			}
		}
		if (linY != null)
		{
			if (!linYInverted)
			{
				linY.SetValue(FlightGlobals.ActiveVessel.ctrlState.float_1);
			}
			else
			{
				linY.SetValue(0f - FlightGlobals.ActiveVessel.ctrlState.float_1);
			}
		}
		if (linZ != null)
		{
			if (!linZInverted)
			{
				linZ.SetValue(FlightGlobals.ActiveVessel.ctrlState.float_2);
			}
			else
			{
				linZ.SetValue(0f - FlightGlobals.ActiveVessel.ctrlState.float_2);
			}
		}
	}
}
