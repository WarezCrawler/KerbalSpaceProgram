using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KSP.Localization;
using UnityEngine;

namespace InterstellarFuelSwitch;

[KSPModule("#LOC_IFS_MeshSwitch_moduleName")]
public class InterstellarMeshSwitch : PartModule, IHaveFuelTankSetup
{
	[KSPField]
	public string moduleID = "0";

	[KSPField]
	public string switcherDescription = "#LOC_IFS_MeshSwitch_MeshName";

	[KSPField]
	public string tankSwitchNames = string.Empty;

	[KSPField]
	public string indexNames = string.Empty;

	[KSPField]
	public string objectDisplayNames = string.Empty;

	[KSPField]
	public bool showPreviousButton = true;

	[KSPField]
	public bool useFuelSwitchModule;

	[KSPField]
	public string searchTankId = "";

	[KSPField]
	public string fuelTankSetups = "0";

	[KSPField]
	public string objects = string.Empty;

	[KSPField]
	public bool updateSymmetry = true;

	[KSPField]
	public bool affectColliders = true;

	[KSPField]
	public bool showInfo = true;

	[KSPField]
	public bool debugMode;

	[KSPField]
	public bool showSwitchButtons;

	[KSPField]
	public bool orderByIndexNames;

	[KSPField]
	public bool showCurrentObjectName;

	[KSPField]
	public bool hasSwitchChooseOption = true;

	[KSPField]
	public bool initialized;

	[KSPField(guiActiveEditor = false, guiName = "#LOC_IFS_MeshSwitch_currentObjectName")]
	public string currentObjectName = string.Empty;

	[KSPField(isPersistant = true, guiActiveEditor = true)]
	[UI_ChooseOption(affectSymCounterparts = UI_Scene.None, scene = UI_Scene.Editor, suppressEditorShipModified = true)]
	public int selectedObject;

	private List<List<Transform>> objectTransforms = new List<List<Transform>>();

	private List<meshConfiguration> meshConfigurationList = new List<meshConfiguration>();

	private InterstellarFuelSwitch fuelSwitch;

	[KSPEvent(guiActive = false, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "#LOC_IFS_MeshSwitch_nextSetup")]
	public void nextObjectEvent()
	{
		selectedObject++;
		if (selectedObject >= meshConfigurationList.Count)
		{
			selectedObject = 0;
		}
		SwitchToObject(selectedObject, calledByPlayer: true);
	}

	[KSPEvent(guiActive = false, guiActiveEditor = true, guiActiveUnfocused = false, guiName = "#LOC_IFS_MeshSwitch_previousetup")]
	public void previousObjectEvent()
	{
		selectedObject--;
		if (selectedObject < 0)
		{
			selectedObject = meshConfigurationList.Count - 1;
		}
		SwitchToObject(selectedObject, calledByPlayer: true);
	}

