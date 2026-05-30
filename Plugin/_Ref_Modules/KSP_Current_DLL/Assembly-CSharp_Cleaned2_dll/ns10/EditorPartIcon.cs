using System.Collections.Generic;
using Expansions.Missions.Runtime;
using KSP.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ns2;

namespace ns10;

[RequireComponent(typeof(Button), typeof(PointerEnterExitHandler))]
public class EditorPartIcon : MonoBehaviour
{
	public Color experimentalPartColor = new Color32(128, 128, byte.MaxValue, byte.MaxValue);

	public Button btnSpawnPart;

	public Button btnRemove;

	public Button btnAdd;

	public Button btnSwapTexture;

	public Button btnPlacePart;

	private PointerEnterExitHandler hoverHandler;

	private EditorPartList partList;

	private AvailablePart availablePart;

	public float iconSize;

	public float iconOverScale;

	public float iconOverSpin;

	public int variantIndex;

	private bool checkedExperimental;

	public Material[] materials;

	public Color missionRequiredPartColor = new Color32(128, 128, byte.MaxValue, byte.MaxValue);

	private bool checkedMissionRequired;

	public Callback<EditorPartIcon> PlacePartCallback;

	private Transform partIcon;

	private SpriteState buttonState;

	public bool isPart = true;

	public bool isEmptySlot;

	public PartUpgradeHandler.Upgrade upgrade;

	private bool changingDefaultVariant;

	private bool mouseOver;

	private bool stillFocused;

	private Vector3 partScale;

	private float partRotation;

	private Quaternion startRot;

	public AvailablePart partInfo => availablePart;

	public Transform PartIcon => partIcon;

	public bool MouseOver => mouseOver;

	public bool StillFocused => stillFocused;

	public bool Focused
	{
		get
		{
			if (!mouseOver)
			{
				return stillFocused;
			}
			return true;
		}
	}

	public bool isGrey { get; private set; }

	public string greyoutToolTipMessage { get; private set; }

	private void Awake()
	{
		hoverHandler = GetComponent<PointerEnterExitHandler>();
		hoverHandler.onPointerEnter.AddListener(MouseInput_PointerEnter);
		hoverHandler.onPointerExit.AddListener(MouseInput_PointerExit);
		if (btnSpawnPart != null)
		{
			buttonState = btnSpawnPart.spriteState;
			btnSpawnPart.onClick.AddListener(MouseInput_SpawnPart);
		}
		if (btnRemove != null)
		{
			btnRemove.gameObject.SetActive(value: false);
			btnRemove.onClick.AddListener(MouseInput_Delete);
		}
		if (btnAdd != null)
		{
			btnAdd.gameObject.SetActive(value: false);
			btnAdd.onClick.AddListener(MouseInput_Add);
		}
		if (btnPlacePart != null)
		{
			btnPlacePart.gameObject.SetActive(value: false);
			btnPlacePart.onClick.AddListener(MouseInput_PlacePart);
		}
		greyoutToolTipMessage = "";
		isGrey = false;
		checkedExperimental = false;
		variantIndex = 0;
		GameEvents.onEditorDefaultVariantChanged.Add(OnEditorDefaultVariantChanged);
	}

	protected void Start()
	{
		if (btnSpawnPart != null)
		{
			btnSpawnPart.interactable = !isGrey;
		}
	}

	private void OnDestroy()
	{
		if (partIcon != null)
		{
			CleanUpMaterials(partIcon.gameObject);
		}
		GameEvents.onEditorDefaultVariantChanged.Remove(OnEditorDefaultVariantChanged);
	}

	public void Create(EditorPartList partList, AvailablePart part, float iconSize, float iconOverScale, float iconOverSpin)
	{
		Create(partList, part, iconSize, iconOverScale, iconOverSpin, null, btnPlacePartActive: false);
	}

	public void Create(EditorPartList partList, AvailablePart part, float iconSize, float iconOverScale, float iconOverSpin, Callback<EditorPartIcon> placePartCallback, bool btnPlacePartActive)
	{
		this.iconSize = iconSize;
		this.iconOverScale = iconOverScale;
		this.iconOverSpin = iconOverSpin;
		if (placePartCallback != null)
		{
			PlacePartCallback = placePartCallback;
			if (HighLogic.LoadedSceneIsFlight && btnPlacePart != null && part != null && part.partPrefab.FindModuleImplementing<ModuleGroundPart>() != null)
			{
				btnPlacePart.gameObject.SetActive(btnPlacePartActive);
			}
		}
		isEmptySlot = false;
		this.partList = partList;
		availablePart = part;
		InstantiatePartIcon();
		if (part.Variants != null && part.Variants.Count > 0)
		{
			if (btnSwapTexture != null)
			{
				btnSwapTexture.gameObject.SetActive(value: true);
				btnSwapTexture.onClick.AddListener(ToggleVariant);
			}
			if (part.variant == null)
			{
				part.variant = part.partPrefab.baseVariant;
			}
			variantIndex = part.partPrefab.variants.GetVariantIndex(part.variant.Name);
			ModulePartVariants.ApplyVariant(null, partIcon, part.variant, materials, skipShader: true);
		}
		else if (btnSwapTexture != null)
		{
			btnSwapTexture.gameObject.SetActive(value: false);
		}
	}

