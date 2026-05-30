using UnityEngine;

public class ReentryFXParticleTrail : MonoBehaviour
{
	private Vessel vessel;

	public ParticleSystem pSys;

	public AerodynamicsFX fxLogic;

	public Vector3 velocity;

	public float effectSize = 1f;

	public float effectIntensity;

	private Rigidbody _vesselRigidbody;

	private void Start()
	{
		vessel = GetComponent<Vessel>();
		if ((Object)(object)pSys == null)
		{
			pSys = (Object.Instantiate(Resources.Load("Effects/fx_reentryTrail")) as GameObject).GetComponent<ParticleSystem>();
		}
	}

	private void FixedUpdate()
	{
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		if ((bool)vessel)
		{
			velocity = vessel.srf_velocity;
			((Component)(object)pSys).transform.position = vessel.CoM + vessel.GetComponentCached(ref _vesselRigidbody).velocity * Time.fixedDeltaTime;
		}
		if ((bool)fxLogic)
		{
			velocity = fxLogic.velocity * (float)fxLogic.airSpeed;
			effectIntensity = fxLogic.FxScalar;
		}
		((Component)(object)pSys).transform.forward = -velocity.normalized;
		MainModule main = pSys.main;
		((MainModule)(ref main)).startDelay = MinMaxCurve.op_Implicit(velocity.magnitude);
	}
}
