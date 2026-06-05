using System.Collections.Generic;
using System.Text;
using KSP.Localization;
using UnityEngine;

namespace InterstellarFuelSwitch;

[KSPModule("#LOC_IFS_TextureSwitch_moduleName")]
public class InterstellarTextureSwitch : PartModule
{
	[KSPField]
	public string displayName = "Texture switcher";

	[KSPField]
	public string moduleID = "0";

	[KSPField(isPersistant = true)]
	public int selectedTexture;

	[KSPField(isPersistant = true)]
	public string selectedTextureURL = string.Empty;

	[KSPField(isPersistant = true)]
	public string selectedMapURL = string.Empty;

	[KSPField]
	public bool showListButton;

	[KSPField]
	public bool debugMode;

	[KSPField]
	public bool switchableInFlight;

	[KSPField]
	public string additionalMapType = "_BumpMap";

	[KSPField]
	public bool mapIsNormal = true;

	[KSPField]
	public bool repaintableEVA = true;

	[KSPField]
	public Vector4 GUIposition = new Vector4(FSGUIwindowID.standardRect.x, FSGUIwindowID.standardRect.y, FSGUIwindowID.standardRect.width, FSGUIwindowID.standardRect.height);

	[KSPField]
	public bool showPreviousButton = true;

	private List<Transform> targetObjectTransforms = new List<Transform>();

	private List<Material> targetMats = new List<Material>();

	private InterstellarNodeLoader textureNode;

	private string textureNodeName = "textures";

	private string textureValueName = "name";

	private List<string> texList = new List<string>();

	private InterstellarNodeLoader mapNode;

	private string mapNodeName = "additionalMap";

	private string mapValueName = "name";

	private List<string> mapList = new List<string>();

	private InterstellarNodeLoader objectNode;

	private string objectNodeName = "objects";

	private string objectValueName = "name";

	private List<string> objectList = new List<string>();

	private InterstellarDebugMessages debug = new InterstellarDebugMessages(_debugMode: false, InterstellarDebugMessages.OutputMode.both, 2f);

	public static Dictionary<string, List<string>> texListDictionary = new Dictionary<string, List<string>>();

	public static Dictionary<string, List<string>> mapListDictionary = new Dictionary<string, List<string>>();

	public static Dictionary<string, List<string>> objectListDictionary = new Dictionary<string, List<string>>();

	public string uniqueModuleID => base.part.name.Split('(')[0].Split(' ')[0] + moduleID;

	[KSPEvent(guiActive = false, guiActiveEditor = false, guiName = "Debug: Log Objects")]
	public void listAllObjects()
	{
		List<Transform> list = ListChildren(base.part.transform);
		foreach (Transform item in list)
		{
			Debug.Log("object: " + item.name);
		}
	}

	private List<Transform> ListChildren(Transform a)
	{
		List<Transform> list = new List<Transform>();
		foreach (Transform item in a)
		{
			list.Add(item);
			list.AddRange(ListChildren(item));
		}
		return list;
	}

