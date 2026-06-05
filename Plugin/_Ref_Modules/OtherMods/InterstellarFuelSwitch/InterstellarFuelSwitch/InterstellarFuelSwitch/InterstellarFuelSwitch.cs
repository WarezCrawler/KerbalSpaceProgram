using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using KSP.Localization;
using TweakScale;
using UnityEngine;

namespace InterstellarFuelSwitch;

[KSPModule("#LOC_IFS_FuelSwitch_moduleName")]
public class InterstellarFuelSwitch : PartModule, IRescalable<InterstellarFuelSwitch>, IRescalable, IPartCostModifier, IPartMassModifier
{
	[UI_ChooseOption(affectSymCounterparts = UI_Scene.None, scene = UI_Scene.All, suppressEditorShipModified = true)]
	[KSPField(isPersistant = true)]
	public int selectedTankSetup = -1;

	[KSPField(isPersistant = true)]
	public string configuredAmounts = "";

	[KSPField(isPersistant = true)]
	public string configuredFlowStates = "";

	[KSPField(isPersistant = true)]
	public string selectedTankSetupTxt;

	[KSPField(isPersistant = true)]
	public bool configLoaded;

	[KSPField(isPersistant = true)]
	public string initialTankSetup;

	[KSPField(isPersistant = true)]
	public double storedFactorMultiplier = 1.0;

	[KSPField(isPersistant = true)]
	public double storedSurfaceMultiplier = 1.0;

	[KSPField(isPersistant = true)]
	public double storedVolumeMultiplier = 1.0;

	[KSPField(isPersistant = true)]
	public double baseMassMultiplier = 1.0;

	[KSPField(isPersistant = true)]
	public double initialMassMultiplier = 1.0;

	[KSPField(isPersistant = true)]
	public float windowPositionX = 1200f;

	[KSPField(isPersistant = true)]
	public float windowPositionY = 150f;

	[KSPField]
	public float windowWidth = 200f;

	[KSPField]
	public string moduleID = "0";

	[KSPField]
	public string tankId = string.Empty;

	[KSPField]
	public string resourceGui = string.Empty;

	[KSPField]
	public string tankSwitchNames = string.Empty;

	[KSPField]
	public string crewCapacity = string.Empty;

	[KSPField]
	public string habitatVolume = string.Empty;

	[KSPField]
	public string habitatSurface = string.Empty;

	[KSPField]
	public string bannedResourceNames = string.Empty;

	[KSPField]
	public string switcherDescription = "#LOC_IFS_FuelSwitch_switcherDescription";

	[KSPField]
	public string resourceNames = "ElectricCharge;LiquidFuel,Oxidizer;MonoPropellant";

	[KSPField]
	public string resourceAmounts = string.Empty;

	[KSPField]
	public string resourceRatios = string.Empty;

	[KSPField]
	public string initialResourceAmounts = string.Empty;

	[KSPField]
	public bool ignoreInitialCost;

	[KSPField]
	public bool adaptiveTankSelection;

	[KSPField]
	public float basePartMass;

	[KSPField]
	public double baseResourceMassDivider;

	[KSPField]
	public string tankResourceMassDivider = string.Empty;

	[KSPField]
	public string tankResourceMassDividerAddition = string.Empty;

	[KSPField]
	public bool overrideMassWithTankDividers;

	[KSPField]
	public bool orderBySwitchName;

	[KSPField]
	public string tankMass = "";

	[KSPField]
	public string tankTechReq = "";

	[KSPField]
	public string tankCost = "";

	[KSPField]
	public string boilOffTemp = "";

	[KSPField]
	public bool displayTankCost;

	[KSPField]
	public bool displayWetDryMass = true;

	[KSPField]
	public bool hasSwitchChooseOption = true;

	[KSPField]
	public bool hasGUI = true;

	[KSPField]
	public bool availableInFlight;

	[KSPField]
	public bool availableInEditor = true;

	[KSPField]
	public bool returnDryMass;

	[KSPField]
	public string inEditorSwitchingTechReq;

	[KSPField]
	public string inFlightSwitchingTechReq;

	[KSPField]
	public bool useTextureSwitchModule;

	[KSPField]
	public bool showTankName = true;

	[KSPField]
	public bool showInfo = true;

	[KSPField]
	public string moduleInfoTemplate;

	[KSPField]
	public string moduleInfoParams;

	[KSPField]
	public string resourcesFormat = "0.0000";

	[KSPField]
	public bool canSwitchWithFullTanks;

	[KSPField]
	public bool allowedToSwitch;

	[KSPField]
	public bool updateModuleCost = true;

	[KSPField]
	public bool controlCrewCapacity;

	[KSPField(guiActive = false, guiActiveEditor = false, guiName = "#LOC_IFS_FuelSwitch_tankGuiName")]
	public string tankGuiName = "";

	[KSPField(guiActive = true, guiActiveEditor = true, guiName = "#LOC_IFS_FuelSwitch_maxWetDryMass")]
	public string maxWetDryMass = "";

	[KSPField(guiActive = false, guiActiveEditor = true, guiName = "#LOC_IFS_FuelSwitch_massRatioStr")]
	public string massRatioStr = "";

	[KSPField(guiActive = true, guiActiveEditor = true, guiName = "#LOC_IFS_FuelSwitch_crewCapacityStr")]
	public string crewCapacityStr = "";

	[UI_Toggle(disabledText = "Hidden", enabledText = "Shown", affectSymCounterparts = UI_Scene.None)]
	[KSPField(guiActive = false, guiActiveEditor = true, guiName = "#LOC_IFS_FuelSwitch_switchWindow")]
	public bool render_window;

	[KSPField]
	public bool debugMode;

	[KSPField]
	public float moduleCost;

	[KSPField]
	public double dryMass;

	[KSPField]
	public double initialMass;

	[KSPField]
	public double moduleMassDelta;

	[KSPField]
	public float defaultMass;

	[KSPField]
	public string defaultTank = "";

	[KSPField]
	public string resourceAmountStr0 = "";

	[KSPField]
	public string resourceAmountStr1 = "";

	[KSPField]
	public string resourceAmountStr2 = "";

	[KSPField]
	public string resourceAmountStr3 = "";

	[KSPField]
	public double volumeExponent = 3.0;

	[KSPField]
	public double massExponent = 3.0;

	[KSPField]
	public double baseMassExponent;

	[KSPField]
	public double tweakscaleMassExponent = 3.0;

	[KSPField(guiActive = true, guiActiveEditor = true, guiName = "#LOC_IFS_FuelSwitch_totalMass", guiUnits = " t", guiFormat = "F4")]
	public double totalMass;

	[KSPField(guiActive = false, guiActiveEditor = false, guiName = "#LOC_IFS_FuelSwitch_maxResourceCost", guiFormat = "F3", guiUnits = " Ѵ")]
	public double maxResourceCost;

	[KSPField(guiActive = false, guiActiveEditor = true, guiName = "#LOC_IFS_FuelSwitch_dryCost", guiFormat = "F3", guiUnits = " Ѵ")]
	public double dryCost;

	[KSPField(guiActive = false, guiActiveEditor = true, guiName = "#LOC_IFS_FuelSwitch_resourceCost", guiFormat = "F3", guiUnits = " Ѵ")]
	public double resourceCost;

