using System;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[AddComponentMenu("KSP/Particle Emitter")]
public class KSPParticleEmitter : MonoBehaviour
{
	public enum EmissionShape
	{
		Ellipsoid,
		Ellipse,
		Sphere,
		Ring,
		Cuboid,
		Plane,
		Line,
		Point
	}

	[SerializeField]
	public ParticleSystem ps;

	public bool emit;

	public EmissionShape shape;

	public Vector3 shape3D;

	public Vector2 shape2D;

	public float shape1D;

	public Color color;

	public bool useWorldSpace;

	public float minSize;

	public float maxSize;

	public float minEnergy;

	public float maxEnergy;

	public int minEmission;

	public int maxEmission;

	public Vector3 worldVelocity;

	public Vector3 localVelocity;

	public Vector3 rndVelocity;

	public float emitterVelocityScale;

	public float angularVelocity;

	public float rndAngularVelocity;

	public bool rndRotation;

	public bool doesAnimateColor;

	public Color[] colorAnimation;

	public Vector3 worldRotationAxis;

	public Vector3 localRotationAxis;

	public float sizeGrow;

	public Vector3 rndForce;

	public Vector3 force;

	public float damping;

	public bool castShadows;

	public bool recieveShadows;

	public float lengthScale;

	public float velocityScale;

	public float maxParticleSize;

	public ParticleSystemRenderMode particleRenderMode;

	public int uvAnimationXTile;

	public int uvAnimationYTile;

	public int uvAnimationCycles;

	public Material material;

	private bool dirty = true;

	private int framesAlive;

	private float particlesThisFrame;

	private float amountPerSec;

	private float overflow;

	private int itr;

	private Vector3 position;

	private float posFloat;

	private Vector3 velocity;

	private float rotation;

	private void EnsureParticleSystemCreated()
	{
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		if ((UnityEngine.Object)(object)ps == null)
		{
			ps = base.gameObject.GetComponent<ParticleSystem>();
			if ((UnityEngine.Object)(object)ps == null)
			{
				ps = base.gameObject.AddComponent<ParticleSystem>();
			}
		}
		EmissionModule emission = ps.emission;
		((EmissionModule)(ref emission)).enabled = false;
	}

	private void Reset()
	{
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		StopCoroutine("EmitCoroutine");
		emit = true;
		shape = EmissionShape.Ellipsoid;
		shape3D = Vector3.one;
		shape2D = Vector2.one;
		shape1D = 1f;
		color = Color.white;
		useWorldSpace = true;
		minSize = 0.1f;
		maxSize = 0.25f;
		minEnergy = 2f;
		maxEnergy = 3f;
		minEmission = 80;
		maxEmission = 100;
		worldVelocity = Vector3.zero;
		localVelocity = Vector3.zero;
		rndVelocity = Vector3.zero;
		emitterVelocityScale = 0.05f;
		angularVelocity = 0f;
		rndAngularVelocity = 0.05f;
		rndRotation = false;
		doesAnimateColor = true;
		colorAnimation = new Color[5]
		{
			new Color(1f, 1f, 1f, 2f / 51f),
			new Color(1f, 1f, 1f, 0.7058824f),
			new Color(1f, 1f, 1f, 1f),
			new Color(1f, 1f, 1f, 0.7058824f),
			new Color(1f, 1f, 1f, 2f / 51f)
		};
		worldRotationAxis = Vector3.zero;
		localRotationAxis = Vector3.zero;
		sizeGrow = 0f;
		rndForce = Vector3.zero;
		force = Vector3.zero;
		damping = 1f;
		castShadows = false;
		recieveShadows = false;
		lengthScale = 0f;
		velocityScale = 0f;
		maxParticleSize = 0.25f;
		particleRenderMode = (ParticleSystemRenderMode)0;
		uvAnimationXTile = 1;
		uvAnimationYTile = 1;
		uvAnimationCycles = 1;
		EnsureParticleSystemCreated();
		SetDirty();
	}

	private void Awake()
	{
		if (GetComponent<KSPParticleEmitter>() != this)
		{
			Debug.LogError("Cannot have more than one KSPParticleEmitter on a single GameObject");
			UnityEngine.Object.DestroyImmediate(this);
		}
		else
		{
			EnsureParticleSystemCreated();
			SetDirty();
		}
	}

