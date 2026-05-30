using UnityEngine;

public class DetonatorBurstEmitter : DetonatorComponent
{
	private ParticleSystem _particleSystem;

	private float _baseDamping = 0.1300004f;

	private float _baseSize = 1f;

	private Color _baseColor = Color.white;

	public float damping = 1f;

	public float startRadius = 1f;

	public float maxScreenSize = 2f;

	public bool explodeOnAwake;

	public bool oneShot = true;

	public float sizeVariation;

	public float particleSize = 1f;

	public float count = 1f;

	public float sizeGrow = 20f;

	public bool exponentialGrowth = true;

	public float durationVariation;

	public bool useWorldSpace = true;

	public float upwardsBias;

	public float angularVelocity = 20f;

	public bool randomRotation = true;

	public ParticleSystemRenderMode renderModeNewSystem;

	public bool useExplicitColorAnimation;

	public Color[] colorAnimation = new Color[5];

	private bool _delayedExplosionStarted;

	private float _explodeDelay;

	public Material material;

	private float _emitTime;

	private float speed = 3f;

	private float initFraction = 0.1f;

	private static float epsilon = 0.01f;

	private float _tmpParticleSize;

	private Vector3 _tmpPos;

	private Vector3 _thisPos;

	private float _tmpDuration;

	private float _tmpCount;

	private float _scaledDuration;

	private float _scaledDurationVariation;

	private float _scaledStartRadius;

	private float _scaledColor;

	private float _tmpAngularVelocity;

	public override void Init()
	{
		MonoBehaviour.print("UNUSED");
	}

