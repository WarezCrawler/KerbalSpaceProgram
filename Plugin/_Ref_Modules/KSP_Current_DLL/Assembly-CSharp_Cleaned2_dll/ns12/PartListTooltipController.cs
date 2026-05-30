using PreFlightTests;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using ns10;
using ns2;
using ns9;

namespace ns12;

public class PartListTooltipController : PinnableTooltipController
{
	public PartListTooltip tooltipPrefab;

	private AvailablePart partInfo;

	private PartUpgradeHandler.Upgrade upgrade;

	[SerializeField]
	private EditorPartIcon editorPartIcon;

	private bool isGrey;

	private static Vector3[] gridCorners = new Vector3[4];

	private static float minX;

	private static float maxX;

	private static float minY;

	private static float maxY;

	private PartListTooltip TooltipInstance => (PartListTooltip)base.TooltipPrefabInstance;

	public override void OnPointerClick(PointerEventData eventData)
	{
		if (PartListTooltipMasterController.Instance.currentTooltip == null || !(UIMasterController.Instance.CurrentTooltip is IPinnableTooltipController pinnableTooltipController) || !((PartListTooltipController)pinnableTooltipController == this))
		{
			return;
		}
		if (!pinned)
		{
			if (!PartListTooltipMasterController.Instance.displayExtendedInfo && eventData.button == PointerEventData.InputButton.Right)
			{
				PartListTooltipMasterController.Instance.displayExtendedInfo = true;
				PartListTooltipMasterController.Instance.currentTooltip.DisplayExtendedInfo(display: true, GetTooltipHintText(PartListTooltipMasterController.Instance.currentTooltip));
			}
			base.OnPointerClick(eventData);
		}
		else if (eventData.button == PointerEventData.InputButton.Right && PartListTooltipMasterController.Instance.displayExtendedInfo)
		{
			PartListTooltipMasterController.Instance.displayExtendedInfo = false;
			PartListTooltipMasterController.Instance.currentTooltip.DisplayExtendedInfo(display: false, GetTooltipHintText(PartListTooltipMasterController.Instance.currentTooltip));
			base.OnPointerClick(eventData);
		}
		Canvas.ForceUpdateCanvases();
		UIMasterController.RepositionTooltip(PartListTooltipMasterController.Instance.currentTooltip.RectTransform, Vector2.one);
	}

	public override void OnTooltipPinned()
	{
	}

	public override void OnTooltipUnpinned()
	{
	}

	public override bool OnTooltipAboutToSpawn()
	{
		return true;
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		if (!(editorPartIcon != null) || !editorPartIcon.isEmptySlot)
		{
			base.OnPointerEnter(eventData);
		}
	}

	public override void OnTooltipSpawned(Tooltip tooltip)
	{
		PartListTooltipMasterController.Instance.currentTooltip = tooltip as PartListTooltip;
		if (editorPartIcon == null)
		{
			editorPartIcon = GetComponent<EditorPartIcon>();
		}
		if (editorPartIcon != null)
		{
			CreateTooltip(tooltip as PartListTooltip, editorPartIcon);
		}
	}

	public override bool OnTooltipAboutToDespawn()
	{
		return true;
	}

	public override void OnTooltipDespawned(Tooltip instance)
	{
		DestroyTooltip();
		PartListTooltipMasterController.Instance.currentTooltip = null;
	}