	public void SetDirty()
	{
		dirty = true;
	}

	public void SetupProperties()
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0239: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0445: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		if (!dirty)
		{
			return;
		}
		if ((UnityEngine.Object)(object)ps == null)
		{
			EnsureParticleSystemCreated();
			if ((UnityEngine.Object)(object)ps == null)
			{
				Debug.Log("[KSPParticleEmitter]: Cannot Setup, ParticleSystem is null.");
				return;
			}
		}
		MainModule main = ps.main;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)(useWorldSpace ? 1 : 0);
		RotationOverLifetimeModule rotationOverLifetime = ps.rotationOverLifetime;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).enabled = true;
		MinMaxCurve z = default(MinMaxCurve);
		((MinMaxCurve)(ref z)).constantMin = angularVelocity / 57.29578f;
		((MinMaxCurve)(ref z)).constantMax = (angularVelocity + rndAngularVelocity) / 57.29578f;
		((MinMaxCurve)(ref z)).mode = (ParticleSystemCurveMode)3;
		((RotationOverLifetimeModule)(ref rotationOverLifetime)).z = z;
		ColorOverLifetimeModule colorOverLifetime = ps.colorOverLifetime;
		((ColorOverLifetimeModule)(ref colorOverLifetime)).enabled = doesAnimateColor;
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
		MinMaxGradient val = default(MinMaxGradient);
		((MinMaxGradient)(ref val))._002Ector(gradient);
		((ColorOverLifetimeModule)(ref colorOverLifetime)).color = val;
		SizeOverLifetimeModule sizeOverLifetime = ps.sizeOverLifetime;
		((SizeOverLifetimeModule)(ref sizeOverLifetime)).enabled = true;
		float p = Mathf.Lerp(minEnergy, maxEnergy, 0.5f);
		float valueEnd = 1f * Mathf.Pow(1f + sizeGrow, p);
		((SizeOverLifetimeModule)(ref sizeOverLifetime)).size = new MinMaxCurve(1f, AnimationCurve.EaseInOut(0f, 1f, 1f, valueEnd));
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
		ParticleSystemRenderer component = ((Component)(object)ps).GetComponent<ParticleSystemRenderer>();
		if (castShadows)
		{
			((Renderer)(object)component).shadowCastingMode = ShadowCastingMode.On;
		}
		else
		{
			((Renderer)(object)component).shadowCastingMode = ShadowCastingMode.Off;
		}
		((Renderer)(object)component).receiveShadows = recieveShadows;
		component.lengthScale = lengthScale;
		component.velocityScale = velocityScale;
		component.maxParticleSize = maxParticleSize;
		component.renderMode = particleRenderMode;
		if (uvAnimationXTile != 1 || uvAnimationYTile != 1)
		{
			TextureSheetAnimationModule textureSheetAnimation = ps.textureSheetAnimation;
			((TextureSheetAnimationModule)(ref textureSheetAnimation)).enabled = true;
			((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesX = uvAnimationXTile;
			((TextureSheetAnimationModule)(ref textureSheetAnimation)).numTilesY = uvAnimationYTile;
			((TextureSheetAnimationModule)(ref textureSheetAnimation)).frameOverTime = new MinMaxCurve(1f, AnimationCurve.Linear(0f, 0f, 1f, 1f));
		}
		((Renderer)(object)component).material = material;
		dirty = false;
	}

	private void Update()
	{
		framesAlive++;
		if (framesAlive < 2)
		{
			return;
		}
		SetupProperties();
		if (maxEmission < 0 || !emit)
		{
			return;
		}
		if (amountPerSec == 0f)
		{
			amountPerSec = UnityEngine.Random.Range(minEmission, maxEmission);
		}
		particlesThisFrame = amountPerSec * Time.deltaTime + overflow;
		if (particlesThisFrame < 1f)
		{
			overflow += particlesThisFrame;
			return;
		}
		itr = (int)particlesThisFrame;
		particlesThisFrame -= itr;
		for (overflow = 0f; itr >= 0; itr--)
		{
			EmitParticle();
		}
		amountPerSec = UnityEngine.Random.Range(minEmission, maxEmission);
	}

	public void EmitParticle()
	{
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_024f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		if ((UnityEngine.Object)(object)ps == null)
		{
			EnsureParticleSystemCreated();
			if ((UnityEngine.Object)(object)ps == null)
			{
				Debug.Log("[KSPParticleEmitter]: Cannot Emit, ParticleSystem is null.");
				return;
			}
		}
		switch (shape)
		{
		case EmissionShape.Ellipsoid:
			position = UnityEngine.Random.insideUnitSphere;
			position.Scale(shape3D);
			break;
		case EmissionShape.Ellipse:
			position = UnityEngine.Random.insideUnitCircle;
			position.x *= shape2D.x;
			position.z = position.y * shape2D.y;
			position.y = 0f;
			break;
		case EmissionShape.Sphere:
			position = UnityEngine.Random.insideUnitSphere * shape1D;
			break;
		case EmissionShape.Ring:
			posFloat = UnityEngine.Random.Range(0f, (float)Math.PI * 2f);
			position = new Vector3(Mathf.Sin(posFloat) * shape2D.x, 0f, Mathf.Cos(posFloat) * shape2D.y);
			break;
		case EmissionShape.Cuboid:
			position = new Vector3(UnityEngine.Random.Range(0f - shape3D.x, shape3D.x), UnityEngine.Random.Range(0f - shape3D.y, shape3D.y), UnityEngine.Random.Range(0f - shape3D.z, shape3D.z));
			break;
		case EmissionShape.Plane:
			position = new Vector3(UnityEngine.Random.Range(0f - shape2D.x, shape2D.x), 0f, UnityEngine.Random.Range(0f - shape2D.y, shape2D.y));
			break;
		case EmissionShape.Line:
			position = new Vector3(UnityEngine.Random.Range(0f - shape1D, shape1D) * 0.5f, 0f, 0f);
			break;
		case EmissionShape.Point:
			position = Vector3.zero;
			break;
		}
		MainModule main = ps.main;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)(useWorldSpace ? 1 : 0);
		if (useWorldSpace)
		{
			position = base.transform.TransformPoint(position);
			velocity = worldVelocity + base.transform.TransformDirection(localVelocity + new Vector3(UnityEngine.Random.Range(0f - rndVelocity.x, rndVelocity.x), UnityEngine.Random.Range(0f - rndVelocity.y, rndVelocity.y), UnityEngine.Random.Range(0f - rndVelocity.z, rndVelocity.z)));
		}
		else
		{
			velocity = localVelocity + new Vector3(UnityEngine.Random.Range(0f - rndVelocity.x, rndVelocity.x), UnityEngine.Random.Range(0f - rndVelocity.y, rndVelocity.y), UnityEngine.Random.Range(0f - rndVelocity.z, rndVelocity.z)) + base.transform.InverseTransformDirection(worldVelocity);
		}
		rotation = (rndRotation ? (UnityEngine.Random.value * 360f) : 0f);
		if (framesAlive > 2)
		{
			EmitParams val = default(EmitParams);
			((EmitParams)(ref val)).position = position;
			((EmitParams)(ref val)).velocity = velocity;
			((EmitParams)(ref val)).startSize = UnityEngine.Random.Range(minSize, maxSize);
			((EmitParams)(ref val)).startLifetime = UnityEngine.Random.Range(minEnergy, maxEnergy);
			if (doesAnimateColor)
			{
				((EmitParams)(ref val)).startColor = new Color(color.r, color.g, color.b, 1f);
			}
			else
			{
				((EmitParams)(ref val)).startColor = new Color(color.r, color.g, color.b, color.a);
			}
			((EmitParams)(ref val)).rotation = rotation;
			if (Application.isPlaying)
			{
				ps.Emit(val, 1);
			}
			else
			{
				ps.Simulate(Time.fixedDeltaTime, true, false, true);
			}
		}
	}

	public void Emit()
	{
		SetupProperties();
		int num = UnityEngine.Random.Range(minEmission, maxEmission);
		for (int i = 0; i < num; i++)
		{
			EmitParticle();
		}
	}
}
