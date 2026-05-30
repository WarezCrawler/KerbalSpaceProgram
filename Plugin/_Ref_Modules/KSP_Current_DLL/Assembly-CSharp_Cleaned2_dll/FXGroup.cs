using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FXGroup
{
	public List<ParticleSystem> fxEmittersNewSystem = new List<ParticleSystem>();

	public List<Light> lights = new List<Light>();

	public AudioClip sfx;

	public AudioSource audio;

	private List<float> initSizeValuesNewSystem;

	private List<float> initLifeValuesNewSystem;

	private List<float> initLightValues;

	private List<float> initSizeValuesNewSystemVariation;

	private List<float> initLifeValuesNewSystemVariation;

	private bool valid;

	public string name;

	private bool active;

	private float power;

	public float powerVariation;

	public bool activeLatch;

	public bool isValid => valid;

	public bool Active => active;

	public float Power
	{
		get
		{
			return power;
		}
		set
		{
			SetPower(value);
		}
	}

	public FXGroup(string groupID)
	{
		name = groupID;
	}

	public void begin(AudioSource audioRef)
	{
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00db: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_012c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		valid = true;
		initSizeValuesNewSystem = new List<float>();
		initLifeValuesNewSystem = new List<float>();
		initLightValues = new List<float>();
		initSizeValuesNewSystemVariation = new List<float>();
		initLifeValuesNewSystemVariation = new List<float>();
		for (int i = 0; i < lights.Count; i++)
		{
			Light light = lights[i];
			initLightValues.Add(light.intensity);
			light.enabled = false;
		}
		for (int j = 0; j < fxEmittersNewSystem.Count; j++)
		{
			ParticleSystem obj = fxEmittersNewSystem[j];
			MainModule main = obj.main;
			List<float> list = initSizeValuesNewSystem;
			MinMaxCurve val = ((MainModule)(ref main)).startSize;
			float constantMax = ((MinMaxCurve)(ref val)).constantMax;
			val = ((MainModule)(ref main)).startSize;
			list.Add(Mathf.Max(constantMax, ((MinMaxCurve)(ref val)).constantMin));
			List<float> list2 = initLifeValuesNewSystem;
			val = ((MainModule)(ref main)).startLifetime;
			float constantMax2 = ((MinMaxCurve)(ref val)).constantMax;
			val = ((MainModule)(ref main)).startLifetime;
			list2.Add(Mathf.Max(constantMax2, ((MinMaxCurve)(ref val)).constantMin));
			List<float> list3 = initSizeValuesNewSystemVariation;
			val = ((MainModule)(ref main)).startSize;
			float constantMax3 = ((MinMaxCurve)(ref val)).constantMax;
			val = ((MainModule)(ref main)).startSize;
			list3.Add(Mathf.Abs(constantMax3 - ((MinMaxCurve)(ref val)).constantMin));
			List<float> list4 = initLifeValuesNewSystemVariation;
			val = ((MainModule)(ref main)).startLifetime;
			float constantMax4 = ((MinMaxCurve)(ref val)).constantMax;
			val = ((MainModule)(ref main)).startLifetime;
			list4.Add(Mathf.Abs(constantMax4 - ((MinMaxCurve)(ref val)).constantMin));
			obj.Stop();
		}
		if (audio == null)
		{
			audio = audioRef;
		}
		if (sfx != null && audio != null)
		{
			audio.clip = sfx;
		}
		active = false;
	}

	public void setActive(bool value)
	{
		if (!valid)
		{
			return;
		}
		for (int i = 0; i < lights.Count; i++)
		{
			lights[i].enabled = value;
		}
		for (int j = 0; j < fxEmittersNewSystem.Count; j++)
		{
			if (value)
			{
				fxEmittersNewSystem[j].Play();
			}
			else
			{
				fxEmittersNewSystem[j].Stop();
			}
		}
		if (sfx != null && audio != null)
		{
			audio.clip = sfx;
			if (value)
			{
				if (!audio.isPlaying)
				{
					audio.time = UnityEngine.Random.Range(0f, audio.clip.length);
					audio.Play();
				}
			}
			else
			{
				audio.Stop();
			}
		}
		active = value;
	}

	public void Burst()
	{
		if (valid)
		{
			for (int i = 0; i < fxEmittersNewSystem.Count; i++)
			{
				fxEmittersNewSystem[i].Play();
			}
			if (sfx != null && audio != null)
			{
				audio.PlayOneShot(sfx);
			}
		}
	}

	public void SetLatch(bool latch)
	{
		if (valid && latch)
		{
			setActive(value: true);
			activeLatch = true;
		}
	}

	public void SetPower(float pwr)
	{
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		power = pwr;
		if (valid)
		{
			int i = 0;
			for (int count = lights.Count; i < count; i++)
			{
				lights[i].intensity = initLightValues[i] * pwr;
			}
			int j = 0;
			for (int count2 = fxEmittersNewSystem.Count; j < count2; j++)
			{
				MainModule main = fxEmittersNewSystem[j].main;
				MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
				((MinMaxCurve)(ref startLifetime)).constantMin = (initLifeValuesNewSystem[j] - initLifeValuesNewSystemVariation[j]) * power;
				((MinMaxCurve)(ref startLifetime)).constantMax = initLifeValuesNewSystem[j] * power;
				((MinMaxCurve)(ref startLifetime)).mode = (ParticleSystemCurveMode)3;
				((MainModule)(ref main)).startLifetime = startLifetime;
				MinMaxCurve startSize = ((MainModule)(ref main)).startSize;
				((MinMaxCurve)(ref startSize)).constantMin = (initSizeValuesNewSystem[j] - initSizeValuesNewSystemVariation[j]) * power;
				((MinMaxCurve)(ref startSize)).constantMax = initSizeValuesNewSystem[j] * power;
				((MinMaxCurve)(ref startSize)).mode = (ParticleSystemCurveMode)3;
				((MainModule)(ref main)).startSize = startSize;
			}
			if (sfx != null && audio != null)
			{
				audio.pitch = pwr;
			}
		}
	}

	public void SetPowerLatch(float pwr)
	{
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0112: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		if (!activeLatch)
		{
			return;
		}
		power = Mathf.Clamp01(pwr);
		if (valid)
		{
			int i = 0;
			for (int count = lights.Count; i < count; i++)
			{
				Light light = lights[i];
				light.intensity = Mathf.Max(initLightValues[i] * pwr, light.intensity);
			}
			int j = 0;
			for (int count2 = fxEmittersNewSystem.Count; j < count2; j++)
			{
				MainModule main = fxEmittersNewSystem[j].main;
				MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
				((MinMaxCurve)(ref startLifetime)).constantMin = Mathf.Max(initLifeValuesNewSystem[j] * power - powerVariation * 0.5f, ((MinMaxCurve)(ref startLifetime)).constantMin);
				((MinMaxCurve)(ref startLifetime)).constantMax = Mathf.Max(initLifeValuesNewSystem[j] * power + powerVariation * 0.5f, ((MinMaxCurve)(ref startLifetime)).constantMax);
				((MinMaxCurve)(ref startLifetime)).mode = (ParticleSystemCurveMode)3;
				((MainModule)(ref main)).startLifetime = startLifetime;
				MinMaxCurve startSize = ((MainModule)(ref main)).startSize;
				((MinMaxCurve)(ref startSize)).constantMin = Mathf.Max(initSizeValuesNewSystem[j] * power - powerVariation * 0.5f, ((MinMaxCurve)(ref startSize)).constantMin);
				((MinMaxCurve)(ref startSize)).constantMax = Mathf.Max(initSizeValuesNewSystem[j] * power + powerVariation * 0.5f, ((MinMaxCurve)(ref startSize)).constantMax);
				((MinMaxCurve)(ref startSize)).mode = (ParticleSystemCurveMode)3;
				((MainModule)(ref main)).startSize = startSize;
			}
			if (sfx != null && audio != null)
			{
				audio.pitch = Mathf.Max(pwr, audio.pitch);
			}
		}
	}

	public void Unlatch()
	{
		if (valid && activeLatch)
		{
			setActive(value: false);
			activeLatch = false;
			Power = power;
		}
	}
}
