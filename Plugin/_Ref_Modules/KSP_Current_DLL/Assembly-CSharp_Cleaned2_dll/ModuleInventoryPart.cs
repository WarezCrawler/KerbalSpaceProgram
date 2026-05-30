using System;
using System.Collections;
using System.Collections.Generic;
using Experience.Effects;
using Highlighting;
using UnityEngine;
using ns9;

public class ModuleInventoryPart : PartModule, IPartCostModifier, IPartMassModifier
{
	[KSPField]
	public string Inventory;

	[UI_Grid(columnCount = 3)]
	[KSPField(guiActiveUnfocused = true, guiActive = true, unfocusedRange = 5f, guiName = "#autoLOC_8320000")]
	public int InventorySlots = 9;

	[KSPField]
	public float placementRotateSpeed = 80f;

	[KSPField]
	public float placementOpacity = 0.4f;

	[KSPField]
	public float placementGroundOffset = 0.1f;

	[KSPField]
	public float placementGroundOffsetCap = 0.5f;

	[KSPField]
	public string backPackTransformName = "kerbalCargoContainerPack";

	[KSPField]
	public float allowedKerbalEvaDistance = 5f;

	private RaycastHit positionFromTerrainHit;

	private UI_Grid grid;

	private List<string> inventoryPartsByName;

	private bool hasInitialized;

	private List<string> availableCargoParts = new List<string>();

	private List<string> availableCargoInventoryParts = new List<string>();

	[SerializeField]
	private float spawnDistance = 1f;

	[SerializeField]
	private int maxInventorySlots = 30;

	public int transferToSlot = -1;

	public int transferFromSlot = -1;

	public Transform backpackTransform;

	private bool isGridShowing;

	[SerializeField]
	private int placementPendingIndex = -1;

	[SerializeField]
	private Part selectedPart;

	[SerializeField]
	private ModuleGroundPart selectedPartModuleGround;

	[SerializeField]
	private Bounds selectedPartBounds;

	[SerializeField]
	private bool placementAllowXRotation;

	[SerializeField]
	private bool placementAllowYRotation;

	[SerializeField]
	private bool placementAllowZRotation;

	private bool placementCoolDown;

	private double placementCooldownTimer;

	private AudioSource placementNotAllowedsFX;

	public FXGroup placementNotAllowedGroup;

	private bool placementPositionOk;

	private bool placementInsideCap;

	private bool placementonTerrain;

	private bool partFullyCreated;

	public bool partBeingRetrieved;

	private Vector3 placementStartingPosition;

	private List<string> listedItemsToPut = new List<string>();

	public List<string> InventoryPartsList
	{
		get
		{
			if (inventoryPartsByName == null)
			{
				return new List<string>();
			}
			return new List<string>(inventoryPartsByName);
		}
	}

	public int InventoryItemCount
	{
		get
		{
			if (inventoryPartsByName == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < inventoryPartsByName.Count; i++)
			{
				if (!string.IsNullOrEmpty(inventoryPartsByName[i]))
				{
					num++;
				}
			}
			return num;
		}
	}

	public bool AbleToPlaceParts
	{
		get
		{
			if (base.part != null && base.part.vessel != null && base.part.vessel.isEVA && base.part.vessel.Landed && base.part.vessel.isActiveVessel)
			{
				return selectedPart == null;
			}
			return false;
		}
	}

	public bool IsKerbalOnEVA
	{
		get
		{
			if (base.part != null && base.part.vessel != null)
			{
				return base.part.vessel.isEVA;
			}
			return false;
		}
	}

	public Part SelectedPart => selectedPart;

	public bool PlacementAllowXRotation => placementAllowXRotation;

	public bool PlacementAllowYRotation => placementAllowYRotation;

	public bool PlacementAllowZRotation => placementAllowZRotation;

	private void OnEditorPartEvent(ConstructionEventType evt, Part p)
	{
		switch (evt)
		{
		case ConstructionEventType.PartDropped:
			PartDropppedOnInventory(evt, p);
			break;
		case ConstructionEventType.PartCreated:
		case ConstructionEventType.PartPicked:
		case ConstructionEventType.PartDragging:
			VesselEditorPartHighlighter(p);
			break;
		case ConstructionEventType.PartDeleted:
			PartDropppedOnInventory(evt, p);
			break;
		case ConstructionEventType.PartAttached:
		case ConstructionEventType.PartDetached:
			break;
		}
	}

	public void OnFlightPartEvent(bool held)
	{
		if (held)
		{
			if (TotalEmptySlots() >= 1)
			{
				base.part.highlightColor = Part.defaultHighlightPart;
				base.part.Highlight(active: true);
				base.part.HighlightActive = true;
			}
			else
			{
				base.part.highlightColor = Highlighter.colorPartInventoryUnAvailableSpace;
				base.part.Highlight(active: true);
				base.part.HighlightActive = true;
			}
		}
		else
		{
			base.part.highlightColor = Part.defaultHighlightPart;
			base.part.Highlight(active: false);
			base.part.HighlightActive = false;
		}
	}

