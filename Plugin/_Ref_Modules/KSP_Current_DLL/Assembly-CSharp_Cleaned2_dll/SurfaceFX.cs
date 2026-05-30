using System;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceFX : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem srfCloud;

	private MainModule srfCloudMain;

	private Particle[] cloudParticles;

	private int cloudCount;

	[SerializeField]
	private ParticleSystem srfDust;

	private MainModule srfDustMain;

	private Particle[] dustParticles;

	private int dustCount;

	[SerializeField]
	private ParticleSystem srfWake;

	private MainModule srfWakeMain;

	private int wakeCount;

	private float fxScale = 0.5f;

	[SerializeField]
	private float pushThreshold = 0.5f;

	[SerializeField]
	private float pushScale = 5f;

	[SerializeField]
	private float linger = 10f;

	private List<ModuleSurfaceFX> sources = new List<ModuleSurfaceFX>();

	public GameObject prefab;

	private bool atmosphere;

	private static float mergeThreshold = 5f;

	private static List<SurfaceFX> fxs;

	private Transform trf;

	private Vector3 Vsrf;

	private Vector3 upAxis;

	private Vector3 barycenter;

	private float lastUpdate;

	public ModuleSurfaceFX leadSource { get; private set; }

	public float ScaledFX => fxScale;

	public static SurfaceFX FindNearestFX(ModuleSurfaceFX src, Vector3 wPos)
	{
		if (fxs == null)
		{
			fxs = new List<SurfaceFX>();
		}
		SurfaceFX result = null;
		float num = mergeThreshold;
		int count = fxs.Count;
		while (count-- > 0)
		{
			if (!(fxs[count].leadSource == src) && (fxs[count].trf.position - wPos).sqrMagnitude < num)
			{
				result = fxs[count];
			}
		}
		return result;
	}

	public void Terminate()
	{
		fxs.Remove(this);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private void Awake()
	{
		trf = base.transform;
		fxs.Add(this);
	}

	private void OnDestroy()
	{
		fxs.Remove(this);
		FloatingOrigin.UnregisterParticleSystem(srfCloud);
		FloatingOrigin.UnregisterParticleSystem(srfDust);
		FloatingOrigin.UnregisterParticleSystem(srfWake);
	}

	public void AddSource(ModuleSurfaceFX src)
	{
		if (sources.Count == 0)
		{
			leadSource = src;
		}
		sources.AddUnique(src);
	}

	public void RemoveSource(ModuleSurfaceFX src)
	{
		sources.Remove(src);
		if (src == leadSource)
		{
			if (sources.Count != 0)
			{
				leadSource = sources[0];
			}
			else
			{
				leadSource = null;
			}
		}
	}

	private void Start()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_0099: Unknown result type (might be due to invalid IL or missing references)
		MainModule main = srfCloud.main;
		cloudParticles = (Particle[])(object)new Particle[((MainModule)(ref main)).maxParticles * 2];
		main = srfDust.main;
		dustParticles = (Particle[])(object)new Particle[((MainModule)(ref main)).maxParticles * 2];
		upAxis = FlightGlobals.ActiveVessel.upAxis;
		barycenter = Vector3.zero;
		Vsrf = Vector3.zero;
		srfCloudMain = srfCloud.main;
		srfWakeMain = srfWake.main;
		srfDustMain = srfDust.main;
		FloatingOrigin.RegisterParticleSystem(srfCloud);
		FloatingOrigin.RegisterParticleSystem(srfDust);
		FloatingOrigin.RegisterParticleSystem(srfWake);
	}

	private void LateUpdate()
	{
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0415: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_043f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0464: Unknown result type (might be due to invalid IL or missing references)
		//IL_0469: Unknown result type (might be due to invalid IL or missing references)
		//IL_044f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_0219: Unknown result type (might be due to invalid IL or missing references)
		//IL_021e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0247: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0233: Unknown result type (might be due to invalid IL or missing references)
		//IL_0205: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_032a: Unknown result type (might be due to invalid IL or missing references)
		//IL_032f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_039b: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ee: Unknown result type (might be due to invalid IL or missing references)
		fxScale = 0f;
		int count = sources.Count;
		while (count-- > 0)
		{
			lastUpdate = Time.realtimeSinceStartup;
			fxScale = Mathf.Clamp01(fxScale + sources[count].ScaledFX);
		}
		if (sources.Count > 0)
		{
			atmosphere = leadSource.part.vessel.mainBody.atmosphere;
			if (sources.Count > 1)
			{
				barycenter = GetWeightedAvgVector((int i) => sources[i].point, (int i) => sources[i].ScaledFX);
				upAxis = GetWeightedAvgVector((int i) => sources[i].normal, (int i) => sources[i].ScaledFX);
				Vsrf = GetWeightedAvgVector((int i) => sources[i].Vsrf, (int i) => sources[i].ScaledFX) * pushScale;
			}
			else
			{
				barycenter = leadSource.point;
				upAxis = leadSource.normal;
				Vsrf = leadSource.Vsrf;
			}
			trf.position = barycenter;
			trf.rotation = Quaternion.LookRotation(Vsrf, upAxis);
		}
		EmissionModule emission;
		if (Time.realtimeSinceStartup > lastUpdate + linger)
		{
			Terminate();
		}
		else if (fxScale > 0f)
		{
			if (!srfCloud.isPlaying && atmosphere)
			{
				srfCloud.Play();
			}
			if (!srfDust.isPlaying)
			{
				srfDust.Play();
			}
			if (!srfWake.isPlaying)
			{
				srfWake.Play();
			}
			emission = srfCloud.emission;
			if (!((EmissionModule)(ref emission)).enabled && atmosphere)
			{
				EmissionModule emission2 = srfCloud.emission;
				((EmissionModule)(ref emission2)).enabled = true;
			}
			emission = srfDust.emission;
			if (!((EmissionModule)(ref emission)).enabled)
			{
				EmissionModule emission3 = srfDust.emission;
				((EmissionModule)(ref emission3)).enabled = true;
			}
			emission = srfWake.emission;
			if (!((EmissionModule)(ref emission)).enabled)
			{
				EmissionModule emission4 = srfWake.emission;
				((EmissionModule)(ref emission4)).enabled = true;
			}
			MinMaxGradient startColor;
			MinMaxCurve startLifetime;
			if (atmosphere)
			{
				ref MainModule reference = ref srfCloudMain;
				startColor = ((MainModule)(ref srfCloudMain)).startColor;
				((MainModule)(ref reference)).startColor = MinMaxGradient.op_Implicit(((MinMaxGradient)(ref startColor)).color.smethod_0(Mathf.Lerp(0f, 0.5f, fxScale)));
				cloudCount = srfCloud.GetParticles(cloudParticles);
				int num = cloudCount;
				while (num-- > 0)
				{
					ref Particle p = ref cloudParticles[num];
					startLifetime = ((MainModule)(ref srfCloudMain)).startLifetime;
					UpdateParticle(ref p, 1f / ((MinMaxCurve)(ref startLifetime)).constant);
				}
				srfCloud.SetParticles(cloudParticles, cloudCount);
			}
			ref MainModule reference2 = ref srfDustMain;
			startColor = ((MainModule)(ref srfDustMain)).startColor;
			((MainModule)(ref reference2)).startColor = MinMaxGradient.op_Implicit(((MinMaxGradient)(ref startColor)).color.smethod_0(Mathf.Lerp(0f, 0.5f, fxScale)));
			dustCount = srfDust.GetParticles(dustParticles);
			int num2 = dustCount;
			while (num2-- > 0)
			{
				ref Particle p2 = ref dustParticles[num2];
				startLifetime = ((MainModule)(ref srfDustMain)).startLifetime;
				UpdateParticle(ref p2, 1f / ((MinMaxCurve)(ref startLifetime)).constant);
			}
			srfDust.SetParticles(dustParticles, dustCount);
			ref MainModule reference3 = ref srfWakeMain;
			startColor = ((MainModule)(ref srfWakeMain)).startColor;
			((MainModule)(ref reference3)).startColor = MinMaxGradient.op_Implicit(((MinMaxGradient)(ref startColor)).color.smethod_0(Mathf.Lerp(0f, 0.5f, fxScale)));
		}
		else
		{
			emission = srfCloud.emission;
			if (((EmissionModule)(ref emission)).enabled)
			{
				EmissionModule emission5 = srfCloud.emission;
				((EmissionModule)(ref emission5)).enabled = false;
			}
			emission = srfDust.emission;
			if (((EmissionModule)(ref emission)).enabled)
			{
				EmissionModule emission6 = srfDust.emission;
				((EmissionModule)(ref emission6)).enabled = false;
			}
			emission = srfWake.emission;
			if (((EmissionModule)(ref emission)).enabled)
			{
				EmissionModule emission7 = srfWake.emission;
				((EmissionModule)(ref emission7)).enabled = false;
			}
		}
	}

	private void UpdateParticle(ref Particle p, float lifeTimeThreshold)
	{
		((Particle)(ref p)).velocity = ((Particle)(ref p)).velocity + Vsrf * Mathf.Pow(((Particle)(ref p)).remainingLifetime * lifeTimeThreshold, pushThreshold);
	}

	private Vector3 GetWeightedAvgVector(Func<int, Vector3> getVector, Func<int, float> getWeight)
	{
		Vector3 zero = Vector3.zero;
		float num = 0f;
		int count = sources.Count;
		while (count-- > 0)
		{
			if (sources[count] == null)
			{
				sources.RemoveAt(count);
				continue;
			}
			float num2 = getWeight(count);
			zero += getVector(count) * num2;
			num += num2;
		}
		if (num != 0f)
		{
			return zero / num;
		}
		return zero;
	}
}
