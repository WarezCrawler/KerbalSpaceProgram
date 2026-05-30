using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Expansions;
using Expansions.Missions;
using Expansions.Missions.Runtime;
using UnityEngine;
using ns9;

namespace ns10;

public class CraftProfileInfo : IConfigNode
{
	public string shipName = string.Empty;

	public string description = string.Empty;

	public VersionCompareResult compatibility = VersionCompareResult.INVALID;

	public string version = string.Empty;

	public string saveMD5 = "";

	public int partCount;

	public int stageCount;

	public float totalCost;

	public float totalMass;

	public Vector3 shipSize = Vector3.zero;

	public EditorFacility shipFacility;

	public ulong steamPublishedFileId;

	public List<string> partNames;

	public List<string> partModules;

	public bool shipPartsExperimental;

	public List<string> UnavailableShipParts = new List<string>();

	public List<string> UnavailableShipPartModules = new List<string>();

	public bool duplicatedParts;

	private static HashSet<string> excludedParts = new HashSet<string>();

	public bool shipPartsUnlocked
	{
		get
		{
			if (UnavailableShipParts.Count == 0)
			{
				return !duplicatedParts;
			}
			return false;
		}
	}

	public bool shipPartModulesAvailable => UnavailableShipPartModules.Count == 0;

	public CraftProfileInfo LoadDetailsFromCraftFile(ConfigNode root, string filePath)
	{
		if (root != null)
		{
			if (root.TryGetValue("version", ref version))
			{
				compatibility = KSPUtil.CheckVersion(version, ShipConstruct.lastCompatibleMajor, ShipConstruct.lastCompatibleMinor, ShipConstruct.lastCompatibleRev);
			}
			if (root.HasValue("description"))
			{
				description = root.GetValue("description");
			}
			root.TryGetEnum("type", ref shipFacility, EditorFacility.const_1);
			root.TryGetValue("steamPublishedFileId", ref steamPublishedFileId);
			if (steamPublishedFileId == 0L)
			{
				steamPublishedFileId = KSPSteamUtils.GetSteamIDFromSteamFolder(filePath);
			}
			ShipTemplate shipTemplate = new ShipTemplate();
			shipTemplate.LoadShip(root);
			shipName = shipTemplate.shipName;
			partCount = shipTemplate.partCount;
			stageCount = shipTemplate.stageCount;
			totalCost = shipTemplate.totalCost;
			totalMass = shipTemplate.totalMass;
			shipSize = shipTemplate.GetShipSize();
			shipPartsExperimental = shipTemplate.shipPartsExperimental;
			duplicatedParts = shipTemplate.duplicatedParts;
			partNames = new List<string>();
			partModules = new List<string>();
			ConfigNode[] nodes = root.GetNodes("PART");
			if (nodes != null)
			{
				for (int i = 0; i < nodes.Length; i++)
				{
					string value = nodes[i].GetValue("part");
					if (value == null)
					{
						continue;
					}
					value = value.Substring(0, value.IndexOf('_'));
					partNames.AddUnique(value);
					AvailablePart partInfoByName = PartLoader.getPartInfoByName(value);
					if (partInfoByName == null || !ResearchAndDevelopment.PartTechAvailable(partInfoByName) || excludedParts.Contains(value))
					{
						UnavailableShipParts.AddUnique(value);
					}
					ConfigNode[] nodes2 = nodes[i].GetNodes("MODULE");
					if (nodes2 == null)
					{
						continue;
					}
					for (int j = 0; j < nodes2.Length; j++)
					{
						string value2 = nodes2[j].GetValue("name");
						if (value2 == null)
						{
							continue;
						}
						partModules.AddUnique(value2);
						if (AssemblyLoader.GetClassByName(typeof(PartModule), value2) == null)
						{
							UnavailableShipPartModules.AddUnique(value2);
						}
						if (!(value2 == "ModuleInventoryPart"))
						{
							continue;
						}
						string value3 = "";
						if (!nodes2[j].TryGetValue("inventory", ref value3))
						{
							continue;
						}
						string[] array = value3.Split(',');
						for (int k = 0; k < array.Length; k++)
						{
							partNames.AddUnique(array[k]);
							AvailablePart partInfoByName2 = PartLoader.getPartInfoByName(array[k]);
							if (partInfoByName2 == null || !ResearchAndDevelopment.PartTechAvailable(partInfoByName2) || excludedParts.Contains(array[k]))
							{
								UnavailableShipParts.AddUnique(array[k]);
							}
						}
					}
				}
			}
		}
		return this;
	}

