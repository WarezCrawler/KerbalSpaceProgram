using UnityEngine;
using ns9;

namespace ModuleWheels;

public class ModuleWheelDamage : ModuleWheelSubmodule
{
	[KSPField]
	public float stressTolerance = 10f;

	[KSPField]
	public float impactTolerance = 40f;

	[KSPField]
	public float deflectionMagnitude = 1f;

	[KSPField]
	public float slipMagnitude = 1f;

	[KSPField]
	public float deflectionSharpness = 1f;

	[KSPField]
	public float slipSharpness = 2f;

	[KSPField]
	public string damagedTransformName = "";

	[KSPField]
	public string undamagedTransformName = "";

	[KSPField]
	public bool isRepairable = true;

	[KSPField]
	public float explodeMultiplier = 5f;

	public float currentDeflection;

	public float lastDeflection;

	public float currentSlip;

	public float lastSlip;

	public float currentDownForce;

	public float lastDownForce;

	private const float repairImmunityMin = 30f;

	private const float repairImmunityMax = 90f;

	private float repairImmunityTimeTotal = 30f;

	private float repairImmunityTimeLeft;

	internal float startupTime = 8f;

	public bool initialized;

	[KSPField(isPersistant = true)]
	public bool isDamaged;

	public float totalStress;

	private float stressTime;

	private Transform dmgTransform;

	private Transform undmgTransform;

	[UI_ProgressBar(scene = UI_Scene.Flight, maxValue = 100f, minValue = 0f)]
	[KSPField(guiFormat = "0.0", guiActive = true, guiActiveEditor = false, guiName = "#autoLOC_6001459")]
	public float stressPercent;

	public float stressVariability;

	private BaseEvent eventRepairExternal;

	private WheelSubsystem subsystemDamage;

	public override void OnStart(StartState state)
	{
		base.OnStart(state);
		if (HighLogic.LoadedSceneIsFlight)
		{
			GameEvents.onPartCoupleComplete.Add(onPartCouple);
			GameEvents.onPartDeCoupleComplete.Add(onPartDecouple);
			GameEvents.onVesselWasModified.Add(OnVesselModified);
		}
		eventRepairExternal = base.Events["EventRepairExternal"];
		eventRepairExternal.active = isDamaged && isRepairable;
		if (!string.IsNullOrEmpty(damagedTransformName))
		{
			Transform transform = base.part.FindModelTransform(damagedTransformName);
			if (transform != null)
			{
				dmgTransform = transform;
			}
			else
			{
				Debug.LogError(("[ModuleWheelDamage]: No damaged transform object found with id " + damagedTransformName + " for " + base.part.partName) ?? "", base.gameObject);
			}
		}
		if (!string.IsNullOrEmpty(undamagedTransformName))
		{
			Transform transform2 = base.part.FindModelTransform(undamagedTransformName);
			if (transform2 != null)
			{
				undmgTransform = transform2;
			}
			else
			{
				Debug.LogError(("[ModuleWheelDamage]: No undamaged transform object found with id " + undamagedTransformName + " for " + base.part.partName) ?? "", base.gameObject);
			}
		}
		subsystemDamage = new WheelSubsystem("Wheel Damage", (WheelSubsystem.SystemTypes)33, this);
		isDamaged = !isDamaged;
		SetDamaged(!isDamaged);
	}

	public override void OnDestroy()
	{
		base.OnDestroy();
		GameEvents.onPartCoupleComplete.Remove(onPartCouple);
		GameEvents.onPartDeCoupleComplete.Remove(onPartDecouple);
		GameEvents.onVesselWasModified.Remove(OnVesselModified);
	}

	private void onPartCouple(GameEvents.FromToAction<Part, Part> partAction)
	{
		if ((base.vessel != null && base.vessel.Landed && partAction.from.vessel.persistentId == base.part.vessel.persistentId) || partAction.to.vessel.persistentId == base.part.vessel.persistentId)
		{
			ResetImmunity();
		}
	}

	private void onPartDecouple(Part part)
	{
		if (base.vessel != null && base.vessel.Landed && part.vessel.persistentId == base.part.vessel.persistentId)
		{
			ResetImmunity();
		}
	}

	private void OnVesselModified(Vessel vsl)
	{
		if (base.vessel != null && base.vessel.Landed && base.part.vessel.persistentId == vsl.persistentId)
		{
			ResetImmunity();
		}
	}

	internal void ResetImmunity()
	{
		startupTime = 8f;
		repairImmunityTimeTotal = 30f;
	}

	protected override void OnWheelSetup()
	{
		StartCoroutine(CallbackUtil.DelayedCallback(1, delegate
		{
			isDamaged = !isDamaged;
			SetDamaged(!isDamaged);
		}));
	}

	private void Initialize()
	{
		currentDeflection = deflectionMagnitude * wheel.LegacyWheelLoad;
		currentSlip = slipMagnitude * wheel.LegacyWheelLoad * wheelBase.slipDisplacement.magnitude;
		currentDownForce = wheel.currentState.downforce;
		lastDeflection = currentDeflection;
		lastSlip = currentSlip;
		lastDownForce = currentDownForce;
		initialized = true;
	}

