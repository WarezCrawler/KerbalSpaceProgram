using Expansions.Missions.Editor;
using ns9;

namespace Expansions.Missions.Actions;

public class ActionVessel : ActionModule
{
	[MEGUI_VesselSelect(order = 0, onControlCreated = "vesselSelectorCreated", resetValue = "0", gapDisplay = true, guiName = "#autoLOC_8000001")]
	public uint vesselID;

	protected bool useActiveVessel;

	private Vessel _vessel;

	public Vessel vessel
	{
		get
		{
			if (_vessel == null)
			{
				if (vesselID == 0)
				{
					if (FlightGlobals.ActiveVessel != null)
					{
						_vessel = FlightGlobals.ActiveVessel;
					}
				}
				else if (FlightGlobals.PersistentVesselIds.ContainsKey(vesselID))
				{
					_vessel = FlightGlobals.PersistentVesselIds[vesselID];
				}
			}
			return _vessel;
		}
	}

	public override void Awake()
	{
		base.Awake();
	}

	public override void OnVesselPersistentIdChanged(uint oldId, uint newId)
	{
		if (vesselID == oldId)
		{
			vesselID = newId;
		}
	}

	public override string GetNodeBodyParameterString(BaseAPField field)
	{
		if (field.name == "vesselID")
		{
			VesselSituation vesselSituationByVesselID = node.mission.GetVesselSituationByVesselID(vesselID);
			if (vesselSituationByVesselID == null)
			{
				return Localizer.Format("#autoLOC_8000069", useActiveVessel ? Localizer.Format("#autoLOC_8004217") : Localizer.Format("#autoLOC_8001004"));
			}
			string text = vesselSituationByVesselID.vesselName;
			if (HighLogic.LoadedSceneIsMissionBuilder)
			{
				MissionCraft craftBySituationsVesselID = MissionEditorLogic.Instance.EditorMission.GetCraftBySituationsVesselID(vesselSituationByVesselID.persistentId);
				if (craftBySituationsVesselID != null && MissionEditorLogic.Instance.incompatibleCraft.Contains(craftBySituationsVesselID.craftFile))
				{
					text = Localizer.Format("#autoLOC_8004245", text);
				}
			}
			return Localizer.Format("#autoLOC_8000069", text);
		}
		return base.GetNodeBodyParameterString(field);
	}

	public void vesselSelectorCreated(MEGUIParameter parameter)
	{
		(parameter as MEGUIParameterVesselDropdownList).OverrideDefaultValue(useActiveVessel);
	}

	public override void Save(ConfigNode node)
	{
		base.Save(node);
		node.AddValue("vesselID", vesselID);
	}

	public override void Load(ConfigNode node)
	{
		base.Load(node);
		node.TryGetValue("vesselID", ref vesselID);
	}
}
