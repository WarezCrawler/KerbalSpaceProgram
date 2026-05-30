using System.Collections;
using System.Collections.Generic;
using Expansions.Missions.Editor;
using ns9;

namespace Expansions.Missions.Actions;

public class ActionMissionScore : ActionModule
{
	[MEGUI_DynamicModuleList(onControlCreated = "OnMissionScoreModuleCreated", guiName = "#autoLOC_8000079")]
	public MissionScore score;

	private MEGUIParameterDynamicModuleList dynamicModuleListRef;

	public override void Awake()
	{
		base.Awake();
		title = Localizer.Format("#autoLOC_8000078");
	}

	public override void Initialize(MENode node)
	{
		base.Initialize(node);
		score = new MissionScore(node);
		if (score.activeModules.Count == 0)
		{
			score.activeModules.Add(new ScoreModule_Completion(node));
		}
	}

	public override IEnumerator Fire()
	{
		if (node.mission.isScoreEnabled)
		{
			score.AwardScore(node.mission);
		}
		yield return null;
	}

	public string GetScoreDescription()
	{
		return score.ScoreDescription();
	}

	public string GetAwardedScoreDescription()
	{
		if (!node.mission.isScoreEnabled)
		{
			return "";
		}
		return score.AwarededScoreDescription();
	}

	public override List<IMENodeDisplay> GetInternalParametersToDisplay()
	{
		return score.GetInternalParameters();
	}

	private void OnMissionScoreModuleCreated(MEGUIParameterDynamicModuleList parameter)
	{
		dynamicModuleListRef = parameter;
		if (node.guiNode != null)
		{
			node.guiNode.DockStatusChange += OnDockStatusChange;
		}
	}

	private void OnDockStatusChange(MEGUINode node)
	{
		dynamicModuleListRef.RefreshUI();
	}

	public override string GetInfo()
	{
		return Localizer.Format("#autoLOC_8004002");
	}

	public override void Save(ConfigNode node)
	{
		base.Save(node);
		score.Save(node);
	}

	public override void Load(ConfigNode node)
	{
		base.Load(node);
		score.Load(node);
	}
}