	public void Load(ConfigNode node)
	{
		node.TryGetValue("shipName", ref shipName);
		description = node.GetValue("description").Replace('\u00a8', '\n');
		node.TryGetValue("version", ref version);
		compatibility = KSPUtil.CheckVersion(version, ShipConstruct.lastCompatibleMajor, ShipConstruct.lastCompatibleMinor, ShipConstruct.lastCompatibleRev);
		node.TryGetValue("partCount", ref partCount);
		node.TryGetValue("stageCount", ref stageCount);
		node.TryGetValue("totalCost", ref totalCost);
		node.TryGetValue("totalMass", ref totalMass);
		node.TryGetValue("shipSize", ref shipSize);
		node.TryGetValue("steamPublishedFileId", ref steamPublishedFileId);
		node.TryGetEnum("type", ref shipFacility, EditorFacility.const_1);
		node.TryGetValue("duplicatedParts", ref duplicatedParts);
		if (node.HasValue("partNames"))
		{
			partNames = node.GetValuesList("partNames");
		}
		if (shipPartsUnlocked && partNames != null)
		{
			int num = 0;
			while (shipPartsUnlocked && num < partNames.Count)
			{
				AvailablePart partInfoByName = PartLoader.getPartInfoByName(partNames[num]);
				if (partInfoByName == null || !ResearchAndDevelopment.PartTechAvailable(partInfoByName) || excludedParts.Contains(partNames[num]))
				{
					UnavailableShipParts.AddUnique(partNames[num]);
				}
				num++;
			}
		}
		if (node.HasValue("partModules"))
		{
			partModules = node.GetValuesList("partModules");
		}
		if (shipPartsUnlocked && partModules != null)
		{
			int num2 = 0;
			while (shipPartsUnlocked && num2 < partModules.Count)
			{
				if (AssemblyLoader.GetClassByName(typeof(PartModule), partModules[num2]) == null)
				{
					UnavailableShipPartModules.AddUnique(partModules[num2]);
				}
				num2++;
			}
		}
		node.TryGetValue("saveMD5", ref saveMD5);
	}

	public void Save(ConfigNode node)
	{
		node.AddValue("shipName", shipName);
		node.AddValue("description", description.Replace('\n', '\u00a8'));
		node.AddValue("version", version);
		node.AddValue("partCount", partCount);
		node.AddValue("stageCount", stageCount);
		node.AddValue("totalCost", totalCost);
		node.AddValue("totalMass", totalMass);
		node.AddValue("shipSize", shipSize);
		node.AddValue("type", shipFacility);
		node.AddValue("duplicatedParts", duplicatedParts);
		node.AddValue("steamPublishedFileId", steamPublishedFileId);
		for (int i = 0; i < partNames.Count; i++)
		{
			node.AddValue("partNames", partNames[i]);
		}
		for (int j = 0; j < partModules.Count; j++)
		{
			node.AddValue("partModules", partModules[j]);
		}
		node.AddValue("saveMD5", saveMD5);
	}

	public void LoadFromMetaFile(string loadmetaPath)
	{
		if (!File.Exists(loadmetaPath))
		{
			Debug.Log("No meta file found for path: " + loadmetaPath);
		}
		else
		{
			Load(ConfigNode.Load(loadmetaPath));
		}
	}

