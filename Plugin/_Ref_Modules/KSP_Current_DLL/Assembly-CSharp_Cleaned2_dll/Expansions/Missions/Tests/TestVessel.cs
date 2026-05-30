using System.Collections.Generic;
using Expansions.Missions.Editor;
using UnityEngine;
using ns9;

namespace Expansions.Missions.Tests;

public class TestVessel : TestModule
{
	[MEGUI_VesselSelect(order = 0, onControlCreated = "vesselSelectorCreated", resetValue = "0", gapDisplay = true, guiName = "#autoLOC_8000001")]
	public uint vesselID;

	protected bool useActiveVessel;

	protected Vessel vessel;

	private List<uint> missingVessels;

	private string bodyScore;

	public override void Awake()
	{
		base.Awake();
		missingVessels = new List<uint>();
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
			VesselSituation vesselSituationByVesselID = base.node.mission.GetVesselSituationByVesselID(vesselID);
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
		if (field.name == "resourceName")
		{
			PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition(field.GetValue().ToString());
			return Localizer.Format("#autoLOC_8004190", field.guiName, definition.displayName);
		}
		return base.GetNodeBodyParameterString(field);
	}

	public override bool Test()
	{
		if (vesselID == 0)
		{
			vessel = FlightGlobals.ActiveVessel;
		}
		else if (FlightGlobals.PersistentVesselIds.ContainsKey(vesselID))
		{
			vessel = FlightGlobals.PersistentVesselIds[vesselID];
		}
		else if (vessel != null)
		{
			if (!missingVessels.Contains(vesselID))
			{
				missingVessels.Add(vesselID);
				Debug.LogErrorFormat("[TestVessel] Unable to find VesselID ({0}) from Node ({1}) in FlightGlobals.", vesselID, base.node.Title);
			}
			vessel = null;
		}
		return true;
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
