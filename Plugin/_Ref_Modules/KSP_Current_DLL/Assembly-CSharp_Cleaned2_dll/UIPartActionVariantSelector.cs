using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ns9;

[UI_VariantSelector]
public class UIPartActionVariantSelector : UIPartActionFieldItem
{
	public GameObject prefabVariantButton;

	public Button buttonPrevious;

	public Button buttonNext;

	public ScrollRect scrollMain;

	public TextMeshProUGUI variantName;

	private int fieldValue;

	private UI_VariantSelector variantSelector;

	private List<UIPartActionVariantButton> buttonList;

	public override void Setup(UIPartActionWindow window, Part part, PartModule partModule, UI_Scene scene, UI_Control control, BaseField field)
	{
		base.Setup(window, part, partModule, scene, control, field);
		buttonNext.onClick.AddListener(OnButtonNext);
		buttonPrevious.onClick.AddListener(OnButtonPrev);
		fieldValue = GetFieldValue();
		RefreshVariantButtons();
		if (variantSelector == null)
		{
			variantSelector = (UI_VariantSelector)control;
		}
	}

	public override void UpdateItem()
	{
		base.transform.SetAsLastSibling();
	}

	public void RefreshVariantButtons()
	{
		buttonList = new List<UIPartActionVariantButton>();
		if (variantSelector == null)
		{
			variantSelector = (UI_VariantSelector)control;
		}
		if (variantSelector != null && variantSelector.variants != null)
		{
			for (int i = 0; i < variantSelector.variants.Count; i++)
			{
				string hexSecondaryColor = variantSelector.variants[i].PrimaryColor;
				if (!string.IsNullOrEmpty(variantSelector.variants[i].SecondaryColor))
				{
					hexSecondaryColor = variantSelector.variants[i].SecondaryColor;
				}
				UIPartActionVariantButton uIPartActionVariantButton = AddVariantButton(i, variantSelector.variants[i].PrimaryColor, hexSecondaryColor);
				if (GetSurfaceAttachedPartCount() > 0)
				{
					uIPartActionVariantButton.Locked = variantSelector.variants[i].SizeGroup != variantSelector.variants[fieldValue].SizeGroup;
				}
				buttonList.Add(uIPartActionVariantButton);
			}
			if (buttonList.Count > fieldValue)
			{
				buttonList[fieldValue].Select();
				variantName.text = variantSelector.variants[fieldValue].DisplayName;
			}
		}
		else
		{
			Debug.Log("UIPartActionVariantSelector: Could not find any variants - on part " + part.name);
		}
	}

	private int GetSurfaceAttachedPartCount()
	{
		int num = 0;
		for (int i = 0; i < part.transform.childCount; i++)
		{
			Part component = part.transform.GetChild(i).GetComponent<Part>();
			if (component != null && component.attachMode == AttachModes.SRF_ATTACH)
			{
				num++;
			}
		}
		return num;
	}

	public UIPartActionVariantButton AddVariantButton(int entryIndex, string hexPrimaryColor, string hexSecondaryColor)
	{
		UIPartActionVariantButton component = Object.Instantiate(prefabVariantButton, scrollMain.content).GetComponent<UIPartActionVariantButton>();
		component.Setup(this, entryIndex, hexPrimaryColor, hexSecondaryColor);
		return component;
	}

	private int GetFieldValue()
	{
		return field.GetValue<int>(field.host);
	}

	public void OnButtonPressed(int newIndex)
	{
		SelectVariant(newIndex);
	}

	public void SetButtonOverText(int entryIndex)
	{
		if (entryIndex != fieldValue)
		{
			string text = Localizer.Format(variantSelector.variants[entryIndex].DisplayName);
			if (buttonList[entryIndex].Locked)
			{
				variantName.text = Localizer.Format("#autoLOC_8007003", text);
			}
			else
			{
				variantName.text = Localizer.Format("#autoLOC_8007004", text);
			}
		}
		else
		{
			variantName.text = variantSelector.variants[fieldValue].DisplayName;
		}
	}

	public void ResetButtonOverText()
	{
		variantName.text = variantSelector.variants[fieldValue].DisplayName;
	}

	public void OnButtonNext()
	{
		int newIndex = (fieldValue + 1) % variantSelector.variants.Count;
		SelectVariant(newIndex);
	}

	public void OnButtonPrev()
	{
		int newIndex = (fieldValue + variantSelector.variants.Count - 1) % variantSelector.variants.Count;
		SelectVariant(newIndex);
	}

	private bool SelectVariant(int newIndex)
	{
		buttonList[fieldValue].Reset();
		buttonList[newIndex].Select();
		variantName.text = variantSelector.variants[newIndex].DisplayName;
		fieldValue = newIndex;
		SetFieldValue(fieldValue);
		return true;
	}
}