	private void VesselEditorPartHighlighter(Part p)
	{
		if (availableCargoParts.Contains(p.name))
		{
			if (TotalEmptySlots() >= 1)
			{
				base.part.Highlight(active: true);
				base.part.HighlightActive = true;
			}
			else
			{
				base.part.highlightColor = Highlighter.colorPartInventoryUnAvailableSpace;
				base.part.Highlight(active: true);
				base.part.HighlightActive = true;
			}
		}
	}

	private void PartDropppedOnInventory(ConstructionEventType evt, Part p)
	{
		if (availableCargoInventoryParts.Contains(p.name))
		{
			return;
		}
		if (base.part.HighlightActive)
		{
			base.part.highlightColor = Part.defaultHighlightPart;
			base.part.Highlight(active: false);
		}
		if (evt != ConstructionEventType.PartDropped || !base.part.MouseOver || TotalEmptySlots() <= 0)
		{
			return;
		}
		int index = FirstEmptySlot();
		if (StoreCargoPartAtSlot(p.partInfo.name, updatePAW: false, index))
		{
			if (grid != null && grid.pawInventory != null && grid.pawInventory.gameObject.activeInHierarchy)
			{
				grid.pawInventory.slotPartIcon[index].Create(null, p.partInfo, grid.pawInventory.iconSize, 1f, 1f, grid.pawInventory.StartPartPlacement, AbleToPlaceParts);
			}
			UnityEngine.Object.Destroy(p.gameObject);
		}
	}

	public void SlotClickedFirst(int index)
	{
		transferFromSlot = index;
	}

	public void SlotClickedSetParts(AvailablePart ap, int index)
	{
		if (UIPartActionController.Instance != null && UIPartActionController.Instance.InventoryAndCargoPartExist())
		{
			transferFromSlot = UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.slotIndex;
			transferToSlot = index;
			ExchangeItemsBetweenSlots(ap);
		}
	}

	public void ResetInventoryCacheVars()
	{
		if (grid != null)
		{
			grid.updateSlotItems = true;
		}
		transferFromSlot = -1;
		transferToSlot = -1;
	}

	public virtual void DeployInventoryItem(int index)
	{
		if (AbleToPlaceParts && InventoryItemCanBeDeployed(index) && !partBeingRetrieved)
		{
			UIPartActionController.Instance.partInventory.DestroyTooltip();
			placementPendingIndex = index;
			StartCoroutine(CreatePartObject(inventoryPartsByName[index]));
		}
		else
		{
			grid.pawInventory.ResetPlacePartIcons(active: true);
		}
	}

	public virtual bool InventoryItemCanBeDeployed(int index)
	{
		if (index > -1 && inventoryPartsByName.Count > index && selectedPart == null && !string.IsNullOrEmpty(inventoryPartsByName[index]))
		{
			AvailablePart partInfoByName = PartLoader.getPartInfoByName(inventoryPartsByName[index]);
			if (partInfoByName != null && partInfoByName.partPrefab.FindModuleImplementing<ModuleGroundPart>() != null)
			{
				return true;
			}
		}
		return false;
	}

