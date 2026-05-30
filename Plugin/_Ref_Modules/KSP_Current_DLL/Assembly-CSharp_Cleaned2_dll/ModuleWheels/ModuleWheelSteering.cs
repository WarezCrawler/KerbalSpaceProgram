using System;
using UnityEngine;
using ns9;

namespace ModuleWheels;

public class ModuleWheelSteering : ModuleWheelSubmodule
{
	[KSPField]
	public string caliperTransformName = "";

	[KSPField]
	public FloatCurve steeringCurve;

	[KSPField]
	public float steeringResponse = 8f;

	public Transform caliperTransform;

	private bool caliperUpdating;

	private Quaternion caliperRot0 = Quaternion.identity;

	public float steeringRange;

	public float steeringInput;

	public float steeringInputLast;

	private Vector3 upAxis;

	private Vector3 fwdAxis;

	private Vector3 leftAxis;

	public float steerAngle;

	public float steerRange;

	[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001467")]
	[UI_Toggle(disabledText = "#autoLOC_6001071", scene = UI_Scene.All, enabledText = "#autoLOC_6001072", affectSymCounterparts = UI_Scene.All)]
	public bool steeringEnabled = true;

	[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001468")]
	[UI_Toggle(disabledText = "#autoLOC_6001075", scene = UI_Scene.All, enabledText = "#autoLOC_6001077", affectSymCounterparts = UI_Scene.All)]
	public bool steeringInvert;

	private Vector3 CoM;

	private Vector3 wCenter;

	private Vector3 wLeft0;

	private Vector3 wLeft;

	private Vector3 wRight;

	private Vector3 wAxis;

	private Vector3 sAxis;

	private float CoMfwdLength;

	private Vector3 updateCoordFrameReferenceForward;

	public Vector3 tPivot;

	public Vector3 tOrt;

	public Vector3 tCenter;

	public float h;

	public override void OnAwake()
	{
		base.OnAwake();
		if (steeringCurve == null)
		{
			steeringCurve = new FloatCurve();
		}
	}

	public override void OnLoad(ConfigNode node)
	{
	}

	public override void OnSave(ConfigNode node)
	{
	}

	public override void OnStart(StartState state)
	{
		base.OnStart(state);
		if (!string.IsNullOrEmpty(caliperTransformName))
		{
			caliperTransform = base.part.FindModelTransform(caliperTransformName);
			if (caliperTransform == null)
			{
				Debug.LogError("[ModuleWheelBase]: No transform called " + caliperTransformName + " found in " + base.part.partName + " hierarchy", base.gameObject);
				return;
			}
			caliperRot0 = caliperTransform.localRotation;
		}
		base.part.PartValues.SteeringRadius.Add(GetCoMFwdLength);
		steeringCurve.FindMinMaxValue(out var _, out var max);
		steeringRange = Mathf.Max(max, 0.01f);
		GameEvents.onWheelRepaired.Add(OnWheelRepaired);
	}

	[KSPAction("#autoLOC_6002418")]
	public void SteeringToggle(KSPActionParam act)
	{
		steeringEnabled = act.type == KSPActionType.Activate || (act.type == KSPActionType.Toggle && !steeringEnabled);
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		base.part.PartValues.SteeringRadius.Remove(GetCoMFwdLength);
		GameEvents.onWheelRepaired.Remove(OnWheelRepaired);
	}

	protected override void OnWheelSetup()
	{
		if (caliperTransform != null)
		{
			wheel.wheelCollider.caliperTransform = caliperTransform;
		}
		wheel.maxSteerAngle = steeringCurve.maxTime;
		wheel.steeringResponse = steeringResponse;
		SetCaliperUpdate(!wheelBase.InopSystems.HasType(WheelSubsystem.SystemTypes.Steering));
		if (wheelBase.wheelTransform != null)
		{
			wLeft0 = base.part.partTransform.InverseTransformDirection(-wheelBase.wheelTransform.right);
		}
		else
		{
			wLeft0 = Vector3.left;
		}
	}

	private void FixedUpdate()
	{
		if (!HighLogic.LoadedSceneIsFlight || !baseSetup)
		{
			return;
		}
		if (!base.part.packed && steeringEnabled && caliperUpdating)
		{
			updateCoordFrame();
			steeringInput = Mathf.Clamp(base.vessel.ctrlState.wheelSteer + base.vessel.ctrlState.wheelSteerTrim, -1f, 1f);
			if (steeringInvert)
			{
				steeringInput *= -1f;
			}
			wheel.steerInput = updateSteering(steeringInput, steeringRange);
			wheel.steeringResponse = steeringResponse;
		}
		else
		{
			wheel.steeringResponse = steeringResponse;
			wheel.steerInput = 0f;
		}
	}

	protected float GetSteeringResponseScale(float steerDelta)
	{
		float num = Mathf.Abs(wheel.currentState.localWheelVelocity.y);
		if (steerDelta < 0f)
		{
			num = 0f;
		}
		num *= 1f - ((WheelHit)(ref wheel.currentState.hit)).sidewaysSlip;
		return steeringCurve.Evaluate(num) / steeringRange;
	}

	private void updateCoordFrame()
	{
		float num = Vector3.Dot(base.vessel.ReferenceTransform.up, base.vessel.upAxis);
		upAxis = base.vessel.ReferenceTransform.up;
		if (num < Vector3.Dot(-base.vessel.ReferenceTransform.forward, base.vessel.upAxis))
		{
			upAxis = -base.vessel.ReferenceTransform.forward;
		}
		updateCoordFrameReferenceForward = -base.vessel.ReferenceTransform.forward;
		if (upAxis == base.vessel.ReferenceTransform.up)
		{
			updateCoordFrameReferenceForward = base.vessel.ReferenceTransform.forward;
		}
		fwdAxis = base.vessel.ReferenceTransform.up;
		if (num > Vector3.Dot(updateCoordFrameReferenceForward, base.vessel.upAxis))
		{
			fwdAxis = updateCoordFrameReferenceForward;
		}
		leftAxis = Vector3.Cross(fwdAxis, upAxis);
		CoM = base.vessel.CurrentCoM;
		wCenter = base.part.partTransform.TransformPoint(wheelBase.WheelOrgPosR);
		wLeft = Vector3.ProjectOnPlane(base.part.partTransform.TransformDirection(wLeft0), upAxis).normalized;
		wRight = -wLeft;
	}

	private float updateSteering(float input, float steeringRange)
	{
		float num = steeringRange * input;
		Vector3 vector = findTurnCenter(num, base.vessel.VesselValues.SteeringRadius.value, CoM);
		if (vector != Vector3.zero)
		{
			sAxis = Vector3.ProjectOnPlane(vector - wCenter, upAxis).normalized;
			Debug.DrawRay(wCenter, sAxis, XKCDColors.Teal);
			Debug.DrawRay(wCenter, wLeft, XKCDColors.DarkYellow);
			float a = Vector3.Dot(wRight, sAxis);
			a = Mathf.Max(a, Vector3.Dot(wLeft, sAxis));
			steerAngle = Mathf.Acos(a) * 57.29578f * Mathf.Sign(input);
		}
		else
		{
			steerAngle = 0f;
		}
		if (CoMfwdLength > 0f)
		{
			steerAngle = 0f - steerAngle;
		}
		if (steerAngle > 90f)
		{
			steerAngle = 180f - steerAngle;
		}
		else if (steerAngle < -90f)
		{
			steerAngle = -180f - steerAngle;
		}
		return Mathf.Clamp(steerAngle / wheel.maxSteerAngle, -1f, 1f);
	}

	private float findCoMfwdLength(Vector3 vesselCoM, Vector3 wheelCenter, Vector3 fwdAxis)
	{
		return Vector3.Dot(wheelCenter - vesselCoM, fwdAxis);
	}

	private float GetCoMFwdLength()
	{
		CoMfwdLength = findCoMfwdLength(CoM, wCenter, fwdAxis);
		return Mathf.Abs(CoMfwdLength);
	}

	private Vector3 findTurnCenter(float steerAngle, float length, Vector3 CoM)
	{
		if (steerAngle != 0f)
		{
			tPivot = fwdAxis * length;
			tOrt = Quaternion.AngleAxis(Mathf.Abs(steerAngle), -upAxis * Mathf.Sign(steerAngle)) * leftAxis * Mathf.Sign(steerAngle);
			h = length * (1f / Mathf.Tan(Mathf.Abs(steerAngle) * ((float)Math.PI / 180f)));
			tCenter = CoM + tPivot + tOrt * h;
		}
		else
		{
			tCenter = Vector3.zero;
		}
		Debug.DrawRay(CoM + tPivot, tOrt * h, XKCDColors.Magenta);
		DebugDrawUtil.DrawCrosshairs(tCenter, 0.5f, XKCDColors.BrightCyan, 0f);
		return tCenter;
	}

	public override string OnGatherInfo()
	{
		return Localizer.Format("#autoLOC_248404", steeringCurve.maxTime.ToString("0.0"));
	}

	protected override void OnSubsystemsModified(WheelSubsystems s)
	{
		if (s == wheelBase.InopSystems)
		{
			SetCaliperUpdate(!s.HasType(WheelSubsystem.SystemTypes.Steering));
		}
	}

	private void SetCaliperUpdate(bool update)
	{
		caliperUpdating = update;
		if (wheel != null && wheel.wheelCollider != null)
		{
			wheel.wheelCollider.updateCaliper = update;
		}
		ResetCaliper(!caliperUpdating);
	}

	private void ResetCaliper(bool resetTransform)
	{
		steerAngle = 0f;
		steeringInput = 0f;
		if (resetTransform && caliperTransform != null)
		{
			caliperTransform.localRotation = caliperRot0;
		}
	}

	private void OnWheelRepaired(Part p)
	{
		if (p != null && p == base.part)
		{
			ResetCaliper(resetTransform: true);
		}
	}
}
