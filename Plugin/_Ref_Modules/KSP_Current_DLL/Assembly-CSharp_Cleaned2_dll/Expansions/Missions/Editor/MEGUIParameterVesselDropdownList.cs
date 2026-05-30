using System.Collections.Generic;
using TMPro;
using ns9;

namespace Expansions.Missions.Editor;

[MEGUI_VesselSelect]
public class MEGUIParameterVesselDropdownList : MEGUIParameterVessel
{
	public TMP_Dropdown dropdownList;

	protected DictionaryValueList<uint, string> dropdownOptions;

	protected GAPVesselDisplay vesselDisplay;

	protected bool overrideDefaultOptionIsActiveVessel;

	protected bool defaultOptionIsActiveVessel;

	public int FieldValue
	{
		get
		{
			uint value = (field.GetValue() as uint?).Value;
			if (dropdownOptions.KeysList.Contains(value))
			{
				return dropdownOptions.KeysList.IndexOf(value);
			}
			field.SetValue(dropdownOptions.KeysList[dropdownList.value]);
			return dropdownList.value;
		}
		set
		{
			if (value < dropdownOptions.Count && value > -1)
			{
				MissionEditorHistory.PushUndoAction(this, OnHistoryValueChange);
				field.SetValue(dropdownOptions.KeysList[value]);
			}
		}
	}

	protected override void Setup(string name)
	{
		base.Setup(name);
		title.text = name;
		SetDropdownValues();
		dropdownList.value = FieldValue;
		dropdownList.onValueChanged.AddListener(OnParameterValueChanged);
		GameEvents.Mission.onBuilderNodeAdded.Add(onBuilderNodeAdded);
		GameEvents.Mission.onBuilderNodeDeleted.Add(onBuilderNodeDeleted);
		GameEvents.Mission.onVesselSituationChanged.Add(SetDropdownValues);
	}

	private new void OnDestroy()
	{
		GameEvents.Mission.onBuilderNodeAdded.Remove(onBuilderNodeAdded);
		GameEvents.Mission.onBuilderNodeDeleted.Remove(onBuilderNodeDeleted);
		GameEvents.Mission.onVesselSituationChanged.Remove(SetDropdownValues);
	}

	private void onBuilderNodeAdded(MENode node)
	{
		if (node.IsVesselNode)
		{
			SetDropdownValues();
		}
	}

	private void onBuilderNodeDeleted(MENode node)
	{
		if (node.IsVesselNode)
		{
			SetDropdownValues();
		}
	}

	protected void SetDropdownValues()
	{
		dropdownList.ClearOptions();
		dropdownOptions = new DictionaryValueList<uint, string>();
		List<string> list = new List<string>();
		defaultOptionIsActiveVessel = ((!overrideDefaultOptionIsActiveVessel) ? ((MEGUI_VesselSelect)field.Attribute).defaultOptionIsActiveVessel : defaultOptionIsActiveVessel);
		if (((MEGUI_VesselSelect)field.Attribute).addDefaultOption)
		{
			if (!defaultOptionIsActiveVessel)
			{
				dropdownOptions.Add(0u, "#autoLOC_8001004");
				list.Add("#autoLOC_8001004");
			}
			else
			{
				dropdownOptions.Add(0u, "#autoLOC_8004217");
				list.Add("#autoLOC_8004217");
			}
		}
		int i = 0;
		for (int count = base.VesselList.Count; i < count; i++)
		{
			dropdownOptions.Add(base.VesselList[i].persistentId, base.VesselList[i].vesselName);
			MissionCraft craftBySituationsVesselID = MissionEditorLogic.Instance.EditorMission.GetCraftBySituationsVesselID(base.VesselList[i].persistentId);
			if (craftBySituationsVesselID != null && MissionEditorLogic.Instance.incompatibleCraft.Contains(craftBySituationsVesselID.craftFile))
			{
				list.Add(Localizer.Format("#autoLOC_8004245", base.VesselList[i].vesselName));
			}
			else
			{
				list.Add(base.VesselList[i].vesselName);
			}
		}
		if (list.Count == 0)
		{
			dropdownOptions.Add(0u, "#autoLOC_6003000");
			list.Add("#autoLOC_6003000");
		}
		dropdownList.AddOptions(list);
		if (dropdownList.value >= dropdownOptions.Count)
		{
			dropdownList.value = dropdownOptions.Count - 1;
		}
	}

	public void OverrideDefaultValue(bool defaultOptionIsActiveVessel)
	{
		overrideDefaultOptionIsActiveVessel = true;
		this.defaultOptionIsActiveVessel = defaultOptionIsActiveVessel;
	}

	protected override void ResetDefaultValue(string value)
	{
		uint result = 0u;
		if (uint.TryParse(value, out result) && dropdownOptions.KeysList.Contains(result))
		{
			FieldValue = dropdownOptions.KeysList.IndexOf(result);
		}
	}

	public override void RefreshUI()
	{
		dropdownList.value = FieldValue;
	}

	public override void Display()
	{
		base.Display();
		dropdownList.onValueChanged.RemoveListener(OnParameterValueChanged);
		bool flag = true;
		int i = 0;
		for (int count = base.VesselList.Count; i < count; i++)
		{
			if (base.VesselList[i].vesselName == dropdownList.options[dropdownList.value].text)
			{
				flag = false;
			}
		}
		if (flag)
		{
			SetDropdownValues();
			dropdownList.value = 0;
		}
		else
		{
			SetDropdownValues();
			dropdownList.value = FieldValue;
		}
		dropdownList.onValueChanged.AddListener(OnParameterValueChanged);
	}

	private void OnParameterValueChanged(int value)
	{
		FieldValue = value;
		UpdateNodeBodyUI();
		if (vesselDisplay != null && value < dropdownOptions.Count)
		{
			vesselDisplay.SetupVessel(mission.GetCraftBySituationsVesselID(dropdownOptions.KeyAt(value)), mission.GetVesselSituationByVesselID(dropdownOptions.KeyAt(value)), this);
		}
	}

	public override void DisplayGAP()
	{
		base.DisplayGAP();
		vesselDisplay = MissionEditorLogic.Instance.actionPane.GAPInitialize<GAPVesselDisplay>();
		vesselDisplay.SetupVessel(mission.GetCraftBySituationsVesselID(dropdownOptions.KeyAt(FieldValue)), mission.GetVesselSituationByVesselID(dropdownOptions.KeyAt(FieldValue)), this);
	}

	public override void OnNextVessel()
	{
		dropdownList.value = (dropdownList.value + 1) % dropdownList.options.Count;
	}

	public override void OnPrevVessel()
	{
		dropdownList.value = ((dropdownList.value - 1 < 0) ? (dropdownList.options.Count - 1) : (dropdownList.value - 1));
	}

	public void OnHistoryValueChange(ConfigNode data, HistoryType type)
	{
		uint value = 0u;
		if (data.TryGetValue("value", ref value))
		{
			field.SetValue(value);
			dropdownList.value = FieldValue;
		}
	}
}
