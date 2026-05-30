using Expansions;
using Expansions.Missions;
using UnityEngine;

public class ScaledMovement : MapObject
{
	public static ScaledMovement Create(string name, Vessel vessel)
	{
		ScaledMovement scaledMovement = new GameObject(name).AddComponent<ScaledMovement>();
		scaledMovement.tgtRef = vessel.transform;
		scaledMovement.type = ObjectType.Vessel;
		scaledMovement.vessel = FlightGlobals.ActiveVessel;
		scaledMovement.missionsNode = null;
		scaledMovement.celestialBody = null;
		scaledMovement.maneuverNode = null;
		scaledMovement.orbit = vessel.orbit;
		return scaledMovement;
	}

	public static ScaledMovement Create(string name, CelestialBody cb, GameObject scaledObject)
	{
		ScaledMovement scaledMovement = scaledObject.AddComponent<ScaledMovement>();
		scaledMovement.tgtRef = cb.transform;
		scaledMovement.type = ObjectType.CelestialBody;
		scaledMovement.vessel = null;
		scaledMovement.missionsNode = null;
		scaledMovement.maneuverNode = null;
		scaledMovement.celestialBody = cb;
		scaledMovement.orbit = cb.orbit;
		return scaledMovement;
	}

	public static ScaledMovement Create(string name, MENode newNode)
	{
		if (!ExpansionsLoader.IsExpansionInstalled("MakingHistory"))
		{
			return null;
		}
		ScaledMovement scaledMovement = new GameObject(name).AddComponent<ScaledMovement>();
		scaledMovement.tgtRef = newNode.transform;
		scaledMovement.type = ObjectType.MENode;
		scaledMovement.vessel = null;
		scaledMovement.celestialBody = null;
		scaledMovement.maneuverNode = null;
		scaledMovement.orbit = newNode.orbitDriver.orbit;
		scaledMovement.missionsNode = newNode;
		return scaledMovement;
	}

	protected override void OnLateUpdate()
	{
		if ((bool)tgtRef)
		{
			switch (type)
			{
			case ObjectType.MENode:
				ScaledSpace.LocalToScaledSpace(missionsNode.GetNodeLocationInWorld());
				break;
			case ObjectType.Generic:
				base.transform.position = ScaledSpace.LocalToScaledSpace(tgtRef.position);
				break;
			case ObjectType.CelestialBody:
				base.transform.position = ScaledSpace.LocalToScaledSpace(celestialBody.position);
				break;
			case ObjectType.Vessel:
				base.transform.position = ScaledSpace.LocalToScaledSpace(vessel.GetWorldPos3D());
				break;
			}
			base.transform.localRotation = tgtRef.rotation;
		}
	}
}
