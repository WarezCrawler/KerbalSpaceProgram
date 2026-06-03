using System;
using System.Collections;
using System.Collections.Generic;
using B9PartSwitch.Fishbones;
using B9PartSwitch.UI;
using B9PartSwitch.Utils;
using UniLinq;
using UnityEngine;

namespace B9PartSwitch;

public class ModuleB9PartSwitch : CustomPartModule, IPartMassModifier, IPartCostModifier, IModuleInfo, ILinearScaleProvider
{
	private static readonly string[] INCOMAPTIBLE_MODULES_FOR_RESOURCE_SWITCHING = new string[3] { "FSfuelSwitch", "InterstellarFuelSwitch", "ModuleFuelTanks" };

	[NodeData(name = "SUBTYPE", alwaysSerialize = true)]
	public List<PartSubtype> subtypes = new List<PartSubtype>();

	[NodeData]
	public float baseVolume;

	[NodeData]
	public string switcherDescription = Localization.ModuleB9PartSwitch_DefaultSwitcherDescription;

	[NodeData]
	public string switcherDescriptionPlural = Localization.ModuleB9PartSwitch_DefaultSwitcherDescriptionPlural;

	[NodeData]
	public bool affectDragCubes = true;

	[NodeData]
	public bool affectFARVoxels = true;

	[NodeData]
	public string parentID;

	[NodeData]
	public bool switchInFlight;

	[NodeData]
	public bool advancedTweakablesOnly;

	[NodeData]
	public string uiGroupName;

	[NodeData]
	public string uiGroupDisplayName;

