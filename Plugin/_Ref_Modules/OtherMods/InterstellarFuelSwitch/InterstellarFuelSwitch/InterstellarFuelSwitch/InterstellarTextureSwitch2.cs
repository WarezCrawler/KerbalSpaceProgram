using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using KSP.Localization;
using UnityEngine;

namespace InterstellarFuelSwitch;

[KSPModule("#LOC_IFS_TextureSwitch_moduleName")]
public class InterstellarTextureSwitch2 : PartModule, IHaveFuelTankSetup
{
	[KSPField]
	public string moduleID = "0";

	[KSPField]
	public string textureRootFolder = string.Empty;

	[KSPField]
	public string objectNames = string.Empty;

	[KSPField]
	public string textureNames = string.Empty;

	[KSPField]
	public string mapNames = string.Empty;

	[KSPField]
	public string textureDisplayNames = "Default";

	[KSPField]
	public string statusText = "Current Texture";

	[UI_ChooseOption(affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
	[KSPField(isPersistant = true, guiActiveEditor = true)]
	public int selectedTexture;

	[KSPField]
	public string switcherDescription = "#LOC_IFS_TextureSwitch_TextureName";

	[KSPField]
	public bool hasSwitchChooseOption = true;

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
	public bool showCurrentTextureName;

	[KSPField]
	public bool showSwitchButtons;

	[KSPField]
	public bool showPreviousButton = true;

	[KSPField]
	public bool useFuelSwitchModule;

	[KSPField]
	public string fuelTankSetups = "0";

	[KSPField]
	public bool showInfo = true;

	[KSPField]
	public bool updateSymmetry = true;

	private List<List<Material>> targetMats = new List<List<Material>>();

	private List<List<string>> texList = new List<List<string>>();

	private List<string> mapList = new List<string>();

	private List<string> objectList = new List<string>();

	private List<string> textureDisplayList = new List<string>();

	private List<string> fuelTankSetupList = new List<string>();

	private InterstellarFuelSwitch fuelSwitch;

	private bool initialized;

	private InterstellarDebugMessages debug;

	[KSPField(guiActiveEditor = true, guiName = "Current Texture")]
	public string currentTextureName = string.Empty;

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
		if (selectedTexture >= texList.Count && selectedTexture >= mapList.Count)
		{
			selectedTexture = 0;
		}
		UseTextureAll(calledByPlayer: true);
	}

	[KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "#LOC_IFS_TextureSwitch_previousSetup")]
	public void previousTextureEvent()
	{
		selectedTexture--;
		if (selectedTexture < 0)
		{
			selectedTexture = Mathf.Max(texList.Count - 1, mapList.Count - 1);
		}
		UseTextureAll(calledByPlayer: true);
	}

	public void SelectTankSetup(int newTankIndex, bool calledByPlayer)
	{
		bool flag = false;
		if (fuelTankSetupList != null)
		{
			int num = fuelTankSetupList.IndexOf(newTankIndex.ToString(CultureInfo.InvariantCulture));
			if (num >= 0)
			{
				selectedTexture = num;
				flag = true;
			}
		}
		if (!flag && newTankIndex < texList.Count && newTankIndex < mapList.Count)
		{
			selectedTexture = newTankIndex;
		}
		UseTextureAll(calledByPlayer);
	}

	public void SwitchToFuelTankSetup(string fuelTankSetup)
	{
		int num = textureDisplayList.IndexOf(fuelTankSetup);
		if (num < 0 && string.IsNullOrEmpty(fuelTankSetup))
		{
			for (int i = 0; i < textureDisplayList.Count; i++)
			{
				if (textureDisplayList[i].Contains(fuelTankSetup))
				{
					num = i;
					break;
				}
				if (fuelTankSetup.Contains(textureDisplayList[i]))
				{
					num = i;
					break;
				}
			}
		}
		if (num >= 0)
		{
			Debug.Log("[IFS] - SwitchToFuelTankSetup found " + fuelTankSetup);
			selectedTexture = num;
			UseTextureAll(calledByPlayer: true);
		}
		else
		{
			Debug.LogWarning("[IFS] - SwitchToFuelTankSetup is missing " + fuelTankSetup);
		}
	}

	[KSPEvent(guiActiveUnfocused = true, unfocusedRange = 5f, guiActive = false, guiActiveEditor = false, guiName = "Repaint")]
	public void nextTextureEVAEvent()
	{
		nextTextureEvent();
	}

