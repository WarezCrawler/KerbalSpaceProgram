using System.Collections;
using Expansions;
using Expansions.Serenity.DeployedScience.Runtime;
using UnityEngine;
using ns9;

public class ModuleGroundPart : ModuleCargoPart, IAnimatedModule
{
	private ModuleInventoryPart kerbalInventoryModule;

	private int firstEmptyInvSlot;

	[KSPField]
	public bool placementAllowXRotation = true;

	[KSPField]
	public bool placementAllowYRotation = true;

	[KSPField]
	public bool placementAllowZRotation = true;

	[KSPField]
	public float placementMaxRivotVelocity = 0.003f;

	[KSPField]
	public float kinematicDelay = 3f;

	private ModuleGroundSciencePart groundScienceModule;

	private ModuleGroundExpControl groundExpConModule;

	private ModuleAnimationGroup animationGroup;

	private UIPartActionWindow partActionWindow;

	protected bool beingRetrieved;

	[KSPEvent(guiActiveEditor = false, guiActiveUnfocused = true, externalToEVAOnly = true, guiActive = true, unfocusedRange = 4f, guiName = "#autoLOC_8002230")]
	public virtual void RetrievePart()
	{
		beingRetrieved = true;
		base.vessel.vesselType = VesselType.DeployedSciencePart;
		if (kerbalInventoryModule == null)
		{
			OnVesselChange(FlightGlobals.ActiveVessel);
		}
		if (kerbalInventoryModule != null)
		{
			kerbalInventoryModule.partBeingRetrieved = true;
			if (kerbalInventoryModule.StoreCargoPartAtSlot(base.part.partInfo.name) && ExpansionsLoader.IsExpansionInstalled("Serenity") && groundScienceModule != null)
			{
				GameEvents.onGroundSciencePartRemoved.Fire(groundScienceModule);
			}
			PlayRetractAnimation();
		}
	}

	protected void RetrieveScienceData()
	{
		if (!(kerbalInventoryModule != null) || !ExpansionsLoader.IsExpansionInstalled("Serenity") || !(groundScienceModule != null))
		{
			return;
		}
		ModuleScienceContainer moduleScienceContainer = FlightGlobals.ActiveVessel.FindPartModuleImplementing<ModuleScienceContainer>();
		if (moduleScienceContainer != null && groundScienceModule.ScienceClusterData != null)
		{
			DeployedScienceExperiment experimentByPartID = groundScienceModule.ScienceClusterData.DeployedScienceParts.GetExperimentByPartID(base.part.persistentId);
			if (experimentByPartID != null && experimentByPartID.GatherScienceData(out var experimentData))
			{
				moduleScienceContainer.AddData(experimentData);
			}
		}
	}

	private void Start()
	{
		if (!ExpansionsLoader.IsExpansionInstalled("Serenity") && HighLogic.LoadedSceneIsGame)
		{
			base.enabled = false;
			Object.Destroy(this);
		}
	}

	public void OnPartPack()
	{
		base.part.Rigidbody.constraints = RigidbodyConstraints.None;
	}

	public void OnPartUnpack()
	{
		if (animationGroup == null)
		{
			animationGroup = base.part.FindModuleImplementing<ModuleAnimationGroup>();
		}
		StartCoroutine(MakePartKinematic());
		OnVesselChange(null);
	}

	public override void OnStart(StartState state)
	{
		UpdateModuleUI();
		GameEvents.onPartActionUIShown.Add(OnPartActionUIShown);
		GameEvents.onPartActionUIDismiss.Add(OnPartActionUIDismiss);
		GameEvents.onVesselChange.Add(OnVesselChange);
		GameEvents.onSceneConfirmExit.Add(OnLeavingScene);
		if (HighLogic.LoadedSceneIsFlight)
		{
			GameEvents.onPartWillDie.Add(OnPartWillDie);
		}
		animationGroup = base.part.FindModuleImplementing<ModuleAnimationGroup>();
		groundScienceModule = base.part.FindModuleImplementing<ModuleGroundSciencePart>();
		groundExpConModule = base.part.FindModuleImplementing<ModuleGroundExpControl>();
	}

	public override void OnDestroy()
	{
		StopCoroutine("MakePartKinematic");
		base.OnDestroy();
		GameEvents.onPartActionUIShown.Remove(OnPartActionUIShown);
		GameEvents.onPartActionUIDismiss.Remove(OnPartActionUIDismiss);
		GameEvents.onVesselChange.Remove(OnVesselChange);
		GameEvents.OnAnimationGroupRetractComplete.Remove(OnRetractCompleted);
		GameEvents.onPartWillDie.Remove(OnPartWillDie);
		GameEvents.onSceneConfirmExit.Remove(OnLeavingScene);
	}

	public override void OnUpdate()
	{
		base.OnUpdate();
		if (partActionWindow != null)
		{
			UpdateModuleUI();
		}
	}

