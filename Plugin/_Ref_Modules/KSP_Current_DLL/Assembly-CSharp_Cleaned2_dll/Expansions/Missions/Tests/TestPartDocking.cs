using System;
using System.Collections.Generic;
using Expansions.Missions.Editor;
using UnityEngine;
using ns9;

namespace Expansions.Missions.Tests;

[MEScoreModule(new Type[]
{
	typeof(ScoreModule_Resource),
	typeof(ScoreModule_Time)
})]
public class TestPartDocking : TestModule
{
	[MEGUI_VesselPartSelect(resetValue = "0", gapDisplay = true, guiName = "#autoLOC_8002008", Tooltip = "#autoLOC_8002009")]
	public VesselPartIDPair partOnevesselPartIDs;

	[MEGUI_VesselPartSelect(resetValue = "0", gapDisplay = true, guiName = "#autoLOC_8002010", Tooltip = "#autoLOC_8002011")]
	public VesselPartIDPair partTwovesselPartIDs;

	[MEGUI_Dropdown(SetDropDownItems = "SetDockedUndockedDropdown", canBePinned = true, guiName = "#autoLOC_8002012")]
	public string dockedUndocked;

	private bool firstRunTest;

	private bool testSuccess;

	public override void Awake()
	{
		base.Awake();
		title = "#autoLOC_8002013";
		partOnevesselPartIDs = new VesselPartIDPair();
		partTwovesselPartIDs = new VesselPartIDPair();
		firstRunTest = true;
		testSuccess = false;
	}

	public List<MEGUIDropDownItem> SetDockedUndockedDropdown()
	{
		return new List<MEGUIDropDownItem>
		{
			new MEGUIDropDownItem("Docked", "Docked", "#autoLOC_8002014"),
			new MEGUIDropDownItem("Undocked", "Undocked", "#autoLOC_8002015")
		};
	}

	public override void Initialized()
	{
		testSuccess = false;
		if (dockedUndocked == "Docked")
		{
			GameEvents.onDockingComplete.Add(onPartCouple);
			GameEvents.onPartCoupleComplete.Add(onPartCouple);
		}
		else
		{
			GameEvents.onPartUndockComplete.Add(onPartUndock);
			GameEvents.onPartDeCoupleComplete.Add(onPartUndock);
		}
	}

	private void setPlayerCreatedRootParts()
	{
		if (partOnevesselPartIDs.partID == 0)
		{
			bool flag = false;
			if (FlightGlobals.PersistentVesselIds.ContainsKey(partOnevesselPartIDs.VesselID))
			{
				Vessel vessel = FlightGlobals.PersistentVesselIds[partOnevesselPartIDs.VesselID];
				if (vessel != null)
				{
					if (vessel.loaded)
					{
						partOnevesselPartIDs.partID = vessel.rootPart.persistentId;
						flag = true;
					}
					else if (vessel.protoVessel.protoPartSnapshots.Count > vessel.protoVessel.rootIndex)
					{
						partOnevesselPartIDs.partID = vessel.protoVessel.protoPartSnapshots[vessel.protoVessel.rootIndex].persistentId;
						flag = true;
					}
				}
			}
			if (!flag)
			{
				Debug.LogFormat("[TestPartDocking]: Node ({0}) Part One Player Created Vessel. Unable to find root part. Bypassing Test Node.", base.node.id);
				testSuccess = true;
			}
		}
		if (partTwovesselPartIDs.partID != 0)
		{
			return;
		}
		bool flag2 = false;
		if (FlightGlobals.PersistentVesselIds.ContainsKey(partTwovesselPartIDs.VesselID))
		{
			Vessel vessel2 = FlightGlobals.PersistentVesselIds[partTwovesselPartIDs.VesselID];
			if (vessel2 != null)
			{
				if (vessel2.loaded)
				{
					partTwovesselPartIDs.partID = vessel2.rootPart.persistentId;
					flag2 = true;
				}
				else if (vessel2.protoVessel.protoPartSnapshots.Count > vessel2.protoVessel.rootIndex)
				{
					partOnevesselPartIDs.partID = vessel2.protoVessel.protoPartSnapshots[vessel2.protoVessel.rootIndex].persistentId;
					flag2 = true;
				}
			}
		}
		if (!flag2)
		{
			Debug.LogFormat("[TestPartDocking]: Node ({0}) Part Two Player Created Vessel. Unable to find root part. Bypassing Test Node.", base.node.id);
			testSuccess = true;
		}
	}