	public void SetEmptySlot()
	{
		partList = null;
		availablePart = null;
		isEmptySlot = true;
		if (partIcon != null)
		{
			Object.Destroy(partIcon.gameObject);
		}
		if (btnSwapTexture != null)
		{
			btnSwapTexture.gameObject.SetActive(value: false);
		}
		if (btnPlacePart != null)
		{
			btnPlacePart.gameObject.SetActive(value: false);
		}
	}

	public void MouseInput_SpawnPart()
	{
		if (partList != null && InputLockManager.IsUnlocked(ControlTypes.EDITOR_ICON_PICK) && !isGrey)
		{
			partList.TapIcon(availablePart);
		}
	}

	public void EnableDeleteButton()
	{
		btnRemove.gameObject.SetActive(value: true);
	}

	public void DisableDeleteButton()
	{
		btnRemove.gameObject.SetActive(value: false);
	}

	private void MouseInput_Delete()
	{
		if (EditorLogic.SelectedPart == null)
		{
			PartCategorizer.Instance.RemovePartFromCategory(availablePart);
		}
	}

	public void EnableAddButton()
	{
		btnAdd.gameObject.SetActive(value: true);
	}

	public void DisableAddButton()
	{
		btnAdd.gameObject.SetActive(value: false);
	}

	private void MouseInput_Add()
	{
		if (EditorLogic.SelectedPart == null)
		{
			PartCategorizer.Instance.AddPartToCustomCategoryViaPopup(availablePart);
		}
	}

	private void MouseInput_PlacePart()
	{
		if (PlacePartCallback != null)
		{
			PlacePartCallback(this);
		}
	}

	public void ToggleVariant()
	{
		List<PartVariant> variants = availablePart.Variants;
		if (variants.Count > 0)
		{
			variantIndex = (variantIndex + 1) % variants.Count;
			PartVariant partVariant = variants[variantIndex];
			availablePart.variant = partVariant;
			ModulePartVariants.ApplyVariant(null, partIcon, partVariant, materials, skipShader: true);
			if (isGrey)
			{
				SetPartColor(partIcon.gameObject, new Color(0.25f, 0.25f, 0.25f, 1f));
			}
			changingDefaultVariant = true;
			GameEvents.onEditorDefaultVariantChanged.Fire(availablePart, partVariant);
			changingDefaultVariant = false;
		}
	}

	private void OnEditorDefaultVariantChanged(AvailablePart ap, PartVariant variant)
	{
		if (!changingDefaultVariant && ap == availablePart)
		{
			ModulePartVariants.ApplyVariant(null, partIcon, variant, materials, skipShader: true);
			variantIndex = availablePart.Variants.IndexOf(ap.variant);
			if (isGrey)
			{
				SetPartColor(partIcon.gameObject, new Color(0.25f, 0.25f, 0.25f, 1f));
			}
		}
	}

	public bool VariantsAvailable()
	{
		if (availablePart.Variants != null)
		{
			return availablePart.Variants.Count > 0;
		}
		return false;
	}

	public PartVariant GetCurrentVariant()
	{
		return availablePart.variant;
	}

	private void InstantiatePartIcon()
	{
		GameObject gameObject = Object.Instantiate(availablePart.iconPrefab);
		gameObject.SetActive(value: true);
		partIcon = gameObject.transform;
		partIcon.parent = base.transform;
		partIcon.localPosition = new Vector3(0f, 0f, 0f - iconSize / 2f);
		partIcon.localScale = Vector3.one * (iconSize / 2f);
		partScale = partIcon.localScale;
		partIcon.rotation = Quaternion.Euler(-15f, 0f, 0f);
		partIcon.Rotate(0f, -30f, 0f);
		startRot = partIcon.rotation;
		U5Util.SetLayerRecursive(gameObject, LayerMask.NameToLayer("UIAdditional"));
		materials = CreateMaterialArray(gameObject);
	}

	public static Material[] CreateMaterialArray(GameObject gameObject)
	{
		return CreateMaterialArray(gameObject, includeInactiveRenderers: false);
	}