	[KSPField(guiActiveEditor = true, guiName = "Subtype")]
	[UI_SubtypeSelector(affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
	public int currentSubtypeIndex = -1;

	[KSPField]
	public string currentSubtypeTitle;

	private readonly float scale = 1f;

	private readonly List<ModuleB9PartSwitch> children = new List<ModuleB9PartSwitch>(0);

	private ChangeTransactionManager reinitialzeModelTransactionManager;

	private bool needsRecalculateDragCubes;

	private bool needsNotifyFARToRevoxelize;

	[NodeData(name = "currentSubtype", persistent = true)]
	public string CurrentSubtypeName
	{
		get
		{
			if (subtypes.Count <= 0)
			{
				return null;
			}
			return CurrentSubtype?.Name;
		}
		private set
		{
			int num = subtypes.FindIndex((PartSubtype subtype) => subtype.Name == value);
			if (num == -1)
			{
				LogError("Cannot assign subtype because no subtype with name = '" + value + "' exists");
			}
			else
			{
				currentSubtypeIndex = num;
			}
		}
	}

	public int SubtypesCount => subtypes.Count;

	public int SubtypeIndex
	{
		get
		{
			if (!subtypes.ValidIndex(currentSubtypeIndex))
			{
				return 0;
			}
			return currentSubtypeIndex;
		}
	}

	public PartSubtype CurrentSubtype => subtypes[SubtypeIndex];

	public IEnumerable<PartSubtype> InactiveSubtypes => subtypes.Where((PartSubtype subtype) => subtype != CurrentSubtype);

	public TankType CurrentTankType => CurrentSubtype.tankType;

	public float VolumeFromChildren { get; private set; }

	public float VolumeAddedToParent => CurrentSubtype.volumeAddedToParent;

	public IEnumerable<string> ManagedResourceNames => subtypes.SelectMany((PartSubtype subtype) => subtype.ResourceNames);

	public bool ChangesGeometry => subtypes.Any((PartSubtype subtype) => subtype.ChangesGeometry);

	public bool ManagesResources => subtypes.Any((PartSubtype s) => !s.tankType.IsStructuralTankType);

	public bool ChangesDryMass => subtypes.Any((PartSubtype s) => s.ChangesDryMass);

	public bool ChangesResourceMass => subtypes.Any((PartSubtype s) => s.tankType.ChangesResourceMass);

	public bool ChangesMass => subtypes.Any((PartSubtype s) => s.ChangesMass);

	public bool ChangesDryCost => subtypes.Any((PartSubtype s) => s.ChangesDryCost);

	public bool ChangesResourceCost => subtypes.Any((PartSubtype s) => s.tankType.ChangesResourceCost);

	public bool ChangesCost => subtypes.Any((PartSubtype s) => s.ChangesCost);

	public float Scale => scale;

	public float LinearScale => scale;

	public float VolumeScale => scale * scale * scale;

	public IEnumerable<object> PartAspectLocks => subtypes.SelectMany((PartSubtype subtype) => subtype.PartAspectLocks);

	public IEnumerable<object> PartAspectLocksOnOtherModules => (from module in base.part.Modules.OfType<ModuleB9PartSwitch>()
		where module != this
		select module).SelectMany((ModuleB9PartSwitch module) => module.PartAspectLocks);

	public ModuleB9PartSwitch Parent { get; private set; }

	[KSPEvent(guiActiveEditor = true)]
	public void ShowSubtypesWindow()
	{
		PartSwitchFlightDialog.Spawn(this);
	}

	[KSPEvent]
	public void OnPartModelChanged()
	{
		reinitialzeModelTransactionManager.RequestChange();
	}

	[KSPEvent]
	public void DragCubesWereRecalculated()
	{
		needsRecalculateDragCubes = false;
	}

	[KSPEvent]
	public void FarWasNotifiedToRevoxelize()
	{
		needsNotifyFARToRevoxelize = false;
	}

	public override void OnAwake()
	{
		base.OnAwake();
		reinitialzeModelTransactionManager = new ChangeTransactionManager(ReinitializeModel);
	}

	protected override void OnLoadPrefab(ConfigNode node)
	{
		base.OnLoadPrefab(node);
		if (subtypes.Count == 0)
		{
			Exception ex = new Exception($"No subtypes found on {this}");
			FatalErrorHandler.HandleFatalError(ex);
			throw ex;
		}
		string[] array = (from s in subtypes
			group s by s.Name into g
			where g.Count() > 1
			select g.Key).ToArray();
		if (array.Length != 0)
		{
			string text = string.Join(", ", array);
			SeriousWarningHandler.DisplaySeriousWarning($"Duplicated subtype names found on {this}: {text}");
			LogError("Duplicate subtype names detected: " + text);
		}
	}

	public override void OnIconCreate()
	{
		base.OnIconCreate();
		InitializeSubtypes(displayWarnings: false);
		SetupForIcon();
	}

	public override void OnStart(StartState state)
	{
		base.OnStart(state);
		FindParent();
		InitializeSubtypes();
		EnsureAtLeastOneUnrestrictedSubtype();
		FindBestSubtype();
		SetupGUI();
	}

	public void Start()
	{
		CheckOtherModules();
		UpdateOnStart();
	}

	public override void OnStartFinished(StartState state)
	{
		base.OnStartFinished(state);
		UpdateOnStartFinished();
	}

	public float GetModuleMass(float baseMass, ModifierStagingSituation situation)
	{
		return GetDryMass(CurrentSubtype);
	}

	public ModifierChangeWhen GetModuleMassChangeWhen()
	{
		return ModifierChangeWhen.FIXED;
	}

	public float GetModuleCost(float baseCost, ModifierStagingSituation situation)
	{
		return GetWetCost(CurrentSubtype);
	}

	public ModifierChangeWhen GetModuleCostChangeWhen()
	{
		return ModifierChangeWhen.FIXED;
	}

	public override string GetInfo()
	{
		InitializeSubtypes();
		string text = $"<b><color=#7fdfffff>{SubtypesCount} {switcherDescriptionPlural}</color></b>";
		foreach (PartSubtype subtype in subtypes)
		{
			text = text + "\n<b>- " + subtype.title + "</b>";
			foreach (TankResource item in subtype.tankType)
			{
				text += $"\n  <color=#99ff00ff>- {item.resourceDefinition.displayName}</color>: {item.unitsPerVolume * GetTotalVolume(subtype):0.#}";
			}
		}
		return text;
	}

	public string GetModuleTitle()
	{
		return Localization.ModuleB9PartSwitch_ModuleTitle;
	}

	public string GetPrimaryField()
	{
		string text = $"<b>{subtypes.Count} {switcherDescriptionPlural}</b>";
		if (baseVolume > 0f)
		{
			text += $" (<b>{Localization.ModuleB9PartSwitch_TankVolumeString}:</b> {baseVolume:F0})";
		}
		return text;
	}

	public Callback<Rect> GetDrawModulePanelCallback()
	{
		return null;
	}

	private void OnSliderUpdate(BaseField field, object oldFieldValueObj)
	{
		int oldIndex = (int)oldFieldValueObj;
		reinitialzeModelTransactionManager.WithTransaction(delegate
		{
			subtypes[oldIndex].DeactivateOnSwitch();
			UpdateSubtype();
		});
		UpdateOnSwitch();
	}

	public void SwitchSubtype(string name)
	{
		reinitialzeModelTransactionManager.WithTransaction(delegate
		{
			CurrentSubtype.DeactivateOnSwitch();
			CurrentSubtypeName = name;
			UpdateSubtype();
		});
		UpdateOnSwitch();
	}

	public bool IsManagedResource(string resourceName)
	{
		return ManagedResourceNames.Contains(resourceName);
	}

	public bool TransformShouldBeEnabled(Transform transform)
	{
		if (CurrentSubtype.TransformIsManaged(transform))
		{
			return true;
		}
		foreach (PartSubtype subtype in subtypes)
		{
			if (subtype != CurrentSubtype && subtype.TransformIsManaged(transform))
			{
				return false;
			}
		}
		return true;
	}

	public bool NodeShouldBeEnabled(AttachNode node)
	{
		if (CurrentSubtype.NodeManaged(node))
		{
			return true;
		}
		foreach (PartSubtype subtype in subtypes)
		{
			if (subtype != CurrentSubtype && subtype.NodeManaged(node))
			{
				return false;
			}
		}
		return true;
	}

	public bool ModuleShouldBeEnabled(PartModule module)
	{
		return CurrentSubtype.ModuleShouldBeEnabled(module);
	}

	public void AddChild(ModuleB9PartSwitch child)
	{
		child.ThrowIfNullArgument("child");
		if (children.Contains(child))
		{
			LogError("Child module with id '" + child.moduleID + "' has already been added!");
		}
		else
		{
			children.Add(child);
		}
	}

	public void UpdateVolume()
	{
		UpdateVolumeFromChildren();
		CurrentSubtype.UpdateVolume();
	}

	public override void OnWillBeCopied(bool asSymCounterpart)
	{
		base.OnWillBeCopied(asSymCounterpart);
		foreach (PartSubtype inactiveSubtype in InactiveSubtypes)
		{
			inactiveSubtype.OnWillBeCopiedInactiveSubtype();
		}
		CurrentSubtype.OnWillBeCopiedActiveSubtype();
	}

	public override void OnWasCopied(PartModule copyPartModule, bool asSymCounterpart)
	{
		base.OnWasCopied(copyPartModule, asSymCounterpart);
		foreach (PartSubtype inactiveSubtype in InactiveSubtypes)
		{
			inactiveSubtype.OnWasCopiedInactiveSubtype();
		}
		CurrentSubtype.OnWasCopiedActiveSubtype();
		if (asSymCounterpart && base.part.symMethod == SymmetryMethod.Mirror && CurrentSubtype.mirrorSymmetrySubtype != CurrentSubtypeName)
		{
			((ModuleB9PartSwitch)copyPartModule).UpdateFromSymmetry(CurrentSubtype.mirrorSymmetrySubtype);
		}
	}

	public bool HasPartAspectLock(object partAspectLock)
	{
		return PartAspectLocks.Contains(partAspectLock);
	}

	public float GetTotalVolume(PartSubtype subtype)
	{
		return (baseVolume * subtype.volumeMultiplier + subtype.volumeAdded + VolumeFromChildren) * VolumeScale;
	}

	public float GetDryMass(PartSubtype subtype)
	{
		return GetTotalVolume(subtype) * subtype.tankType.tankMass + subtype.addedMass * VolumeScale;
	}

	public float GetWetMass(PartSubtype subtype)
	{
		return GetTotalVolume(subtype) * subtype.tankType.TotalUnitMass + subtype.addedMass * VolumeScale;
	}

	public float GetDryCost(PartSubtype subtype)
	{
		return GetTotalVolume(subtype) * subtype.tankType.tankCost + subtype.addedCost * VolumeScale;
	}

	public float GetWetCost(PartSubtype subtype)
	{
		return GetTotalVolume(subtype) * subtype.tankType.TotalUnitCost + subtype.addedCost * VolumeScale;
	}

	public float GetParentDryMass(PartSubtype subtype)
	{
		if (!Parent.IsNull())
		{
			return subtype.volumeAddedToParent * Parent.CurrentSubtype.tankType.tankMass * VolumeScale;
		}
		return 0f;
	}

	public float GetParentWetMass(PartSubtype subtype)
	{
		if (!Parent.IsNull())
		{
			return subtype.volumeAddedToParent * Parent.CurrentSubtype.tankType.TotalUnitMass * VolumeScale;
		}
		return 0f;
	}

	public float GetParentDryCost(PartSubtype subtype)
	{
		if (!Parent.IsNull())
		{
			return subtype.volumeAddedToParent * Parent.CurrentSubtype.tankType.tankCost * VolumeScale;
		}
		return 0f;
	}

	public float GetParentWetCost(PartSubtype subtype)
	{
		if (!Parent.IsNull())
		{
			return subtype.volumeAddedToParent * Parent.CurrentSubtype.tankType.TotalUnitCost * VolumeScale;
		}
		return 0f;
	}

	public BaseEventDetails CreateModuleDataChangedEventDetails()
	{
		BaseEventDetails baseEventDetails = new BaseEventDetails(BaseEventDetails.Sender.USER);
		baseEventDetails.Set<Action>("requestNotifyFARToRevoxelize", delegate
		{
			needsNotifyFARToRevoxelize = true;
		});
		baseEventDetails.Set<Action>("requestRecalculateDragCubes", delegate
		{
			needsRecalculateDragCubes = true;
		});
		return baseEventDetails;
	}

	private void FindParent()
	{
		Parent = null;
		if (!parentID.IsNullOrEmpty())
		{
			Parent = base.part.Modules.OfType<ModuleB9PartSwitch>().FirstOrDefault((ModuleB9PartSwitch module) => module.moduleID == parentID);
			if (Parent.IsNull())
			{
				LogError("Cannot find parent module with id '" + parentID + "'");
			}
			else
			{
				Parent.AddChild(this);
			}
		}
	}

	private void InitializeSubtypes(bool displayWarnings = true)
	{
		if (subtypes.Count == 0)
		{
			return;
		}
		foreach (PartSubtype inactiveSubtype in InactiveSubtypes)
		{
			inactiveSubtype.OnBeforeReinitializeInactiveSubtype();
		}
		CurrentSubtype.OnBeforeReinitializeActiveSubtype();
		foreach (PartSubtype subtype in subtypes)
		{
			subtype.Setup(this, displayWarnings);
		}
		reinitialzeModelTransactionManager.Initialize();
	}

	private void EnsureAtLeastOneUnrestrictedSubtype()
	{
		if (!subtypes.Any((PartSubtype subtype) => !subtype.HasUpgradeRequired))
		{
			SeriousWarningHandler.DisplaySeriousWarning($"{this}: must have at least one subtype without tech restrictions, removing tech restriction on first subtype");
			LogError("must have at least one subtype without tech restrictions, removing tech restriction on first subtype");
			subtypes[0].upgradeRequired = null;
		}
	}

	private void SetupForIcon()
	{
		PartSubtype item = subtypes.Where((PartSubtype s) => !s.HasUpgradeRequired).MaxBy((PartSubtype s) => s.defaultSubtypePriority);
		currentSubtypeIndex = subtypes.IndexOf(item);
		foreach (PartSubtype inactiveSubtype in InactiveSubtypes)
		{
			inactiveSubtype.DeactivateForIcon();
		}
		CurrentSubtype.ActivateForIcon();
	}

	private void FindBestSubtype()
	{
		PartSubtype partSubtype = null;
		if (subtypes.ValidIndex(currentSubtypeIndex))
		{
			if (CurrentSubtype.IsUnlocked())
			{
				return;
			}
			partSubtype = CurrentSubtype;
		}
		PartSubtype item = new BestSubtypeDeterminator().FindBestSubtype(resourceNamesOnPart: base.part.Resources.Select((PartResource resource) => resource.resourceName), subtypes: subtypes.Where((PartSubtype s) => s.IsUnlocked()));
		currentSubtypeIndex = subtypes.IndexOf(item);
		if (partSubtype.IsNotNull())
		{
			LockedSubtypeWarningHandler.WarnSubtypeLocked($"{this}: locked subtype '{partSubtype.title}' replaced with '{CurrentSubtype.title}'");
		}
	}

	private void SetupGUI()
	{
		int num = subtypes.Count((PartSubtype subtype) => subtype.IsUnlocked());
		BaseField baseField = base.Fields["currentSubtypeIndex"];
		baseField.guiName = switcherDescription;
		baseField.advancedTweakable = advancedTweakablesOnly;
		baseField.guiActiveEditor = num > 1;
		baseField.uiControlEditor.onFieldChanged = OnSliderUpdate;
		BaseEvent baseEvent = base.Events["ShowSubtypesWindow"];
		baseEvent.guiName = Localization.ModuleB9PartSwitch_SelectSubtype(switcherDescription);
		baseEvent.advancedTweakable = advancedTweakablesOnly;
		baseEvent.guiActiveEditor = num > 1;
		BaseField baseField2 = base.Fields["currentSubtypeTitle"];
		baseField2.guiName = switcherDescription;
		baseField2.advancedTweakable = advancedTweakablesOnly;
		baseField2.guiActiveEditor = num == 1;
		if (HighLogic.LoadedSceneIsFlight)
		{
			UpdateSwitchEventFlightVisibility();
		}
		this.SetUiGroups(uiGroupName, uiGroupDisplayName);
	}

	private void UpdateSwitchEventFlightVisibility()
	{
		bool flag = subtypes.Any((PartSubtype s) => s != CurrentSubtype && s.allowSwitchInFlight && s.IsUnlocked());
		base.Events["ShowSubtypesWindow"].guiActive = switchInFlight && flag;
		base.Fields["currentSubtypeTitle"].guiActive = switchInFlight && !flag;
	}

	private void UpdateOnStart()
	{
		reinitialzeModelTransactionManager.WithTransaction(delegate
		{
			foreach (PartSubtype inactiveSubtype in InactiveSubtypes)
			{
				inactiveSubtype.DeactivateOnStart();
			}
			RemoveUnusedResources();
			UpdateVolumeFromChildren();
			CurrentSubtype.ActivateOnStart();
		});
		needsNotifyFARToRevoxelize |= ChangesGeometry && affectFARVoxels;
		needsRecalculateDragCubes |= ChangesGeometry && affectDragCubes;
		currentSubtypeTitle = CurrentSubtype.title;
		LogInfo("Switched subtype to " + CurrentSubtype.Name);
	}

	private void UpdateOnStartFinished()
	{
		if (needsRecalculateDragCubes)
		{
			base.part.FixModuleJettison();
		}
		NotifyFARToRevoxelize();
		RecalculateDragCubes();
		foreach (PartSubtype inactiveSubtype in InactiveSubtypes)
		{
			inactiveSubtype.DeactivateOnStartFinished();
		}
		CurrentSubtype.ActivateOnStartFinished();
	}

	private void RemoveUnusedResources()
	{
		for (int num = base.part.Resources.Count - 1; num >= 0; num--)
		{
			PartResource partResource = base.part.Resources[num];
			if (IsManagedResource(partResource.resourceName) && !CurrentTankType.ContainsResource(partResource.resourceName))
			{
				base.part.Resources.Remove(partResource);
			}
		}
	}

	private void CheckOtherModules()
	{
		if (!ManagesResources)
		{
			return;
		}
		string[] array = INCOMAPTIBLE_MODULES_FOR_RESOURCE_SWITCHING.Where((string modName) => base.part.Modules.Contains(modName)).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		foreach (PartSubtype subtype in subtypes)
		{
			subtype.AssignStructuralTankType();
		}
		SeriousWarningHandler.DisplaySeriousWarning(string.Format("{0} and {1} - cannot both manage resources on the same part, B9 resource switching will be disabled", this, string.Join(", ", array)));
	}

	private void UpdateOnSwitch()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			string newSubtypeName = ((base.part.symMethod != SymmetryMethod.Mirror) ? CurrentSubtypeName : CurrentSubtype.mirrorSymmetrySubtype);
			foreach (ModuleB9PartSwitch item in this.FindSymmetryCounterparts())
			{
				item.UpdateFromSymmetry(newSubtypeName);
			}
			GameEvents.onEditorPartEvent.Fire(ConstructionEventType.PartTweaked, base.part);
			GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
		}
		else if (HighLogic.LoadedSceneIsFlight)
		{
			GameEvents.onVesselWasModified.Fire(base.vessel);
			UpdateSwitchEventFlightVisibility();
		}
		UpdatePartActionWindow();
	}