	public void FixedUpdate()
	{
		if (!HighLogic.LoadedSceneIsFlight || !baseSetup || (!(base.part.vessel == null) && base.part.vessel.situation == Vessel.Situations.PRELAUNCH))
		{
			return;
		}
		if (!base.part.packed && !isDamaged)
		{
			if (!initialized)
			{
				Initialize();
			}
			lastDeflection = currentDeflection;
			lastSlip = currentSlip;
			lastDownForce = currentDownForce;
			if (repairImmunityTimeLeft > 0f)
			{
				repairImmunityTimeLeft = Mathf.Max(0f, repairImmunityTimeLeft - Time.fixedDeltaTime);
			}
			float num = 1f - repairImmunityTimeLeft / repairImmunityTimeTotal;
			currentDeflection = deflectionMagnitude * wheel.LegacyWheelLoad;
			currentSlip = slipMagnitude * wheel.LegacyWheelLoad * wheelBase.slipDisplacement.magnitude;
			currentDeflection = Mathf.Lerp(lastDeflection, currentDeflection, deflectionSharpness * TimeWarp.fixedDeltaTime);
			currentSlip = Mathf.Lerp(lastSlip, currentSlip, slipSharpness * TimeWarp.fixedDeltaTime);
			currentDownForce = wheel.currentState.downforce;
			if (startupTime > 0f)
			{
				startupTime -= Time.deltaTime;
				return;
			}
			float num2 = currentDeflection * num * GameSettings.WHEEL_WEIGHT_STRESS_MULTIPLIER;
			float num3 = currentSlip * num * GameSettings.WHEEL_SLIP_STRESS_MULTIPLIER;
			totalStress = Mathf.Clamp(num2 + num3, 0f, stressTolerance);
			stressPercent = totalStress / stressTolerance * 100f;
			float num4 = stressTolerance + stressVariability;
			if (totalStress >= num4)
			{
				stressTime += TimeWarp.fixedDeltaTime;
				float num5 = 1f - Mathf.Clamp01((totalStress - num4) / (num4 / 2f));
				float num6 = 0.25f + 0.25f * num5;
				if (stressTime > num6)
				{
					SetDamaged(damaged: true);
				}
			}
			else
			{
				stressTime = 0f;
			}
			float num7 = (currentDownForce - lastDownForce) * wheel.gravity.Magnitude / (float)PhysicsGlobals.GravitationalAcceleration;
			if (!CheatOptions.NoCrashDamage)
			{
				if (num7 >= impactTolerance * explodeMultiplier)
				{
					base.part.explode();
					ScreenMessages.PostScreenMessage(Localizer.Format("#autoLOC_246817", base.part.partInfo.title), 6f, ScreenMessageStyle.UPPER_LEFT);
					FlightLogger.fetch.LogEvent(Localizer.Format("<<1>> overstressed and was destroyed.", base.part.partInfo.title));
				}
				else if (num7 >= impactTolerance)
				{
					SetDamaged(damaged: true);
				}
			}
		}
		else
		{
			currentDeflection = 0f;
			currentSlip = 0f;
			currentDownForce = 0f;
			stressTime = 0f;
			initialized = false;
			startupTime = 8f;
			repairImmunityTimeTotal = 30f;
			repairImmunityTimeLeft = 0f;
			totalStress = 0f;
			stressPercent = 0f;
		}
	}

	public void SetDamaged(bool damaged)
	{
		if (damaged && !isDamaged)
		{
			if (dmgTransform != null)
			{
				dmgTransform.gameObject.SetActive(value: true);
			}
			if (undmgTransform != null)
			{
				undmgTransform.gameObject.SetActive(value: false);
			}
			if (baseSetup)
			{
				wheelBase.InopSystems.AddSubsystem(subsystemDamage);
			}
			eventRepairExternal.active = damaged && isRepairable;
		}
		if (!damaged && isDamaged)
		{
			repairImmunityTimeTotal = Random.Range(30f, 90f);
			repairImmunityTimeLeft = repairImmunityTimeTotal;
			if (dmgTransform != null)
			{
				dmgTransform.gameObject.SetActive(value: false);
			}
			if (undmgTransform != null)
			{
				undmgTransform.gameObject.SetActive(value: true);
			}
			if (baseSetup)
			{
				wheelBase.InopSystems.RemoveSubsystem(subsystemDamage);
			}
			eventRepairExternal.active = damaged && isRepairable;
			GameEvents.onWheelRepaired.Fire(base.part);
		}
		isDamaged = damaged;
		stressVariability = (0f - Random.value) * stressTolerance * 0.02f;
	}

	public override string OnGatherInfo()
	{
		return Localizer.Format("#autoLOC_246889", stressTolerance.ToString("0.0"));
	}

	[ContextMenu("Damage Wheel")]
	public void DamageWheel()
	{
		SetDamaged(damaged: true);
	}

	[KSPEvent(guiActiveUnfocused = true, externalToEVAOnly = true, guiActive = false, unfocusedRange = 4f, guiName = "#autoLOC_6001882")]
	public void EventRepairExternal()
	{
		if (HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().KerbalExperienceEnabled(HighLogic.CurrentGame.Mode) && FlightGlobals.ActiveVessel.VesselValues.RepairSkill.value < 3)
		{
			ScreenMessages.PostScreenMessage(Localizer.Format("#autoLOC_246904", 3.ToString()));
		}
		else
		{
			SetDamaged(damaged: false);
		}
	}
}
