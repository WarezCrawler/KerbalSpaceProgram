using System.Collections.Generic;
using UnityEngine;

[EffectDefinition("PREFAB_MULTI_PARTICLE")]
public class PrefabMultiParticleFX : EffectBehaviour
{
	[Persistent]
	public string prefabName = "";

	[Persistent]
	public string transformName = "";

	public FXCurve emission = new FXCurve("emission", 1f);

	public FXCurve energy = new FXCurve("energy", 1f);

	public FXCurve speed = new FXCurve("speed", 1f);

	[Persistent]
	public Vector3 localOffset = Vector3.zero;

	[Persistent]
	public Vector4 localRotation = Vector4.zero;

	[Persistent]
	public Vector3 localScale = Vector3.one;

	[Persistent]
	public bool oneShot;

	private List<Transform> modelParents;

	private List<ParticleSystem> emitters;

	private ParticleSystem emitter;

	private float minEmission;

	private float maxEmission;

	private float minEnergy;

	private float maxEnergy;

	private Vector3 velocity;

	private float emissionPower;

	private float energyPower;

	private float speedPower;

	public override void OnLoad(ConfigNode node)
	{
		ConfigNode.LoadObjectFromConfig(this, node);
		emission.Load("emission", node);
		energy.Load("energy", node);
		speed.Load("speed", node);
	}

	public override void OnSave(ConfigNode node)
	{
		ConfigNode.CreateConfigFromObject(this, node);
		emission.Save(node);
		energy.Save(node);
		speed.Save(node);
	}

