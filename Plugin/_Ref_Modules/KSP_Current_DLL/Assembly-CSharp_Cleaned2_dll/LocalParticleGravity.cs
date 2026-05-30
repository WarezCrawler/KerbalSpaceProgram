using UnityEngine;

public class LocalParticleGravity : MonoBehaviour
{
	public float gravScale = 1f;

	private void Start()
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_008f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a0: Unknown result type (might be due to invalid IL or missing references)
		ParticleSystem component = GetComponent<ParticleSystem>();
		if ((Object)(object)component != null)
		{
			ForceOverLifetimeModule forceOverLifetime = component.forceOverLifetime;
			Vector3 vector = ((!(FlightGlobals.fetch != null)) ? (Vector3.down * (float)PhysicsGlobals.GravitationalAcceleration * gravScale) : ((Vector3)(FlightGlobals.getGeeForceAtPosition(base.transform.position) * gravScale)));
			((ForceOverLifetimeModule)(ref forceOverLifetime)).randomized = false;
			((ForceOverLifetimeModule)(ref forceOverLifetime)).x = new MinMaxCurve(0f);
			((ForceOverLifetimeModule)(ref forceOverLifetime)).y = new MinMaxCurve(0f - vector.magnitude);
			((ForceOverLifetimeModule)(ref forceOverLifetime)).z = new MinMaxCurve(0f);
			((ForceOverLifetimeModule)(ref forceOverLifetime)).enabled = true;
		}
	}
}
