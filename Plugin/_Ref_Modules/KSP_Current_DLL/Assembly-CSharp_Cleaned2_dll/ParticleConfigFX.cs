using System;
using UnityEngine;

[EffectDefinition("PARTICLE_CONFIG")]
public class ParticleConfigFX : EffectBehaviour
{
	[Serializable]
	public class PFXMaterial
	{
		public enum MaterialType
		{
			Additive,
			AlphaBlended
		}

		[Persistent]
		public MaterialType type;

		[Persistent]
		public Color color = new Color(1f, 1f, 1f, 1f);

		[Persistent]
		public string texture = "";

		public Material CreateMaterial()
		{
			Material material = null;
			switch (type)
			{
			case MaterialType.AlphaBlended:
				material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
				break;
			case MaterialType.Additive:
				material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
				break;
			}
			material.color = color;
			Texture2D texture2D = GameDatabase.Instance.GetTexture(texture, asNormalMap: false);
			if (texture2D != null)
			{
				material.mainTexture = texture2D;
			}
			else
			{
				Debug.LogError("Cannot assign texture to ParticleFX");
			}
			return material;
		}
	}

	public GameObject pHost;

	public ParticleSystem ps;

	[Persistent]
	public float power;

	[Persistent]
	public bool useWorldSpace = true;

	[Persistent]
	public Vector3 ellipsoid = Vector3.one;

	public FXCurve minSize = new FXCurve("minSize", 0.1f);

	public FXCurve maxSize = new FXCurve("maxSize", 0.1f);

	public FXCurve minEnergy = new FXCurve("minEnergy", 2f);

	public FXCurve maxEnergy = new FXCurve("maxEnergy", 3f);

	public FXCurve minEmission = new FXCurve("minEmission", 80f);

	public FXCurve maxEmission = new FXCurve("maxEmission", 100f);

	[Persistent]
	public Vector3 worldVelocity = Vector3.zero;

	[Persistent]
	public Vector3 localVelocity = Vector3.zero;

	[Persistent]
	public Vector3 rndVelocity = Vector3.zero;

	[Persistent]
	public float emitterVelocityScale = 0.05f;

	[Persistent]
	public Vector3 tangentVelocity = Vector3.zero;

	[Persistent]
	public float angularVelocity;

	[Persistent]
	public float rndAngularVelocity = 0.05f;

	[Persistent]
	public bool rndRotation;

	[Persistent]
	public bool oneShot;

	[Persistent]
	public bool doesAnimateColor = true;

	[Persistent]
	public Color[] colorAnimation = new Color[5]
	{
		new Color(1f, 1f, 1f, dC * 10f),
		new Color(1f, 1f, 1f, dC * 180f),
		new Color(1f, 1f, 1f, 1f),
		new Color(1f, 1f, 1f, dC * 180f),
		new Color(1f, 1f, 1f, dC * 10f)
	};

	private static float dC = 0.003921569f;

	[Persistent]
	public Vector3 worldRotationAxis = Vector3.zero;

	[Persistent]
	public Vector3 localRotationAxis = Vector3.zero;

	public FXCurve sizeGrow = new FXCurve("sizeGrow", 0f);

	[Persistent]
	public Vector3 rndForce = Vector3.zero;

	[Persistent]
	public Vector3 force = Vector3.zero;

	[Persistent]
	public float damping = 1f;

	[Persistent]
	public bool autodestruct;

	[Persistent]
	public bool castShadows;

	[Persistent]
	public bool recieveShadows;

	public FXCurve lengthScale = new FXCurve("lengthScale", 0f);

	[Persistent]
	public float velocityScale;

	[Persistent]
	public float maxParticleSize = 0.25f;

	[Persistent]
	public ParticleSystemRenderMode particleRenderModeNewSystem;

	[Persistent]
	public int uvAnimationXTile = 1;

	[Persistent]
	public int uvAnimationYTile = 1;

	[Persistent]
	public int uvAnimationCycles = 1;

	[Persistent]
	public PFXMaterial material = new PFXMaterial();

