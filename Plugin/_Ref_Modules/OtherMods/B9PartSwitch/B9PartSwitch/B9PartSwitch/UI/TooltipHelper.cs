using KSP.UI;
using KSP.UI.TooltipTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace B9PartSwitch.UI;

public static class TooltipHelper
{
	private static Tooltip subtypeInfoTooltip;

	public static void EnsurePrefabs()
	{
		if (!subtypeInfoTooltip.IsNotNull())
		{
			subtypeInfoTooltip = CreateSubtypeInfoTooltipPrefab();
			Debug.Log("[B9PartSwitch.UI.TooltipHelper] created subtype info tooltip prefab");
		}
	}

	public static TooltipController_TitleAndText SetupSubtypeInfoTooltip(GameObject gameObject, string titleString, string textString)
	{
		TooltipController_TitleAndText tooltipController_TitleAndText = gameObject.AddOrGetComponent<TooltipController_TitleAndText>();
		tooltipController_TitleAndText.TooltipPrefabType = subtypeInfoTooltip;
		tooltipController_TitleAndText.titleString = titleString;
		tooltipController_TitleAndText.textString = textString;
		return tooltipController_TitleAndText;
	}

	public static void SetupSubtypeInfoTooltip(TooltipController_TitleAndText tooltipController, string titleString, string textString)
	{
		tooltipController.TooltipPrefabType = subtypeInfoTooltip;
		tooltipController.titleString = titleString;
		tooltipController.textString = textString;
	}

	private static Tooltip CreateSubtypeInfoTooltipPrefab()
	{
		GameObject gameObject = Object.Instantiate(AssetBase.GetPrefab<Tooltip>("Tooltip_TitleAndText").gameObject);
		Object.DontDestroyOnLoad(gameObject);
		gameObject.GetChild("Text").GetComponent<TextMeshProUGUI>().alpha = 0.9f;
		gameObject.GetChild("Title").GetComponent<LayoutElement>().minWidth = 300f;
		return gameObject.GetComponent<Tooltip>();
	}
}