	public override void Cleared()
	{
		GameEvents.onDockingComplete.Remove(onPartCouple);
		GameEvents.onPartCoupleComplete.Remove(onPartCouple);
		GameEvents.onPartUndockComplete.Remove(onPartUndock);
		GameEvents.onPartDeCoupleComplete.Remove(onPartUndock);
	}

	private void onPartCouple(GameEvents.FromToAction<Part, Part> partAction)
	{
		testPartVessels();
	}

	private void onPartUndock(Part part)
	{
		testPartVessels();
	}

	private void testPartVessels()
	{
		uint vesselId = 0u;
		uint vesselId2 = 0u;
		if (partOnevesselPartIDs.partID == 0 || partTwovesselPartIDs.partID == 0)
		{
			setPlayerCreatedRootParts();
		}
		if (!(findPartVesselId(partOnevesselPartIDs.partID, out vesselId) & findPartVesselId(partTwovesselPartIDs.partID, out vesselId2)))
		{
			return;
		}
		if (dockedUndocked == "Docked")
		{
			if (vesselId == vesselId2)
			{
				testSuccess = true;
			}
		}
		else if (vesselId != vesselId2)
		{
			testSuccess = true;
		}
	}

	private bool findPartVesselId(uint partId, out uint vesselId)
	{
		vesselId = 0u;
		bool result = false;
		ProtoPartSnapshot partout2;
		if (FlightGlobals.FindLoadedPart(partId, out var partout))
		{
			result = true;
			vesselId = partout.vessel.persistentId;
		}
		else if (FlightGlobals.FindUnloadedPart(partId, out partout2))
		{
			result = true;
			vesselId = partout2.pVesselRef.persistentId;
		}
		return result;
	}

	public override bool Test()
	{
		if (firstRunTest)
		{
			testPartVessels();
			firstRunTest = false;
		}
		return testSuccess;
	}

	public override void OnPartPersistentIdChanged(uint vesselID, uint oldId, uint newId)
	{
		if (partOnevesselPartIDs.VesselID == vesselID && partOnevesselPartIDs.partID == oldId)
		{
			partOnevesselPartIDs.partID = newId;
			Debug.LogFormat("[TestPartDocking]: Node ({0}) Part One PartId changed from {1} to {2}", base.node.id, oldId, newId);
		}
		if (partTwovesselPartIDs.VesselID == vesselID && partTwovesselPartIDs.partID == oldId)
		{
			partTwovesselPartIDs.partID = newId;
			Debug.LogFormat("[TestPartDocking]: Node ({0}) Part Two PartId changed from {1} to {2}", base.node.id, oldId, newId);
		}
	}

	public override void OnVesselPersistentIdChanged(uint oldId, uint newId)
	{
		if (partOnevesselPartIDs.VesselID == oldId)
		{
			partOnevesselPartIDs.VesselID = newId;
			Debug.LogFormat("[TestPartDocking]: Node ({0}) Part One VesselId changed from {1} to {2}", base.node.id, oldId, newId);
		}
		if (partTwovesselPartIDs.VesselID == oldId)
		{
			partTwovesselPartIDs.VesselID = newId;
			Debug.LogFormat("[TestPartDocking]: Node ({0}) Part Two VesselId changed from {1} to {2}", base.node.id, oldId, newId);
		}
	}

	public override void OnVesselDocking(uint oldId, uint newId)
	{
		if ((partOnevesselPartIDs.VesselID == oldId && partOnevesselPartIDs.partID == 0) || (partTwovesselPartIDs.VesselID == oldId && partTwovesselPartIDs.partID == 0))
		{
			setPlayerCreatedRootParts();
		}
	}

	public override string GetInfo()
	{
		return Localizer.Format("#autoLOC_8002016");
	}