	private void InitializeComponents()
	{
		power = 0f;
		if ((UnityEngine.Object)(object)ps == null)
		{
			ps = pHost.AddComponent<ParticleSystem>();
		}
		InitializeParticleSystem();
		UpdateComponents();
	}

	private void InitializeParticleSystem()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_0085: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0280: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0294: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0394: Unknown result type (might be due to invalid IL or missing references)
		//IL_039d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_026e: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0410: Unknown result type (might be due to invalid IL or missing references)
		//IL_0454: Unknown result type (might be due to invalid IL or missing references)
		MainModule main = ps.main;
		ParticleSystemRenderer component = pHost.GetComponent<ParticleSystemRenderer>();
		VelocityOverLifetimeModule velocityOverLifetime = ps.velocityOverLifetime;
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).enabled = true;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)(useWorldSpace ? 1 : 0);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).x = MinMaxCurve.op_Implicit(localVelocity.x);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).y = MinMaxCurve.op_Implicit(localVelocity.y);
		((VelocityOverLifetimeModule)(ref velocityOverLifetime)).z = MinMaxCurve.op_Implicit(localVelocity.z);
		MinMaxCurve z = default(MinMaxCurve);
		((MinMaxCurve)(ref z)).constantMin = angularVelocity / 57.29578f;
		((MinMaxCurve)(ref z)).constantMax = (angularVelocity + rndAngularVelocity) / 57.29578f;
		((MinMaxCurve)(ref z)).mode = (ParticleSystemCurveMode)3;
		RotationOverLifetimeModule rotationOverLifetime = ps.rotationOverLifetime;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).enabled = true;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).z = z;
		if (doesAnimateColor)
		{
			ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
			((ColorOverLifetimeModule)(ref colorOverLifetime)).enabled = true;
			Gradient gradient = new Gradient();
			gradient.SetKeys(new GradientColorKey[5]
			{
				new GradientColorKey(colorAnimation[0], 0f),
				new GradientColorKey(colorAnimation[1], 0.25f),
				new GradientColorKey(colorAnimation[2], 0.5f),
				new GradientColorKey(colorAnimation[3], 0.75f),
				new GradientColorKey(colorAnimation[4], 1f)
			}, new GradientAlphaKey[5]
			{
				new GradientAlphaKey(colorAnimation[0].a, 0f),
				new GradientAlphaKey(colorAnimation[1].a, 0.25f),
				new GradientAlphaKey(colorAnimation[2].a, 0.5f),
				new GradientAlphaKey(colorAnimation[3].a, 0.75f),
				new GradientAlphaKey(colorAnimation[4].a, 1f)
			});
			MinMaxGradient color = default(MinMaxGradient);
			((MinMaxGradient)(ref color))._002Ector(gradient);
			((ColorOverLifetimeModule)(ref colorOverLifetime)).color = color;
		}
		ForceOverLifetimeModule forceOverLifetime = ps.forceOverLifetime;
		((ForceOverLifetimeModule)(ref forceOverLifetime)).enabled = true;
		MinMaxCurve x = default(MinMaxCurve);
		MinMaxCurve y = default(MinMaxCurve);
		MinMaxCurve z2 = default(MinMaxCurve);
		((MinMaxCurve)(ref x)).mode = (ParticleSystemCurveMode)3;
		((MinMaxCurve)(ref y)).mode = (ParticleSystemCurveMode)3;
		((MinMaxCurve)(ref z2)).mode = (ParticleSystemCurveMode)3;
		((MinMaxCurve)(ref x)).constantMin = force.x - rndForce.x / 2f;
		((MinMaxCurve)(ref y)).constantMin = force.y - rndForce.y / 2f;
		((MinMaxCurve)(ref z2)).constantMin = force.z - rndForce.z / 2f;
		((MinMaxCurve)(ref x)).constantMax = force.x + rndForce.x / 2f;
		((MinMaxCurve)(ref y)).constantMax = force.y + rndForce.y / 2f;
		((MinMaxCurve)(ref z2)).constantMax = force.z + rndForce.z / 2f;
		((ForceOverLifetimeModule)(ref forceOverLifetime)).x = x;
		((ForceOverLifetimeModule)(ref forceOverLifetime)).y = y;
		((ForceOverLifetimeModule)(ref forceOverLifetime)).z = z2;
		LimitVelocityOverLifetimeModule limitVelocityOverLifetime = ps.limitVelocityOverLifetime;
		((LimitVelocityOverLifetimeModule)(ref limitVelocityOverLifetime)).enabled = true;
		((LimitVelocityOverLifetimeModule)(ref limitVelocityOverLifetime)).dampen = damping;
		component.renderMode = particleRenderModeNewSystem;
		component.velocityScale = velocityScale;
		component.maxParticleSize = maxParticleSize;
		if (uvAnimationXTile != 1 || uvAnimationYTile != 1)
		{
			TextureSheetAnimationModule textureSheetAnimation = ps.textureSheetAnimation;
			((TextureSheetAnimationModule)(ref textureSheetAnimation)).enabled = true;
			((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesX = uvAnimationXTile;
			((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesY = uvAnimationYTile;
			((TextureSheetAnimationModule)(ref textureSheetAnimation)).frameOverTime = new MinMaxCurve(1f, AnimationCurve.Linear(0f, 0f, 1f, 1f));
		}
		((Renderer)(object)component).material = material.CreateMaterial();
	}

	private void UpdateComponents()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_004e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		MainModule main = ps.main;
		MinMaxCurve startSize = ((MainModule)(ref main)).startSize;
		((MinMaxCurve)(ref startSize)).constantMin = minSize.Value(power);
		((MinMaxCurve)(ref startSize)).constantMax = maxSize.Value(power);
		((MinMaxCurve)(ref startSize)).mode = (ParticleSystemCurveMode)3;
		((MainModule)(ref main)).startSize = startSize;
		MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
		((MinMaxCurve)(ref startLifetime)).constantMin = minEnergy.Value(power);
		((MinMaxCurve)(ref startLifetime)).constantMax = maxEnergy.Value(power);
		((MinMaxCurve)(ref startLifetime)).mode = (ParticleSystemCurveMode)3;
		((MainModule)(ref main)).startLifetime = startLifetime;
		SizeOverLifetimeModule sizeOverLifetime = ps.sizeOverLifetime;
		((SizeOverLifetimeModule)(ref sizeOverLifetime)).enabled = true;
		float valueEnd = (1f + sizeGrow.Value(power)) * maxEnergy.Value(power);
		((SizeOverLifetimeModule)(ref sizeOverLifetime)).size = new MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, valueEnd));
		pHost.GetComponent<ParticleSystemRenderer>().lengthScale = lengthScale.Value(power);
	}

	public override void OnLoad(ConfigNode node)
	{
		ConfigNode.LoadObjectFromConfig(this, node);
		string value = node.GetValue("transform");
		if (value != null)
		{
			Transform transform = KSPUtil.FindInPartModel(hostPart.transform, value);
			if (transform != null)
			{
				pHost = transform.gameObject;
			}
		}
		minSize.Load("minSize", node);
		maxSize.Load("maxSize", node);
		minEnergy.Load("minEnergy", node);
		maxEnergy.Load("maxEnergy", node);
		minEmission.Load("minEmission", node);
		maxEmission.Load("maxEmission", node);
	}

	public override void OnSave(ConfigNode node)
	{
		ConfigNode configNode = ConfigNode.CreateConfigFromObject(this);
		configNode.name = "PARTICLE";
		configNode.CopyTo(node);
		minSize.Save(node);
		maxSize.Save(node);
		minEnergy.Save(node);
		maxEnergy.Save(node);
		minEmission.Save(node);
		maxEmission.Save(node);
	}

	public override void OnInitialize()
	{
		InitializeComponents();
	}

	public override void OnEvent(float power)
	{
		if (!oneShot)
		{
			power = Mathf.Clamp01(power);
			UpdateComponents();
		}
	}

	public override void OnEvent()
	{
		if (oneShot)
		{
			power = 1f;
			UpdateComponents();
			ps.Emit(Mathf.FloorToInt(UnityEngine.Random.Range(minEmission, maxEmission)));
		}
	}
}