	private List<List<Transform>> ParseObjectNames()
	{
		List<List<Transform>> list = new List<List<Transform>>();
		string[] array = objects.Split(';');
		if (array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				List<Transform> list2 = new List<Transform>();
				string[] array2 = array[i].Split(',');
				for (int j = 0; j < array2.Length; j++)
				{
					Transform transform = base.part.FindModelTransform(array2[j].Trim(' '));
					if (transform != null)
					{
						list2.Add(transform);
					}
					else
					{
						list2.Add(null);
					}
				}
				list.Add(list2);
			}
		}
		return list;
	}

	private void SwitchToObject(int objectNumber, bool calledByPlayer)
	{
		SetObject(objectNumber, calledByPlayer);
		if (HighLogic.LoadedSceneIsFlight || !updateSymmetry)
		{
			return;
		}
		for (int i = 0; i < base.part.symmetryCounterparts.Count; i++)
		{
			InterstellarMeshSwitch[] components = base.part.symmetryCounterparts[i].GetComponents<InterstellarMeshSwitch>();
			for (int j = 0; j < components.Length; j++)
			{
				if (!(components[j].moduleID != moduleID))
				{
					components[j].selectedObject = selectedObject;
					components[j].SetObject(objectNumber, calledByPlayer);
				}
			}
		}
	}

	private void SetObject(int objectNumber, bool calledByPlayer)
	{
		InitializeData();
		for (int i = 0; i < objectTransforms.Count; i++)
		{
			List<Transform> list = objectTransforms[i];
			for (int j = 0; j < list.Count; j++)
			{
				Transform transform = list[j];
				if (transform == null)
				{
					continue;
				}
				transform.gameObject.SetActive(value: false);
				if (affectColliders)
				{
					Collider component = transform.gameObject.GetComponent<Collider>();
					if (component != null)
					{
						component.enabled = false;
					}
				}
			}
		}
		if (objectNumber >= 0 && objectNumber < meshConfigurationList.Count)
		{
			List<Transform> list2 = meshConfigurationList[objectNumber].objectTransforms;
			for (int k = 0; k < list2.Count; k++)
			{
				Transform transform2 = list2[k];
				if (transform2 == null)
				{
					continue;
				}
				transform2.gameObject.SetActive(value: true);
				if (affectColliders)
				{
					Collider component2 = transform2.gameObject.GetComponent<Collider>();
					if (!(component2 == null))
					{
						component2.enabled = true;
					}
				}
			}
		}
		if (useFuelSwitchModule && fuelSwitch != null && objectNumber >= 0 && objectNumber < meshConfigurationList.Count)
		{
			fuelSwitch.SelectTankSetup(meshConfigurationList[objectNumber].fuelTankSetup, calledByPlayer);
		}
		SetCurrentObjectName();
	}

	private void SetCurrentObjectName()
	{
		currentObjectName = ((selectedObject >= 0 && selectedObject < meshConfigurationList.Count) ? Localizer.Format(meshConfigurationList[selectedObject].objectDisplay) : "");
	}

	public override void OnStart(StartState state)
	{
		InitializeData(forced: true);
		SwitchToObject(selectedObject, calledByPlayer: false);
		base.Fields["currentObjectName"].guiActiveEditor = showCurrentObjectName;
		BaseEvent baseEvent = base.Events["nextObjectEvent"];
		baseEvent.guiActiveEditor = showSwitchButtons;
		BaseEvent baseEvent2 = base.Events["previousObjectEvent"];
		baseEvent2.guiActiveEditor = showSwitchButtons;
		BaseField baseField = base.Fields["selectedObject"];
		baseField.guiName = Localizer.Format(switcherDescription);
		baseField.guiActiveEditor = hasSwitchChooseOption;
		if (baseField.uiControlEditor is UI_ChooseOption uI_ChooseOption)
		{
			uI_ChooseOption.options = meshConfigurationList.Select((meshConfiguration m) => m.tankSwitchName).ToArray();
			uI_ChooseOption.onFieldChanged = UpdateFromGUI;
		}
		if (!showPreviousButton)
		{
			base.Events["previousObjectEvent"].guiActiveEditor = false;
		}
	}

	private void UpdateFromGUI(BaseField field, object oldFieldValueObj)
	{
		SwitchToObject(selectedObject, calledByPlayer: true);
	}

	public void SwitchToFuelTankSetup(string fuelTankSetup)
	{
		meshConfiguration meshConfiguration2 = meshConfigurationList.FirstOrDefault((meshConfiguration m) => m.fuelTankSetup == fuelTankSetup);
		if (meshConfiguration2 != null)
		{
			Debug.Log("[IFS]: SwitchToFuelTankSetup fuelTankSetup matches " + fuelTankSetup);
		}
		if (meshConfiguration2 == null)
		{
			meshConfiguration2 = meshConfigurationList.FirstOrDefault((meshConfiguration m) => m.indexName == fuelTankSetup);
			if (meshConfiguration2 != null)
			{
				Debug.Log("[IFS]: SwitchToFuelTankSetup indexName matches " + fuelTankSetup);
			}
		}
		if (meshConfiguration2 == null)
		{
			meshConfiguration2 = meshConfigurationList.FirstOrDefault((meshConfiguration m) => m.objectDisplay == fuelTankSetup);
			if (meshConfiguration2 != null)
			{
				Debug.Log("[IFS]: SwitchToFuelTankSetup objectDisplay matches " + fuelTankSetup);
			}
		}
		if (meshConfiguration2 == null)
		{
			meshConfiguration2 = meshConfigurationList.FirstOrDefault((meshConfiguration m) => m.tankSwitchName == fuelTankSetup);
			if (meshConfiguration2 != null)
			{
				Debug.Log("[IFS]: SwitchToFuelTankSetup tankSwitchName matches " + fuelTankSetup);
			}
		}
		if (meshConfiguration2 != null)
		{
			selectedObject = meshConfigurationList.IndexOf(meshConfiguration2);
			SwitchToObject(selectedObject, calledByPlayer: true);
		}
		else
		{
			Debug.LogWarning("[IFS]: SwitchToFuelTankSetup is missing " + fuelTankSetup);
		}
	}

	public void InitializeData(bool forced = false)
	{
		try
		{
			if (initialized && !forced)
			{
				return;
			}
			if (useFuelSwitchModule)
			{
				updateSymmetry = true;
			}
			objectTransforms = ParseObjectNames();
			List<string> list = ParseTools.ParseNames(fuelTankSetups);
			List<string> list2 = ParseTools.ParseNames(objectDisplayNames);
			List<string> list3 = ParseTools.ParseNames(indexNames);
			List<string> list4 = ParseTools.ParseNames(tankSwitchNames);
			meshConfigurationList.Clear();
			for (int i = 0; i < list2.Count; i++)
			{
				meshConfigurationList.Add(new meshConfiguration
				{
					objectDisplay = list2[i],
					tankSwitchName = Localizer.Format((i < list4.Count) ? list4[i] : list2[i]),
					indexName = ((i < list3.Count) ? list3[i] : list2[i]),
					fuelTankSetup = ((i < list.Count) ? list[i] : list2[i]),
					objectTransforms = ((i < objectTransforms.Count) ? objectTransforms[i] : new List<Transform>())
				});
			}
			if (orderByIndexNames)
			{
				if (debugMode)
				{
					Debug.Log("[IFS] - InterstellarMeshSwitch " + base.part.GetInstanceID() + " order meshConfigurationList on indexName");
				}
				meshConfigurationList = meshConfigurationList.OrderBy((meshConfiguration m) => m.indexName).ToList();
			}
			if (debugMode)
			{
				foreach (meshConfiguration meshConfiguration in meshConfigurationList)
				{
					Debug.Log("fuelTankSetup:" + meshConfiguration.fuelTankSetup + " indexName:" + meshConfiguration.indexName + " objectDisplay: " + meshConfiguration.objectDisplay + " tankSwitchName:" + meshConfiguration.tankSwitchName);
				}
			}
			if (useFuelSwitchModule)
			{
				List<InterstellarFuelSwitch> source = base.part.FindModulesImplementing<InterstellarFuelSwitch>();
				if (source.Any() && !string.IsNullOrEmpty(searchTankId))
				{
					fuelSwitch = source.FirstOrDefault((InterstellarFuelSwitch m) => m.tankId == searchTankId);
				}
				if (fuelSwitch == null)
				{
					fuelSwitch = source.FirstOrDefault();
				}
				if (fuelSwitch == null)
				{
					useFuelSwitchModule = false;
				}
				else
				{
					int num = fuelSwitch.FindMatchingConfig(this);
					if (HighLogic.LoadedSceneIsFlight || num >= 0)
					{
						if (debugMode)
						{
							Debug.Log("[IFS] - InterstellarMeshSwitch " + base.part.GetInstanceID() + " sets selectedObject to matching object " + num);
						}
						selectedObject = num;
					}
				}
			}
			initialized = true;
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS] - InterstellarMeshSwitch.InitializeData Error: " + ex.Message);
			throw;
		}
	}

	public override string GetInfo()
	{
		if (showInfo)
		{
			List<string> list = ParseTools.ParseNames((objectDisplayNames.Length > 0) ? objectDisplayNames : objects);
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(Localizer.Format("#LOC_IFS_MeshSwitch_GetInfo") + ":");
			foreach (string item in list)
			{
				stringBuilder.AppendLine(item);
			}
			return stringBuilder.ToString();
		}
		return string.Empty;
	}
}
