using System.Collections.Generic;
using UniLinq;

namespace B9PartSwitch;

public class ModuleB9PartInfo : PartModule
{
	public const string DryMassGUIString = "Mass (Dry)";

	public const string MassGUIString = "Mass";

	public const string DryCostGUIString = "Cost (Dry)";

	public const string CostGUIString = "Cost";

	[UI_Toggle(enabledText = "Enabled", disabledText = "Hidden")]
	[KSPField(guiActiveEditor = true, guiName = "Part Info")]
	public bool showInfo;

	[KSPField(guiName = "Mass (Dry)", guiFormat = "N3", guiUnits = "t")]
	public float dryMass;

	[KSPField(guiName = "Mass (Wet)", guiFormat = "N3", guiUnits = "t")]
	public float wetMass;

	[KSPField(guiName = "Cost (Dry)", guiFormat = "N2")]
	public float dryCost;

	[KSPField(guiName = "Cost (Wet)", guiFormat = "N2")]
	public float wetCost;

	[KSPField(guiName = "Max Temp", guiFormat = "F0", guiUnits = "K")]
	public float maxTemp;

	[KSPField(guiName = "Max Skin Temp", guiFormat = "F0", guiUnits = "K")]
	public float skinMaxTemp;

	[KSPField(guiName = "Crash Tolerance", guiFormat = "F0", guiUnits = "m/s")]
	public float crashTolerance;

	private void Start()
	{
		if (HighLogic.LoadedSceneIsFlight || !base.part.Modules.OfType<ModuleB9PartSwitch>().Any((ModuleB9PartSwitch m) => DisplayInfoOnSwitcher(m)))
		{
			base.enabled = false;
			isEnabled = false;
		}
		else
		{
			SetupGUI();
			UpdateFields();
			GameEvents.onEditorShipModified.Add(EditorShipModified);
		}
	}

	private void EditorShipModified(ShipConstruct construct)
	{
		SetupGUI();
		UpdateFields();
	}

	private void SetupGUI()
	{
		List<ModuleB9PartSwitch> source = base.part.FindModulesImplementing<ModuleB9PartSwitch>();
		bool flag = base.part.Resources.Any((PartResource resource) => resource.info.density != 0f);
		bool flag2 = source.Any((ModuleB9PartSwitch module) => module.ChangesMass);
		bool flag3 = source.Any((ModuleB9PartSwitch module) => module.ChangesCost);
		BaseField baseField = base.Fields["dryMass"];
		baseField.guiActiveEditor = showInfo && flag2;
		baseField.guiName = (flag ? "Mass (Dry)" : "Mass");
		base.Fields["wetMass"].guiActiveEditor = showInfo && flag2 && flag;
		BaseField baseField2 = base.Fields["dryCost"];
		baseField2.guiActiveEditor = showInfo && flag3;
		baseField2.guiName = (flag ? "Cost (Dry)" : "Cost");
		base.Fields["wetCost"].guiActiveEditor = showInfo && flag3 && flag;
		base.Fields["maxTemp"].guiActiveEditor = showInfo && source.Any((ModuleB9PartSwitch module) => module.HasPartAspectLock("maxTemp"));
		base.Fields["skinMaxTemp"].guiActiveEditor = showInfo && source.Any((ModuleB9PartSwitch module) => module.HasPartAspectLock("skinMaxTemp"));
		base.Fields["crashTolerance"].guiActiveEditor = showInfo && source.Any((ModuleB9PartSwitch module) => module.HasPartAspectLock("crashTolerance"));
	}

	private void UpdateFields()
	{
		float mass = base.part.GetPrefab().mass;
		dryMass = mass + base.part.GetModuleMass(mass);
		wetMass = dryMass + base.part.GetResourceMass();
		float num = base.part.partInfo.cost + base.part.GetModuleCosts(base.part.partInfo.cost);
		wetCost = num + base.part.GetResourceCostOffset();
		dryCost = num - base.part.GetResourceCostMax();
		maxTemp = (float)base.part.maxTemp;
		skinMaxTemp = (float)base.part.skinMaxTemp;
		crashTolerance = base.part.crashTolerance;
	}

	private bool DisplayInfoOnSwitcher(ModuleB9PartSwitch switcher)
	{
		if (!switcher.ChangesMass && !switcher.ChangesCost && !switcher.HasPartAspectLock("maxTemp") && !switcher.HasPartAspectLock("skinMaxTemp"))
		{
			return switcher.HasPartAspectLock("crashTolerance");
		}
		return true;
	}
}