	[KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "#LOC_IFS_TextureSwitch_nextSetup")]
	public void nextTextureEvent()
	{
		selectedTexture++;
		if (selectedTexture >= texList.Count)
		{
			selectedTexture = 0;
		}
		useTextureAll();
	}

	[KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "#LOC_IFS_TextureSwitch_previousSetup")]
	public void previousTextureEvent()
	{
		selectedTexture--;
		if (selectedTexture < 0)
		{
			selectedTexture = texList.Count - 1;
		}
		useTextureAll();
	}

	[KSPEvent(guiActiveUnfocused = true, unfocusedRange = 5f, guiActive = false, guiActiveEditor = false, guiName = "Repaint")]
	public void nextTextureEVAEvent()
	{
		nextTextureEvent();
	}

	public void useTextureAll()
	{
		foreach (Material targetMat in targetMats)
		{
			useTexture(targetMat);
		}
	}

	public void useTexture(Material targetMat)
	{
		if (targetMat != null && texList.Count > 0)
		{
			if (GameDatabase.Instance.ExistsTexture(texList[selectedTexture]))
			{
				debug.debugMessage("InterstellarTextureSwitch: assigning texture: " + texList[selectedTexture]);
				targetMat.mainTexture = GameDatabase.Instance.GetTexture(texList[selectedTexture], asNormalMap: false);
				selectedTextureURL = texList[selectedTexture];
				if (mapList.Count > selectedTexture)
				{
					targetMat.SetTexture(additionalMapType, GameDatabase.Instance.GetTexture(mapList[selectedTexture], mapIsNormal));
					selectedMapURL = mapList[selectedTexture];
				}
			}
			else
			{
				debug.debugMessage("InterstellarTextureSwitch: no such texture: " + texList[selectedTexture]);
			}
		}
		else
		{
			debug.debugMessage("InterstellarTextureSwitch: No target material in object.");
		}
	}

	public override string GetInfo()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(Localizer.Format("#LOC_IFS_TextureSwitch_GetInfo") + ":");
		if (texList.Count == 0)
		{
			if (!texListDictionary.TryGetValue(uniqueModuleID, out texList))
			{
				stringBuilder.AppendLine("None. Error reading Dictionary");
			}
			else if (texList.Count == 0)
			{
				stringBuilder.AppendLine("None");
			}
		}
		for (int i = 0; i < texList.Count; i++)
		{
			string[] array = texList[i].Split('/');
			if (array.Length > 0)
			{
				stringBuilder.AppendLine(array[array.Length - 1]);
			}
		}
		stringBuilder.AppendLine("\nUse the Next Texture button on the right click menu.");
		return stringBuilder.ToString();
	}

	public override void OnLoad(ConfigNode node)
	{
		getNodeValues(node, textureNode, textureNodeName, textureValueName, texListDictionary, texList);
		getNodeValues(node, mapNode, mapNodeName, mapValueName, mapListDictionary, mapList);
		getNodeValues(node, objectNode, objectNodeName, objectValueName, objectListDictionary, objectList);
	}

	private void getNodeValues(ConfigNode node, InterstellarNodeLoader nodeLoader, string nodeName, string valueName, Dictionary<string, List<string>> outputDict, List<string> outputList)
	{
		nodeLoader = new InterstellarNodeLoader(base.part, moduleName, moduleID.ToString(), nodeName, valueName);
		nodeLoader.debugMode = debugMode;
		outputList = nodeLoader.ProcessNodeAsStringList(node);
		if (!outputDict.ContainsKey(uniqueModuleID))
		{
			outputDict.Add(uniqueModuleID, outputList);
		}
	}

	public override void OnStart(StartState state)
	{
		debug.debugMode = debugMode;
		if (!texListDictionary.TryGetValue(uniqueModuleID, out texList))
		{
			debug.debugMessage("InterstellarTextureSwitch: No matching texture list key: " + uniqueModuleID);
		}
		if (!mapListDictionary.TryGetValue(uniqueModuleID, out mapList))
		{
			debug.debugMessage("InterstellarTextureSwitch: No matching map list key: " + uniqueModuleID);
		}
		if (!objectListDictionary.TryGetValue(uniqueModuleID, out objectList))
		{
			debug.debugMessage("InterstellarTextureSwitch: No matching object list key: " + uniqueModuleID);
		}
		debug.debugMessage("InterstellarTextureSwitch found " + texList.Count + " textures, using number " + selectedTexture + ", found " + objectList.Count + " objects, " + mapList.Count + " maps");
		foreach (string @object in objectList)
		{
			Transform transform = base.part.FindModelTransform(@object);
			if (transform != null)
			{
				Renderer component = transform.gameObject.GetComponent<Renderer>();
				if (!(component != null))
				{
					continue;
				}
				Material material = component.material;
				if (material != null)
				{
					if (!targetMats.Contains(material))
					{
						targetMats.Add(material);
					}
				}
				else
				{
					debug.debugMessage("InterstellarTextureSwitch: No target material in object " + @object);
				}
			}
			else
			{
				debug.debugMessage("FStextureSwitch: Object " + @object + " not found");
			}
		}
		useTextureAll();
		if (switchableInFlight)
		{
			base.Events["nextTextureEvent"].guiActive = true;
		}
		if (switchableInFlight && showPreviousButton)
		{
			base.Events["previousTextureEvent"].guiActive = true;
		}
		if (showListButton)
		{
			base.Events["listAllObjects"].guiActiveEditor = true;
		}
		if (!repaintableEVA)
		{
			base.Events["nextTextureEVAEvent"].guiActiveUnfocused = false;
		}
		if (!showPreviousButton)
		{
			base.Events["previousTextureEvent"].guiActive = false;
			base.Events["previousTextureEvent"].guiActiveEditor = false;
		}
	}
}