	[KSPField(guiActive = false, guiActiveEditor = true, guiName = "#LOC_IFS_FuelSwitch_totalCost", guiFormat = "F3", guiUnits = " Ѵ")]
	public double totalCost;

	private List<IFSmodularTank> _modularTankList = new List<IFSmodularTank>();

	private InterstellarTextureSwitch2 textureSwitch;

	private IFSmodularTank selectedTank;

	private HashSet<string> activeResourceList = new HashSet<string>();

	private Rect windowPosition;

	private bool _initialized;

	private bool closeAterSwitch;

	private int _numberOfAvailableTanks;

	private int _windowID;

	private double _partResourceMaxAmountFraction0;

	private double _partResourceMaxAmountFraction1;

	private double _partResourceMaxAmountFraction2;

	private double _partResourceMaxAmountFraction3;

	private PartResource _partResource0;

	private PartResource _partResource1;

	private PartResource _partResource2;

	private PartResource _partResource3;

	private PartResourceDefinition _partRresourceDefinition0;

	private PartResourceDefinition _partRresourceDefinition1;

	private PartResourceDefinition _partRresourceDefinition2;

	private PartResourceDefinition _partRresourceDefinition3;

	private BaseField _field0;

	private BaseField _field1;

	private BaseField _field2;

	private BaseField _field3;

	private BaseField _massRatioStrField;

	private BaseField _maxWetDryMassField;

	private BaseField _crewCapacityField;

	private BaseField _tankGuiNameField;

	private BaseField _chooseField;

	private BaseEvent _nextTankSetupEvent;

	private BaseEvent _previousTankSetupEvent;

	private PartModule habitatModule;

	private BaseField habitatVolumeField;

	private BaseField habitatSurfaceField;

	private BaseField habitatStateField;

	private BaseField habitatToggleField;

	private BaseField volumeField;

	private BaseField surfaceField;

	private MethodInfo habitatOnStartMethod;

	private IHaveFuelTankSetup _fuelTankSetupControl;

	private static HashSet<string> _researchedTechs;

	[KSPAction("Show Switch Tank Window")]
	public void ToggleSwitchWindowwAction(KSPActionParam param)
	{
		Debug.Log("[IFS] - Toggled Switch Window");
		render_window = !render_window;
	}