	public void UseTextureAll(bool calledByPlayer)
	{
		ApplyTexToPart(calledByPlayer);
		if (!updateSymmetry)
		{
			return;
		}
		for (int i = 0; i < base.part.symmetryCounterparts.Count; i++)
		{
			InterstellarTextureSwitch2[] components = base.part.symmetryCounterparts[i].GetComponents<InterstellarTextureSwitch2>();
			for (int j = 0; j < components.Length; j++)
			{
				if (!(components[j].moduleID != moduleID))
				{
					components[j].selectedTexture = selectedTexture;
					components[j].ApplyTexToPart(calledByPlayer);
				}
			}
		}
	}

	private void ApplyTexToPart(bool calledByPlayer)
	{
		InitializeData();
		for (int i = 0; i < targetMats.Count; i++)
		{
			foreach (Material item in targetMats[i])
			{
				UseTextureOrMap(item, i);
			}
		}
		if (!useFuelSwitchModule)
		{
			return;
		}
		debug.debugMessage("calling on InterstellarFuelSwitch tank setup " + selectedTexture);
		if (selectedTexture < fuelTankSetupList.Count)
		{
			int num = fuelSwitch.SelectTankSetup(fuelTankSetupList[selectedTexture], calledByPlayer);
			if (num != selectedTexture)
			{
				selectedTexture = num;
				UseTextureAll(calledByPlayer);
			}
		}
		else
		{
			debug.debugMessage("no such fuel tank setup");
		}
	}

	public void UseTextureOrMap(Material targetMat, int objectIndex)
	{
		if (targetMat != null)
		{
			UseTexture(targetMat, objectIndex);
			UseMap(targetMat);
		}
		else
		{
			debug.debugMessage("No target material in object.");
		}
	}

	private void UseMap(Material targetMat)
	{
		debug.debugMessage("maplist count: " + mapList.Count + ", selectedTexture: " + selectedTexture + ", texlist Count: " + texList.Count);
		if (mapList.Count > selectedTexture)
		{
			if (GameDatabase.Instance.ExistsTexture(mapList[selectedTexture]))
			{
				debug.debugMessage("map " + mapList[selectedTexture] + " exists in db");
				targetMat.SetTexture(additionalMapType, GameDatabase.Instance.GetTexture(mapList[selectedTexture], mapIsNormal));
				selectedMapURL = mapList[selectedTexture];
				if (selectedTexture < textureDisplayList.Count && texList.Count == 0)
				{
					currentTextureName = textureDisplayList[selectedTexture];
					debug.debugMessage("setting currentTextureName to " + textureDisplayList[selectedTexture]);
					return;
				}
				debug.debugMessage("not setting currentTextureName. selectedTexture is " + selectedTexture + ", texDispList count is" + textureDisplayList.Count + ", texList count is " + texList.Count);
			}
			else
			{
				debug.debugMessage("map " + mapList[selectedTexture] + " does not exist in db");
			}
		}
		else if (mapList.Count > selectedTexture)
		{
			debug.debugMessage("no such map: " + mapList[selectedTexture]);
		}
		else
		{
			debug.debugMessage("useMap, index out of range error, maplist count: " + mapList.Count + ", selectedTexture: " + selectedTexture);
			for (int i = 0; i < mapList.Count; i++)
			{
				debug.debugMessage("map " + i + ": " + mapList[i]);
			}
		}
	}

	private void UseTexture(Material targetMat, int objectIndex)
	{
		if (texList.Count <= selectedTexture)
		{
			return;
		}
		List<string> list = texList[selectedTexture];
		int index = ((list.Count > objectIndex) ? objectIndex : 0);
		string text = list[index];
		if (GameDatabase.Instance.ExistsTexture(text))
		{
			debug.debugMessage("assigning texture: " + text);
			targetMat.mainTexture = GameDatabase.Instance.GetTexture(text, asNormalMap: false);
			if (selectedTexture > textureDisplayList.Count - 1)
			{
				currentTextureName = getTextureDisplayName(text);
			}
			else
			{
				currentTextureName = textureDisplayList[selectedTexture];
			}
		}
		else
		{
			debug.debugMessage("no such texture: " + list[index]);
		}
	}

