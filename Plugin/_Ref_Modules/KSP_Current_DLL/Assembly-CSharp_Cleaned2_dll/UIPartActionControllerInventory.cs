using System.Collections.Generic;
using UnityEngine;
using ns11;
using ns2;

public class UIPartActionControllerInventory : MonoBehaviour
{
	[SerializeField]
	private UIPartActionController pawController;

	protected ModuleInventoryPart currentInventory;

	protected UIPartActionInventorySlot currentInventorySlotClicked;

	protected Part currentCargoPart;

	protected Part exchangedCargoPart;

	private uint currentHoveredInventoryPart;

	protected UIPartActionWindow currentPawAutoOpened;

	internal bool currentPawAutoOpening;

	public bool IsCursorOverAnyPAW;

	[SerializeField]
	private float cargoPartDepthOffset;

	[SerializeField]
	private float partScaleMultiplier;

	[SerializeField]
	private float partScale;

	public bool isHeldPartOutOfActionWindows;

	public bool referenceNextCreatedInventoryPAW;

	public Sprite slot_partItem_normal;

	public Sprite slot_partItem_current;

	[SerializeField]
	private GameObject flight_TooltipPrefab;

	private GameObject currentTooltip;

	private bool currentCargoExists;

	private Vector3 partOriginalScale;

	private List<string> availableCargoParts;

	[HideInInspector]
	public List<string> availableInventoryParts = new List<string>();

	private float currentDistanceFromPart;

	public bool editorPartPickedBlockSfx;

	public bool editorPartDroppedBlockSfx;

	public Shader noAmbientHue;

	public Shader noAmbientHueBumped;

	[SerializeField]
	private Light cargoPartsLight;

	private int cargoInventoriesOpen;

	private bool enableCargoLight;

	private Vector2 heldPartPosition;

	private Vector3 heldPartNewPosition;

	[SerializeField]
	private AudioSource audioSource;

	[SerializeField]
	private AudioClip pickupPart;

	[SerializeField]
	private AudioClip droppedPart;

	public ModuleInventoryPart CurrentInventory
	{
		get
		{
			return currentInventory;
		}
		set
		{
			currentInventory = value;
		}
	}

	public UIPartActionInventorySlot CurrentInventorySlotClicked
	{
		get
		{
			return currentInventorySlotClicked;
		}
		set
		{
			currentInventorySlotClicked = value;
		}
	}

	public Part CurrentCargoPart
	{
		get
		{
			return currentCargoPart;
		}
		set
		{
			currentCargoPart = value;
		}
	}

	public Part ExchangedCargoPart
	{
		get
		{
			return exchangedCargoPart;
		}
		set
		{
			exchangedCargoPart = value;
		}
	}

	public uint CurrentHoveredInventoryPart => currentHoveredInventoryPart;

	public UIPartActionWindow CurrentPawAutoOpened
	{
		get
		{
			return currentPawAutoOpened;
		}
		set
		{
			currentPawAutoOpened = value;
		}
	}