	private void UpdateFromSymmetry(string newSubtypeName)
	{
		reinitialzeModelTransactionManager.WithTransaction(delegate
		{
			CurrentSubtype.DeactivateOnSwitch();
			CurrentSubtypeName = newSubtypeName;
			UpdateSubtype();
		});
	}

	private void ReinitializeModel()
	{
		if (subtypes.Count == 0)
		{
			return;
		}
		foreach (PartSubtype inactiveSubtype in InactiveSubtypes)
		{
			inactiveSubtype.OnBeforeReinitializeInactiveSubtype();
		}
		CurrentSubtype.OnBeforeReinitializeActiveSubtype();
		foreach (PartSubtype subtype in subtypes)
		{
			subtype.Setup(this);
		}
		foreach (PartSubtype inactiveSubtype2 in InactiveSubtypes)
		{
			inactiveSubtype2.OnAfterReinitializeInactiveSubtype();
		}
		CurrentSubtype.OnAfterReinitializeActiveSubtype();
	}

	private void UpdateSubtype()
	{
		CurrentSubtype.ActivateOnSwitch();
		needsNotifyFARToRevoxelize |= ChangesGeometry && affectFARVoxels;
		needsRecalculateDragCubes |= ChangesGeometry && affectDragCubes;
		NotifyFARToRevoxelize();
		if (!HighLogic.LoadedSceneIsFlight)
		{
			RecalculateDragCubes();
		}
		Parent?.UpdateVolume();
		currentSubtypeTitle = CurrentSubtype.title;
		LogInfo("Switched subtype to " + CurrentSubtype.Name);
	}

