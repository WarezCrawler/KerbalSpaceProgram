using System;
using UnityEngine;
using UnityEngine.EventSystems;
using ns12;
using ns2;

namespace ns10;

public class EditorPanels : MonoBehaviour
{
	public enum Panel
	{
		None,
		Parts,
		Actions,
		Crew
	}

	public UIPanelTransitionManager panelManager;

	public UIPanelTransition partsEditor;

	public UIPanelTransition actions;

	public UIPanelTransition crew;

	public UIPanelTransition partsEditorModes;

	public UIPanelTransition partcategorizerModes;

	public UIPanelTransition searchField;

	private Panel panel;

	public static EditorPanels Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		if (Instance != null && Instance == this)
		{
			Instance = null;
		}
	}

	public void updatePartsListMode(Action onFinished = null)
	{
		if (EditorLogic.Mode == EditorLogic.EditorModes.SIMPLE)
		{
			partcategorizerModes.Transition("Simple", onFinished);
		}
		else
		{
			partcategorizerModes.Transition("Advanced", onFinished);
		}
	}

	public bool ShowPartsList(Action onFinished = null)
	{
		if (PartListTooltipMasterController.Instance != null)
		{
			PartListTooltipMasterController.Instance.HideTooltip();
		}
		if (panel == Panel.Parts)
		{
			return false;
		}
		panel = Panel.Parts;
		panelManager.BringIn(partsEditor, onFinished);
		searchField.Transition("In");
		updatePartsListMode(onFinished);
		GameEvents.onEditorShowPartList.Fire();
		return true;
	}

	public bool ShowActionGroups(Action onFinished = null)
	{
		if (PartListTooltipMasterController.Instance != null)
		{
			PartListTooltipMasterController.Instance.HideTooltip();
		}
		if (panel == Panel.Actions)
		{
			return false;
		}
		panel = Panel.Actions;
		panelManager.BringIn(actions, onFinished);
		searchField.Transition("Out");
		return true;
	}

	public bool ShowCrewAssignment(Action onFinished = null)
	{
		if (PartListTooltipMasterController.Instance != null)
		{
			PartListTooltipMasterController.Instance.HideTooltip();
		}
		if (panel == Panel.Crew)
		{
			return false;
		}
		panel = Panel.Crew;
		panelManager.BringIn(crew, onFinished);
		searchField.Transition("Out");
		return true;
	}

	public bool IsMouseOver()
	{
		return EventSystem.current.IsPointerOverGameObject();
	}
}
