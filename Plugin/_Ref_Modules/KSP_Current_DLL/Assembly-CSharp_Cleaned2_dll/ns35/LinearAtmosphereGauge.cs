using UnityEngine;
using ns10;

namespace ns35;

public class LinearAtmosphereGauge : MonoBehaviour
{
	public ns10.LinearGauge gauge;

	private CelestialBody body;

	private double densityRecip = 1.0;

	private void Reset()
	{
		gauge = GetComponent<ns10.LinearGauge>();
	}

	private void LateUpdate()
	{
		if (gauge == null || !FlightGlobals.ready)
		{
			return;
		}
		if (body != FlightGlobals.currentMainBody)
		{
			body = FlightGlobals.currentMainBody;
			if (body.atmosphere)
			{
				densityRecip = 1.0 / body.GetDensity(body.GetPressure(0.0), body.GetTemperature(0.0));
			}
			else
			{
				densityRecip = 1.0;
			}
		}
		if (body.atmosphere)
		{
			double num = FlightGlobals.ActiveVessel.atmDensity * densityRecip;
			if (num > 1.0)
			{
				num = 1.0;
			}
			gauge.SetValue(num);
		}
		else
		{
			gauge.SetValue(0f);
		}
	}
}
