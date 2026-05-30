using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ns10;
using ns12;
using ns2;

[UI_Grid]
public class UIPartActionInventory : UIPartActionFieldItem
{
	public UI_Grid gridControl;

	public TextMeshProUGUI inventoryNameText;

	public RectTransform contentTransform;

	public GridLayoutGroup gridLayout;

	public GameObject slotPrefab;

	public int fieldValue;

	public float partIconDepthDistance = 39f;

	public float iconSize = 35f;

	public List<string> inventoryItems = new List<string>();

	public List<UIPartActionInventorySlot> slotButton = new List<UIPartActionInventorySlot>();

	public List<EditorPartIcon> slotPartIcon = new List<EditorPartIcon>();

	public RectTransform currentSelectedItemPartIcon;

	private AvailablePart ap;

	private Vector2 currentSelectedItemPartIconMovePos;

	private ModuleInventoryPart inventoryPartModule;

	private bool isKerbalOnEva;

	public bool AbleToPlaceParts
	{
		get
		{
			if (inventoryPartModule != null)
			{
				return inventoryPartModule.AbleToPlaceParts;
			}
			return false;
		}
	}

	private void Awake()
	{
		inventoryItems = new List<string>();
		slotButton = new List<UIPartActionInventorySlot>();
		slotPartIcon = new List<EditorPartIcon>();
	}

	private void OnDestroy()
	{
		if (currentSelectedItemPartIcon.childCount > 0)
		{
			SetIconAsPart();
		}
		GameEvents.onVesselSituationChange.Remove(OnVesselSituationChange);
		GameEvents.onVesselChange.Remove(OnVesselChange);
		GameEvents.onGUIAstronautComplexDespawn.Remove(ShowGridObject);
		GameEvents.onGUIAstronautComplexSpawn.Remove(HideGridObject);
		if (UIPartActionController.Instance != null && UIPartActionController.Instance.partInventory != null)
		{
			UIPartActionController.Instance.partInventory.ToggleCargoPartsLight(-1);
		}
		InputLockManager.RemoveControlLock("CargoPartHeld");
	}

	public override void Setup(UIPartActionWindow window, Part part, PartModule partModule, UI_Scene scene, UI_Control control, BaseField field)
	{
		base.Setup(window, part, partModule, scene, control, field);
		inventoryPartModule = partModule as ModuleInventoryPart;
		GameEvents.onGUIAstronautComplexDespawn.Add(ShowGridObject);
		GameEvents.onGUIAstronautComplexSpawn.Add(HideGridObject);
		if (inventoryPartModule != null && inventoryPartModule.IsKerbalOnEVA)
		{
			GameEvents.onVesselSituationChange.Add(OnVesselSituationChange);
			GameEvents.onVesselChange.Add(OnVesselChange);
		}
		if (scene == UI_Scene.Flight || scene == UI_Scene.Editor)
		{
			fieldValue = GetFieldValue();
			if (gridControl == null)
			{
				gridControl = (UI_Grid)control;
				gridControl.pawInventory = this;
			}
			InitializeSlots();
		}
	}

	private void OnVesselSituationChange(GameEvents.HostedFromToAction<Vessel, Vessel.Situations> data)
	{
		if (data.host.id == inventoryPartModule.vessel.id)
		{
			UpdatePlacePartIcons(data.to == Vessel.Situations.LANDED);
		}
	}

	private void OnVesselChange(Vessel vsl)
	{
		UpdatePlacePartIcons(vsl.id == inventoryPartModule.vessel.id);
	}

	private int GetFieldValue()
	{
		return field.GetValue<int>(field.host);
	}