	public static Material[] CreateMaterialArray(GameObject gameObject, bool includeInactiveRenderers)
	{
		List<Material> list = new List<Material>();
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(includeInactiveRenderers);
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			Material[] array = componentsInChildren[i].materials;
			int j = 0;
			for (int num2 = array.Length; j < num2; j++)
			{
				Material material = array[j];
				if (material.HasProperty(PropertyIDs._MinX))
				{
					list.Add(material);
				}
			}
		}
		return list.ToArray();
	}

	public static void CleanUpMaterials(GameObject gameObject)
	{
		Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>();
		int i = 0;
		for (int num = componentsInChildren.Length; i < num; i++)
		{
			if (!(componentsInChildren[i] == null))
			{
				Material[] array = componentsInChildren[i].materials;
				int j = 0;
				for (int num2 = array.Length; j < num2; j++)
				{
					Object.Destroy(array[j]);
				}
			}
		}
	}

	public void MouseInput_PointerEnter(PointerEventData data)
	{
		if (mouseOver || !InputLockManager.IsUnlocked(ControlTypes.EDITOR_ICON_HOVER))
		{
			return;
		}
		if (!isGrey && !stillFocused)
		{
			partRotation = 0f;
			if (partIcon != null)
			{
				partIcon.transform.localScale = partScale * iconOverScale;
			}
		}
		mouseOver = true;
	}

	public void MouseInput_PointerExit(PointerEventData data)
	{
		if (!mouseOver)
		{
			return;
		}
		if (InputLockManager.IsUnlocked(ControlTypes.EDITOR_ICON_HOVER))
		{
			if (!isGrey && partIcon != null)
			{
				partIcon.rotation = startRot;
				partIcon.localScale = partScale;
			}
		}
		else
		{
			stillFocused = true;
		}
		mouseOver = false;
	}

	private void Update()
	{
		CheckExperimental();
		CheckMissionRequired();
		if (mouseOver)
		{
			if (!isGrey)
			{
				partRotation += iconOverSpin * Time.deltaTime;
				if (partIcon != null)
				{
					partIcon.localRotation = startRot * Quaternion.AngleAxis(partRotation, Vector3.up);
				}
			}
		}
		else if (stillFocused)
		{
			stillFocused = false;
			MouseInput_PointerExit(null);
		}
	}

	public void Highlight()
	{
		MouseInput_PointerEnter(null);
	}

	public void Unhighlight()
	{
		MouseInput_PointerExit(null);
	}

	public void SetGrey(string why)
	{
		if (!isGrey)
		{
			btnSpawnPart.spriteState = buttonState;
			greyoutToolTipMessage = why;
			btnSpawnPart.interactable = false;
			SetPartColor(partIcon.gameObject, new Color(0.25f, 0.25f, 0.25f, 1f));
			isGrey = true;
		}
	}

	public void UnsetGrey()
	{
		if (isGrey)
		{
			btnSpawnPart.spriteState = buttonState;
			greyoutToolTipMessage = "";
			btnSpawnPart.interactable = true;
			SetPartColor(partIcon.gameObject, new Color(1f, 1f, 1f, 1f), availablePart);
			isGrey = false;
		}
	}

	public static void SetPartColor(GameObject partIcon, Color color, AvailablePart part = null)
	{
		bool flag = color == Color.white;
		Renderer[] componentsInChildren = partIcon.GetComponentsInChildren<Renderer>();
		int num = componentsInChildren.Length;
		while (num-- > 0)
		{
			Material[] array = componentsInChildren[num].materials;
			int num2 = array.Length;
			while (num2-- > 0)
			{
				bool flag2 = true;
				if (flag && part != null)
				{
					Renderer[] componentsInChildren2 = part.iconPrefab.GetComponentsInChildren<Renderer>();
					for (int i = 0; i < componentsInChildren2.Length; i++)
					{
						Material[] array2 = componentsInChildren2[i].materials;
						for (int j = 0; j < array2.Length; j++)
						{
							if (array2[j].name.Contains(array[num2].name))
							{
								array[num2].color = array2[j].color;
								flag2 = false;
								break;
							}
						}
						if (!flag2)
						{
							break;
						}
					}
				}
				if (flag2)
				{
					array[num2].color = color;
				}
			}
			componentsInChildren[num].materials = array;
		}
	}

	private void CheckExperimental()
	{
		if (checkedExperimental || HighLogic.CurrentGame == null)
		{
			return;
		}
		if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
		{
			checkedExperimental = true;
		}
		else
		{
			if (ResearchAndDevelopment.Instance == null)
			{
				return;
			}
			checkedExperimental = true;
			if (ResearchAndDevelopment.IsExperimentalPart(availablePart))
			{
				Image component = base.gameObject.GetComponent<Image>();
				if (component != null)
				{
					component.color = experimentalPartColor;
				}
			}
		}
	}

	private void CheckMissionRequired()
	{
		if (checkedMissionRequired || HighLogic.CurrentGame == null)
		{
			return;
		}
		if (HighLogic.CurrentGame.Mode != Game.Modes.MISSION)
		{
			checkedMissionRequired = true;
		}
		else
		{
			if (MissionSystem.Instance == null || MissionsApp.Instance == null || MissionsApp.Instance.CurrentVessel == null)
			{
				return;
			}
			checkedMissionRequired = true;
			if (availablePart != null && MissionsApp.Instance.CurrentVessel.vesselSituation.requiredParts.Contains(availablePart.name))
			{
				Image component = base.gameObject.GetComponent<Image>();
				if (component != null)
				{
					component.color = missionRequiredPartColor;
				}
			}
		}
	}
}
