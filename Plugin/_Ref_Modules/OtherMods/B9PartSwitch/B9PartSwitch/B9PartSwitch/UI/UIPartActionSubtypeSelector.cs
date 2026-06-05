using System;
using System.Collections.Generic;
using System.Linq;
using KSP.UI.TooltipTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace B9PartSwitch.UI;

[UI_SubtypeSelector]
public class UIPartActionSubtypeSelector : UIPartActionFieldItem
{
	private static UIPartActionSubtypeSelector partActionSubtypeSelectorPrefab;

	[SerializeField]
	private GameObject prefabVariantButton;

	[SerializeField]
	private Button buttonPrevious;

	[SerializeField]
	private Button buttonNext;

	[SerializeField]
	private ScrollRect scrollMain;

	[SerializeField]
	private TextMeshProUGUI switcherDescriptionText;

	[SerializeField]
	private TextMeshProUGUI subtypeTitleText;

	[SerializeField]
	private TooltipController_TitleAndText buttonPreviousTooltipController;

	[SerializeField]
	private TooltipController_TitleAndText buttonNextTooltipController;

	private List<UIPartActionSubtypeButton> subtypeButtons;

	private int currentButtonIndex = -1;

	public static void EnsurePrefab()
	{
		if (UIPartActionController.Instance.IsNull())
		{
			throw new InvalidOperationException("UIPartActionController.Instance is null");
		}
		if (partActionSubtypeSelectorPrefab.IsNull())
		{
			partActionSubtypeSelectorPrefab = CreatePrefab();
		}
		else if (UIPartActionController.Instance.fieldPrefabs.Contains(partActionSubtypeSelectorPrefab))
		{
			return;
		}
		UIPartActionController.Instance.fieldPrefabs.Add(partActionSubtypeSelectorPrefab);
		Debug.Log("[B9PartSwitch.UI.UIPartActionSubtypeSelector] added prefab to UIPartActionController");
	}

	public override void Setup(UIPartActionWindow window, Part part, PartModule partModule, UI_Scene scene, UI_Control control, BaseField field)
	{
		base.Setup(window, part, partModule, scene, control, field);
		ModuleB9PartSwitch moduleB9PartSwitch = (ModuleB9PartSwitch)partModule;
		SwitcherSubtypeDescriptionGenerator switcherSubtypeDescriptionGenerator = new SwitcherSubtypeDescriptionGenerator(moduleB9PartSwitch);
		subtypeButtons = new List<UIPartActionSubtypeButton>(moduleB9PartSwitch.subtypes.Count);
		int num = 0;
		for (int i = 0; i < moduleB9PartSwitch.subtypes.Count; i++)
		{
			PartSubtype partSubtype = moduleB9PartSwitch.subtypes[i];
			if (partSubtype.IsUnlocked())
			{
				UIPartActionSubtypeButton component = UnityEngine.Object.Instantiate(prefabVariantButton, scrollMain.content).GetComponent<UIPartActionSubtypeButton>();
				int subtypeIndex = i;
				int theButtonIndex = num;
				component.Setup(partSubtype.title, switcherSubtypeDescriptionGenerator.GetFullSubtypeDescription(partSubtype), partSubtype.PrimaryColor, partSubtype.SecondaryColor, delegate
				{
					SetSubtype(subtypeIndex, theButtonIndex);
				});
				subtypeButtons.Add(component);
				if (partSubtype == moduleB9PartSwitch.CurrentSubtype)
				{
					currentButtonIndex = num;
				}
				num++;
			}
		}
		subtypeButtons[0].previousItem = subtypeButtons[subtypeButtons.Count - 1];
		subtypeButtons[0].nextItem = subtypeButtons[1];
		for (int j = 1; j < subtypeButtons.Count - 1; j++)
		{
			subtypeButtons[j].previousItem = subtypeButtons[j - 1];
			subtypeButtons[j].nextItem = subtypeButtons[j + 1];
		}
		subtypeButtons[subtypeButtons.Count - 1].previousItem = subtypeButtons[subtypeButtons.Count - 2];
		subtypeButtons[subtypeButtons.Count - 1].nextItem = subtypeButtons[0];
		switcherDescriptionText.text = moduleB9PartSwitch.switcherDescription;
		TooltipHelper.SetupSubtypeInfoTooltip(buttonPreviousTooltipController, "", "");
		TooltipHelper.SetupSubtypeInfoTooltip(buttonNextTooltipController, "", "");
		SetTooltips(currentButtonIndex);
		buttonPrevious.onClick.AddListener(PreviousSubtype);
		buttonNext.onClick.AddListener(NextSubtype);
		subtypeTitleText.text = moduleB9PartSwitch.CurrentSubtype.title;
		subtypeButtons[currentButtonIndex].Activate();
	}

