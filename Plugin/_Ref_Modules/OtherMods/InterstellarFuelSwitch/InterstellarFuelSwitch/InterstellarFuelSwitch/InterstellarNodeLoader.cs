using System;
using System.Collections.Generic;
using UnityEngine;

namespace InterstellarFuelSwitch;

public class InterstellarNodeLoader
{
	public Part part;

	public string moduleName;

	public string nodeName;

	public string valueName;

	public List<string> valueList = new List<string>();

	private float[] trimArray = new float[1];

	public bool testOnloadMemory;

	public bool foundExistingNodes;

	private string[] values = new string[1];

	[KSPField]
	public string moduleID = "0";

	public bool debugMode;

	public InterstellarNodeLoader(Part _part, string _moduleName, string _moduleID, string _nodeName, string _valueName)
	{
		part = _part;
		moduleName = _moduleName;
		moduleID = _moduleID;
		nodeName = _nodeName;
		valueName = _valueName;
	}

	public void debugMessage(string input)
	{
		_ = debugMode;
	}

	public FloatCurve ProcessNodeAsFloatCurve(ConfigNode node)
	{
		FloatCurve floatCurve = new FloatCurve();
		ConfigNode[] nodes = node.GetNodes(nodeName);
		debugMessage("ProcessNodeAsFloatCurve: moduleNodeArray.length " + nodes.Length);
		for (int i = 0; i < nodes.Length; i++)
		{
			debugMessage("found node");
			string[] array = nodes[i].GetValues(valueName);
			debugMessage("found " + array.Length + " values");
			for (int j = 0; j < array.Length; j++)
			{
				string[] array2 = array[j].Split(' ');
				try
				{
					Vector2 vector = new Vector2(float.Parse(array2[0]), float.Parse(array2[1]));
					floatCurve.Add(vector.x, vector.y, 0f, 0f);
				}
				catch (Exception ex)
				{
					Debug.Log("[IFS] - Error parsing vector2: " + ex.Message);
				}
			}
		}
		return floatCurve;
	}

	public List<string> ProcessNodeAsStringList(ConfigNode node)
	{
		List<string> list = new List<string>();
		ConfigNode[] nodes = node.GetNodes(nodeName);
		debugMessage("ProcessNodeAsStringList: moduleNodeArray.length " + nodes.Length);
		for (int i = 0; i < nodes.Length; i++)
		{
			debugMessage("found node");
			string[] array = nodes[i].GetValues(valueName);
			debugMessage("found " + array.Length + " values");
			for (int j = 0; j < array.Length; j++)
			{
				debugMessage("Adding value to node " + array[j]);
				list.Add(array[j]);
			}
		}
		return list;
	}

	[Obsolete("Use processNode and fill a static list from OnLoad instead", true)]
	public List<string> OnStart()
	{
		if (valueList.Count == 0)
		{
			debugMessage("OnStart: no existing " + nodeName + " nodes, filling values from part.cfg");
			if (part.partInfo != null)
			{
				debugMessage("OnStart moduleName is " + moduleName);
				debugMessage("OnStart partName is " + part.partName);
				debugMessage("OnStart partInfo.name is " + part.partInfo.name);
				debugMessage("getting configs");
				UrlDir.UrlConfig[] configs = GameDatabase.Instance.GetConfigs("PART");
				debugMessage("looping through " + configs.Length);
				for (int i = 0; i < configs.Length; i++)
				{
					if (!(part.partInfo.name == configs[i].name))
					{
						continue;
					}
					debugMessage("found this part");
					ConfigNode[] nodes = configs[i].config.GetNodes("MODULE");
					debugMessage("nodes: " + nodes.Length);
					for (int j = 0; j < nodes.Length; j++)
					{
						debugMessage("node loop: " + nodes[j].GetValue("name"));
						if (nodes[j].GetValue("name") == moduleName)
						{
							debugMessage("found this type of module");
							bool flag = false;
							string[] array = nodes[j].GetValues("moduleID");
							if (array.Length > 0)
							{
								flag = ((array[0] == moduleID) ? true : false);
							}
							else
							{
								moduleID = "0";
								flag = true;
							}
							if (flag)
							{
								debugMessage("Found module with matching or blank ID, proceeding");
								valueList = ProcessNodeAsStringList(nodes[j]);
							}
							else
							{
								debugMessage("Found module with wrong ID, skipping");
							}
						}
					}
				}
			}
		}
		else
		{
			debugMessage("OnStart: found " + values.Length + " existing values, valueList.Count is " + valueList.Count);
		}
		return valueList;
	}

	public void OnLoad(ConfigNode node)
	{
		testOnloadMemory = true;
		ConfigNode[] nodes = node.GetNodes(nodeName);
		if (nodes.Length > 0)
		{
			debugMessage("OnLoad: Found " + nodes.Length + " " + nodeName + " nodes");
			for (int i = 0; i < nodes.Length; i++)
			{
				values = nodes[i].GetValues(valueName);
				for (int j = 0; j < values.Length; j++)
				{
					valueList.Add(values[j]);
					debugMessage("OnLoad: adding to list: " + values[j]);
				}
				if (valueList.Count > 0)
				{
					foundExistingNodes = true;
				}
				else
				{
					foundExistingNodes = false;
				}
			}
		}
		else
		{
			debugMessage("OnLoad: Found no existing " + nodeName + " nodes");
			foundExistingNodes = false;
		}
	}

	public ConfigNode OnSave(ConfigNode node)
	{
		debugMessage("OnSave testOnLoadMemory == " + testOnloadMemory);
		ConfigNode configNode = new ConfigNode(nodeName);
		debugMessage("Value List count: " + valueList.Count);
		for (int i = 0; i < valueList.Count; i++)
		{
			debugMessage("Add " + valueList[i] + " to the node");
			configNode.AddValue(valueName, valueList[i]);
		}
		node.AddNode(configNode);
		return node;
	}
}