	private IEnumerator CreatePartObject(string partName)
	{
		partFullyCreated = false;
		AvailablePart partInfoByName = PartLoader.getPartInfoByName(partName);
		if (partInfoByName == null)
		{
			yield break;
		}
		if (base.vessel.isEVA && base.vessel.evaController != null)
		{
			if (!base.vessel.evaController.SetPartPlacementMode(mode: true, this))
			{
				yield break;
			}
			if (base.vessel.evaController.JetpackDeployed)
			{
				base.vessel.evaController.ToggleJetpack();
			}
		}
		Part part = UnityEngine.Object.Instantiate(partInfoByName.partPrefab);
		part.ResumeState = PartStates.PLACEMENT;
		part.State = PartStates.PLACEMENT;
		part.name = partInfoByName.name;
		part.persistentId = FlightGlobals.CheckPartpersistentId(part.persistentId, part, removeOldId: false, addNewId: true);
		if (part.variants != null && partInfoByName.variant != null && partInfoByName.variant.Name != null)
		{
			part.variants.SetVariant(partInfoByName.variant.Name);
		}
		selectedPart = part;
		selectedPartModuleGround = selectedPart.FindModuleImplementing<ModuleGroundPart>();
		if (selectedPartModuleGround != null)
		{
			placementAllowXRotation = selectedPartModuleGround.placementAllowXRotation;
			placementAllowYRotation = selectedPartModuleGround.placementAllowYRotation;
			placementAllowZRotation = selectedPartModuleGround.placementAllowZRotation;
		}
		else
		{
			placementAllowXRotation = true;
			placementAllowYRotation = true;
			placementAllowZRotation = true;
		}
		selectedPart.transform.rotation = (base.part ? base.part.transform.rotation : selectedPart.initRotation);
		Collider[] componentsInChildren = selectedPart.transform.Find("model").GetComponentsInChildren<Collider>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].isTrigger = true;
		}
		selectedPart.gameObject.SetActive(value: true);
		selectedPartBounds = selectedPart.GetPartRendererBound();
		MeshRenderer[] componentsInChildren2 = selectedPart.GetComponentsInChildren<MeshRenderer>();
		SkinnedMeshRenderer[] componentsInChildren3 = selectedPart.GetComponentsInChildren<SkinnedMeshRenderer>();
		Bounds bounds = default(Bounds);
		for (int j = 0; j < componentsInChildren2.Length; j++)
		{
			bounds.Encapsulate(componentsInChildren2[j].bounds);
		}
		for (int k = 0; k < componentsInChildren3.Length; k++)
		{
			bounds.Encapsulate(componentsInChildren3[k].sharedMesh.bounds);
		}
		UnityEngine.Object.Destroy(selectedPart.rb);
		selectedPart.physicalSignificance = Part.PhysicalSignificance.NONE;
		selectedPart.partInfo = partInfoByName;
		spawnDistance = Mathf.Max(selectedPartBounds.extents.x, selectedPartBounds.extents.y, selectedPartBounds.extents.z) + 0.2f;
		selectedPart.transform.position = base.vessel.vesselTransform.position + base.vessel.vesselTransform.forward * spawnDistance;
		SetPositionFromTerrain();
		selectedPart.attRotation = Quaternion.identity;
		selectedPart.attRotation0 = selectedPart.transform.localRotation;
		selectedPart.attPos0 = selectedPart.transform.localPosition;
		selectedPart.transform.SetParent(base.vessel.transform);
		yield return null;
		selectedPart.highlighter.ReinitMaterials();
		yield return null;
		placementPositionOk = true;
		selectedPart.SetHighlightColor(Highlighter.colorPartHighlightDefault);
		selectedPart.SetHighlightType(Part.HighlightType.AlwaysOn);
		selectedPart.SetHighlight(active: true, recursive: true);
		for (int l = 0; l < FlightGlobals.PersistentLoadedPartIds.ValuesList.Count; l++)
		{
			if (FlightGlobals.PersistentLoadedPartIds.ValuesList[l].persistentId != selectedPart.persistentId)
			{
				FlightGlobals.PersistentLoadedPartIds.ValuesList[l].SetHighlightType(Part.HighlightType.Disabled);
			}
		}
		for (int m = 0; m < selectedPart.Modules.Count; m++)
		{
			selectedPart.Modules[m].enabled = false;
		}
		UIPartActionWindow item = UIPartActionController.Instance.GetItem(base.part, includeSymmetryCounterparts: false);
		if (item != null)
		{
			item.isValid = false;
		}
		partFullyCreated = true;
	}

	private void SetPositionFromTerrain()
	{
		float num = 50f;
		placementStartingPosition = base.vessel.vesselTransform.position + base.vessel.vesselTransform.forward * spawnDistance;
		Vector3 vector = placementStartingPosition;
		Vector3d upAxis = FlightGlobals.getUpAxis(base.vessel.mainBody, placementStartingPosition);
		float maxDistance = (float)(vector - base.vessel.mainBody.position).magnitude;
		vector += upAxis * 50.0;
		selectedPart.transform.position = upAxis * 100.0 + placementStartingPosition;
		if (Physics.Raycast(vector, -upAxis, out positionFromTerrainHit, maxDistance, 0x8000 | LayerUtil.DefaultEquivalent))
		{
			if (positionFromTerrainHit.collider.gameObject.layer == 15 && !positionFromTerrainHit.collider.gameObject.CompareTag("ROC"))
			{
				placementonTerrain = true;
			}
			else
			{
				placementonTerrain = false;
			}
			if (base.vessel != null && base.vessel.Splashed)
			{
				placementonTerrain = false;
			}
			Debug.DrawLine(vector, vector + -upAxis * positionFromTerrainHit.distance, Color.green, 2f);
			float num2 = positionFromTerrainHit.distance - num;
			float centerPointOffset = 0f;
			float boundsPoints = selectedPart.GetBoundsPoints(positionFromTerrainHit.normal, out centerPointOffset);
			boundsPoints += placementGroundOffset;
			float num3 = Mathf.Cos(Mathf.Abs((float)Vector3d.Angle(positionFromTerrainHit.normal, upAxis)) * ((float)Math.PI / 180f)) * boundsPoints;
			if (Mathf.Abs(boundsPoints - num3) > 0.1f)
			{
				boundsPoints = num3;
			}
			if (Mathf.Abs(num2) > placementGroundOffsetCap)
			{
				placementInsideCap = false;
				num2 = Mathf.Clamp(num2, 0f - placementGroundOffsetCap + centerPointOffset, placementGroundOffsetCap + centerPointOffset);
			}
			else
			{
				placementInsideCap = true;
			}
			if (num2 > boundsPoints)
			{
				boundsPoints = num2 - boundsPoints;
				selectedPart.transform.position = placementStartingPosition + upAxis * (0f - boundsPoints);
			}
			else
			{
				boundsPoints -= num2;
				selectedPart.transform.position = placementStartingPosition + upAxis * boundsPoints;
			}
		}
		else
		{
			selectedPart.transform.position = placementStartingPosition;
			placementonTerrain = false;
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (selectedPart != null)
		{
			Vector3 center = selectedPartBounds.center;
			float magnitude = selectedPartBounds.extents.magnitude;
			Gizmos.color = Color.magenta;
			Gizmos.DrawSphere(selectedPart.transform.position + center, 0.02f);
			Gizmos.DrawWireSphere(selectedPart.transform.position + center, magnitude);
			Gizmos.color = Color.red;
			Gizmos.DrawWireCube(selectedPart.transform.position + selectedPartBounds.center, selectedPartBounds.size);
		}
	}

	private void DeletePartObject()
	{
		if (selectedPart != null)
		{
			selectedPart.OnDelete();
			UnityEngine.Object.Destroy(selectedPart.gameObject);
			selectedPart = null;
		}
		for (int i = 0; i < FlightGlobals.PersistentLoadedPartIds.ValuesList.Count; i++)
		{
			Part part = FlightGlobals.PersistentLoadedPartIds.ValuesList[i];
			if (!(part != null))
			{
				continue;
			}
			if (part.partInfo.category == PartCategories.Cargo)
			{
				if (part.State != PartStates.PLACEMENT)
				{
					ResetLoadedPartsHighlight(part);
				}
			}
			else
			{
				ResetLoadedPartsHighlight(part);
			}
		}
	}

	private void ResetLoadedPartsHighlight(Part loadedPart)
	{
		loadedPart.SetHighlightColor(Highlighter.colorPartHighlightDefault);
		loadedPart.SetHighlightType(Part.HighlightType.OnMouseOver);
	}

	public bool StoreCargoPartAtSlot(string partName, bool updatePAW = true, int index = -1)
	{
		if (!CheckPartStorage(partName, index))
		{
			Debug.LogWarningFormat("[ModuleInventoryPart]: Unable to store {0} into {1}-{2} as there are no empty slots available.", partName, base.part.partInfo.title, base.part.persistentId);
			return false;
		}
		if (index == -1)
		{
			index = FirstEmptySlot();
		}
		inventoryPartsByName[index] = partName;
		if (updatePAW)
		{
			UpdatePAW(partName, index);
		}
		UpdateModuleUI();
		GameEvents.onModuleInventoryChanged.Fire(this);
		if (HighLogic.LoadedSceneIsEditor)
		{
			StartCoroutine(UpdateEngineerReport());
		}
		return true;
	}

	public bool CheckPartStorage(string partName, int index = -1)
	{
		if (index == -1)
		{
			index = FirstEmptySlot();
		}
		if (index == -1)
		{
			return false;
		}
		return true;
	}

	private void UpdatePAW(string partName, int indexToStore)
	{
		if (grid != null && grid.pawInventory != null && grid.pawInventory.gameObject.activeInHierarchy && availableCargoParts != null && availableCargoParts.Contains(partName))
		{
			AvailablePart partInfoByName = PartLoader.getPartInfoByName(partName);
			if (partInfoByName != null)
			{
				grid.pawInventory.slotPartIcon[indexToStore].Create(null, partInfoByName.partPrefab.partInfo, grid.pawInventory.iconSize, 1f, 1f, grid.pawInventory.StartPartPlacement, AbleToPlaceParts);
			}
		}
	}

	private void ExchangeItemsBetweenSlots(AvailablePart ap)
	{
		if (UIPartActionController.Instance == null || !UIPartActionController.Instance.InventoryAndCargoPartExist())
		{
			return;
		}
		string partName = UIPartActionController.Instance.partInventory.CurrentCargoPart.name;
		string partName2 = ((ap == null) ? "" : ap.name);
		if (UIPartActionController.Instance.partInventory.CurrentInventory != null)
		{
			if (UIPartActionController.Instance.partInventory.CurrentInventory.StoreCargoPartAtSlot(partName2, updatePAW: false, UIPartActionController.Instance.partInventory.CurrentInventory.transferFromSlot))
			{
				if (UIPartActionController.Instance.partInventory.ExchangedCargoPart != null)
				{
					partName2 = UIPartActionController.Instance.partInventory.ExchangedCargoPart.name;
					StoreCargoPartAtSlot(partName2, updatePAW: false, transferToSlot);
				}
				else
				{
					StoreCargoPartAtSlot(partName, updatePAW: false, transferToSlot);
				}
			}
		}
		else
		{
			StoreCargoPartAtSlot(partName, updatePAW: false, transferToSlot);
		}
		if (UIPartActionController.Instance.partInventory.CurrentInventory != null)
		{
			if (UIPartActionController.Instance.partInventory.CurrentInventory != this)
			{
				UIPartActionController.Instance.partInventory.CurrentInventory.grid.pawInventory.SetAllSlotsNotSelected();
			}
			else
			{
				UIPartActionController.Instance.partInventory.CurrentInventory.ResetInventoryCacheVars();
				UIPartActionController.Instance.partInventory.CurrentInventory.grid.pawInventory.SetAllSlotsNotSelected();
			}
		}
		ResetInventoryCacheVars();
		UIPartActionController.Instance.partInventory.CurrentCargoPart = null;
		UIPartActionController.Instance.partInventory.CurrentInventory = null;
	}

	private IEnumerator UpdateEngineerReport()
	{
		yield return null;
		if (EditorLogic.fetch != null && EditorLogic.fetch.ship != null)
		{
			GameEvents.onEditorShipModified.Fire(EditorLogic.fetch.ship);
		}
	}

	private void OnModuleInventoryChanged(ModuleInventoryPart moduleInventory)
	{
		if (moduleInventory == this && InventoryItemCount > 0 && backpackTransform != null && !backpackTransform.gameObject.activeSelf)
		{
			backpackTransform.gameObject.SetActive(value: true);
		}
	}

	public override void OnStart(StartState state)
	{
		if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
		{
			return;
		}
		InitializeInventory();
		if (HighLogic.LoadedSceneIsFlight)
		{
			grid = base.Fields["InventorySlots"].uiControlFlight as UI_Grid;
			GameEvents.onFlightCargoPartHeld.Add(OnFlightPartEvent);
			base.Fields["InventorySlots"].guiActive = true;
		}
		if (HighLogic.LoadedSceneIsEditor)
		{
			grid = base.Fields["InventorySlots"].uiControlEditor as UI_Grid;
			GameEvents.onEditorPartEvent.Add(OnEditorPartEvent);
		}
		grid.inventoryItems = inventoryPartsByName;
		grid.inventoryPart = this;
		if (HighLogic.LoadedSceneIsFlight && base.part.HasModuleImplementing<KerbalEVA>())
		{
			backpackTransform = base.part.FindModelTransform(backPackTransformName);
			if (backpackTransform != null)
			{
				Shader shader = Shader.Find("KSP/Bumped Specular");
				MeshRenderer[] componentsInChildren = backpackTransform.GetComponentsInChildren<MeshRenderer>();
				if (componentsInChildren != null)
				{
					for (int i = 0; i < componentsInChildren.Length; i++)
					{
						componentsInChildren[i].material.shader = shader;
					}
				}
				if (InventoryItemCount <= 0)
				{
					backpackTransform.gameObject.SetActive(value: false);
					GameEvents.onModuleInventoryChanged.Add(OnModuleInventoryChanged);
				}
			}
		}
		isGridShowing = HighLogic.LoadedSceneIsEditor || KerbalEVA.alwaysShowInventory || !base.vessel.isEVA || (base.vessel.isEVA && PartLoader.Instance.CargoPartsLoaded);
		ToggleInventoryVisibility(isGridShowing);
		GameEvents.onPartActionUICreate.Add(onPartActionUIOpened);
		SetupSoundFX();
	}

	private void SetupSoundFX()
	{
		placementNotAllowedGroup = base.part.findFxGroup("placementNotAllowed");
		if (placementNotAllowedGroup != null)
		{
			placementNotAllowedGroup.setActive(value: false);
			placementNotAllowedsFX = base.gameObject.GetComponent<AudioSource>();
			if (placementNotAllowedsFX == null)
			{
				placementNotAllowedsFX = base.gameObject.AddComponent<AudioSource>();
			}
			placementNotAllowedsFX.playOnAwake = false;
			placementNotAllowedsFX.loop = false;
			placementNotAllowedsFX.rolloffMode = AudioRolloffMode.Linear;
			placementNotAllowedsFX.dopplerLevel = 0f;
			placementNotAllowedsFX.volume = GameSettings.SHIP_VOLUME;
			placementNotAllowedsFX.spatialBlend = 1f;
			placementNotAllowedGroup.begin(placementNotAllowedsFX);
		}
	}

	public void OnDestroy()
	{
		GameEvents.onPartActionUICreate.Remove(onPartActionUIOpened);
		GameEvents.onEditorPartEvent.Remove(OnEditorPartEvent);
		GameEvents.onFlightCargoPartHeld.Remove(OnFlightPartEvent);
		GameEvents.onModuleInventoryChanged.Remove(OnModuleInventoryChanged);
	}

	public override void OnLoad(ConfigNode node)
	{
		InitializeInventory();
		if (!node.TryGetValue("inventory", ref Inventory))
		{
			return;
		}
		string[] array = Inventory.Split(',');
		for (int i = 0; i < array.Length && i < InventorySlots; i++)
		{
			if (!string.IsNullOrEmpty(array[i]) && availableCargoParts.Contains(array[i]))
			{
				inventoryPartsByName[i] = array[i];
			}
		}
	}

	public override void OnSave(ConfigNode node)
	{
		Inventory = "";
		bool flag = false;
		if (inventoryPartsByName != null)
		{
			for (int i = 0; i < inventoryPartsByName.Count; i++)
			{
				if (!string.IsNullOrEmpty(inventoryPartsByName[i]))
				{
					if (flag)
					{
						Inventory += ",";
					}
					Inventory += inventoryPartsByName[i];
					flag = true;
				}
			}
		}
		if (!string.IsNullOrEmpty(Inventory))
		{
			node.AddValue("inventory", Inventory);
		}
	}

	private void Update()
	{
		if ((HighLogic.LoadedSceneIsFlight || HighLogic.LoadedSceneIsEditor) && base.part.MouseOver && UIPartActionController.Instance.partInventory.CurrentCargoPart != null)
		{
			UIPartActionController.Instance.partInventory.OnInventoryPartHover(base.part.persistentId, base.part);
		}
	}

	public override void OnUpdate()
	{
		if (!HighLogic.LoadedSceneIsFlight && !HighLogic.LoadedSceneIsEditor)
		{
			return;
		}
		if (selectedPart != null)
		{
			if (!Input.GetKeyDown(KeyCode.Escape) && (!base.vessel.isEVA || !base.vessel.evaController.JetpackDeployed))
			{
				if (GameSettings.EVA_Jump.GetKeyDown() && base.vessel.isActiveVessel)
				{
					if (selectedPart.currentCollisions.Count == 0 && partFullyCreated && placementInsideCap && placementonTerrain)
					{
						Vector3 position = selectedPart.transform.position;
						Quaternion rotation = Quaternion.Inverse(base.vessel.mainBody.bodyTransform.rotation) * selectedPart.transform.rotation;
						DeletePartObject();
						DeployGroundPart(inventoryPartsByName[placementPendingIndex], position, rotation);
						GameEvents.onDeployGroundPart.Fire(inventoryPartsByName[placementPendingIndex]);
						StoreCargoPartAtSlot("", updatePAW: false, placementPendingIndex);
						if (grid.pawInventory != null && grid.pawInventory.gameObject.activeInHierarchy)
						{
							grid.pawInventory.DestroyItem(placementPendingIndex);
						}
						placementPendingIndex = -1;
						placementCoolDown = true;
						placementCooldownTimer = Planetarium.GetUniversalTime();
						partFullyCreated = false;
					}
					else if (placementNotAllowedsFX != null && !placementNotAllowedsFX.isPlaying)
					{
						placementNotAllowedsFX.Play();
					}
					return;
				}
				if (GameSettings.RCS_TOGGLE.GetKeyDown())
				{
					selectedPart.transform.rotation = (base.part ? base.part.transform.rotation : selectedPart.initRotation);
				}
				if (placementAllowXRotation)
				{
					if (GameSettings.TRANSLATE_UP.GetKey())
					{
						selectedPart.transform.Rotate(Vector3.right * placementRotateSpeed * Time.deltaTime);
					}
					if (GameSettings.TRANSLATE_DOWN.GetKey())
					{
						selectedPart.transform.Rotate(-Vector3.right * placementRotateSpeed * Time.deltaTime);
					}
				}
				if (placementAllowYRotation)
				{
					if (GameSettings.TRANSLATE_LEFT.GetKey())
					{
						selectedPart.transform.Rotate(Vector3.up * placementRotateSpeed * Time.deltaTime);
					}
					if (GameSettings.TRANSLATE_RIGHT.GetKey())
					{
						selectedPart.transform.Rotate(-Vector3.up * placementRotateSpeed * Time.deltaTime);
					}
				}
				if (placementAllowZRotation)
				{
					if (GameSettings.TRANSLATE_FWD.GetKey())
					{
						selectedPart.transform.Rotate(Vector3.forward * placementRotateSpeed * Time.deltaTime);
					}
					if (GameSettings.TRANSLATE_BACK.GetKey())
					{
						selectedPart.transform.Rotate(-Vector3.forward * placementRotateSpeed * Time.deltaTime);
					}
				}
				SetPositionFromTerrain();
				if (selectedPart.currentCollisions.Count == 0 && placementInsideCap && placementonTerrain)
				{
					if (!placementPositionOk)
					{
						selectedPart.SetHighlightColor(Highlighter.colorPartHighlightDefault);
						selectedPart.SetHighlight(active: true, recursive: true);
						placementPositionOk = true;
					}
				}
				else if (placementPositionOk)
				{
					selectedPart.SetHighlightColor(Highlighter.colorPartEditorDetached);
					selectedPart.SetHighlight(active: true, recursive: true);
					placementPositionOk = false;
				}
			}
			else
			{
				CancelPartPlacementMode();
			}
		}
		else if (placementCoolDown && Planetarium.GetUniversalTime() - placementCooldownTimer > 1.0)
		{
			placementCoolDown = false;
			if (base.vessel.isEVA && base.vessel.evaController != null)
			{
				base.vessel.evaController.SetPartPlacementMode(mode: false, null);
			}
			grid.pawInventory.ResetPlacePartIcons(active: true);
		}
	}

	public override string GetInfo()
	{
		return Localizer.Format("#autoLOC_8004190", Localizer.Format("#autoLOC_8002218"), InventorySlots) + "\n";
	}

	public override string GetModuleDisplayName()
	{
		return Localizer.Format("#autoLOC_8002219");
	}

	internal void KerbalStateChanged()
	{
		if (grid != null && grid.pawInventory != null)
		{
			grid.pawInventory.ResetPlacePartIcons(active: true);
		}
	}

	private void ToggleInventoryVisibility(bool show)
	{
		base.Fields["InventorySlots"].guiActive = show;
		base.Fields["InventorySlots"].guiActiveEditor = show;
	}

	private void onPartActionUIOpened(Part p)
	{
		if (p == base.part)
		{
			UpdateModuleUI();
		}
	}

	private void InitializeInventory()
	{
		availableCargoParts = PartLoader.Instance.GetAvailableCargoPartNames();
		availableCargoInventoryParts = PartLoader.Instance.GetAvailableCargoInventoryPartNames();
		if (hasInitialized)
		{
			return;
		}
		if (InventorySlots > maxInventorySlots)
		{
			InventorySlots = maxInventorySlots;
		}
		if (inventoryPartsByName == null)
		{
			inventoryPartsByName = new List<string>();
			for (int i = 0; i < InventorySlots; i++)
			{
				inventoryPartsByName.Add("");
			}
		}
		hasInitialized = true;
	}

	public Part CreatePartForInventoryUse(AvailablePart partInfo)
	{
		Part part = UnityEngine.Object.Instantiate(partInfo.partPrefab);
		if (availableCargoParts != null && availableCargoParts.Contains(partInfo.name))
		{
			part.ResumeState = PartStates.PLACEMENT;
			part.State = PartStates.PLACEMENT;
			part.SetupHighlighter();
			part.RefreshHighlighter();
			part.SetHighlightType(Part.HighlightType.AlwaysOn);
			part.SetHighlight(active: true, recursive: true);
		}
		part.gameObject.SetActive(value: true);
		part.name = partInfo.name;
		part.persistentId = FlightGlobals.CheckPartpersistentId(part.persistentId, part, removeOldId: false, addNewId: true);
		if (part.variants != null && partInfo.variant != null && partInfo.variant.Name != null)
		{
			part.variants.SetVariant(partInfo.variant.Name);
		}
		return part;
	}

	private void UpdateModuleUI()
	{
		if (hasInitialized && grid != null)
		{
			grid.updateSlotItems = true;
		}
	}

	public void CancelPartPlacementMode()
	{
		if (selectedPart != null)
		{
			placementCoolDown = true;
			placementCooldownTimer = Planetarium.GetUniversalTime();
			DeletePartObject();
		}
	}

	public int FirstEmptySlot()
	{
		int num = 0;
		while (true)
		{
			if (num < inventoryPartsByName.Count)
			{
				if (string.IsNullOrEmpty(inventoryPartsByName[num]))
				{
					break;
				}
				num++;
				continue;
			}
			return -1;
		}
		return num;
	}

	public int FirstFullSlot()
	{
		int num = 0;
		while (true)
		{
			if (num < inventoryPartsByName.Count)
			{
				if (!string.IsNullOrEmpty(inventoryPartsByName[num]))
				{
					break;
				}
				num++;
				continue;
			}
			return -1;
		}
		return num;
	}

	public int TotalEmptySlots()
	{
		int num = 0;
		for (int i = 0; i < InventorySlots; i++)
		{
			if (string.IsNullOrEmpty(inventoryPartsByName[i]))
			{
				num++;
			}
		}
		return num;
	}

	public override void OnCopy(PartModule fromModule)
	{
		ModuleInventoryPart moduleInventoryPart = fromModule as ModuleInventoryPart;
		inventoryPartsByName = new List<string>();
		inventoryPartsByName.AddRange(moduleInventoryPart.inventoryPartsByName);
	}

	private void DeployGroundPart(string partName, Vector3 partPosition, Quaternion rotation)
	{
		ConfigNode protoVesselNode = GetProtoVesselNode(partName, partPosition, rotation);
		HighLogic.CurrentGame.AddVessel(protoVesselNode);
	}

	private ConfigNode GetProtoVesselNode(string partName, Vector3 partPosition, Quaternion rotation)
	{
		uint uniqueFlightID = ShipConstruction.GetUniqueFlightID(HighLogic.CurrentGame.flightState);
		Orbit orbit = Orbit.CreateRandomOrbitAround(base.vessel.mainBody);
		AvailablePart partInfoByName = PartLoader.getPartInfoByName(partName);
		ModuleGroundSciencePart sciencePart = null;
		ModuleGroundExperiment experimentPart = null;
		string vesselName = partName;
		if (partInfoByName != null)
		{
			sciencePart = partInfoByName.partPrefab.FindModuleImplementing<ModuleGroundSciencePart>();
			experimentPart = partInfoByName.partPrefab.FindModuleImplementing<ModuleGroundExperiment>();
			partInfoByName.partPrefab.FindModuleImplementing<ModuleGroundExpControl>();
			vesselName = partInfoByName.partPrefab.partInfo.title;
		}
		new ConfigNode();
		ConfigNode configNode = ProtoVessel.CreateVesselNode(vesselName, VesselType.DeployedScienceController, orbit, 0, new ConfigNode[1] { CreatePartNode(uniqueFlightID, partName, partInfoByName, sciencePart, experimentPart) });
		configNode.SetValue("landedAt", base.vessel.mainBody.name);
		double alt = base.vessel.altitude;
		double lat = base.vessel.latitude;
		double lon = base.vessel.longitude;
		base.vessel.mainBody.GetLatLonAlt(partPosition, out lat, out lon, out alt);
		configNode.SetValue("lat", lat);
		configNode.SetValue("lon", lon);
		configNode.SetValue("sit", Vessel.Situations.LANDED.ToString());
		configNode.SetValue("alt", alt);
		configNode.SetValue("landed", newValue: true);
		configNode.SetValue("splashed", newValue: false);
		configNode.SetValue("skipGroundPositioning", newValue: false);
		configNode.SetValue("vesselSpawning", newValue: true);
		configNode.AddValue("prst", value: true);
		_ = (Vector3)base.vessel.mainBody.GetRelSurfacePosition(lat, lon, alt);
		configNode.AddValue("rot", rotation);
		configNode.SetValue("skipGroundPositioning", newValue: false);
		configNode.SetValue("PQSMin", 0, createIfNotFound: true);
		configNode.SetValue("PQSMax", 0, createIfNotFound: true);
		return configNode;
	}

	private ConfigNode CreatePartNode(uint id, string partName, AvailablePart availablePart, ModuleGroundSciencePart sciencePart, ModuleGroundExperiment experimentPart)
	{
		ConfigNode configNode = ProtoVessel.CreatePartNode(partName, id, (ProtoCrewMember[])null);
		configNode.SetValue("flag", base.part.flagURL, createIfNotFound: true);
		if (availablePart != null)
		{
			float num = 0f;
			if (sciencePart != null && sciencePart.PowerUnitsProduced > 0 && base.vessel.isEVA && base.vessel.parts[0].protoModuleCrew[0].HasEffect<DeployedSciencePowerSkill>())
			{
				DeployedSciencePowerSkill effect = base.vessel.parts[0].protoModuleCrew[0].GetEffect<DeployedSciencePowerSkill>();
				if (effect != null)
				{
					num = effect.GetValue();
				}
			}
			float num2 = 0f;
			if (experimentPart != null && base.vessel.isEVA && base.vessel.parts[0].protoModuleCrew[0].HasEffect<DeployedScienceExpSkill>())
			{
				DeployedScienceExpSkill effect2 = base.vessel.parts[0].protoModuleCrew[0].GetEffect<DeployedScienceExpSkill>();
				if (effect2 != null)
				{
					num2 = effect2.GetValue() * 100f;
				}
			}
			ConfigNode[] nodes = availablePart.partConfig.GetNodes("MODULE");
			for (int i = 0; i < nodes.Length; i++)
			{
				ConfigNode configNode2 = nodes[i].CreateCopy();
				if (num > 0f)
				{
					string value = "";
					if (configNode2.TryGetValue("name", ref value) && value == "ModuleGroundSciencePart")
					{
						int value2 = 0;
						if (configNode2.TryGetValue("powerUnitsProduced", ref value2))
						{
							configNode2.SetValue("powerUnitsProduced", (float)value2 + num);
						}
					}
				}
				if (num2 > 0f)
				{
					string value3 = "";
					if (configNode2.TryGetValue("name", ref value3) && value3 == "ModuleGroundExperiment")
					{
						configNode2.SetValue("ScienceModifierRate", num2, createIfNotFound: true);
					}
				}
				configNode.AddNode(configNode2);
			}
		}
		return configNode;
	}

	public float GetModuleMass(float defaultMass, ModifierStagingSituation sit)
	{
		float num = 0f;
		if (inventoryPartsByName == null)
		{
			return num;
		}
		if (inventoryPartsByName.Count > 0)
		{
			for (int i = 0; i < inventoryPartsByName.Count; i++)
			{
				if (inventoryPartsByName[i] != null && !string.IsNullOrEmpty(inventoryPartsByName[i]))
				{
					AvailablePart partInfoByName = PartLoader.getPartInfoByName(inventoryPartsByName[i]);
					if (partInfoByName != null)
					{
						ConfigNode partConfig = partInfoByName.partConfig;
						float value = 0f;
						partConfig?.TryGetValue("mass", ref value);
						num += value;
					}
				}
			}
		}
		return Mathf.Clamp(num, base.part.partInfo.MinimumMass, num);
	}

	public ModifierChangeWhen GetModuleMassChangeWhen()
	{
		return ModifierChangeWhen.CONSTANTLY;
	}

	public float GetModuleCost(float defaultCost, ModifierStagingSituation sit)
	{
		float num = 0f;
		if (InventoryItemCount > 0)
		{
			for (int i = 0; i < inventoryPartsByName.Count; i++)
			{
				if (!string.IsNullOrEmpty(inventoryPartsByName[i]))
				{
					AvailablePart partInfoByName = PartLoader.getPartInfoByName(inventoryPartsByName[i]);
					if (partInfoByName != null)
					{
						num += partInfoByName.cost;
					}
				}
			}
		}
		return num;
	}

	public ModifierChangeWhen GetModuleCostChangeWhen()
	{
		return ModifierChangeWhen.CONSTANTLY;
	}
}
