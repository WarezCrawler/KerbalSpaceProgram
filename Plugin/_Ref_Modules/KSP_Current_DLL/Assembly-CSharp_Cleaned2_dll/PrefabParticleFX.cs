using UnityEngine;

[EffectDefinition("PREFAB_PARTICLE")]
public class PrefabParticleFX : EffectBehaviour
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

	private Transform modelParent;

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
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025f: Unknown result type (might be due to invalid IL or missing references)
		modelParent = hostPart.FindModelTransform(transformName);
		if (modelParent == null)
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
		((Component)(object)emitter).transform.NestToParent(modelParent);
		((Component)(object)emitter).transform.localPosition = localOffset;
		((Component)(object)emitter).transform.localRotation = Quaternion.AngleAxis(localRotation.w, localRotation);
		((Component)(object)emitter).transform.localScale = Vector3.Scale(((Component)(object)emitter).transform.localScale, localScale);
		emitter.Stop();
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
		EffectBehaviour.AddParticleEmitter(emitter);
	}

	public override void OnEvent()
	{
		if (!((Object)(object)emitter == null))
		{
			emitter.Play();
		}
	}

	public override void OnEvent(float power)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0086: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0141: Unknown result type (might be due to invalid IL or missing references)
		//IL_0145: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)emitter == null)
		{
			return;
		}
		if (power <= 0f)
		{
			emitter.Stop();
			return;
		}
		EmissionModule val = emitter.emission;
		emissionPower = emission.Value(power);
		if (((EmissionModule)(ref val)).burstCount > 0)
		{
			((EmissionModule)(ref val)).SetBursts((Burst[])(object)new Burst[1]
			{
				new Burst(0f, (short)Mathf.FloorToInt(minEmission * emissionPower), (short)Mathf.FloorToInt(maxEmission * emissionPower))
			});
		}
		else
		{
			((EmissionModule)(ref val)).rateOverTime = MinMaxCurve.op_Implicit((float)Mathf.FloorToInt(minEmission * emissionPower));
		}
		energyPower = energy.Value(power);
		MainModule main = emitter.main;
		MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
		((MinMaxCurve)(ref startLifetime)).constantMin = minEnergy * energyPower;
		((MinMaxCurve)(ref startLifetime)).constantMax = maxEnergy * energyPower;
		((MinMaxCurve)(ref startLifetime)).mode = (ParticleSystemCurveMode)3;
		((MainModule)(ref main)).startLifetime = startLifetime;
		speedPower = speed.Value(power);
		VelocityOverLifetimeModule velocityOverLifetime = emitter.velocityOverLifetime;
		MinMaxCurve x = ((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x;
		MinMaxCurve y = ((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y;
		MinMaxCurve z = ((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z;
		((MinMaxCurve)(ref x)).constant = velocity.x * speedPower;
		((MinMaxCurve)(ref y)).constant = velocity.y * speedPower;
		((MinMaxCurve)(ref z)).constant = velocity.z * speedPower;
		emitter.Play();
	}

	private void OnDestroy()
	{
		EffectBehaviour.RemoveParticleEmitter(emitter);
	}
}