	public virtual void OnRescale(ScalingFactor factor)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			FactorSet absolute = ((ScalingFactor)(ref factor)).absolute;
			double num = (storedFactorMultiplier = (double)(decimal)((FactorSet)(ref absolute)).linear);
			storedSurfaceMultiplier = num * num;
			storedVolumeMultiplier = Math.Pow(num, volumeExponent);
			baseMassMultiplier = Math.Pow(num, (baseMassExponent == 0.0) ? massExponent : baseMassExponent);
			initialMassMultiplier = Math.Pow(num, tweakscaleMassExponent);
			initialMass = (double)(decimal)base.part.prefabMass * initialMassMultiplier;
			UpdateHabitat(selectedTank);
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: OnRescale Error: " + ex.Message);
			throw;
		}
	}

	public int FindMatchingConfig(IHaveFuelTankSetup control = null)
	{
		if (control != null)
		{
			_fuelTankSetupControl = control;
		}
		InitializeData();
		if (selectedTankSetup == -1 && !string.IsNullOrEmpty(defaultTank))
		{
			selectedTankSetupTxt = Localizer.Format(defaultTank);
		}
		IFSmodularTank iFSmodularTank = _modularTankList.FirstOrDefault((IFSmodularTank t) => t.GuiName == selectedTankSetupTxt) ?? _modularTankList.FirstOrDefault((IFSmodularTank t) => t.SwitchName == selectedTankSetupTxt) ?? _modularTankList.FirstOrDefault((IFSmodularTank t) => t.Composition == selectedTankSetupTxt);
		if (iFSmodularTank != null)
		{
			return _modularTankList.IndexOf(iFSmodularTank);
		}
		if (base.part.Resources.Count((PartResource r) => activeResourceList.Contains(r.resourceName)) == 0)
		{
			return -1;
		}
		for (int i = 0; i < _modularTankList.Count; i++)
		{
			IFSmodularTank iFSmodularTank2 = _modularTankList[i];
			bool flag = true;
			if (iFSmodularTank2.Resources.Count != base.part.Resources.Count((PartResource r) => activeResourceList.Contains(r.resourceName)))
			{
				flag = false;
			}
			else
			{
				foreach (IFSresource resource in iFSmodularTank2.Resources)
				{
					if (!base.part.Resources.Contains(resource.name))
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				return i;
			}
		}
		return -1;
	}

	public override void OnStart(StartState state)
	{
		try
		{
			_crewCapacityField = base.Fields["crewCapacityStr"];
			_crewCapacityField.guiActive = controlCrewCapacity;
			_crewCapacityField.guiActiveEditor = controlCrewCapacity;
			_massRatioStrField = base.Fields["massRatioStr"];
			_massRatioStrField.guiActive = displayWetDryMass;
			_massRatioStrField.guiActiveEditor = displayWetDryMass;
			_maxWetDryMassField = base.Fields["maxWetDryMass"];
			_maxWetDryMassField.guiActive = displayWetDryMass;
			_maxWetDryMassField.guiActiveEditor = displayWetDryMass;
			initialMass = (double)(decimal)base.part.prefabMass * initialMassMultiplier;
			if (initialMass == 0.0)
			{
				initialMass = (double)(decimal)base.part.prefabMass;
			}
			defaultTank = Localizer.Format(defaultTank);
			_windowID = new System.Random(base.part.GetInstanceID()).Next(int.MaxValue);
			windowPosition = new Rect(windowPositionX, windowPositionY, windowWidth, 10f);
			InitializeData();
			InitializeKerbalismHabitat();
			if (adaptiveTankSelection || selectedTankSetup == -1)
			{
				if (selectedTankSetup == -1)
				{
					initialTankSetup = string.Join(";", base.part.Resources.Select((PartResource m) => m.resourceName).ToArray());
				}
				int num = FindMatchingConfig();
				if (num != -1)
				{
					selectedTank = _modularTankList[num];
					selectedTankSetupTxt = selectedTank.GuiName;
				}
				else if (state == StartState.Editor)
				{
					IFSmodularTank iFSmodularTank = _modularTankList.FirstOrDefault((IFSmodularTank m) => m.GuiName == defaultTank) ?? _modularTankList.FirstOrDefault((IFSmodularTank m) => m.SwitchName == defaultTank) ?? _modularTankList.FirstOrDefault((IFSmodularTank m) => m.Composition == defaultTank);
					if (iFSmodularTank == null)
					{
						selectedTank = _modularTankList[0];
					}
					else
					{
						selectedTank = iFSmodularTank;
					}
					selectedTankSetupTxt = selectedTank.GuiName;
				}
			}
			base.enabled = true;
			AssignResourcesToPart();
			_chooseField = base.Fields["selectedTankSetup"];
			_chooseField.guiName = Localizer.Format(switcherDescription);
			_chooseField.guiActiveEditor = hasSwitchChooseOption && availableInEditor && _modularTankList.Count > 1;
			_chooseField.guiActive = hasSwitchChooseOption && availableInFlight && _modularTankList.Count > 1;
			if (_chooseField.uiControlEditor is UI_ChooseOption uI_ChooseOption)
			{
				uI_ChooseOption.options = _modularTankList.Select((IFSmodularTank s) => s.SwitchName).ToArray();
				uI_ChooseOption.onFieldChanged = UpdateFromGUI;
			}
			if (_chooseField.uiControlFlight is UI_ChooseOption uI_ChooseOption2)
			{
				uI_ChooseOption2.options = _modularTankList.Select((IFSmodularTank s) => s.SwitchName).ToArray();
				uI_ChooseOption2.onFieldChanged = UpdateFromGUI;
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: OnStart Error: " + ex.Message);
			throw;
		}
	}

	private void UpdateFromGUI(BaseField field, object oldFieldValueObj)
	{
		SwitchOrAssign((int)oldFieldValueObj);
	}

	private void SwitchOrAssign(int oldFieldValueObj)
	{
		try
		{
			IFSmodularTank iFSmodularTank = _modularTankList[selectedTankSetup];
			if (!iFSmodularTank.hasTech || (controlCrewCapacity && base.part.protoModuleCrew != null && iFSmodularTank.crewCapacity < base.part.protoModuleCrew.Count))
			{
				if (oldFieldValueObj < selectedTankSetup || (oldFieldValueObj == _modularTankList.Count - 1 && selectedTankSetup == 0))
				{
					nextTankSetupEvent();
				}
				else
				{
					previousTankSetupEvent();
				}
			}
			else
			{
				AssignResourcesToPart(calledByPlayer: true, affectSymCounterparts: true);
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: SwitchOrAssign Error: " + ex.Message);
			throw;
		}
	}

	public int SelectTankSetup(int newTankIndex, bool calledByPlayer)
	{
		return SelectTankSetup(newTankIndex.ToString(CultureInfo.InvariantCulture), calledByPlayer);
	}

	public int SelectTankSetup(string newTankName, bool calledByPlayer)
	{
		try
		{
			InitializeData();
			IFSmodularTank iFSmodularTank = _modularTankList.FirstOrDefault((IFSmodularTank m) => m.GuiName == newTankName) ?? _modularTankList.FirstOrDefault((IFSmodularTank m) => m.SwitchName == newTankName) ?? _modularTankList.FirstOrDefault((IFSmodularTank m) => m.Composition == newTankName);
			if (iFSmodularTank == null && int.TryParse(newTankName, out var result))
			{
				iFSmodularTank = ((result < _modularTankList.Count) ? _modularTankList[result] : null);
			}
			if (iFSmodularTank == null)
			{
				return -1;
			}
			int oldFieldValueObj = selectedTankSetup;
			selectedTankSetup = _modularTankList.IndexOf(iFSmodularTank);
			selectedTankSetupTxt = iFSmodularTank.GuiName;
			SwitchOrAssign(oldFieldValueObj);
			return selectedTankSetup;
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: SelectTankSetup Error: " + ex.Message);
			throw;
		}
	}

	public override void OnAwake()
	{
		try
		{
			if (configLoaded)
			{
				InitializeData();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: OnAwake Error: " + ex.Message);
			throw;
		}
	}

	public override void OnLoad(ConfigNode partNode)
	{
		base.OnLoad(partNode);
		if (!configLoaded)
		{
			InitializeData();
		}
		configLoaded = true;
	}

	private void InitializeData()
	{
		try
		{
			if (_initialized)
			{
				return;
			}
			_field0 = base.Fields["resourceAmountStr0"];
			_field1 = base.Fields["resourceAmountStr1"];
			_field2 = base.Fields["resourceAmountStr2"];
			_field3 = base.Fields["resourceAmountStr3"];
			_tankGuiNameField = base.Fields["tankGuiName"];
			availableInEditor = (string.IsNullOrEmpty(inEditorSwitchingTechReq) ? availableInEditor : HasTech(inEditorSwitchingTechReq));
			availableInFlight = (string.IsNullOrEmpty(inFlightSwitchingTechReq) ? availableInFlight : HasTech(inFlightSwitchingTechReq));
			SetupTankList();
			if (HighLogic.LoadedSceneIsGame)
			{
				foreach (IFSmodularTank modularTank in _modularTankList)
				{
					modularTank.hasTech = HasTech(modularTank.techReq);
				}
				_numberOfAvailableTanks = _modularTankList.Count((IFSmodularTank m) => m.hasTech);
			}
			_nextTankSetupEvent = base.Events["nextTankSetupEvent"];
			_nextTankSetupEvent.guiActive = hasGUI && availableInFlight;
			_previousTankSetupEvent = base.Events["previousTankSetupEvent"];
			_previousTankSetupEvent.guiActive = hasGUI && availableInFlight;
			base.Fields["dryCost"].guiActiveEditor = displayTankCost && HighLogic.LoadedSceneIsEditor;
			base.Fields["resourceCost"].guiActiveEditor = displayTankCost && HighLogic.LoadedSceneIsEditor;
			base.Fields["maxResourceCost"].guiActiveEditor = displayTankCost && HighLogic.LoadedSceneIsEditor;
			base.Fields["totalCost"].guiActiveEditor = displayTankCost && HighLogic.LoadedSceneIsEditor;
			if (useTextureSwitchModule)
			{
				textureSwitch = base.part.GetComponent<InterstellarTextureSwitch2>();
				if (textureSwitch == null)
				{
					useTextureSwitchModule = false;
				}
			}
			_initialized = true;
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: InitializeData Error: " + ex.Message);
			throw;
		}
	}

	[KSPEvent(guiActive = false, guiActiveEditor = true, guiName = "Switch Tank")]
	public void switchTankEvent()
	{
		closeAterSwitch = true;
		render_window = true;
	}

	[KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "#LOC_IFS_FuelSwitch_nextTankSetupText")]
	public void nextTankSetupEvent()
	{
		try
		{
			selectedTankSetup++;
			if (selectedTankSetup >= _modularTankList.Count)
			{
				selectedTankSetup = 0;
			}
			IFSmodularTank iFSmodularTank = _modularTankList[selectedTankSetup];
			if (!iFSmodularTank.hasTech || (controlCrewCapacity && iFSmodularTank.crewCapacity < base.part.protoModuleCrew.Count))
			{
				nextTankSetupEvent();
			}
			AssignResourcesToPart(calledByPlayer: true, affectSymCounterparts: true);
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: nextTankSetupEvent Error: " + ex.Message);
			throw;
		}
	}

	[KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "#LOC_IFS_FuelSwitch_previousTankSetupText")]
	public void previousTankSetupEvent()
	{
		try
		{
			selectedTankSetup--;
			if (selectedTankSetup < 0)
			{
				selectedTankSetup = _modularTankList.Count - 1;
			}
			if (!_modularTankList[selectedTankSetup].hasTech)
			{
				previousTankSetupEvent();
			}
			AssignResourcesToPart(calledByPlayer: true, affectSymCounterparts: true);
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: previousTankSetupEvent Error: " + ex.Message);
			throw;
		}
	}

	private void AssignResourcesToPart(bool calledByPlayer = false, bool affectSymCounterparts = false)
	{
		try
		{
			List<string> newResources = SetupTankInPart(base.part, calledByPlayer);
			ConfigureResourceMassGui(newResources);
			UpdateTankName();
			UpdateTexture(calledByPlayer);
			dryMass = 0.0;
			UpdateDryMass();
			UpdateGuiResourceMass();
			UpdateCost();
			if (!HighLogic.LoadedSceneIsEditor || !affectSymCounterparts)
			{
				return;
			}
			foreach (Part symmetryCounterpart in base.part.symmetryCounterparts)
			{
				InterstellarFuelSwitch interstellarFuelSwitch = (string.IsNullOrEmpty(tankId) ? symmetryCounterpart.FindModulesImplementing<InterstellarFuelSwitch>().FirstOrDefault() : symmetryCounterpart.FindModulesImplementing<InterstellarFuelSwitch>().FirstOrDefault((InterstellarFuelSwitch m) => m.tankId == tankId));
				if (!(interstellarFuelSwitch == null))
				{
					interstellarFuelSwitch.selectedTankSetup = selectedTankSetup;
					interstellarFuelSwitch.selectedTankSetupTxt = selectedTankSetupTxt;
					interstellarFuelSwitch.AssignResourcesToPart(calledByPlayer);
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: AssignResourcesToPart Error: " + ex.Message);
			throw;
		}
	}

	public void UpdateTexture(bool calledByPlayer)
	{
		if (textureSwitch != null)
		{
			textureSwitch.SelectTankSetup(selectedTankSetup, calledByPlayer);
		}
	}

	public void UpdateTankName()
	{
		tankGuiName = _modularTankList[selectedTankSetup].GuiName;
		bool flag = !string.IsNullOrEmpty(tankGuiName);
		_tankGuiNameField.guiActive = showTankName && flag;
		_tankGuiNameField.guiActiveEditor = showTankName && flag;
	}

	private List<string> SetupTankInPart(Part currentPart, bool calledByPlayer)
	{
		try
		{
			FindSelectedTank(calledByPlayer);
			selectedTankSetupTxt = selectedTank.GuiName;
			selectedTankSetup = _modularTankList.IndexOf(selectedTank);
			List<string> list = new List<string>();
			List<ConfigNode> list2 = new List<ConfigNode>();
			List<double> list3 = new List<double>();
			List<bool> list4 = new List<bool>();
			if (configuredAmounts.Length > 0)
			{
				if (calledByPlayer)
				{
					configuredAmounts = string.Empty;
				}
				string[] array = configuredAmounts.Split(',');
				string[] array2 = array;
				foreach (string s in array2)
				{
					if (double.TryParse(s, out var result))
					{
						list3.Add(result);
					}
				}
				if (!HighLogic.LoadedSceneIsEditor)
				{
					configuredAmounts = string.Empty;
				}
			}
			if (configuredFlowStates.Length > 0)
			{
				if (calledByPlayer)
				{
					configuredFlowStates = string.Empty;
				}
				string[] array3 = configuredFlowStates.Split(',');
				string[] array4 = array3;
				foreach (string value in array4)
				{
					if (bool.TryParse(value, out var result2))
					{
						list4.Add(result2);
					}
				}
				if (!HighLogic.LoadedSceneIsEditor)
				{
					configuredFlowStates = string.Empty;
				}
			}
			UpdateHabitat(selectedTank);
			for (int k = 0; k < selectedTank.Resources.Count; k++)
			{
				IFSresource iFSresource = selectedTank.Resources[k];
				if (!(iFSresource.name == "Structural"))
				{
					list.Add(iFSresource.name);
					ConfigNode configNode = new ConfigNode("RESOURCE");
					double num = iFSresource.maxAmount * storedVolumeMultiplier;
					configNode.AddValue("name", iFSresource.name);
					configNode.AddValue("maxAmount", num);
					PartResource partResource = currentPart.Resources[iFSresource.name];
					double value2 = ((partResource != null) ? Math.Min(partResource.amount / partResource.maxAmount * num, num) : ((!HighLogic.LoadedSceneIsEditor && k < list3.Count) ? list3[k] : ((HighLogic.LoadedSceneIsEditor || !calledByPlayer) ? (selectedTank.Resources[k].amount * storedVolumeMultiplier) : 0.0)));
					configNode.AddValue("amount", value2);
					if (partResource != null)
					{
						configNode.AddValue("flowState", partResource.flowState);
					}
					else if (k < list4.Count)
					{
						configNode.AddValue("flowState", list4[k]);
					}
					list2.Add(configNode);
				}
			}
			List<ConfigNode> list5 = new List<ConfigNode>();
			if (list2.Count > 0)
			{
				list5.AddRange(list2);
				list2.Clear();
			}
			foreach (PartResource resource in currentPart.Resources)
			{
				if (!activeResourceList.Contains(resource.resourceName))
				{
					ConfigNode configNode2 = new ConfigNode("RESOURCE");
					configNode2.AddValue("name", resource.resourceName);
					configNode2.AddValue("maxAmount", resource.maxAmount);
					configNode2.AddValue("amount", resource.amount);
					configNode2.AddValue("flowState", resource.flowState);
					list5.Add(configNode2);
				}
			}
			if (list2.Count > 0)
			{
				list5.AddRange(list2);
				list2.Clear();
			}
			currentPart.Resources.Clear();
			if (list5.Count > 0)
			{
				foreach (ConfigNode item in list5)
				{
					currentPart.AddResource(item);
				}
			}
			UpdateCost();
			UpdatePartActionWindow();
			return list;
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: SetupTankInPart Error: " + ex.Message);
			throw;
		}
	}

	private void UpdatePartActionWindow()
	{
		UIPartActionWindow uIPartActionWindow = UnityEngine.Object.FindObjectsOfType<UIPartActionWindow>().FirstOrDefault((UIPartActionWindow w) => w.part == base.part);
		if (!(uIPartActionWindow != null))
		{
			return;
		}
		UIPartActionWindow[] array = UnityEngine.Object.FindObjectsOfType<UIPartActionWindow>();
		foreach (UIPartActionWindow uIPartActionWindow2 in array)
		{
			if (!(uIPartActionWindow.part != base.part))
			{
				uIPartActionWindow2.ClearList();
				uIPartActionWindow2.displayDirty = true;
			}
		}
	}

	private void FindSelectedTank(bool calledByPlayer)
	{
		try
		{
			selectedTank = ((calledByPlayer && selectedTankSetup >= 0 && selectedTankSetup < _modularTankList.Count) ? _modularTankList[selectedTankSetup] : null);
			if (selectedTank == null)
			{
				int num = FindMatchingConfig();
				if (num >= 0)
				{
					selectedTank = _modularTankList[num];
				}
			}
			if (selectedTank == null && (HighLogic.LoadedSceneIsFlight || _modularTankList.Count == 0) && base.part.Resources.Any((PartResource m) => m.info.density > 0f))
			{
				IEnumerable<PartResource> source = base.part.Resources.Where((PartResource m) => m.info.density > 0f);
				string text = string.Join("+", source.Select((PartResource r) => r.info.displayName).ToArray());
				Debug.LogWarning("[IFS]: Constructing new tank definition for " + base.part.name + " with name " + text);
				List<IFSresource> resources = source.Select((PartResource r) => new IFSresource(r.resourceName)
				{
					amount = r.amount / storedVolumeMultiplier,
					maxAmount = r.maxAmount / storedVolumeMultiplier
				}).ToList();
				string text2 = string.Join("+", source.Select((PartResource r) => r.info.abbreviation).ToArray());
				selectedTank = new IFSmodularTank
				{
					SwitchName = text2,
					Composition = text2,
					GuiName = text,
					Resources = resources
				};
				_modularTankList.Add(selectedTank);
			}
			if (selectedTank == null)
			{
				Debug.Log("[IFS]: Defaulting selected tank to first tank in collection");
				selectedTank = _modularTankList[0];
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: FindSelectedTank " + ex.Message);
		}
	}

	public void ConfigureResourceMassGui(List<string> newResources)
	{
		_partRresourceDefinition0 = ((newResources.Count > 0) ? PartResourceLibrary.Instance.GetDefinition(newResources[0]) : null);
		_partRresourceDefinition1 = ((newResources.Count > 1) ? PartResourceLibrary.Instance.GetDefinition(newResources[1]) : null);
		_partRresourceDefinition2 = ((newResources.Count > 2) ? PartResourceLibrary.Instance.GetDefinition(newResources[2]) : null);
		_partRresourceDefinition3 = ((newResources.Count > 3) ? PartResourceLibrary.Instance.GetDefinition(newResources[3]) : null);
		_field0.guiName = ((_partRresourceDefinition0 != null) ? _partRresourceDefinition0.name : ":");
		_field1.guiName = ((_partRresourceDefinition1 != null) ? _partRresourceDefinition1.name : ":");
		_field2.guiName = ((_partRresourceDefinition2 != null) ? _partRresourceDefinition2.name : ":");
		_field3.guiName = ((_partRresourceDefinition3 != null) ? _partRresourceDefinition3.name : ":");
		_field0.guiActive = _partRresourceDefinition0 != null;
		_field1.guiActive = _partRresourceDefinition1 != null;
		_field2.guiActive = _partRresourceDefinition2 != null;
		_field3.guiActive = _partRresourceDefinition3 != null;
		_field0.guiActiveEditor = _partRresourceDefinition0 != null;
		_field1.guiActiveEditor = _partRresourceDefinition1 != null;
		_field2.guiActiveEditor = _partRresourceDefinition2 != null;
		_field3.guiActiveEditor = _partRresourceDefinition3 != null;
		_partResource0 = ((_partRresourceDefinition0 == null) ? null : base.part.Resources[newResources[0]]);
		_partResource1 = ((_partRresourceDefinition1 == null) ? null : base.part.Resources[newResources[1]]);
		_partResource2 = ((_partRresourceDefinition2 == null) ? null : base.part.Resources[newResources[2]]);
		_partResource3 = ((_partRresourceDefinition3 == null) ? null : base.part.Resources[newResources[3]]);
		_partResourceMaxAmountFraction0 = ((_partResource0 == null) ? 0.0 : (_partResource0.maxAmount * 0.001));
		_partResourceMaxAmountFraction1 = ((_partResource1 == null) ? 0.0 : (_partResource1.maxAmount * 0.001));
		_partResourceMaxAmountFraction2 = ((_partResource2 == null) ? 0.0 : (_partResource2.maxAmount * 0.001));
		_partResourceMaxAmountFraction3 = ((_partResource3 == null) ? 0.0 : (_partResource3.maxAmount * 0.001));
	}

	private double UpdateCost()
	{
		dryCost = (double)(decimal)base.part.partInfo.cost * initialMassMultiplier;
		if (selectedTankSetup >= 0 && selectedTankSetup < _modularTankList.Count)
		{
			dryCost += _modularTankList[selectedTankSetup].tankCost * initialMassMultiplier;
		}
		resourceCost = 0.0;
		maxResourceCost = 0.0;
		if (_partRresourceDefinition0 == null || _partResource0 == null)
		{
			totalCost = dryCost;
			return 0.0;
		}
		bool flag = false;
		if (!ignoreInitialCost && !string.IsNullOrEmpty(initialTankSetup))
		{
			flag = true;
			string[] array = initialTankSetup.Split(';');
			int num = array.Count();
			for (int i = 0; i < num; i++)
			{
				string text = array[i];
				if (!base.part.Resources.Contains(text))
				{
					flag = false;
					break;
				}
			}
		}
		bool flag2 = storedFactorMultiplier < 0.999;
		bool flag3 = storedFactorMultiplier > 1.001;
		double num2 = (double)(decimal)_partRresourceDefinition0.unitCost;
		resourceCost += num2 * _partResource0.amount;
		maxResourceCost += num2 * _partResource0.maxAmount;
		if (_partRresourceDefinition1 == null || _partResource1 == null)
		{
			if (flag)
			{
				totalCost = dryCost - maxResourceCost + resourceCost;
				return 0.0;
			}
			totalCost = dryCost + resourceCost;
			if (flag2 || flag3)
			{
				if (!flag2)
				{
					return dryCost * storedFactorMultiplier * 0.125;
				}
				return (0.0 - dryCost) * storedFactorMultiplier;
			}
			return maxResourceCost;
		}
		double num3 = (double)(decimal)_partRresourceDefinition1.unitCost;
		resourceCost += num3 * _partResource1.amount;
		maxResourceCost += num3 * _partResource1.maxAmount;
		if (_partRresourceDefinition2 == null || _partResource2 == null)
		{
			if (flag)
			{
				totalCost = dryCost - maxResourceCost + resourceCost;
				return 0.0;
			}
			totalCost = dryCost + resourceCost;
			if (flag2 || flag3)
			{
				if (!flag2)
				{
					return dryCost * storedFactorMultiplier * 0.125;
				}
				return (0.0 - dryCost) * storedFactorMultiplier;
			}
			return maxResourceCost;
		}
		double num4 = (double)(decimal)_partRresourceDefinition2.unitCost;
		resourceCost += num4 * _partResource2.amount;
		maxResourceCost += num4 * _partResource2.maxAmount;
		if (_partRresourceDefinition3 == null || _partResource3 == null)
		{
			if (flag)
			{
				totalCost = dryCost - maxResourceCost + resourceCost;
				return 0.0;
			}
			totalCost = dryCost + resourceCost;
			if (flag2 || flag3)
			{
				if (!flag2)
				{
					return dryCost * storedFactorMultiplier * 0.125;
				}
				return (0.0 - dryCost) * storedFactorMultiplier;
			}
			return maxResourceCost;
		}
		double num5 = (double)(decimal)_partRresourceDefinition3.unitCost;
		resourceCost += num5 * _partResource3.amount;
		maxResourceCost += num5 * _partResource3.maxAmount;
		if (flag)
		{
			totalCost = dryCost - maxResourceCost + resourceCost;
			return 0.0;
		}
		totalCost = dryCost + resourceCost;
		if (flag2 || flag3)
		{
			if (!flag2)
			{
				return dryCost * storedFactorMultiplier * 0.125;
			}
			return (0.0 - dryCost) * storedFactorMultiplier;
		}
		return maxResourceCost;
	}

	private void UpdateDryMass()
	{
		if (dryMass == 0.0 || !HighLogic.LoadedSceneIsFlight)
		{
			dryMass = CalculateDryMass();
			UpdateMassRatio();
		}
	}

	private double CalculateDryMass()
	{
		if (selectedTank == null && selectedTankSetup >= 0 && selectedTankSetup < _modularTankList.Count)
		{
			selectedTank = _modularTankList[selectedTankSetup];
		}
		double num = (double)basePartMass * baseMassMultiplier;
		if (selectedTank != null)
		{
			double num2 = selectedTank.resourceMassDivider + selectedTank.resourceMassDividerAddition;
			if (overrideMassWithTankDividers && num2 > 0.0)
			{
				num = selectedTank.FullResourceMass / num2 * initialMassMultiplier;
			}
			else
			{
				num += selectedTank.tankMass * baseMassMultiplier;
				if (baseResourceMassDivider > 0.0)
				{
					num += selectedTank.FullResourceMass / baseResourceMassDivider * initialMassMultiplier;
				}
				if (num2 > 0.0)
				{
					num += selectedTank.FullResourceMass / num2 * initialMassMultiplier;
				}
			}
		}
		if (num <= 0.0)
		{
			num = (double)(decimal)base.part.prefabMass * initialMassMultiplier;
		}
		return num;
	}

	private string FormatMassStr(double amount)
	{
		if (amount >= 1.0)
		{
			return amount.ToString(resourcesFormat) + " t";
		}
		if (amount >= 0.001)
		{
			return (amount * 1000.0).ToString(resourcesFormat) + " kg";
		}
		if (amount >= 1E-06)
		{
			return (amount * 1000000.0).ToString(resourcesFormat) + " g";
		}
		return (amount * 1000000000.0).ToString(resourcesFormat) + " mg";
	}

	private void UpdateGuiResourceMass()
	{
		bool flag = _partRresourceDefinition0 == null || _partResource0 == null;
		bool flag2 = _partRresourceDefinition1 == null || _partResource1 == null;
		bool flag3 = _partRresourceDefinition2 == null || _partResource2 == null;
		bool flag4 = _partRresourceDefinition3 == null || _partResource3 == null;
		if (_massRatioStrField != null)
		{
			_massRatioStrField.guiActive = !flag;
			_massRatioStrField.guiActiveEditor = !flag;
		}
		if (_maxWetDryMassField != null)
		{
			_maxWetDryMassField.guiActive = !flag;
			_maxWetDryMassField.guiActiveEditor = !flag;
		}
		double num = (flag ? 0.0 : ((double)(decimal)_partRresourceDefinition0.density * _partResource0.amount));
		double num2 = (flag2 ? 0.0 : ((double)(decimal)_partRresourceDefinition1.density * _partResource1.amount));
		double num3 = (flag3 ? 0.0 : ((double)(decimal)_partRresourceDefinition2.density * _partResource2.amount));
		double num4 = (flag4 ? 0.0 : ((double)(decimal)_partRresourceDefinition3.density * _partResource3.amount));
		totalMass = dryMass + num + num2 + num3 + num4;
		resourceAmountStr0 = (flag ? string.Empty : FormatMassStr(num));
		resourceAmountStr1 = (flag2 ? string.Empty : FormatMassStr(num2));
		resourceAmountStr2 = (flag3 ? string.Empty : FormatMassStr(num3));
		resourceAmountStr3 = (flag4 ? string.Empty : FormatMassStr(num4));
	}

	private void UpdateMassRatio()
	{
		double num = ((_partRresourceDefinition0 == null || _partResource0 == null) ? 0.0 : ((double)(decimal)_partRresourceDefinition0.density * _partResource0.maxAmount));
		double num2 = ((_partRresourceDefinition1 == null || _partResource1 == null) ? 0.0 : ((double)(decimal)_partRresourceDefinition1.density * _partResource1.maxAmount));
		double num3 = ((_partRresourceDefinition2 == null || _partResource2 == null) ? 0.0 : ((double)(decimal)_partRresourceDefinition2.density * _partResource2.maxAmount));
		double num4 = ((_partRresourceDefinition3 == null || _partResource3 == null) ? 0.0 : ((double)(decimal)_partRresourceDefinition3.density * _partResource3.maxAmount));
		if (displayWetDryMass)
		{
			double num5 = num + num2 + num3 + num4;
			if (num5 > 0.0 && dryMass > 0.0)
			{
				massRatioStr = ToRoundedString(1.0 / (dryMass / num5));
			}
			maxWetDryMass = $"{ToStringWithFixedDigits(dryMass)} t / {ToStringWithFixedDigits(num5)} t";
			crewCapacityStr = $"{base.part.protoModuleCrew.Count} / {base.part.CrewCapacity}";
		}
	}

	private string ToRoundedString(double value)
	{
		double num = Math.Abs(value - Math.Round(value, 0));
		if (num > 0.05)
		{
			return "1 : " + value.ToString("0.0");
		}
		if (num > 0.005)
		{
			return "1 : " + value.ToString("0.00");
		}
		if (num > 0.0005)
		{
			return "1 : " + value.ToString("0.000");
		}
		return "1 : " + value.ToString("0");
	}

	private string ToStringWithFixedDigits(double value)
	{
		if (value >= 1000000.0)
		{
			return value.ToString("0");
		}
		if (value >= 100000.0)
		{
			return value.ToString("0.0");
		}
		if (value >= 10000.0)
		{
			return value.ToString("0.00");
		}
		if (value >= 1000.0)
		{
			return value.ToString("0.000");
		}
		if (value >= 100.0)
		{
			return value.ToString("0.0000");
		}
		if (value >= 10.0)
		{
			return value.ToString("0.00000");
		}
		return value.ToString("0.000000");
	}

	public void Update()
	{
		if (HighLogic.LoadedSceneIsFlight)
		{
			UpdateGuiResourceMass();
			allowedToSwitch = availableInFlight && _numberOfAvailableTanks > 1 && (canSwitchWithFullTanks || ((_partResource0 == null || _partResource0.amount < _partResourceMaxAmountFraction0) && (_partResource1 == null || _partResource1.amount < _partResourceMaxAmountFraction1) && (_partResource2 == null || _partResource2.amount < _partResourceMaxAmountFraction2) && (_partResource3 == null || _partResource3.amount < _partResourceMaxAmountFraction3)));
			_chooseField.guiActive = allowedToSwitch;
			_nextTankSetupEvent.guiActive = hasGUI && allowedToSwitch;
			_previousTankSetupEvent.guiActive = hasGUI && allowedToSwitch;
			return;
		}
		UpdateDryMass();
		UpdateGuiResourceMass();
		UpdateCost();
		configuredAmounts = string.Empty;
		configuredFlowStates = string.Empty;
		foreach (PartResource resource in base.part.Resources)
		{
			configuredAmounts = configuredAmounts + resource.amount + ",";
			configuredFlowStates = configuredFlowStates + resource.flowState + ",";
		}
	}

	private void SetupTankList()
	{
		try
		{
			List<double> list = ParseTools.ParseDoubles(tankMass, () => tankMass);
			List<double> list2 = ParseTools.ParseDoubles(tankCost, () => tankCost);
			List<double> list3 = ParseTools.ParseDoubles(tankResourceMassDivider, () => tankResourceMassDivider);
			List<double> list4 = ParseTools.ParseDoubles(tankResourceMassDividerAddition, () => tankResourceMassDividerAddition);
			List<double> list5 = ParseTools.ParseDoubles(crewCapacity, () => crewCapacity);
			List<double> list6 = ParseTools.ParseDoubles(habitatVolume, () => habitatVolume);
			List<double> list7 = ParseTools.ParseDoubles(habitatSurface, () => habitatSurface);
			List<List<double>> list8 = new List<List<double>>();
			List<List<double>> list9 = new List<List<double>>();
			List<List<double>> list10 = new List<List<double>>();
			List<List<double>> list11 = new List<List<double>>();
			string[] array = resourceAmounts.Split(';');
			string[] array2 = resourceRatios.Split(';');
			string[] array3 = initialResourceAmounts.Split(';');
			boilOffTemp.Split(';');
			string[] array4 = resourceNames.Split(';');
			string[] array5 = tankTechReq.Split(';');
			string[] array6 = resourceGui.Split(';');
			string[] array7 = tankSwitchNames.Split(';');
			if (initialResourceAmounts.Equals(string.Empty) || array3.Length != array.Length)
			{
				array3 = array;
			}
			int num = Math.Max(array.Length, array2.Length);
			for (int i = 0; i < num; i++)
			{
				list8.Add(new List<double>());
				list9.Add(new List<double>());
				list10.Add(new List<double>());
				list11.Add(new List<double>());
				string[] array8 = array[i].Trim().Split(',');
				string[] array9 = array3[i].Trim().Split(',');
				if (initialResourceAmounts.Equals(string.Empty) || array9.Length != array8.Length)
				{
					array9 = array8;
				}
				for (int j = 0; j < array8.Length; j++)
				{
					try
					{
						if (i >= list8.Count || j >= array8.Count())
						{
							continue;
						}
						list8[i].Add(double.Parse(array8[j].Trim()));
						goto IL_044a;
					}
					catch (Exception ex)
					{
						Debug.LogWarning("[IFS]: " + base.part.name + " error parsing resourceTankAmountArray amount " + i + "/" + j + ": '" + array[i] + "': '" + array8[j].Trim() + "' with error: " + ex.Message);
						goto IL_044a;
					}
					IL_044a:
					try
					{
						if (i < list9.Count && j < array9.Count())
						{
							list9[i].Add(ParseTools.ParseDouble(array9[j]));
						}
					}
					catch (Exception ex2)
					{
						Debug.LogWarning(string.Concat("[IFS]: ", base.part.name, " error parsing initialResourceList amount ", i, "/", j, ": '", list9[i], "': '", array9[j].Trim(), "' with error: ", ex2.Message));
					}
				}
			}
			for (int k = 0; k < array4.Length; k++)
			{
				IFSmodularTank iFSmodularTank = new IFSmodularTank();
				_modularTankList.Add(iFSmodularTank);
				if (k < array7.Length)
				{
					iFSmodularTank.SwitchName = Localizer.Format(array7[k]);
				}
				if (k < array6.Length)
				{
					iFSmodularTank.GuiName = Localizer.Format(array6[k]);
				}
				if (k < list5.Count)
				{
					iFSmodularTank.crewCapacity = (int)list5[k];
				}
				if (k < list6.Count)
				{
					iFSmodularTank.habitatVolume = list6[k];
				}
				if (k < list7.Count)
				{
					iFSmodularTank.habitatSurface = list7[k];
				}
				if (k != 0 && k < array5.Length)
				{
					iFSmodularTank.techReq = array5[k].Trim(' ');
				}
				if (k < list.Count)
				{
					iFSmodularTank.tankMass = list[k];
				}
				if (k < list3.Count)
				{
					iFSmodularTank.resourceMassDivider = list3[k];
				}
				if (k < list4.Count)
				{
					iFSmodularTank.resourceMassDividerAddition = list4[k];
				}
				if (k < list2.Count)
				{
					iFSmodularTank.tankCost = list2[k];
				}
				string[] array10 = array4[k].Split(',');
				iFSmodularTank.Composition = string.Join("+", array10.Select((string r) => r).ToArray());
				for (int l = 0; l < array10.Length; l++)
				{
					string item = array10[l].Trim(' ');
					IFSresource iFSresource = new IFSresource(item);
					if (!activeResourceList.Contains(item))
					{
						activeResourceList.Add(item);
					}
					if (list8[k] != null && l < list8[k].Count)
					{
						iFSresource.maxAmount = list8[k][l];
						iFSresource.amount = list9[k][l];
					}
					iFSmodularTank.Resources.Add(iFSresource);
				}
				string[] array11 = bannedResourceNames.Split(';');
				string[] array12 = array11;
				foreach (string item2 in array12)
				{
					if (!activeResourceList.Contains(item2))
					{
						activeResourceList.Add(item2);
					}
				}
				if (string.IsNullOrEmpty(iFSmodularTank.GuiName))
				{
					Debug.Log("[IFS]: " + base.part.name + " modularTank.GuiName is null");
					IEnumerable<string> enumerable = iFSmodularTank.Resources.Select((IFSresource m) => m.name);
					iFSmodularTank.GuiName = string.Empty;
					foreach (string item3 in enumerable)
					{
						if (!string.IsNullOrEmpty(iFSmodularTank.GuiName))
						{
							iFSmodularTank.GuiName += "+";
						}
						iFSmodularTank.GuiName += item3;
					}
				}
				if (string.IsNullOrEmpty(iFSmodularTank.SwitchName))
				{
					iFSmodularTank.SwitchName = iFSmodularTank.GuiName;
				}
			}
			if (orderBySwitchName)
			{
				_modularTankList = _modularTankList.OrderBy((IFSmodularTank m) => m.SwitchName).ToList();
			}
			if (!debugMode)
			{
				return;
			}
			foreach (IFSmodularTank modularTank in _modularTankList)
			{
				string text = "[IFS]: " + base.part.name + " Composition :" + modularTank.Composition + " GuiName:" + modularTank.GuiName + " SwitchName: " + modularTank.SwitchName + " Resources: ";
				text += string.Join(",", modularTank.Resources.Select((IFSresource m) => m.name).ToArray());
				Debug.Log(text);
			}
		}
		catch (Exception ex3)
		{
			Debug.LogError("[IFS]: SetupTankList Error: " + ex3.Message);
			throw;
		}
	}

	public float GetModuleCost(float defaultCost, ModifierStagingSituation sit)
	{
		moduleCost = (updateModuleCost ? ((float)UpdateCost()) : 0f);
		return moduleCost;
	}

	public ModifierChangeWhen GetModuleCostChangeWhen()
	{
		return ModifierChangeWhen.STAGED;
	}

	public ModifierChangeWhen GetModuleMassChangeWhen()
	{
		if (!HighLogic.LoadedSceneIsFlight)
		{
			return ModifierChangeWhen.CONSTANTLY;
		}
		return ModifierChangeWhen.STAGED;
	}

	public float GetModuleMass(float defaultMass, ModifierStagingSituation sit)
	{
		this.defaultMass = defaultMass;
		if (returnDryMass)
		{
			return (float)dryMass;
		}
		UpdateDryMass();
		moduleMassDelta = dryMass - initialMass;
		return (float)moduleMassDelta;
	}

	public override string GetInfo()
	{
		if (!showInfo)
		{
			return string.Empty;
		}
		StringBuilder info = new StringBuilder();
		if (!string.IsNullOrEmpty(moduleInfoTemplate))
		{
			List<string> list = new List<string>();
			if (!string.IsNullOrEmpty(moduleInfoParams))
			{
				list = moduleInfoParams.Split(';').ToList();
				for (int i = 0; i < list.Count; i++)
				{
					list[i] = Localizer.Format(list[i]);
				}
			}
			List<string> list2 = moduleInfoTemplate.Split(new string[1] { "<br/>" }, StringSplitOptions.None).ToList();
			string[] parameterArray = list.ToArray();
			list2.ForEach(delegate(string line)
			{
				info.AppendLine(Localizer.Format(line, parameterArray));
			});
			return info.ToString();
		}
		info.AppendLine(Localizer.Format("#LOC_IFS_FuelSwitch_GetInfo") + ":");
		info.Append("<size=10>");
		info.AppendLine();
		foreach (IFSmodularTank modularTank in _modularTankList)
		{
			bool flag = modularTank.Resources.Count > 1;
			if (flag)
			{
				info.Append("<color=#00ff00ff>");
				info.Append(modularTank.SwitchName);
				info.Append("</color>");
				info.AppendLine();
			}
			foreach (IFSresource resource in modularTank.Resources)
			{
				if (flag)
				{
					info.Append("* ");
				}
				info.Append(Math.Round(resource.maxAmount, 0));
				info.Append(" ");
				info.Append("<color=#00ffffff>");
				info.Append(resource.name);
				info.Append("</color>");
				info.AppendLine();
			}
		}
		info.Append("</size>");
		return info.ToString();
	}

	private static bool HasTech(string techid)
	{
		try
		{
			if (string.IsNullOrEmpty(techid))
			{
				return true;
			}
			if (!HighLogic.LoadedSceneIsEditor && !HighLogic.LoadedSceneIsFlight)
			{
				return true;
			}
			if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER && HighLogic.CurrentGame.Mode != Game.Modes.SCIENCE_SANDBOX)
			{
				return true;
			}
			if (ResearchAndDevelopment.Instance == null)
			{
				if (_researchedTechs == null)
				{
					LoadSaveFile();
				}
				return _researchedTechs != null && _researchedTechs.Contains(techid);
			}
			ProtoTechNode techState = ResearchAndDevelopment.Instance.GetTechState(techid);
			if (techState != null)
			{
				return techState.state == RDTech.State.Available;
			}
			return false;
		}
		catch
		{
			Debug.LogError("[IFS] - Verify HasTech: " + techid);
			throw;
		}
	}

	private static void LoadSaveFile()
	{
		_researchedTechs = new HashSet<string>();
		string fileFullName = KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs";
		ConfigNode configNode = ConfigNode.Load(fileFullName);
		ConfigNode node = configNode.GetNode("GAME");
		ConfigNode[] nodes = node.GetNodes("SCENARIO");
		ConfigNode[] array = nodes;
		foreach (ConfigNode configNode2 in array)
		{
			if (!(configNode2.GetValue("name") != "ResearchAndDevelopment"))
			{
				ConfigNode[] nodes2 = configNode2.GetNodes("Tech");
				ConfigNode[] array2 = nodes2;
				foreach (ConfigNode configNode3 in array2)
				{
					string value = configNode3.GetValue("id");
					_researchedTechs.Add(value);
				}
			}
		}
	}

	public void OnGUI()
	{
		if (base.vessel == FlightGlobals.ActiveVessel && render_window)
		{
			windowPosition = GUILayout.Window(_windowID, windowPosition, Window, base.part.partInfo.title);
		}
	}

	private void Window(int windowID)
	{
		try
		{
			windowPositionX = windowPosition.x;
			windowPositionY = windowPosition.y;
			if (GUI.Button(new Rect(windowPosition.width - 20f, 2f, 18f, 18f), "x"))
			{
				closeAterSwitch = false;
				render_window = false;
			}
			GUILayout.BeginVertical();
			foreach (IFSmodularTank modularTank in _modularTankList)
			{
				if (!modularTank.hasTech)
				{
					continue;
				}
				GUILayout.BeginHorizontal();
				if (GUILayout.Button(modularTank.GuiName, GUILayout.ExpandWidth(expand: true)))
				{
					selectedTankSetup = _modularTankList.IndexOf(modularTank);
					AssignResourcesToPart(calledByPlayer: true, affectSymCounterparts: true);
					if (_fuelTankSetupControl != null)
					{
						_fuelTankSetupControl.SwitchToFuelTankSetup(modularTank.SwitchName);
					}
					if (closeAterSwitch)
					{
						closeAterSwitch = false;
						render_window = false;
					}
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndVertical();
			GUI.DragWindow();
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: InterstellarFuelSwitch Window(" + windowID + "): " + ex.Message);
			throw;
		}
	}

	private void InitializeKerbalismHabitat()
	{
		foreach (PartModule module in base.part.Modules)
		{
			if (module.moduleName == "Habitat")
			{
				habitatModule = module;
				habitatVolumeField = module.Fields["volume"];
				habitatSurfaceField = module.Fields["surface"];
				habitatStateField = module.Fields["state"];
				habitatToggleField = module.Fields["toggle"];
				habitatOnStartMethod = module.GetType().GetMethod("OnStart");
				volumeField = habitatModule.Fields["Volume"];
				surfaceField = habitatModule.Fields["Surface"];
				if (habitatOnStartMethod != null)
				{
					Debug.Log("[IFS]: Found onStartMethod");
				}
				break;
			}
		}
	}

	private void UpdateHabitat(IFSmodularTank currentTank)
	{
		if (currentTank == null)
		{
			return;
		}
		if (controlCrewCapacity)
		{
			int num = (int)Math.Round((double)selectedTank.crewCapacity * storedSurfaceMultiplier);
			Debug.Log("[IFS]: SetupTankInPart Set CrewCapacity : " + num);
			base.part.CrewCapacity = num;
			base.part.crewTransferAvailable = num > 0;
		}
		double num2 = currentTank.habitatVolume;
		double num3 = currentTank.habitatSurface;
		try
		{
			if (!(habitatModule == null))
			{
				if (habitatVolumeField != null)
				{
					habitatVolumeField.SetValue(num2 * storedVolumeMultiplier, habitatModule);
				}
				if (habitatSurfaceField != null)
				{
					habitatSurfaceField.SetValue(num3 * storedSurfaceMultiplier, habitatModule);
				}
				if (habitatToggleField != null)
				{
					habitatToggleField.SetValue(num2 > 0.0, habitatModule);
				}
				if (habitatStateField != null)
				{
					State state = ((num2 > 0.0) ? State.enabled : State.disabled);
					habitatStateField.SetValue((int)state, habitatModule);
				}
				PartResource partResource = base.part.Resources["Atmosphere"];
				if (partResource != null)
				{
					partResource.amount = num2 * 1000.0 * storedVolumeMultiplier;
					partResource.maxAmount = num2 * 1000.0 * storedVolumeMultiplier;
				}
				PartResource partResource2 = base.part.Resources["WasteAtmosphere"];
				if (partResource2 != null)
				{
					partResource2.amount = 0.0;
					partResource2.maxAmount = num2 * 1000.0 * storedVolumeMultiplier;
				}
				PartResource partResource3 = base.part.Resources["MoistAtmosphere"];
				if (partResource3 != null)
				{
					partResource3.amount = 0.0;
					partResource3.maxAmount = num2 * 1000.0 * storedVolumeMultiplier;
				}
				if (habitatOnStartMethod != null)
				{
					habitatOnStartMethod.Invoke(habitatModule, new object[1] { 0 });
				}
				if (volumeField != null)
				{
					volumeField.guiActive = num2 > 0.0;
					volumeField.guiActiveEditor = num2 > 0.0;
				}
				if (surfaceField != null)
				{
					surfaceField.guiActive = num3 > 0.0;
					surfaceField.guiActiveEditor = num3 > 0.0;
				}
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[IFS]: UpdateKerbalismHabitat " + ex.Message);
		}
	}
}