	private IEnumerator MakePartKinematic()
	{
		bool adjustMass = false;
		float prefabMass = base.part.prefabMass;
		if (base.vessel != null && base.vessel.mainBody != null)
		{
			if (base.vessel.mainBody.GeeASL < 0.800000011920929)
			{
				adjustMass = true;
				base.part.prefabMass = 20f;
			}
			else if (base.vessel.mainBody.GeeASL < 1.100000023841858)
			{
				adjustMass = true;
				base.part.prefabMass = 4f;
			}
		}
		yield return new WaitForSeconds(kinematicDelay);
		if (base.vessel != null)
		{
			double timeWaitingForSrfVelocity = Planetarium.GetUniversalTime();
			float settleDelay = kinematicDelay * 10f;
			while (!(Planetarium.GetUniversalTime() - timeWaitingForSrfVelocity > (double)settleDelay) && !(base.vessel.srf_velocity.magnitude < (double)placementMaxRivotVelocity))
			{
				yield return null;
			}
		}
		if (base.part.GroundContact)
		{
			Debug.LogFormat("[ModuleGroundPart]: Part {0} velocity {1} riveting to the ground.", base.part.partInfo.title, base.vessel.srf_velocity.magnitude);
			if (adjustMass)
			{
				base.part.prefabMass = prefabMass;
			}
			base.part.Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
			PlayDeployAnimation();
			SendMessage("SetDeployedOnGround");
		}
		else
		{
			Debug.LogFormat("[ModuleGroundPart]: Part {0} cannot deploy unless on the ground.", base.part.partInfo.title);
			ScreenMessages.PostScreenMessage(Localizer.Format("#autoLOC_8002326"), 3f, ScreenMessageStyle.UPPER_CENTER, persist: true);
			EnableModule();
		}
	}

	private void OnLeavingScene(GameScenes scn)
	{
		if (beingRetrieved)
		{
			base.vessel.state = Vessel.State.DEAD;
		}
	}

	private void OnPartWillDie(Part p)
	{
		if (base.part != null && p.persistentId == base.part.persistentId && base.part.Rigidbody != null)
		{
			base.part.Rigidbody.constraints = RigidbodyConstraints.None;
			base.part.Rigidbody.ResetInertiaTensor();
		}
	}

	private void PlayDeployAnimation()
	{
		if (animationGroup != null && !animationGroup.isDeployed)
		{
			animationGroup.DeployModule();
		}
	}

	private void PlayRetractAnimation()
	{
		if (animationGroup != null && animationGroup.isDeployed)
		{
			GameEvents.OnAnimationGroupRetractComplete.Add(OnRetractCompleted);
			animationGroup.RetractModule();
		}
		else
		{
			OnRetractCompleted(animationGroup);
		}
	}

	private void OnRetractCompleted(ModuleAnimationGroup aGroup)
	{
		if (!(aGroup == animationGroup))
		{
			return;
		}
		GameEvents.OnAnimationGroupRetractComplete.Remove(OnRetractCompleted);
		if (ExpansionsLoader.IsExpansionInstalled("Serenity"))
		{
			if (groundScienceModule != null)
			{
				GameEvents.onGroundSciencePartRemoved.Fire(groundScienceModule);
			}
			if (groundExpConModule != null)
			{
				GameEvents.onGroundScienceDeregisterCluster.Fire(base.part.persistentId);
			}
		}
		if (kerbalInventoryModule != null)
		{
			kerbalInventoryModule.partBeingRetrieved = false;
		}
		base.vessel.Die();
	}

	private void OnPartActionUIShown(UIPartActionWindow window, Part p)
	{
		if (p == base.part)
		{
			partActionWindow = window;
			UpdateModuleUI();
		}
	}

	private void OnPartActionUIDismiss(Part p)
	{
		if (p == base.part)
		{
			partActionWindow = null;
		}
	}

	private void OnVesselChange(Vessel vessel)
	{
		if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA)
		{
			kerbalInventoryModule = FlightGlobals.ActiveVessel.parts[0].FindModuleImplementing<ModuleInventoryPart>();
		}
	}

	private void UpdateModuleUI()
	{
		base.Events["RetrievePart"].active = false;
		if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA && kerbalInventoryModule != null)
		{
			firstEmptyInvSlot = kerbalInventoryModule.FirstEmptySlot();
			if (firstEmptyInvSlot > -1)
			{
				base.Events["RetrievePart"].active = true;
			}
		}
	}

	public override string GetModuleDisplayName()
	{
		return Localizer.Format("#autoLOC_6012033");
	}

	public virtual void EnableModule()
	{
		isEnabled = true;
	}

	public virtual void DisableModule()
	{
		isEnabled = false;
	}

	public virtual bool ModuleIsActive()
	{
		if (groundScienceModule != null)
		{
			if (isEnabled)
			{
				return groundScienceModule.Enabled;
			}
			return false;
		}
		if (groundExpConModule != null)
		{
			if (isEnabled)
			{
				return groundExpConModule.Enabled;
			}
			return false;
		}
		return isEnabled;
	}

	public virtual bool IsSituationValid()
	{
		if (HighLogic.LoadedScene != GameScenes.FLIGHT)
		{
			return false;
		}
		return base.part.State != PartStates.PLACEMENT;
	}
}