	private void CreateTooltip(PartListTooltip tooltip, EditorPartIcon partIcon)
	{
		if (partIcon != null)
		{
			if (partIcon.isEmptySlot)
			{
				return;
			}
			partInfo = partIcon.partInfo;
			if (partIcon.isPart)
			{
				tooltip.Setup(partInfo, onPurchase, PartListTooltipMasterController.Instance.thumbnailRenderTexture);
				ModulePartVariants module = partIcon.partInfo.partPrefab.Modules.GetModule<ModulePartVariants>();
				if (module != null)
				{
					tooltip.UpdateVariantText(module.SelectedVariant.Name);
				}
			}
			else
			{
				upgrade = partIcon.upgrade;
				tooltip.Setup(partInfo, partIcon.upgrade, onPurchaseUpgrade, PartListTooltipMasterController.Instance.thumbnailRenderTexture);
			}
		}
		if (PartListTooltipMasterController.Instance.useRenderTextureCamera)
		{
			GameObject val = null;
			PartListTooltipMasterController.Instance.iconDictionary.TryGetValue(partInfo.iconPrefab, out val);
			if (val != null)
			{
				tooltip.icon = val;
				tooltip.iconTransform = tooltip.icon.transform;
			}
			else
			{
				tooltip.icon = Object.Instantiate(partInfo.iconPrefab);
				if (PartListTooltipMasterController.Instance.iconDictionary.ContainsKey(partInfo.iconPrefab))
				{
					PartListTooltipMasterController.Instance.iconDictionary.Remove(partInfo.iconPrefab);
				}
				PartListTooltipMasterController.Instance.iconDictionary.Add(partInfo.iconPrefab, tooltip.icon);
				tooltip.iconTransform = tooltip.icon.transform;
				tooltip.iconTransform.SetLayerRecursive(LayerMask.NameToLayer("UIAdditional"));
				float iconSize = PartListTooltipMasterController.Instance.iconSize;
				tooltip.iconTransform.SetParent(PartListTooltipMasterController.Instance.thumbnailCamera.transform, worldPositionStays: false);
				tooltip.iconTransform.localPosition = new Vector3(0f, 0f, iconSize);
				tooltip.iconTransform.localScale = new Vector3(iconSize, iconSize, iconSize);
			}
			tooltip.iconTransform.rotation = Quaternion.Euler(-15f, 0f, 0f);
			tooltip.iconTransform.Rotate(0f, -30f, 0f);
			tooltip.icon.SetActive(value: true);
			tooltip.DisplayExtendedInfo(PartListTooltipMasterController.Instance.displayExtendedInfo, GetTooltipHintText(tooltip));
			PartListTooltipMasterController.Instance.thumbnailCamera.enabled = true;
			tooltip.materials = EditorPartIcon.CreateMaterialArray(tooltip.icon, includeInactiveRenderers: true);
			if (partIcon.VariantsAvailable())
			{
				ModulePartVariants.ApplyVariant(null, tooltip.icon.transform, partIcon.GetCurrentVariant(), tooltip.materials, skipShader: true);
				GameEvents.onEditorDefaultVariantChanged.Add(OnEditorDefaultVariantChanged);
				tooltip.buttonToggleVariant.onClick.AddListener(OnButtonVariantToggle);
				tooltip.buttonToggleVariant.gameObject.SetActive(value: true);
			}
			else
			{
				tooltip.buttonToggleVariant.gameObject.SetActive(value: false);
			}
		}
		isGrey = partIcon.isGrey;
		if (isGrey)
		{
			tooltip.SetupGrayout(partIcon.greyoutToolTipMessage);
			if (tooltip.icon != null)
			{
				EditorPartIcon.SetPartColor(tooltip.icon, new Color(0.25f, 0.25f, 0.25f, 1f));
			}
		}
		else
		{
			tooltip.textGreyoutMessage.enabled = true;
			tooltip.textGreyoutMessage.text = "";
		}
		Canvas.ForceUpdateCanvases();
		UIMasterController.RepositionTooltip((RectTransform)tooltip.transform, Vector2.one);
	}

	private void DestroyTooltip()
	{
		if (PartListTooltipMasterController.Instance != null)
		{
			if (PartListTooltipMasterController.Instance.thumbnailCamera != null)
			{
				PartListTooltipMasterController.Instance.thumbnailCamera.enabled = false;
			}
			if (PartListTooltipMasterController.Instance.currentTooltip != null)
			{
				if (PartListTooltipMasterController.Instance.currentTooltip.icon != null)
				{
					PartListTooltipMasterController.Instance.currentTooltip.icon.SetActive(value: false);
				}
				PartListTooltipMasterController.Instance.currentTooltip.buttonPurchase.onClick.RemoveAllListeners();
				PartListTooltipMasterController.Instance.currentTooltip.buttonPurchaseRed.onClick.RemoveAllListeners();
				PartListTooltipMasterController.Instance.currentTooltip.buttonToggleVariant.onClick.RemoveAllListeners();
			}
		}
		GameEvents.onEditorDefaultVariantChanged.Remove(OnEditorDefaultVariantChanged);
	}

	protected string GetTooltipHintText(PartListTooltip tooltip)
	{
		if (tooltip.HasExtendedInfo)
		{
			if (!PartListTooltipMasterController.Instance.displayExtendedInfo)
			{
				return Localizer.Format("#autoLOC_456638");
			}
			if (!pinned)
			{
				return Localizer.Format("#autoLOC_456642");
			}
			return "[RMB]: Less Info";
		}
		return "<color=orange>" + Localizer.Format("#autoLOC_456651") + "</color>";
	}

	protected void onPurchase(PartListTooltip tTip)
	{
		DestroyTooltip();
		InputLockManager.SetControlLock("partTooltipCheckingRnDState");
		PreFlightCheck preFlightCheck = new PreFlightCheck(onPurchaseProceed, onPurchaseDismiss);
		preFlightCheck.AddTest(new FacilityOperational("RnD", "Research & Development"));
		preFlightCheck.RunTests();
	}

	protected void onVariantToggle(PartVariant variant)
	{
		PartListTooltip tooltipInstance = TooltipInstance;
		ModulePartVariants.ApplyVariant(null, tooltipInstance.icon.transform, variant, tooltipInstance.materials, skipShader: true);
		tooltipInstance.UpdateVariantText(variant.Name);
		if (editorPartIcon.isGrey && tooltipInstance.icon != null)
		{
			EditorPartIcon.SetPartColor(tooltipInstance.icon, new Color(0.25f, 0.25f, 0.25f, 1f));
		}
	}

