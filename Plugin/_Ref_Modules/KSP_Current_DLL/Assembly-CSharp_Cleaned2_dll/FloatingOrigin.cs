using System.Collections.Generic;
using UnityEngine;

public class FloatingOrigin : MonoBehaviour
{
	public float threshold = 500f;

	public float thresholdSqr;

	public double velForContinuous = 5000.0;

	public double velForContinuousSqr;

	public double altToStopMovingExplosions = 10000.0;

	public double CoMRecalcOffsetMaxSqr = 100000000.0;

	public bool continuous;

	public bool bypassAuto;

	public Vector3d offset;

	public Vector3d offsetNonKrakensbane;

	public Vector3d reverseoffset;

	public Vector3d outOfFrameAdditional = Vector3d.zero;

	public Transform forcedCenterTransform;

	public static FloatingOrigin fetch;

	public bool canEngageThisFrame = true;

	public List<ParticleSystem> particleSystems = new List<ParticleSystem>();

	public int pCount;

	public static int particlesLength = 1024;

	private bool SetOffsetThisFrame;

	private Vector3d terrainShaderOffset;

	public static Vector3d Offset
	{
		get
		{
			if (!fetch)
			{
				return Vector3d.zero;
			}
			return fetch.offset;
		}
	}

	public static Vector3d ReverseOffset
	{
		get
		{
			if (!fetch)
			{
				return Vector3d.zero;
			}
			return fetch.reverseoffset;
		}
	}

	public static Vector3d OffsetNonKrakensbane
	{
		get
		{
			if (!fetch)
			{
				return Vector3d.zero;
			}
			return fetch.offsetNonKrakensbane;
		}
	}

	public static Vector3d TerrainShaderOffset
	{
		get
		{
			if (!fetch)
			{
				return Vector3d.zero;
			}
			return fetch.terrainShaderOffset;
		}
		set
		{
			if ((bool)fetch)
			{
				fetch.terrainShaderOffset = value;
				Shader.SetGlobalVector("_floatingOriginOffset", new Vector4((float)value.x, (float)value.y, (float)value.z, 0f));
			}
		}
	}

	protected void Awake()
	{
		fetch = this;
		offset = Vector3d.zero;
		thresholdSqr = threshold * threshold;
		velForContinuousSqr = velForContinuous * velForContinuous;
		GameEvents.onLevelWasLoaded.Add(OnLevelLoaded);
		GameEvents.OnMapExited.Add(OnMapExited);
	}

	protected void OnDestroy()
	{
		if (fetch != null && fetch == this)
		{
			fetch = null;
		}
		particleSystems.Clear();
		GameEvents.onLevelWasLoaded.Remove(OnLevelLoaded);
		GameEvents.OnMapExited.Remove(OnMapExited);
	}

	protected void FixedUpdate()
	{
		if (FlightGlobals.ready)
		{
			bool setOffsetThisFrame = SetOffsetThisFrame;
			SetOffsetThisFrame = false;
			if (!bypassAuto)
			{
				continuous = (TimeWarp.CurrentRate > TimeWarp.MaxPhysicsRate && TimeWarp.WarpMode == TimeWarp.Modes.HIGH && !FlightGlobals.ActiveVessel.LandedOrSplashed) || FlightGlobals.ship_velocity.sqrMagnitude > velForContinuousSqr;
			}
			bool flag = !outOfFrameAdditional.IsZero();
			bool flag2 = false;
			if (canEngageThisFrame)
			{
				flag2 = continuous || FlightGlobals.ActiveVessel.transform.position.sqrMagnitude > thresholdSqr;
			}
			if (flag2)
			{
				CollisionEnhancer.bypass = true;
				setOffset(FlightGlobals.ActiveVessel.transform.position, outOfFrameAdditional);
			}
			else if (canEngageThisFrame && flag)
			{
				CollisionEnhancer.bypass = false;
				setOffset(FlightGlobals.ActiveVessel.transform.position, outOfFrameAdditional);
			}
			else
			{
				offset.Zero();
				reverseoffset.Zero();
				CollisionEnhancer.bypass = false;
			}
			if (setOffsetThisFrame && !SetOffsetThisFrame && FlightGlobals.ActiveVessel != null && (FlightGlobals.ActiveVessel.situation == Vessel.Situations.LANDED || FlightGlobals.ship_srfVelocity.magnitude < 100.0))
			{
				FlightGlobals.ActiveVessel.orbit.referenceBody.PreciseUpdateQuadPositions();
			}
			outOfFrameAdditional.Zero();
			canEngageThisFrame = true;
		}
	}