	private void InitializeSlots()
	{
		inventoryItems = gridControl.inventoryItems;
		if (inventoryItems == null)
		{
			inventoryItems = new List<string>();
		}
		gridLayout.constraintCount = gridControl.columnCount;
		if (UIPartActionController.Instance != null && UIPartActionController.Instance.partInventory != null)
		{
			UIPartActionController.Instance.partInventory.ToggleCargoPartsLight(1);
		}
		for (int i = 0; i < fieldValue; i++)
		{
			GameObject gameObject = Object.Instantiate(slotPrefab, contentTransform);
			gameObject.transform.SetParent(contentTransform);
			gameObject.transform.localPosition = Vector3.zero;
			slotPartIcon.Add(gameObject.GetComponent<EditorPartIcon>());
			UIPartActionInventorySlot item = gameObject.AddComponent<UIPartActionInventorySlot>();
			slotButton.Add(item);
			if (inventoryItems.Count > i)
			{
				SpawnLoadedPartByName(inventoryItems[i], i);
			}
			if (HighLogic.LoadedSceneIsFlight)
			{
				PartListTooltipController component = gameObject.GetComponent<PartListTooltipController>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
		}
	}

	public void SpawnLoadedPartByName(string partName, int slotIndex)
	{
		if (slotIndex > -1 && slotIndex <= slotButton.Count - 1)
		{
			slotButton[slotIndex].Setup(this, slotIndex);
			if (!string.IsNullOrEmpty(partName))
			{
				ap = PartLoader.getPartInfoByName(partName);
				slotPartIcon[slotIndex].Create(null, ap, iconSize, 1f, 60f, StartPartPlacement, AbleToPlaceParts);
				SetPartShaderForInventoryUse(slotPartIcon[slotIndex].transform);
			}
			else
			{
				slotPartIcon[slotIndex].SetEmptySlot();
			}
		}
		else
		{
			Debug.LogWarning("[UIPartActionInventory]: Invalid slotIndex passed Index: " + slotIndex);
		}
	}

	public void SetPartShaderForInventoryUse(Transform part_T)
	{
		if (!HighLogic.LoadedSceneIsFlight || !(UIPartActionController.Instance != null) || !(UIPartActionController.Instance.partInventory != null) || !(UIPartActionController.Instance.partInventory.noAmbientHue != null) || !(UIPartActionController.Instance.partInventory.noAmbientHueBumped != null))
		{
			return;
		}
		Renderer[] componentsInChildren = part_T.GetComponentsInChildren<Renderer>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].material.shader.name.Contains("Bumped"))
			{
				componentsInChildren[i].material.shader = UIPartActionController.Instance.partInventory.noAmbientHueBumped;
			}
			else
			{
				componentsInChildren[i].material.shader = UIPartActionController.Instance.partInventory.noAmbientHue;
			}
		}
	}

	public void SetPartAsIcon(Transform parent, Part part)
	{
		part.transform.SetParent(parent);
		part.transform.localPosition = new Vector3(0f, 0f, 0f - partIconDepthDistance);
		part.transform.localScale = Vector3.one * iconSize;
		part.transform.rotation = Quaternion.Euler(-15f, 0f, 0f);
		part.transform.Rotate(0f, -30f, 0f);
		U5Util.SetLayerRecursive(part.gameObject, LayerMask.NameToLayer("UIAdditional"));
		part.highlighter.ReinitMaterials();
		part.SetHighlightType(Part.HighlightType.OnMouseOver);
		part.SetHighlight(active: false, recursive: true);
	}

	private void SetIconAsPart()
	{
		if (UIPartActionController.Instance != null && UIPartActionController.Instance.InventoryAndCargoPartExist())
		{
			if (HighLogic.LoadedSceneIsEditor)
			{
				EditorLogic.fetch.SetIconAsPart(UIPartActionController.Instance.partInventory.CurrentCargoPart);
			}
			if (HighLogic.LoadedSceneIsFlight)
			{
				UIPartActionController.Instance.partInventory.CurrentCargoPart.transform.SetParent(UIMasterController.Instance.actionCanvas.transform);
				UIPartActionController.Instance.partInventory.isHeldPartOutOfActionWindows = true;
			}
		}
	}

	public void DestroyHeldPart()
	{
		for (int i = 0; i < currentSelectedItemPartIcon.childCount; i++)
		{
			Object.Destroy(currentSelectedItemPartIcon.GetChild(i).gameObject);
		}
		InputLockManager.RemoveControlLock("CargoPartHeld");
	}

	public void DestroyItem(int index)
	{
		if (slotPartIcon.Count >= index && slotPartIcon[index] != null)
		{
			slotPartIcon[index].SetEmptySlot();
		}
	}

	public override void UpdateItem()
	{
		if (field != null)
		{
			inventoryNameText.text = field.guiName;
		}
	}

	private void Update()
	{
		if (base.Window.Hover)
		{
			if (!(UIPartActionController.Instance != null) || !UIPartActionController.Instance.InventoryAndCargoPartExist())
			{
				return;
			}
			if (currentSelectedItemPartIcon.childCount <= 0)
			{
				Part currentCargoPart = UIPartActionController.Instance.partInventory.CurrentCargoPart;
				if (HighLogic.LoadedSceneIsEditor)
				{
					UIPartActionController.Instance.partInventory.editorPartPickedBlockSfx = true;
					UIPartActionController.Instance.partInventory.editorPartDroppedBlockSfx = true;
					EditorLogic.fetch.ReleasePartToIcon();
				}
				UIPartActionController.Instance.partInventory.CurrentCargoPart = currentCargoPart;
				SetPartAsIcon(currentSelectedItemPartIcon.transform, currentCargoPart);
				currentCargoPart.highlighter.ConstantOffImmediate();
			}
			if (currentSelectedItemPartIcon.childCount > 0)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(base.transform as RectTransform, Input.mousePosition, UIMainCamera.Camera, out currentSelectedItemPartIconMovePos);
				currentSelectedItemPartIcon.anchoredPosition = currentSelectedItemPartIconMovePos;
				UIPartActionController.Instance.partInventory.isHeldPartOutOfActionWindows = false;
			}
		}
		else if ((HighLogic.LoadedSceneIsEditor && currentSelectedItemPartIcon.childCount > 0) || (HighLogic.LoadedSceneIsFlight && UIPartActionController.Instance != null && !UIPartActionController.Instance.partInventory.IsCursorOverAnyPAW))
		{
			SetIconAsPart();
		}
	}

	public void SetAllSlotsNotSelected()
	{
		for (int i = 0; i < slotButton.Count; i++)
		{
			slotButton[i].UpdateCurrentSelectedSlot(isCurrent: false);
		}
	}

	public void StartPartPlacement(EditorPartIcon partIcon)
	{
		int num = -1;
		for (int i = 0; i < slotPartIcon.Count; i++)
		{
			if (slotPartIcon[i] == partIcon)
			{
				num = i;
				break;
			}
		}
		if (num > -1 && partModule != null)
		{
			ModuleInventoryPart moduleInventoryPart = partModule as ModuleInventoryPart;
			if (moduleInventoryPart != null)
			{
				UpdatePlacePartIcons(active: false);
				moduleInventoryPart.DeployInventoryItem(num);
			}
		}
	}

	public void ResetPlacePartIcons(bool active)
	{
		if (active)
		{
			if (inventoryPartModule != null && inventoryPartModule.vessel != null && inventoryPartModule.AbleToPlaceParts)
			{
				UpdatePlacePartIcons(active: true);
			}
			else
			{
				UpdatePlacePartIcons(active: false);
			}
		}
		else
		{
			UpdatePlacePartIcons(active: false);
		}
	}

	private void UpdatePlacePartIcons(bool active)
	{
		for (int i = 0; i < slotPartIcon.Count; i++)
		{
			if (slotPartIcon[i].btnPlacePart != null)
			{
				slotPartIcon[i].btnPlacePart.gameObject.SetActive(active && slotPartIcon[i].PartIcon != null);
			}
		}
	}

	private void ShowGridObject()
	{
		ToggleGridObject(toggle: true);
	}

	private void HideGridObject()
	{
		ToggleGridObject(toggle: false);
	}

	private void ToggleGridObject(bool toggle)
	{
		if (gridLayout != null && gridLayout.gameObject != null)
		{
			gridLayout.gameObject.SetActive(toggle);
		}
	}
}
