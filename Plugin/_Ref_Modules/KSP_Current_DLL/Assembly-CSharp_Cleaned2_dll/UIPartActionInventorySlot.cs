using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIPartActionInventorySlot : MonoBehaviour, IEventSystemHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
	public int slotIndex;

	public UIPartActionInventory inventoryPartActionUI;

	private Image slotImage;

	private Part cachePart;

	private AvailablePart cacheAvailablePart;

	private bool isBeingDestroyed;

	public void Setup(UIPartActionInventory inventoryRef, int index)
	{
		inventoryPartActionUI = inventoryRef;
		slotIndex = index;
		slotImage = GetComponent<Image>();
	}

	private void SlotClicked()
	{
		if (!IsPartExchangeAllowed() || inventoryPartActionUI == null)
		{
			return;
		}
		if (UIPartActionController.Instance != null && UIPartActionController.Instance.InventoryAndCargoPartExist())
		{
			GameEvents.onFlightCargoPartHeld.Fire(data: false);
			if (inventoryPartActionUI.slotPartIcon[slotIndex].isEmptySlot)
			{
				if (UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked == null)
				{
					UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked = this;
				}
				else
				{
					UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.UpdateCurrentSelectedSlot(isCurrent: false);
				}
				if (!isBeingDestroyed)
				{
					inventoryPartActionUI.SpawnLoadedPartByName(UIPartActionController.Instance.partInventory.CurrentCargoPart.name, slotIndex);
				}
				inventoryPartActionUI.gridControl.inventoryPart.SlotClickedSetParts(null, slotIndex);
				UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.inventoryPartActionUI.DestroyHeldPart();
				inventoryPartActionUI.DestroyHeldPart();
				UIPartActionController.Instance.partInventory.PlayPartDroppedSFX();
				UIPartActionController.Instance.partInventory.ResetInventoryCacheValues();
				return;
			}
			cacheAvailablePart = inventoryPartActionUI.slotPartIcon[slotIndex].partInfo;
			if (UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked == null)
			{
				AvailablePart partInfo = UIPartActionController.Instance.partInventory.CurrentCargoPart.partInfo;
				UIPartActionController.Instance.partInventory.ExchangedCargoPart = UIPartActionController.Instance.partInventory.CurrentCargoPart;
				inventoryPartActionUI.slotPartIcon[slotIndex].SetEmptySlot();
				inventoryPartActionUI.DestroyHeldPart();
				inventoryPartActionUI.slotPartIcon[slotIndex].Create(null, partInfo, inventoryPartActionUI.iconSize, 1f, 1f, inventoryPartActionUI.StartPartPlacement, inventoryPartActionUI.AbleToPlaceParts);
				CreatePartFromThisSlot(cacheAvailablePart);
				inventoryPartActionUI.gridControl.inventoryPart.SlotClickedSetParts(cacheAvailablePart, slotIndex);
				UIPartActionController.Instance.partInventory.CurrentCargoPart = cachePart;
				UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked = null;
				UIPartActionController.Instance.partInventory.ExchangedCargoPart = null;
				UIPartActionController.Instance.partInventory.PlayPartDroppedSFX();
			}
			else
			{
				UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.UpdateCurrentSelectedSlot(isCurrent: false);
				inventoryPartActionUI.slotPartIcon[slotIndex].SetEmptySlot();
				inventoryPartActionUI.slotPartIcon[slotIndex].Create(null, UIPartActionController.Instance.partInventory.CurrentCargoPart.partInfo, inventoryPartActionUI.iconSize, 1f, 1f, inventoryPartActionUI.StartPartPlacement, inventoryPartActionUI.AbleToPlaceParts);
				UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.inventoryPartActionUI.slotPartIcon[UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.slotIndex].Create(null, cacheAvailablePart, UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.inventoryPartActionUI.iconSize, 1f, 1f, UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.inventoryPartActionUI.StartPartPlacement, UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.inventoryPartActionUI.AbleToPlaceParts);
				inventoryPartActionUI.gridControl.inventoryPart.SlotClickedSetParts(cacheAvailablePart, slotIndex);
				inventoryPartActionUI.DestroyHeldPart();
				UIPartActionController.Instance.partInventory.ResetInventoryCacheValues();
				UIPartActionController.Instance.partInventory.PlayPartDroppedSFX();
			}
		}
		else if (inventoryPartActionUI.slotPartIcon[slotIndex].isEmptySlot)
		{
			GameEvents.onFlightCargoPartHeld.Fire(data: false);
		}
		else
		{
			UpdateCurrentSelectedSlot(isCurrent: true);
			CreatePartFromThisSlot(inventoryPartActionUI.slotPartIcon[slotIndex].partInfo);
			inventoryPartActionUI.slotPartIcon[slotIndex].SetEmptySlot();
			inventoryPartActionUI.gridControl.inventoryPart.StoreCargoPartAtSlot("", updatePAW: false, slotIndex);
			InputLockManager.SetControlLock(ControlTypes.UI_DRAGGING, "CargoPartHeld");
			GameEvents.onFlightCargoPartHeld.Fire(data: true);
		}
	}

	private void CreatePartFromThisSlot(AvailablePart ap)
	{
		UIPartActionController.Instance.partInventory.PlayPartSelectedSFX();
		inventoryPartActionUI.gridControl.inventoryPart.SlotClickedFirst(slotIndex);
		UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked = this;
		UIPartActionController.Instance.partInventory.CurrentInventory = inventoryPartActionUI.gridControl.inventoryPart;
		cachePart = inventoryPartActionUI.gridControl.inventoryPart.CreatePartForInventoryUse(ap);
		inventoryPartActionUI.SetPartShaderForInventoryUse(cachePart.transform);
		UIPartActionController.Instance.partInventory.CurrentCargoPart = cachePart;
		inventoryPartActionUI.SetPartAsIcon(inventoryPartActionUI.currentSelectedItemPartIcon, cachePart);
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Left)
		{
			UIPartActionController.Instance.partInventory.DestroyTooltip();
			SlotClicked();
		}
	}

	public void ReturnHeldPartToThisSlot()
	{
		UIPartActionController.Instance.partInventory.PlayPartDroppedSFX();
		SlotClicked();
	}

	public void UpdateCurrentSelectedSlot(bool isCurrent)
	{
		if (isCurrent)
		{
			if (UIPartActionController.Instance != null && UIPartActionController.Instance.InventoryAndCargoPartExist() && UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked != null)
			{
				UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked.UpdateCurrentSelectedSlot(isCurrent: false);
			}
			if (slotImage != null)
			{
				slotImage.sprite = UIPartActionController.Instance.partInventory.slot_partItem_current;
			}
		}
		else if (slotImage != null)
		{
			slotImage.sprite = UIPartActionController.Instance.partInventory.slot_partItem_normal;
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (!(inventoryPartActionUI == null) && inventoryPartActionUI.inventoryItems != null && slotIndex <= inventoryPartActionUI.inventoryItems.Count - 1 && !(UIPartActionController.Instance == null) && !(UIPartActionController.Instance.partInventory == null) && HighLogic.LoadedSceneIsFlight && !string.IsNullOrEmpty(inventoryPartActionUI.inventoryItems[slotIndex]))
		{
			UIPartActionController.Instance.partInventory.CreateFlightTooltip(inventoryPartActionUI.slotPartIcon[slotIndex].partInfo);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (HighLogic.LoadedSceneIsFlight)
		{
			UIPartActionController.Instance.partInventory.DestroyTooltip();
		}
	}

	private bool IsPartExchangeAllowed()
	{
		bool result = true;
		if (HighLogic.LoadedSceneIsFlight && FlightGlobals.ActiveVessel != null && FlightGlobals.ActiveVessel.isEVA && FlightGlobals.ActiveVessel.evaController.PartPlacementMode && inventoryPartActionUI != null && inventoryPartActionUI.slotPartIcon.Count > slotIndex && !inventoryPartActionUI.slotPartIcon[slotIndex].isEmptySlot && inventoryPartActionUI.slotPartIcon[slotIndex].partInfo != null && FlightGlobals.ActiveVessel.evaController.ModuleInventoryPartReference != null && FlightGlobals.ActiveVessel.evaController.ModuleInventoryPartReference.SelectedPart != null && FlightGlobals.ActiveVessel.evaController.ModuleInventoryPartReference.SelectedPart.partInfo.partUrl == inventoryPartActionUI.slotPartIcon[slotIndex].partInfo.partUrl)
		{
			result = false;
		}
		return result;
	}

	private void OnDestroy()
	{
		if (!HighLogic.LoadedSceneIsFlight)
		{
			return;
		}
		if (UIPartActionController.Instance != null && UIPartActionController.Instance.partInventory != null && UIPartActionController.Instance.partInventory.CurrentInventorySlotClicked == this)
		{
			if (UIPartActionController.Instance.partInventory.currentPawAutoOpening)
			{
				UIPartActionController.Instance.partInventory.currentPawAutoOpening = false;
				return;
			}
			isBeingDestroyed = true;
			UIPartActionController.Instance.partInventory.ReturnHeldPart();
		}
		UIPartActionController.Instance.partInventory.DestroyTooltip();
	}
}
