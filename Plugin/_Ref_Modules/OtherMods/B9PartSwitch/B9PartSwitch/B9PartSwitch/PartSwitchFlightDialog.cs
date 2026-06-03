using System;
using System.Collections.Generic;
using System.Linq;
using B9PartSwitch.UI;
using TMPro;
using UnityEngine;

namespace B9PartSwitch;

public static class PartSwitchFlightDialog
{
	public static void Spawn(ModuleB9PartSwitch module)
	{
		try
		{
			MaybeCreateResourceRemovalWarning(module, delegate
			{
				CreateDialogue(module);
			});
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
			FatalErrorHandler.HandleFatalError(exception);
		}
	}

	private static void MaybeCreateResourceRemovalWarning(ModuleB9PartSwitch module, Action onConfirm)
	{
		if (HighLogic.LoadedSceneIsFlight && module.CurrentTankType.ResourceNames.Any((string name) => module.part.Resources[name].amount > 0.0))
		{
			CreateWarning(module, onConfirm);
		}
		else
		{
			onConfirm();
		}
	}

	private static void CreateWarning(ModuleB9PartSwitch module, Action onConfirm)
	{
		PopupDialog.SpawnPopupDialog(new MultiOptionDialog("B9PartSwitch_SwitchInFlightWarning", Localization.PartSwitchFlightDialog_ResourcesWillBeDumpedWarning(module.part.partInfo.title, module.switcherDescription), Localization.PartSwitchFlightDialog_ConfirmResourceRemovalDialogTitle, HighLogic.UISkin, new DialogGUIButton(Localization.PartSwitchFlightDialog_AcceptString, delegate
		{
			onConfirm();
		}), new DialogGUIButton(Localization.PartSwitchFlightDialog_CancelString, delegate
		{
		})), persistAcrossScenes: false, HighLogic.UISkin);
	}

	private static void CreateDialogue(ModuleB9PartSwitch module)
	{
		List<Callback> list = new List<Callback>();
		PopupDialog.SpawnPopupDialog(new MultiOptionDialog("B9PartSwitch_SwitchInFlight", Localization.PartSwitchFlightDialog_SelectNewSubtypeDialogTitle(module.switcherDescription), module.part.partInfo.title, HighLogic.UISkin, CreateOptions(module, list)), persistAcrossScenes: false, HighLogic.UISkin);
		foreach (Callback item in list)
		{
			item();
		}
	}

	private static DialogGUIBase[] CreateOptions(ModuleB9PartSwitch module, IList<Callback> afterCreateCallbacks)
	{
		List<DialogGUIBase> list = new List<DialogGUIBase>();
		SwitcherSubtypeDescriptionGenerator subtypeDescriptionGenerator = new SwitcherSubtypeDescriptionGenerator(module);
		foreach (PartSubtype subtype in module.subtypes)
		{
			if (!subtype.IsUnlocked())
			{
				continue;
			}
			if (subtype == module.CurrentSubtype)
			{
				string message = Localization.PartSwitchFlightDialog_CurrentSubtypeLabel(subtype.title);
				DialogGUILabel label = new DialogGUILabel(message, HighLogic.UISkin.button);
				afterCreateCallbacks.Add(delegate
				{
					TextMeshProUGUI component = label.uiItem.GetComponent<TextMeshProUGUI>();
					if ((object)component == null)
					{
						throw new Exception("Could not find TextMeshProUGUI");
					}
					component.raycastTarget = true;
				});
				afterCreateCallbacks.Add(delegate
				{
					TooltipHelper.SetupSubtypeInfoTooltip(label.uiItem, subtype.title, subtypeDescriptionGenerator.GetFullSubtypeDescription(subtype));
				});
				list.Add(label);
			}
			else if (HighLogic.LoadedSceneIsEditor || subtype.allowSwitchInFlight)
			{
				DialogGUIButton button = new DialogGUIButton(subtype.title, delegate
				{
					module.SwitchSubtype(subtype.Name);
				});
				afterCreateCallbacks.Add(delegate
				{
					TooltipHelper.SetupSubtypeInfoTooltip(button.uiItem, subtype.title, subtypeDescriptionGenerator.GetFullSubtypeDescription(subtype));
				});
				list.Add(button);
			}
		}
		list.Add(new DialogGUIButton(Localization.PartSwitchFlightDialog_CancelString, delegate
		{
		}));
		return list.ToArray();
	}
}