	public override void OnInitialize()
	{
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_0157: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_0175: Unknown result type (might be due to invalid IL or missing references)
		//IL_0178: Unknown result type (might be due to invalid IL or missing references)
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d6: Unknown result type (might be due to invalid IL or missing references)
		modelParents = new List<Transform>(hostPart.FindModelTransforms(transformName));
		if (modelParents.Count == 0)
		{
			Debug.LogError("PrefabParticleFX: Cannot find transform of name '" + transformName + "'");
			return;
		}
		Object @object = Resources.Load("Effects/" + prefabName);
		if (@object == null)
		{
			Debug.LogError("PrefabParticleFX: Cannot find prefab of name '" + prefabName + "'");
			return;
		}
		GameObject gameObject = (GameObject)Object.Instantiate(@object);
		if (gameObject == null)
		{
			Debug.LogError("PrefabParticleFX: Cannot find prefab of name '" + prefabName + "'");
			return;
		}
		gameObject.SetActive(value: true);
		emitter = gameObject.GetComponentInChildren<ParticleSystem>();
		if ((Object)(object)emitter == null)
		{
			Debug.LogError("PrefabParticleFX: Cannot find particle emitter on model of name '" + prefabName + "'");
			Object.Destroy(gameObject);
			return;
		}
		EmissionModule val = emitter.emission;
		MinMaxCurve val2;
		if (((EmissionModule)(ref val)).burstCount > 0)
		{
			Burst[] array = (Burst[])(object)new Burst[((EmissionModule)(ref val)).burstCount];
			minEmission = ((Burst)(ref array[0])).minCount;
			maxEmission = ((Burst)(ref array[0])).maxCount;
		}
		else
		{
			val2 = ((EmissionModule)(ref val)).rateOverTime;
			minEmission = ((MinMaxCurve)(ref val2)).constant;
			val2 = ((EmissionModule)(ref val)).rateOverTime;
			maxEmission = ((MinMaxCurve)(ref val2)).constant;
		}
		MainModule main = emitter.main;
		MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
		minEnergy = ((MinMaxCurve)(ref startLifetime)).constantMin;
		maxEnergy = ((MinMaxCurve)(ref startLifetime)).constantMax;
		VelocityOverLifetimeModule velocityOverLifetime = emitter.velocityOverLifetime;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).enabled = true;
		val2 = ((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x;
		float constant = ((MinMaxCurve)(ref val2)).constant;
		val2 = ((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y;
		float constant2 = ((MinMaxCurve)(ref val2)).constant;
		val2 = ((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z;
		velocity = new Vector3(constant, constant2, ((MinMaxCurve)(ref val2)).constant);
		if (emitters == null)
		{
			emitters = new List<ParticleSystem>();
		}
		int i = 0;
		for (int count = modelParents.Count; i < count; i++)
		{
			GameObject gameObject2;
			if (i > 0)
			{
				gameObject2 = Object.Instantiate(gameObject);
				emitter = gameObject2.GetComponentInChildren<ParticleSystem>();
			}
			else
			{
				gameObject2 = gameObject;
			}
			emitters.Add(emitter);
			gameObject2.transform.NestToParent(modelParents[i]);
			gameObject2.transform.localPosition = localOffset;
			gameObject2.transform.localRotation = Quaternion.AngleAxis(localRotation.w, localRotation);
			gameObject2.transform.localScale = Vector3.Scale(gameObject2.transform.localScale, localScale);
			emitter.Stop();
			EffectBehaviour.AddParticleEmitter(emitter);
		}
	}

	public override void OnEvent(int transformIdx)
	{
		if (emitters == null)
		{
			return;
		}
		if (transformIdx > -1 && transformIdx < emitters.Count)
		{
			emitters[transformIdx].Play();
			return;
		}
		int i = 0;
		for (int count = emitters.Count; i < count; i++)
		{
			emitters[i].Play();
		}
	}

	public override void OnEvent(float power, int transformIdx)
	{
		if (emitters == null)
		{
			return;
		}
		if (power <= 0f)
		{
			if (transformIdx > -1 && transformIdx < emitters.Count)
			{
				emitters[transformIdx].Stop();
				return;
			}
			int i = 0;
			for (int count = emitters.Count; i < count; i++)
			{
				emitters[i].Stop();
			}
			return;
		}
		emissionPower = emission.Value(power);
		energyPower = energy.Value(power);
		speedPower = speed.Value(power);
		int minEmissionVal = Mathf.FloorToInt(minEmission * emissionPower);
		int maxEmissionVal = Mathf.FloorToInt(maxEmission * emissionPower);
		float minEnergyVal = minEnergy * energyPower;
		float maxEnergyVal = maxEnergy * energyPower;
		Vector3 localVelocityVal = velocity * speedPower;
		if (transformIdx > -1 && transformIdx < emitters.Count)
		{
			SetEmitter(transformIdx, minEmissionVal, maxEmissionVal, minEnergyVal, maxEnergyVal, localVelocityVal);
			return;
		}
		int j = 0;
		for (int count2 = emitters.Count; j < count2; j++)
		{
			SetEmitter(j, minEmissionVal, maxEmissionVal, minEnergyVal, maxEnergyVal, localVelocityVal);
		}
	}

	private void SetEmitter(int transformIdx, int minEmissionVal, int maxEmissionVal, float minEnergyVal, float maxEnergyVal, Vector3 localVelocityVal)
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_008b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		ParticleSystem obj = emitters[transformIdx];
		MainModule main = obj.main;
		((MainModule)(ref main)).duration = 1f;
		EmissionModule val = obj.emission;
		if (((EmissionModule)(ref val)).burstCount > 0)
		{
			((EmissionModule)(ref val)).SetBursts((Burst[])(object)new Burst[1]
			{
				new Burst(0f, (short)minEmissionVal, (short)maxEmissionVal)
			});
		}
		else
		{
			((EmissionModule)(ref val)).rateOverTime = MinMaxCurve.op_Implicit((float)minEmissionVal);
		}
		MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
		((MinMaxCurve)(ref startLifetime)).constantMin = minEnergyVal;
		((MinMaxCurve)(ref startLifetime)).constantMax = maxEnergyVal;
		((MinMaxCurve)(ref startLifetime)).mode = (ParticleSystemCurveMode)3;
		((MainModule)(ref main)).startLifetime = startLifetime;
		VelocityOverLifetimeModule velocityOverLifetime = obj.velocityOverLifetime;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).enabled = true;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = MinMaxCurve.op_Implicit(localVelocityVal.x);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = MinMaxCurve.op_Implicit(localVelocityVal.y);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = MinMaxCurve.op_Implicit(localVelocityVal.z);
		obj.Play();
	}

	private void OnDestroy()
	{
		if (emitters != null)
		{
			int i = 0;
			for (int count = emitters.Count; i < count; i++)
			{
				EffectBehaviour.RemoveParticleEmitter(emitter);
			}
		}
	}
}
