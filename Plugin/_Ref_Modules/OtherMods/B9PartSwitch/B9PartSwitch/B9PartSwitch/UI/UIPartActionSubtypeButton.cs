using KSP.UI.TooltipTypes;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace B9PartSwitch.UI;

public class UIPartActionSubtypeButton : MonoBehaviour, IEventSystemHandler
{
	[SerializeField]
	private Image imagePrimaryColor;

	[SerializeField]
	private Image imageSecondaryColor;

	[SerializeField]
	private Image imageSelected;

	[SerializeField]
	private Image imageInvalid;

	[SerializeField]
	private Button buttonMain;

	[SerializeField]
	private TooltipController_TitleAndText tooltipController;

	public UIPartActionSubtypeButton previousItem;

	public UIPartActionSubtypeButton nextItem;

	private Callback setSubtype;

	public string Title { get; private set; }

	public string Description { get; private set; }

	public static GameObject CreatePrefab(GameObject variantButtonGameObject)
	{
		GameObject gameObject = Object.Instantiate(variantButtonGameObject);
		Object.DontDestroyOnLoad(gameObject);
		UIPartActionVariantButton component = gameObject.GetComponent<UIPartActionVariantButton>();
		UIPartActionSubtypeButton uIPartActionSubtypeButton = gameObject.AddComponent<UIPartActionSubtypeButton>();
		uIPartActionSubtypeButton.imagePrimaryColor = component.imagePrimaryColor;
		uIPartActionSubtypeButton.imageSecondaryColor = component.imageSecomdaryColor;
		uIPartActionSubtypeButton.imageSelected = component.imageSelected;
		uIPartActionSubtypeButton.imageInvalid = component.imageInvalid;
		uIPartActionSubtypeButton.buttonMain = component.buttonMain;
		uIPartActionSubtypeButton.tooltipController = gameObject.AddComponent<TooltipController_TitleAndText>();
		Object.Destroy(component);
		return gameObject;
	}

	public void Setup(string title, string description, Color primaryColor, Color secondaryColor, Callback setSubtype)
	{
		Title = title;
		Description = description;
		imagePrimaryColor.color = primaryColor;
		imageSecondaryColor.color = secondaryColor;
		TooltipHelper.SetupSubtypeInfoTooltip(tooltipController, title, description);
		buttonMain.onClick.AddListener(ButtonClick);
		this.setSubtype = setSubtype;
	}

	public void SetSubtype()
	{
		setSubtype();
	}

	public void Activate()
	{
		imageSelected.gameObject.SetActive(value: true);
	}

	public void Deactivate()
	{
		imageSelected.gameObject.SetActive(value: false);
	}

	private void ButtonClick()
	{
		setSubtype();
	}
}
