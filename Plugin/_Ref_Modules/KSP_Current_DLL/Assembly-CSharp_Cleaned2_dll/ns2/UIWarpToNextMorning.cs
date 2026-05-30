using UnityEngine;
using UnityEngine.UI;

namespace ns2;

public class UIWarpToNextMorning : MonoBehaviour
{
	public Button button;

	public static double timeOfDawn = 0.3;

	private void Start()
	{
		button.onClick.AddListener(WarpToMorning);
	}

	public void WarpToMorning()
	{
		if (!FlightDriver.Pause)
		{
			if (Sun.Instance == null)
			{
				Debug.LogError("Cannot time warp to next morning, because there is no sun!");
			}
			else if (!TimeWarp.fetch.CancelAutoWarp(0))
			{
				Setup(out var localTime, out var rotPeriod);
				double num = rotPeriod * UtilMath.WrapAround(timeOfDawn - localTime, 0.0, 1.0) + 60.0;
				Debug.Log("Warping to morning. Local time is " + localTime + " and desired time is " + timeOfDawn + " so ETA for daylight: " + KSPUtil.PrintDateDelta(0.0 - num, includeTime: true, includeSeconds: true, useAbs: true));
				TimeWarp.fetch.WarpTo(Planetarium.GetUniversalTime() + num, 8.0, 1.0);
			}
		}
	}

	public void Setup(out double localTime, out double rotPeriod)
	{
		CelestialBody celestialBody;
		double latitude;
		double longitude;
		if (SpaceCenter.Instance != null && SpaceCenter.Instance.cb != null)
		{
			celestialBody = SpaceCenter.Instance.cb;
			latitude = SpaceCenter.Instance.Latitude;
			longitude = SpaceCenter.Instance.Longitude;
		}
		else
		{
			celestialBody = FlightGlobals.GetHomeBody();
			latitude = -0.0917535863160035;
			longitude = 285.3703068811043;
		}
		rotPeriod = celestialBody.rotationPeriod;
		if (celestialBody.orbit != null)
		{
			rotPeriod = celestialBody.orbit.period * rotPeriod / (celestialBody.orbit.period - rotPeriod);
		}
		localTime = Sun.Instance.GetLocalTimeAtPosition(latitude, longitude, celestialBody);
	}
}
