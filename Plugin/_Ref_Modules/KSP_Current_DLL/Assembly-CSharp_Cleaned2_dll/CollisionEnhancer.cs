using UnityEngine;
using ns9;

public class CollisionEnhancer : MonoBehaviour
{
	private Vector3 lastPos;

	public static bool bypass;

	public static double UnderTerrainTolerance = 0.5;

	public static float upFactor = 0.25f;

	public static float minDistSqr = 0.010000001f;

	public Part part;

	public bool wasPacked = true;

	private RaycastHit hit;

	public CollisionEnhancerBehaviour OnTerrainPunchThrough;

	public float translateBackVelocityFactor = 0.2f;

	private Rigidbody rb;

	private void Start()
	{
		lastPos = base.transform.position;
		part = GetComponent<Part>();
		rb = GetComponent<Rigidbody>();
	}

	private void FixedUpdate()
	{
		if (part.packed)
		{
			lastPos = base.transform.position;
			wasPacked = true;
			return;
		}
		if (!wasPacked)
		{
			lastPos -= FloatingOrigin.Offset;
		}
		wasPacked = false;
		if (!bypass && part.State != PartStates.DEAD && (lastPos - base.transform.position).sqrMagnitude > minDistSqr)
		{
			bool flag = false;
			double num = 0.0;
			if (Physics.Linecast(lastPos, base.transform.position, out hit, 32768, QueryTriggerInteraction.Ignore))
			{
				Debug.Log("[F: " + Time.frameCount + "]: [" + base.name + "] Collision Enhancer Punch Through - vel: " + rb.velocity.magnitude, base.gameObject);
				Vector3 vector = FlightGlobals.getUpAxis(FlightGlobals.currentMainBody, hit.point);
				switch (OnTerrainPunchThrough)
				{
				case CollisionEnhancerBehaviour.EXPLODE:
					if (!flag && (bool)hit.collider.attachedRigidbody)
					{
						if ((rb.velocity - hit.collider.attachedRigidbody.velocity).magnitude * hit.collider.attachedRigidbody.mass > part.crashTolerance && !CheatOptions.NoCrashDamage)
						{
							GameEvents.onCollision.Fire(new EventReport(FlightEvents.COLLISION, part, part.partInfo.title, hit.collider.attachedRigidbody.name));
							part.explode();
							break;
						}
						if (!hit.point.IsInvalid())
						{
							base.transform.position = hit.point + vector * upFactor;
						}
						Vector3 vector3 = Vector3.Reflect((rb.velocity + Krakensbane.GetFrameVelocityV3f()).normalized, hit.normal * Mathf.Sign(Vector3.Dot(hit.normal, vector))).normalized * (rb.velocity + Krakensbane.GetFrameVelocityV3f()).magnitude * translateBackVelocityFactor - Krakensbane.GetFrameVelocityV3f();
						if (vector3.IsInvalid())
						{
							rb.velocity = Vector3.zero;
						}
						else
						{
							rb.velocity = vector3;
						}
					}
					else if (rb.velocity.sqrMagnitude > part.crashTolerance * part.crashTolerance && !CheatOptions.NoCrashDamage)
					{
						GameEvents.onCollision.Fire(new EventReport(FlightEvents.COLLISION, part, part.partInfo.title, Localizer.Format("#autoLOC_204427")));
						part.explode();
					}
					else if (!flag)
					{
						if (!hit.point.IsInvalid())
						{
							base.transform.position = hit.point + vector * upFactor;
						}
						Vector3 vector4 = Vector3.Reflect((rb.velocity + Krakensbane.GetFrameVelocityV3f()).normalized, hit.normal * Mathf.Sign(Vector3.Dot(hit.normal, vector))).normalized * (rb.velocity + Krakensbane.GetFrameVelocityV3f()).magnitude * translateBackVelocityFactor - Krakensbane.GetFrameVelocityV3f();
						if (vector4.IsInvalid())
						{
							rb.velocity = Vector3.zero;
						}
						else
						{
							rb.velocity = vector4;
						}
					}
					else
					{
						Transform obj3 = base.transform;
						obj3.position += part.vessel.upAxis * num;
						rb.velocity = translateBackVelocityFactor * part.vessel.upAxis - Krakensbane.GetFrameVelocity();
					}
					break;
				case CollisionEnhancerBehaviour.TRANSLATE_BACK:
					if (!flag)
					{
						if (!hit.point.IsInvalid())
						{
							base.transform.position = hit.point + vector * upFactor;
						}
						Vector3 vector2 = Vector3.Reflect((rb.velocity + Krakensbane.GetFrameVelocityV3f()).normalized, hit.normal * Mathf.Sign(Vector3.Dot(hit.normal, vector))).normalized * (rb.velocity + Krakensbane.GetFrameVelocityV3f()).magnitude * translateBackVelocityFactor - Krakensbane.GetFrameVelocityV3f();
						if (vector2.IsInvalid())
						{
							rb.velocity = Vector3.zero;
						}
						else
						{
							rb.velocity = vector2;
						}
					}
					else
					{
						Transform obj2 = base.transform;
						obj2.position += part.vessel.upAxis * num;
						rb.velocity = translateBackVelocityFactor * part.vessel.upAxis - Krakensbane.GetFrameVelocity();
					}
					break;
				case CollisionEnhancerBehaviour.TRANSLATE_BACK_SPLAT:
					if (!flag)
					{
						if (!hit.point.IsInvalid())
						{
							base.transform.position = hit.point + vector * upFactor;
						}
					}
					else
					{
						Transform obj = base.transform;
						obj.position += part.vessel.upAxis * num;
					}
					rb.velocity = Vector3.zero;
					break;
				}
				flag = true;
			}
			if (flag)
			{
				GameEvents.OnCollisionEnhancerHit.Fire(part, hit);
				Debug.DrawLine(lastPos, base.transform.position, Color.red, 10f);
				DebugDrawUtil.DrawCrosshairs(hit.point, 1f, Color.red, 10f);
				GameEvents.onPartExplodeGroundCollision.Fire(part);
			}
		}
		lastPos = base.transform.position;
	}
}