	public override string GetInfo()
	{
		if (showInfo)
		{
			List<string> list = ParseTools.ParseNames((textureNames.Length > 0) ? textureNames : mapNames);
			textureDisplayList = ParseTools.ParseNames(textureDisplayNames);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Alternate textures available:");
			if (list.Count == 0 && list.Count == 0)
			{
				stringBuilder.AppendLine("None");
			}
			for (int i = 0; i < list.Count; i++)
			{
				stringBuilder.AppendLine((i > textureDisplayList.Count - 1) ? getTextureDisplayName(list[i]) : textureDisplayList[i]);
			}
			stringBuilder.AppendLine("\nUse the Next Texture button on the right click menu.");
			return stringBuilder.ToString();
		}
		return string.Empty;
	}

	private string getTextureDisplayName(string longName)
	{
		string[] array = longName.Split('/');
		return array[array.Length - 1];
	}

	public override void OnStart(StartState state)
	{
		InitializeData();
		UseTextureAll(calledByPlayer: false);
		if (showListButton)
		{
			base.Events["listAllObjects"].guiActiveEditor = true;
		}
		if (!repaintableEVA)
		{
			base.Events["nextTextureEVAEvent"].guiActiveUnfocused = false;
		}
		BaseEvent baseEvent = base.Events["nextTextureEvent"];
		baseEvent.guiActive = switchableInFlight && showSwitchButtons;
		baseEvent.guiActiveEditor = showSwitchButtons;
		BaseEvent baseEvent2 = base.Events["previousTextureEvent"];
		baseEvent2.guiActive = switchableInFlight && showSwitchButtons;
		baseEvent2.guiActiveEditor = showSwitchButtons;
		if (!showPreviousButton)
		{
			baseEvent2.guiActive = false;
			baseEvent2.guiActiveEditor = false;
		}
		BaseField baseField = base.Fields["currentTextureName"];
		baseField.guiName = statusText;
		baseField.guiActiveEditor = showCurrentTextureName;
		BaseField baseField2 = base.Fields["selectedTexture"];
		baseField2.guiName = Localizer.Format(switcherDescription);
		baseField2.guiActiveEditor = hasSwitchChooseOption;
		UI_ChooseOption uI_ChooseOption = baseField2.uiControlEditor as UI_ChooseOption;
		uI_ChooseOption.options = textureDisplayList.ToArray();
		uI_ChooseOption.onFieldChanged = UpdateFromGUI;
	}

	private void UpdateFromGUI(BaseField field, object oldFieldValueObj)
	{
		UseTextureAll(calledByPlayer: true);
	}

	private void InitializeData()
	{
		if (initialized)
		{
			return;
		}
		debug = new InterstellarDebugMessages(debugMode, "InterstellarTextureSwitch2");
		if (useFuelSwitchModule)
		{
			updateSymmetry = true;
		}
		objectList = ParseTools.ParseNames(objectNames, replaceBackslashErrors: true);
		mapList = ParseTools.ParseNames(mapNames, replaceBackslashErrors: true, trimWhiteSpace: true, textureRootFolder);
		textureDisplayList = ParseTools.ParseNames(textureDisplayNames);
		fuelTankSetupList = ParseTools.ParseNames(fuelTankSetups);
		string[] array = textureNames.Split(';').ToArray();
		for (int i = 0; i < array.Count(); i++)
		{
			List<string> item = ParseTools.ParseNames(array[i], replaceBackslashErrors: true, trimWhiteSpace: true, textureRootFolder);
			texList.Add(item);
		}
		debug.debugMessage("found " + texList.Count + " textures, using number " + selectedTexture + ", found " + objectList.Count + " objects, " + mapList.Count + " maps");
		for (int j = 0; j < objectList.Count(); j++)
		{
			Transform[] array2 = base.part.FindModelTransforms(objectList[j]);
			List<Material> list = new List<Material>();
			Transform[] array3 = array2;
			foreach (Transform transform in array3)
			{
				if (transform == null)
				{
					continue;
				}
				Renderer component = transform.gameObject.GetComponent<Renderer>();
				if (!(component == null))
				{
					Material material = component.material;
					if (material != null && !list.Contains(material))
					{
						list.Add(material);
					}
				}
			}
			targetMats.Add(list);
		}
		if (useFuelSwitchModule)
		{
			fuelSwitch = base.part.GetComponent<InterstellarFuelSwitch>();
			if (fuelSwitch == null)
			{
				useFuelSwitchModule = false;
				debug.debugMessage("no InterstellarFuelSwitch module found, despite useFuelSwitchModule being true");
			}
			else
			{
				int num = fuelSwitch.FindMatchingConfig(this);
				if (num >= 0)
				{
					selectedTexture = num;
				}
			}
		}
		initialized = true;
	}
}