	private void Start()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			GameEvents.onEditorPartEvent.Add(OnCargoPartConstructionEvent);
		}
		availableCargoParts = PartLoader.Instance.GetAvailableCargoPartNames();
		availableInventoryParts = PartLoader.Instance.GetAvailableCargoInventoryPartNames();
		currentHoveredInventoryPart = 0u;
		CurrentPawAutoOpened = null;
		partOriginalScale = new Vector3(partScale, partScale, partScale);
		cargoPartsLight.gameObject.SetActive(value: false);
		cargoInventoriesOpen = 0;
		enableCargoLight = false;
		UpdateAudioVolume();
		GameEvents.OnGameSettingsApplied.Add(UpdateAudioVolume);
		GameEvents.onVesselDestroy.Add(OnVesselDestroy);
	}

	private void OnDestroy()
	{
		GameEvents.onEditorPartEvent.Remove(OnCargoPartConstructionEvent);
		GameEvents.OnGameSettingsApplied.Remove(UpdateAudioVolume);
		GameEvents.onVesselDestroy.Remove(OnVesselDestroy);
		DestroyTooltip();
	}

	public bool IsKerbalWithinRange(ModuleInventoryPart inventoryPart)
	{
		if (FlightGlobals.ActiveVessel.isEVA)
		{
			currentDistanceFromPart = Vector3.Distance(FlightGlobals.ActiveVessel.GetWorldPos3D(), inventoryPart.transform.position);
		}
		return currentDistanceFromPart <= inventoryPart.allowedKerbalEvaDistance;
	}

	private void OnCargoPartConstructionEvent(ConstructionEventType construction, Part p)
	{
		if (!availableCargoParts.Contains(p.partInfo.name))
		{
			return;
		}
		switch (construction)
		{
		case ConstructionEventType.PartCreated:
			CurrentCargoPart = p;
			break;
		case ConstructionEventType.PartDropped:
			if (!IsCursorOutOfPAWs())
			{
				if (CurrentInventorySlotClicked != null)
				{
					CurrentInventorySlotClicked.UpdateCurrentSelectedSlot(isCurrent: false);
				}
				ResetInventoryCacheValues();
			}
			break;
		case ConstructionEventType.PartPicked:
			CurrentCargoPart = p;
			break;
		case ConstructionEventType.PartDeleted:
			if (CurrentInventorySlotClicked != null)
			{
				CurrentInventorySlotClicked.UpdateCurrentSelectedSlot(isCurrent: false);
			}
			ResetInventoryCacheValues();
			break;
		case ConstructionEventType.PartDragging:
		case ConstructionEventType.PartAttached:
		case ConstructionEventType.PartDetached:
			break;
		}
	}

	public void ResetInventoryCacheValues()
	{
		CurrentInventory = null;
		CurrentInventorySlotClicked = null;
		CurrentCargoPart = null;
		referenceNextCreatedInventoryPAW = false;
		currentHoveredInventoryPart = 0u;
		ExchangedCargoPart = null;
		if (!(CurrentPawAutoOpened != null))
		{
			return;
		}
		for (int i = 0; i < pawController.windows.Count; i++)
		{
			if (!pawController.windows[i].Hover && pawController.windows[i].part.persistentId == CurrentPawAutoOpened.part.persistentId)
			{
				GameEvents.onPartActionUIDismiss.Fire(pawController.windows[i].part);
				Object.DestroyImmediate(pawController.windows[i].gameObject);
				pawController.windows.RemoveAt(i);
				break;
			}
		}
		CurrentPawAutoOpened = null;
	}

	public void OnInventoryPartHover(uint partId, Part p)
	{
		for (int i = 0; i < pawController.windows.Count; i++)
		{
			if (pawController.windows[i].part.persistentId == partId)
			{
				return;
			}
		}
		if (currentHoveredInventoryPart != partId)
		{
			currentHoveredInventoryPart = p.persistentId;
			referenceNextCreatedInventoryPAW = true;
			UIPartActionController.Instance.SpawnPartActionWindow(p, overrideSymmetry: true);
		}
	}

	private bool IsCursorOutOfPAWs()
	{
		bool flag = false;
		for (int i = 0; i < pawController.windows.Count; i++)
		{
			flag |= pawController.windows[i].Hover;
		}
		IsCursorOverAnyPAW = flag;
		return flag;
	}

	public void UpdateCurrentCargoPart()
	{
		currentCargoExists = CurrentCargoPart != null;
		if (currentCargoExists)
		{
			GameEvents.onFlightCargoPartHeld.Fire(data: true);
		}
		MoveSelectedPart();
		UpdatePartIconScale();
	}

	private void MoveSelectedPart()
	{
		if (isHeldPartOutOfActionWindows && currentCargoExists)
		{
			CurrentCargoPart.transform.localPosition = dragOverPlane();
		}
	}

	private Vector3 dragOverPlane()
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(UIMasterController.Instance.actionCanvas.transform as RectTransform, Input.mousePosition, UIMainCamera.Camera, out heldPartPosition);
		heldPartNewPosition.z = cargoPartDepthOffset;
		heldPartNewPosition.x = heldPartPosition.x;
		heldPartNewPosition.y = heldPartPosition.y;
		return heldPartNewPosition;
	}

	public void ReturnHeldPart()
	{
		if (CurrentInventorySlotClicked != null && CurrentCargoPart != null)
		{
			GameObject gameObject = CurrentCargoPart.gameObject;
			CurrentInventorySlotClicked.ReturnHeldPartToThisSlot();
			if (gameObject != null)
			{
				Object.Destroy(gameObject);
			}
		}
	}

	private void OnVesselDestroy(Vessel v)
	{
		if (CurrentInventory != null && CurrentInventory.vessel != null && CurrentInventory.vessel.persistentId == v.persistentId)
		{
			ReturnHeldPart();
		}
	}

	private void UpdatePartIconScale()
	{
		if (currentCargoExists)
		{
			if (!IsCursorOutOfPAWs())
			{
				CurrentCargoPart.transform.localScale = partOriginalScale * partScaleMultiplier;
			}
			else
			{
				CurrentCargoPart.transform.localScale = partOriginalScale;
			}
		}
	}

	public void CreateFlightTooltip(AvailablePart ap)
	{
		ModuleCargoPart component = ap.partPrefab.GetComponent<ModuleCargoPart>();
		if (!(component == null))
		{
			currentTooltip = Object.Instantiate(flight_TooltipPrefab, UIMasterController.Instance.tooltipCanvas.transform);
			Tooltip_TitleAndText component2 = currentTooltip.GetComponent<Tooltip_TitleAndText>();
			component2.title.text = ap.title;
			component2.label.text = component.GetTooltip();
			Canvas.ForceUpdateCanvases();
			UIMasterController.RepositionTooltip((RectTransform)currentTooltip.transform, Vector2.one);
		}
	}

	public void DestroyTooltip()
	{
		if (currentTooltip != null)
		{
			Object.Destroy(currentTooltip);
			currentTooltip = null;
		}
	}

	public void ToggleCargoPartsLight(int counter)
	{
		if (!HighLogic.LoadedSceneIsEditor)
		{
			cargoInventoriesOpen += counter;
			enableCargoLight = cargoInventoriesOpen > 0;
			cargoPartsLight.gameObject.SetActive(enableCargoLight);
		}
	}

	public void PlayPartSelectedSFX()
	{
		audioSource.PlayOneShot(pickupPart);
	}

	public void PlayPartDroppedSFX()
	{
		audioSource.PlayOneShot(droppedPart);
	}

	private void UpdateAudioVolume()
	{
		audioSource.volume = GameSettings.UI_VOLUME;
	}
}