	private void NotifyFARToRevoxelize()
	{
		if (FARWrapper.FARLoaded && needsNotifyFARToRevoxelize)
		{
			base.part.SendMessage("GeometryPartModuleRebuildMeshData");
			base.part.SendMessage("FarWasNotifiedToRevoxelize");
			needsNotifyFARToRevoxelize = false;
		}
	}

	private void RecalculateDragCubes()
	{
		if (needsRecalculateDragCubes)
		{
			if (HighLogic.LoadedSceneIsEditor && base.part.parent == null && EditorLogic.RootPart != base.part)
			{
				Part obj = base.part;
				obj.OnEditorAttach = (Callback)Delegate.Combine(obj.OnEditorAttach, new Callback(UpdateDragCubesOnAttach));
			}
			else
			{
				StartCoroutine(RenderProceduralDragCubes());
			}
			base.part.SendMessage("DragCubesWereRecalculated");
			needsRecalculateDragCubes = false;
		}
		IEnumerator RenderProceduralDragCubes()
		{
			float[] dragCubeWeights = base.part.DragCubes.Cubes.Select((DragCube cube) => cube.Weight).ToArray();
			base.part.DragCubes.ClearCubes();
			yield return DragCubeSystem.Instance.SetupDragCubeCoroutine(base.part, null);
			if (dragCubeWeights.Length == base.part.DragCubes.Cubes.Count)
			{
				for (int i = 0; i < dragCubeWeights.Length; i++)
				{
					base.part.DragCubes.Cubes[i].Weight = dragCubeWeights[i];
				}
			}
			else
			{
				LogError($"Cannot reassign cube weights: had {dragCubeWeights.Length} cubes before but now have {base.part.DragCubes.Cubes.Count}");
			}
			base.part.DragCubes.ForceUpdate(weights: true, occlusion: true);
		}
		void UpdateDragCubesOnAttach()
		{
			Part obj2 = base.part;
			obj2.OnEditorAttach = (Callback)Delegate.Remove(obj2.OnEditorAttach, new Callback(UpdateDragCubesOnAttach));
			StartCoroutine(UpdateDragCubesOnAttachCoroutine());
		}
		IEnumerator UpdateDragCubesOnAttachCoroutine()
		{
			yield return null;
			yield return RenderProceduralDragCubes();
		}
	}

	private void UpdatePartActionWindow()
	{
		UIPartActionWindow uIPartActionWindow = UIPartActionController.Instance?.GetItem(base.part, includeSymmetryCounterparts: false);
		if (!uIPartActionWindow.IsNull())
		{
			uIPartActionWindow.ClearList();
			uIPartActionWindow.displayDirty = true;
		}
	}

	private void UpdateVolumeFromChildren()
	{
		VolumeFromChildren = children.Sum((ModuleB9PartSwitch child) => child.VolumeAddedToParent);
	}
}