	public void Awake()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0092: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_009e: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		_particleSystem = base.gameObject.AddComponent<ParticleSystem>();
		((Object)(object)_particleSystem).hideFlags = HideFlags.HideAndDontSave;
		LimitVelocityOverLifetimeModule limitVelocityOverLifetime = _particleSystem.limitVelocityOverLifetime;
		((LimitVelocityOverLifetimeModule)(ref limitVelocityOverLifetime)).enabled = true;
		((LimitVelocityOverLifetimeModule)(ref limitVelocityOverLifetime)).dampen = _baseDamping;
		MainModule main = _particleSystem.main;
		((MainModule)(ref main)).playOnAwake = false;
		_particleSystem.Stop();
		ParticleSystemRenderer component = ((Component)(object)_particleSystem).GetComponent<ParticleSystemRenderer>();
		component.maxParticleSize = maxScreenSize;
		((Renderer)(object)component).material = material;
		((Renderer)(object)component).material.color = Color.white;
		MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
		SizeOverLifetimeModule sizeOverLifetime = _particleSystem.sizeOverLifetime;
		((SizeOverLifetimeModule)(ref sizeOverLifetime)).enabled = true;
		float valueEnd = (1f + sizeGrow) * ((MinMaxCurve)(ref startLifetime)).constantMax;
		((SizeOverLifetimeModule)(ref sizeOverLifetime)).size = new MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, valueEnd));
		if (explodeOnAwake)
		{
			Explode();
		}
	}

	private void Update()
	{
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0068: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		float num = sizeGrow;
		if (exponentialGrowth)
		{
			float num2 = Time.time - _emitTime;
			float num3 = SizeFunction(num2 - epsilon);
			num = (SizeFunction(num2) / num3 - 1f) / epsilon;
		}
		else
		{
			num = sizeGrow;
		}
		MainModule main = _particleSystem.main;
		MinMaxCurve startLifetime = ((MainModule)(ref main)).startLifetime;
		SizeOverLifetimeModule sizeOverLifetime = _particleSystem.sizeOverLifetime;
		float valueEnd = (1f + num) * ((MinMaxCurve)(ref startLifetime)).constantMax;
		((SizeOverLifetimeModule)(ref sizeOverLifetime)).size = new MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, valueEnd));
		if (_delayedExplosionStarted)
		{
			_explodeDelay -= Time.deltaTime;
			if (_explodeDelay <= 0f)
			{
				Explode();
			}
		}
	}

	private float SizeFunction(float elapsedTime)
	{
		float num = 1f - 1f / (1f + elapsedTime * speed);
		return initFraction + (1f - initFraction) * num;
	}

	public void Reset()
	{
		size = _baseSize;
		color = _baseColor;
		damping = _baseDamping;
	}

	public override void Explode()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_04aa: Invalid comparison between Unknown and I4
		if (!on)
		{
			return;
		}
		MainModule main = _particleSystem.main;
		((MainModule)(ref main)).simulationSpace = (ParticleSystemSimulationSpace)(useWorldSpace ? 1 : 0);
		_scaledDuration = timeScale * duration;
		_scaledDurationVariation = timeScale * durationVariation;
		_scaledStartRadius = size * startRadius;
		ParticleSystemRenderer component = ((Component)(object)_particleSystem).GetComponent<ParticleSystemRenderer>();
		component.renderMode = renderModeNewSystem;
		if (!_delayedExplosionStarted)
		{
			_explodeDelay = explodeDelayMin + Random.value * (explodeDelayMax - explodeDelayMin);
		}
		if (_explodeDelay <= 0f)
		{
			Color[] array = new Color[5];
			ColorOverLifetimeModule colorOverLifetime = _particleSystem.colorOverLifetime;
			((ColorOverLifetimeModule)(ref colorOverLifetime)).enabled = true;
			Gradient gradient = new Gradient();
			if (useExplicitColorAnimation)
			{
				array[0] = colorAnimation[0];
				array[1] = colorAnimation[1];
				array[2] = colorAnimation[2];
				array[3] = colorAnimation[3];
				array[4] = colorAnimation[4];
			}
			else
			{
				array[0] = new Color(color.r, color.g, color.b, color.a * 0.7f);
				array[1] = new Color(color.r, color.g, color.b, color.a * 1f);
				array[2] = new Color(color.r, color.g, color.b, color.a * 0.5f);
				array[3] = new Color(color.r, color.g, color.b, color.a * 0.3f);
				array[4] = new Color(color.r, color.g, color.b, color.a * 0f);
			}
			gradient.SetKeys(new GradientColorKey[5]
			{
				new GradientColorKey(array[0], 0f),
				new GradientColorKey(array[1], 0.25f),
				new GradientColorKey(array[2], 0.5f),
				new GradientColorKey(array[3], 0.75f),
				new GradientColorKey(array[4], 1f)
			}, new GradientAlphaKey[5]
			{
				new GradientAlphaKey(array[0].a, 0f),
				new GradientAlphaKey(array[1].a, 0.25f),
				new GradientAlphaKey(array[2].a, 0.5f),
				new GradientAlphaKey(array[3].a, 0.75f),
				new GradientAlphaKey(array[4].a, 1f)
			});
			MinMaxGradient val = default(MinMaxGradient);
			((MinMaxGradient)(ref val))._002Ector(gradient);
			((ColorOverLifetimeModule)(ref colorOverLifetime)).color = val;
			((Renderer)(object)component).material = material;
			ForceOverLifetimeModule forceOverLifetime = _particleSystem.forceOverLifetime;
			((ForceOverLifetimeModule)(ref forceOverLifetime)).enabled = true;
			MinMaxCurve x = ((ForceOverLifetimeModule)(ref forceOverLifetime)).x;
			MinMaxCurve y = ((ForceOverLifetimeModule)(ref forceOverLifetime)).y;
			MinMaxCurve z = ((ForceOverLifetimeModule)(ref forceOverLifetime)).z;
			((MinMaxCurve)(ref x)).mode = (ParticleSystemCurveMode)3;
			((MinMaxCurve)(ref y)).mode = (ParticleSystemCurveMode)3;
			((MinMaxCurve)(ref z)).mode = (ParticleSystemCurveMode)3;
			((MinMaxCurve)(ref x)).constantMin = force.x;
			((MinMaxCurve)(ref y)).constantMin = force.y;
			((MinMaxCurve)(ref z)).constantMin = force.z;
			((MinMaxCurve)(ref x)).constantMax = force.x;
			((MinMaxCurve)(ref y)).constantMax = force.y;
			((MinMaxCurve)(ref z)).constantMax = force.z;
			_tmpCount = count * detail;
			if (_tmpCount < 1f)
			{
				_tmpCount = 1f;
			}
			if ((int)((MainModule)(ref main)).simulationSpace == 1)
			{
				_thisPos = base.gameObject.transform.position;
			}
			else
			{
				_thisPos = new Vector3(0f, 0f, 0f);
			}
			for (int i = 1; (float)i <= _tmpCount; i++)
			{
				_tmpPos = Vector3.Scale(Random.insideUnitSphere, new Vector3(_scaledStartRadius, _scaledStartRadius, _scaledStartRadius));
				_tmpPos = _thisPos + _tmpPos;
				if (randomRotation)
				{
					_tmpAngularVelocity = Random.Range(-1f, 1f) * angularVelocity;
				}
				else
				{
					_tmpAngularVelocity = angularVelocity;
				}
				_tmpParticleSize = size * (particleSize + Random.value * sizeVariation);
				_tmpDuration = _scaledDuration + Random.value * _scaledDurationVariation;
				_particleSystem.Emit(_tmpPos, new Vector3(_tmpAngularVelocity, _tmpAngularVelocity, _tmpAngularVelocity), _tmpParticleSize, _tmpDuration, (Color32)color);
			}
			_emitTime = Time.time;
			_delayedExplosionStarted = false;
			_explodeDelay = 0f;
		}
		else
		{
			_delayedExplosionStarted = true;
		}
	}
}