	public void SaveToMetaFile(string loadmetaPath)
	{
		ConfigNode configNode = new ConfigNode();
		Save(configNode);
		string directoryName = Path.GetDirectoryName(loadmetaPath);
		if (directoryName != null && !(directoryName == string.Empty))
		{
			if (!Directory.Exists(directoryName))
			{
				Directory.CreateDirectory(directoryName);
			}
			configNode.Save(loadmetaPath);
		}
		else
		{
			Debug.LogWarning("[CraftProfileInfo] Path.GetDirectoryName(loadmetaPath) returned an invalid path!");
		}
	}

	public static string GetSFSMD5(string fullPath)
	{
		if (File.Exists(fullPath))
		{
			return Versioning.FileMD5String(fullPath);
		}
		return "";
	}

	private static void GenerateExcludedParts()
	{
		excludedParts.Clear();
		List<string> list = new List<string>();
		if (ExpansionsLoader.IsExpansionInstalled("MakingHistory") && HighLogic.CurrentGame != null && (HighLogic.CurrentGame.Mode == Game.Modes.MISSION || HighLogic.CurrentGame.Mode == Game.Modes.MISSION_BUILDER) && MissionSystem.missions.Count > 0)
		{
			if (MissionSystem.missions[0].situation.partFilter != null)
			{
				list = MissionSystem.missions[0].situation.partFilter.GetExcludedParts();
			}
			if (MissionSystem.missions[0].situation.VesselsArePending)
			{
				for (int i = 0; i < MissionSystem.missions[0].situation.vesselSituationList.KeysList.Count; i++)
				{
					VesselSituation vesselSituation = MissionSystem.missions[0].situation.vesselSituationList.KeysList[i];
					if (!vesselSituation.launched && vesselSituation.partFilter != null && vesselSituation.partFilter.GetExcludedParts().Count > 0)
					{
						list.AddUniqueRange(vesselSituation.partFilter.GetExcludedParts());
					}
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			excludedParts.Add(list[j]);
		}
	}

	public static void PrepareCraftMetaFileLoad()
	{
		GenerateExcludedParts();
	}

	internal static CraftProfileInfo GetSaveData(string fullPath, string loadMetaPath)
	{
		bool flag = false;
		CraftProfileInfo craftProfileInfo = new CraftProfileInfo();
		string text = "";
		try
		{
			text = GetSFSMD5(fullPath);
		}
		catch (Exception ex)
		{
			Debug.LogFormat("[CraftProfileInfo]: Unable to get MD5 for craft file-{0}\n{1}", fullPath, ex.Message);
		}
		if (text != "")
		{
			try
			{
				craftProfileInfo.LoadFromMetaFile(loadMetaPath);
				if (craftProfileInfo.saveMD5 == text)
				{
					flag = true;
				}
			}
			catch (Exception ex2)
			{
				Debug.LogWarningFormat("[CraftProfileInfo]: Errored when loading .loadmeta file, will fully load craft file-{0}\n{1}", fullPath, ex2.Message);
			}
		}
		if (!flag)
		{
			try
			{
				ConfigNode root = ConfigNode.Load(fullPath);
				craftProfileInfo = craftProfileInfo.LoadDetailsFromCraftFile(root, fullPath);
				craftProfileInfo.saveMD5 = text;
				craftProfileInfo.SaveToMetaFile(loadMetaPath);
			}
			catch (Exception ex3)
			{
				Debug.LogFormat("[CraftProfileInfo]: Failed to save .loadmeta data for craft-{0}\n{1}", fullPath, ex3.Message);
			}
		}
		return craftProfileInfo;
	}

	public string GetErrorMessage()
	{
		StringBuilder stringBuilder = new StringBuilder();
		if (compatibility != VersionCompareResult.COMPATIBLE)
		{
			stringBuilder.Append(Localizer.Format("#autoLOC_8004244", shipName));
		}
		for (int i = 0; i < UnavailableShipParts.Count; i++)
		{
			stringBuilder.Append(Localizer.Format("#autoLOC_8004243", shipName, UnavailableShipParts[i]));
		}
		for (int j = 0; j < UnavailableShipPartModules.Count; j++)
		{
			stringBuilder.Append(Localizer.Format("#autoLOC_8004255", shipName, UnavailableShipPartModules[j]));
		}
		return stringBuilder.ToString().Trim();
	}
}