	private void OnEditorDefaultVariantChanged(AvailablePart ap, PartVariant variant)
	{
		if (ap != partInfo)
		{
			return;
		}
		PartListTooltip tooltipInstance = TooltipInstance;
		if (tooltipInstance != null)
		{
			ModulePartVariants.ApplyVariant(null, tooltipInstance.icon.transform, variant, tooltipInstance.materials, skipShader: true);
			tooltipInstance.UpdateVariantText(variant.Name);
			if (editorPartIcon.isGrey && tooltipInstance.icon != null)
			{
				EditorPartIcon.SetPartColor(tooltipInstance.icon, new Color(0.25f, 0.25f, 0.25f, 1f));
			}
		}
	}

	protected void OnButtonVariantToggle()
	{
		if (editorPartIcon != null)
		{
			editorPartIcon.ToggleVariant();
		}
	}

	private void onPurchaseProceed()
	{
		if (HighLogic.LoadedScene == GameScenes.EDITOR)
		{
			ProtoTechNode techState = ResearchAndDevelopment.Instance.GetTechState(partInfo.TechRequired);
			techState.partsPurchased.Add(partInfo);
			GameEvents.OnPartPurchased.Fire(partInfo);
			string[] array = partInfo.identicalParts.Split(',');
			int num = array.Length;
			while (num-- > 0)
			{
				AvailablePart partInfoByName = PartLoader.getPartInfoByName(array[num].Replace('_', '.').Trim());
				if (partInfoByName != null && partInfoByName.TechRequired == partInfo.TechRequired)
				{
					partInfoByName.costsFunds = false;
					techState.partsPurchased.Add(partInfoByName);
					GameEvents.OnPartPurchased.Fire(partInfoByName);
					partInfoByName.costsFunds = true;
				}
			}
		}
		else if (RDController.Instance != null)
		{
			RDController.Instance.node_selected.tech.PurchasePart(partInfo);
			RDController.Instance.node_selected.UpdateGraphics();
			RDController.Instance.partList.Refresh();
			RDController.Instance.UpdatePanel();
		}
		if (EditorPartList.Instance != null)
		{
			EditorPartList.Instance.Refresh();
		}
		GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
		onPurchaseDismiss();
	}

	private void onPurchaseDismiss()
	{
		InputLockManager.RemoveControlLock("partTooltipCheckingRnDState");
	}

	protected void onPurchaseUpgrade(PartListTooltip tTip)
	{
		DestroyTooltip();
		InputLockManager.SetControlLock("partTooltipCheckingRnDState");
		PreFlightCheck preFlightCheck = new PreFlightCheck(onPurchaseUpgradeProceed, onPurchaseDismiss);
		preFlightCheck.AddTest(new FacilityOperational("RnD", "Research & Development"));
		preFlightCheck.RunTests();
	}

	private void onPurchaseUpgradeProceed()
	{
		if (HighLogic.LoadedScene == GameScenes.EDITOR)
		{
			PartUpgradeManager.Handler.SetUnlocked(upgrade.name, val: true);
			GameEvents.OnPartUpgradePurchased.Fire(upgrade);
		}
		else if (PartUpgradeManager.Instance != null)
		{
			PartUpgradeManager.Handler.SetUnlocked(upgrade.name, val: true);
			GameEvents.OnPartUpgradePurchased.Fire(upgrade);
			RDController.Instance.node_selected.UpdateGraphics();
			RDController.Instance.partList.Refresh();
			RDController.Instance.UpdatePanel();
		}
		if (EditorPartList.Instance != null)
		{
			EditorPartList.Instance.Refresh();
		}
		GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE);
		onPurchaseDismiss();
	}

	public static void SetupScreenSpaceMask(RectTransform uiImageTransform)
	{
		uiImageTransform.GetWorldCorners(gridCorners);
		gridCorners[0] = UIMainCamera.Camera.WorldToViewportPoint(gridCorners[0]);
		gridCorners[1] = UIMainCamera.Camera.WorldToViewportPoint(gridCorners[1]);
		gridCorners[2] = UIMainCamera.Camera.WorldToViewportPoint(gridCorners[2]);
		gridCorners[3] = UIMainCamera.Camera.WorldToViewportPoint(gridCorners[3]);
		minX = gridCorners[0].x;
		maxX = gridCorners[2].x;
		minY = gridCorners[0].y;
		maxY = gridCorners[2].y;
		if (Application.platform != RuntimePlatform.OSXPlayer && Application.platform != 0 && Application.platform != RuntimePlatform.LinuxPlayer && Application.platform != RuntimePlatform.LinuxEditor && (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLCore || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 || SystemInfo.graphicsDeviceType == GraphicsDeviceType.PlayStation4))
		{
			minY = 1f - minY;
			maxY = 1f - maxY;
			float num = minY;
			minY = maxY;
			maxY = num;
		}
	}

	public static void SetScreenSpaceMaskMaterials(Material[] materials)
	{
		foreach (Material material in materials)
		{
			if (!(material == null))
			{
				material.SetFloat(PropertyIDs._MinX, minX);
				material.SetFloat(PropertyIDs._MaxX, maxX);
				material.SetFloat(PropertyIDs._MinY, minY);
				material.SetFloat(PropertyIDs._MaxY, maxY);
			}
		}
	}
}