	private void PreviousSubtype()
	{
		subtypeButtons[currentButtonIndex].previousItem.SetSubtype();
	}

	private void NextSubtype()
	{
		subtypeButtons[currentButtonIndex].nextItem.SetSubtype();
	}

	private void SetSubtype(int subtypeIndex, int buttonIndex)
	{
		if (buttonIndex != currentButtonIndex)
		{
			subtypeButtons[currentButtonIndex].Deactivate();
			currentButtonIndex = buttonIndex;
			subtypeButtons[currentButtonIndex].Activate();
			subtypeTitleText.text = subtypeButtons[currentButtonIndex].Title;
			SetTooltips(currentButtonIndex);
			SetFieldValue(subtypeIndex);
		}
	}

	private void SetTooltips(int index)
	{
		buttonPreviousTooltipController.titleString = subtypeButtons[index].previousItem.Title;
		buttonPreviousTooltipController.textString = subtypeButtons[index].previousItem.Description;
		buttonNextTooltipController.titleString = subtypeButtons[index].nextItem.Title;
		buttonNextTooltipController.textString = subtypeButtons[index].nextItem.Description;
	}

	public static UIPartActionSubtypeSelector CreatePrefab()
	{
		GameObject obj = UIPartActionController.Instance.fieldPrefabs.OfType<UIPartActionVariantSelector>().FirstOrDefault()?.gameObject;
		if (obj.IsNull())
		{
			throw new Exception("Could not find FieldVariantSelector prefab");
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(obj);
		UnityEngine.Object.DontDestroyOnLoad(gameObject);
		UIPartActionVariantSelector component = gameObject.GetComponent<UIPartActionVariantSelector>();
		UIPartActionSubtypeSelector uIPartActionSubtypeSelector = gameObject.AddComponent<UIPartActionSubtypeSelector>();
		uIPartActionSubtypeSelector.buttonPrevious = component.buttonPrevious;
		uIPartActionSubtypeSelector.buttonNext = component.buttonNext;
		uIPartActionSubtypeSelector.scrollMain = component.scrollMain;
		uIPartActionSubtypeSelector.subtypeTitleText = component.variantName;
		uIPartActionSubtypeSelector.buttonPreviousTooltipController = uIPartActionSubtypeSelector.buttonPrevious.gameObject.AddComponent<TooltipController_TitleAndText>();
		uIPartActionSubtypeSelector.buttonNextTooltipController = uIPartActionSubtypeSelector.buttonNext.gameObject.AddComponent<TooltipController_TitleAndText>();
		uIPartActionSubtypeSelector.switcherDescriptionText = gameObject.GetChild("Label_Variants").GetComponent<TextMeshProUGUI>();
		uIPartActionSubtypeSelector.subtypeTitleText.alignment = TextAlignmentOptions.Right;
		RectTransform obj2 = (RectTransform)uIPartActionSubtypeSelector.switcherDescriptionText.gameObject.transform;
		obj2.anchorMin = new Vector2(0f, 1f);
		obj2.anchorMax = new Vector2(0f, 1f);
		obj2.pivot = new Vector2(0f, 0.5f);
		obj2.anchoredPosition = new Vector2(0f, -5f);
		obj2.sizeDelta += new Vector2(45f, 0f);
		RectTransform obj3 = (RectTransform)uIPartActionSubtypeSelector.subtypeTitleText.gameObject.transform;
		obj3.pivot = new Vector2(1f, 0.5f);
		obj3.anchoredPosition = new Vector2(0f, -5f);
		obj3.sizeDelta -= new Vector2(20f, 0f);
		uIPartActionSubtypeSelector.prefabVariantButton = UIPartActionSubtypeButton.CreatePrefab(component.prefabVariantButton);
		UnityEngine.Object.Destroy(component);
		return uIPartActionSubtypeSelector;
	}
}