	public override string GetNodeBodyParameterString(BaseAPField field)
	{
		if (field.name == "partOnevesselPartIDs")
		{
			VesselSituation vesselSituationByVesselID = base.node.mission.GetVesselSituationByVesselID(partOnevesselPartIDs.VesselID);
			string text = "";
			if (vesselSituationByVesselID != null)
			{
				text = Localizer.Format("#autoLOC_8000001") + ": " + Localizer.Format(vesselSituationByVesselID.vesselName) + "\n";
			}
			if (vesselSituationByVesselID != null && vesselSituationByVesselID.playerCreated)
			{
				return text + Localizer.Format("#autoLOC_8006059", Localizer.Format("#autoLOC_8001005"));
			}
			string text2 = ((partOnevesselPartIDs.partName == "Any") ? Localizer.Format("#autoLOC_8001004") : partOnevesselPartIDs.partName);
			return text + Localizer.Format("#autoLOC_8006059", text2);
		}
		if (field.name == "partTwovesselPartIDs")
		{
			VesselSituation vesselSituationByVesselID2 = base.node.mission.GetVesselSituationByVesselID(partTwovesselPartIDs.VesselID);
			string text3 = "";
			if (vesselSituationByVesselID2 != null)
			{
				text3 = Localizer.Format("#autoLOC_8000001") + ": " + Localizer.Format(vesselSituationByVesselID2.vesselName) + "\n";
			}
			if (vesselSituationByVesselID2 != null && vesselSituationByVesselID2.playerCreated)
			{
				return text3 + Localizer.Format("#autoLOC_8006060", Localizer.Format("#autoLOC_8001005"));
			}
			string text4 = ((partTwovesselPartIDs.partName == "Any") ? Localizer.Format("#autoLOC_8001004") : partTwovesselPartIDs.partName);
			return text3 + Localizer.Format("#autoLOC_8006060", text4);
		}
		if (field.name == "dockedUndocked" && field.GetValue() != null)
		{
			if (field.GetValue().ToString() == "Docked")
			{
				return Localizer.Format("#autoLOC_8004190", field.guiName, Localizer.Format("#autoLOC_8002014"));
			}
			return Localizer.Format("#autoLOC_8004190", field.guiName, Localizer.Format("#autoLOC_8002015"));
		}
		return base.GetNodeBodyParameterString(field);
	}

	public override void RunValidation(MissionEditorValidator validator)
	{
		if (partOnevesselPartIDs != null && partTwovesselPartIDs != null)
		{
			VesselSituation vesselSituationByVesselID = base.node.mission.GetVesselSituationByVesselID(partOnevesselPartIDs.VesselID);
			VesselSituation vesselSituationByVesselID2 = base.node.mission.GetVesselSituationByVesselID(partTwovesselPartIDs.VesselID);
			if (vesselSituationByVesselID == null)
			{
				validator.AddNodeFail(base.node, Localizer.Format("#autoLOC_8002043"));
			}
			if (vesselSituationByVesselID2 == null)
			{
				validator.AddNodeFail(base.node, Localizer.Format("#autoLOC_8002044"));
			}
			if (partOnevesselPartIDs.partID == 0 && (vesselSituationByVesselID == null || !vesselSituationByVesselID.playerCreated))
			{
				validator.AddNodeFail(base.node, Localizer.Format("#autoLOC_8002047"));
			}
			if (partTwovesselPartIDs.partID == 0 && (vesselSituationByVesselID2 == null || !vesselSituationByVesselID2.playerCreated))
			{
				validator.AddNodeFail(base.node, Localizer.Format("#autoLOC_8002048"));
			}
			if (partOnevesselPartIDs.VesselID == partTwovesselPartIDs.VesselID && partOnevesselPartIDs.partID == partTwovesselPartIDs.partID)
			{
				validator.AddNodeFail(base.node, Localizer.Format("#autoLOC_8002049"));
			}
		}
		if (partOnevesselPartIDs != null)
		{
			partOnevesselPartIDs.ValidatePartAgainstCraft(base.node, validator);
		}
		if (partTwovesselPartIDs != null)
		{
			partTwovesselPartIDs.ValidatePartAgainstCraft(base.node, validator);
		}
	}

	public override void Save(ConfigNode node)
	{
		base.Save(node);
		ConfigNode configNode = new ConfigNode("PARTONE");
		partOnevesselPartIDs.Save(configNode);
		node.AddNode(configNode);
		ConfigNode configNode2 = new ConfigNode("PARTTWO");
		partTwovesselPartIDs.Save(configNode2);
		node.AddNode(configNode2);
		node.AddValue("dockedUndocked", dockedUndocked);
	}

	public override void Load(ConfigNode node)
	{
		base.Load(node);
		ConfigNode configNode = new ConfigNode("PARTONE");
		if (node.TryGetNode("PARTONE", ref configNode))
		{
			partOnevesselPartIDs.Load(configNode);
		}
		ConfigNode configNode2 = new ConfigNode("PARTTWO");
		if (node.TryGetNode("PARTTWO", ref configNode2))
		{
			partTwovesselPartIDs.Load(configNode2);
		}
		dockedUndocked = "Docked";
		node.TryGetValue("dockedUndocked", ref dockedUndocked);
	}
}