	protected void setOffset(Vector3d refPos, Vector3d nonFrame)
	{
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_035d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Invalid comparison between Unknown and I4
		//IL_01fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_0206: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Invalid comparison between Unknown and I4
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		if (refPos.IsInvalid())
		{
			return;
		}
		if (double.IsInfinity(refPos.sqrMagnitude))
		{
			Debug.Break();
			return;
		}
		SetOffsetThisFrame = true;
		offset = refPos;
		reverseoffset = new Vector3d(0.0 - refPos.x, 0.0 - refPos.y, 0.0 - refPos.z);
		offsetNonKrakensbane = offset + nonFrame;
		int count = FlightGlobals.Bodies.Count;
		for (int i = 0; i < count; i++)
		{
			FlightGlobals.Bodies[i].position -= offsetNonKrakensbane;
		}
		int count2 = FlightGlobals.Vessels.Count;
		bool flag = offset.sqrMagnitude > CoMRecalcOffsetMaxSqr;
		Vector3d vector3d = Vector3d.zero;
		MainModule main;
		for (int j = 0; j < count2; j++)
		{
			Vessel vessel = FlightGlobals.Vessels[j];
			if (vessel.state == Vessel.State.DEAD)
			{
				continue;
			}
			Vector3d vector3d2 = ((!vessel.loaded || vessel.packed || vessel.LandedOrSplashed) ? offsetNonKrakensbane : offset);
			vessel.SetPosition((Vector3d)vessel.transform.position - vector3d2);
			if (flag && vessel.packed)
			{
				vessel.precalc.CalculatePhysicsStats();
			}
			else
			{
				vessel.CoMD -= vector3d2;
				vessel.CoM = vessel.CoMD;
			}
			int count3 = vessel.parts.Count;
			for (int k = 0; k < count3; k++)
			{
				Part part = vessel.parts[k];
				bool flag2 = false;
				if (part.Rigidbody != null)
				{
					flag2 = true;
					vector3d = part.Rigidbody.velocity + Krakensbane.GetFrameVelocity();
				}
				int count4 = part.fxGroups.Count;
				while (count4-- > 0)
				{
					FXGroup fXGroup = part.fxGroups[count4];
					int count5 = fXGroup.fxEmittersNewSystem.Count;
					while (count5-- > 0)
					{
						ParticleSystem val = fXGroup.fxEmittersNewSystem[count5];
						main = val.main;
						if ((int)((MainModule)(ref main)).simulationSpace != 1 || val.particleCount == 0)
						{
							continue;
						}
						Particle[] particleBuffer = val.GetParticleBuffer();
						int particleCount = val.particleCount;
						while (particleCount-- > 0)
						{
							Particle val2 = particleBuffer[particleCount];
							Vector3d vector3d3 = ((Particle)(ref val2)).position;
							if (flag2)
							{
								vector3d3 -= vector3d * Random.value * Time.deltaTime;
							}
							((Particle)(ref val2)).position = vector3d3 - offsetNonKrakensbane;
						}
						val.SetParticles(particleBuffer, val.particleCount);
					}
				}
			}
		}
		EffectBehaviour.OffsetParticles(-offsetNonKrakensbane);
		if (FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.radarAltitude < altToStopMovingExplosions)
		{
			FXMonger.OffsetPositions(-offsetNonKrakensbane);
		}
		int count6 = particleSystems.Count;
		while (count6-- > 0)
		{
			ParticleSystem val3 = particleSystems[count6];
			if ((Object)(object)val3 == null)
			{
				particleSystems.RemoveAt(count6);
				continue;
			}
			main = val3.main;
			if ((int)((MainModule)(ref main)).simulationSpace != 1)
			{
				continue;
			}
			int particleCount2 = val3.particleCount;
			Particle[] particleBuffer2 = val3.GetParticleBuffer();
			if (particleCount2 > 0)
			{
				int num = particleCount2;
				while (num-- > 0)
				{
					((Particle)(ref particleBuffer2[num])).position = (Vector3d)((Particle)(ref particleBuffer2[num])).position - offsetNonKrakensbane;
				}
				val3.SetParticles(particleBuffer2, particleCount2);
			}
		}
		int count7 = FlightGlobals.physicalObjects.Count;
		for (int l = 0; l < count7; l++)
		{
			physicalObject physicalObject2 = FlightGlobals.physicalObjects[l];
			if (!(physicalObject2 == null))
			{
				Transform obj = physicalObject2.transform;
				obj.position -= offset;
			}
		}
		TerrainShaderOffset += offsetNonKrakensbane;
		GameEvents.onFloatingOriginShift.Fire(offset, nonFrame);
	}

	public static bool RegisterParticleSystem(ParticleSystem sys)
	{
		if (!fetch)
		{
			return false;
		}
		if (fetch.particleSystems.Contains(sys))
		{
			return false;
		}
		fetch.particleSystems.Add(sys);
		return true;
	}

	public static bool UnregisterParticleSystem(ParticleSystem sys)
	{
		if (!fetch)
		{
			return false;
		}
		return fetch.particleSystems.Remove(sys);
	}

	public static void SetOffset(Vector3d refPos)
	{
		if ((bool)fetch)
		{
			fetch.setOffset(refPos, Vector3d.zero);
		}
	}

	public static void SetOutOfFrameOffset(Vector3d add)
	{
		if ((bool)fetch)
		{
			fetch.outOfFrameAdditional += add;
		}
	}

	public static void SetSafeToEngage(bool val)
	{
		if ((bool)fetch)
		{
			fetch.canEngageThisFrame = val;
		}
	}

	[ContextMenu("Reset Origin")]
	public void ResetOffset()
	{
		GameObject gameObject = GameObject.Find("LaunchPad_spawn");
		setOffset(gameObject.transform.position, Vector3d.zero);
	}

	public static void ResetTerrainShaderOffset()
	{
		if (!(TerrainShaderOffset == Vector3d.zero))
		{
			TerrainShaderOffset = Vector3d.zero;
		}
	}

	private void OnMapExited()
	{
		ResetTerrainShaderOffset();
	}

	private void OnLevelLoaded(GameScenes scene)
	{
		ResetTerrainShaderOffset();
	}
}
