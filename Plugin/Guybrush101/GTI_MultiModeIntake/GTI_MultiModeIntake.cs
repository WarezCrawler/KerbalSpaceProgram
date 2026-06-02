using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GTI.GTIConfig;
//using GTI.GenericFunctions;
using static GTI.Utilities;
//using static GTI.GenericFunctions.PhysicsUtilities;

namespace GTI
{
    public class IntakeModes : IMultiMode
    {

        public int moduleIndex { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        //public bool modeDisabled = false;
        public string resourceName;


        public override string ToString()
        {
            return base.ToString() + "\t" + resourceName;
        }
    }
    class GTI_MultiModeIntake : GTI_MultiMode<IntakeModes>
    {
        /* Example on an Intake Module
        name = ModuleResourceIntake
        resourceName = IntakeAir

        checkForOxygen = true
        area = 0.0031
        intakeSpeed = 15
        intakeTransformName = Intake
        machCurve
        {
            key = 1 1 0 0
            key = 1.5 0.9 -0.4312553 -0.4312553
            key = 2.5 0.45 -0.5275364 -0.5275364
            key = 3.5 0.1 0 0
        }
        */

        [KSPField]
        public string GUINames = string.Empty;

        [KSPField(isPersistant = true)]
        public bool selectedModeStatus = true;

        [KSPField]
        public string resMaxAmount = string.Empty;

        [KSPField]
        public bool preserveResourceNodes = false;

        #region Auto-manage (auto open/close based on airflow)
        // Master arm switch. Persistent so it survives save/load; the cfg value is the initial default.
        // Closing reads the live ModuleResourceIntake.airFlow; reopening uses a self-computed "potential"
        // airflow (CalcPotentialAirflow) because the live field freezes the moment the intake is closed.
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Auto-manage Intake (GTI)")]
        [UI_Toggle(scene = UI_Scene.All, enabledText = "On", disabledText = "Off", affectSymCounterparts = UI_Scene.All)]
        public bool autoManage = false;

        // Close when the live airFlow (units/s) stays at/below this. Default ~0 ties closing to the part's
        // own kPaThreshold cutoff (KSP forces airFlow to 0 below it) - universal, no per-part tuning needed.
        // Raise it in a part cfg to close earlier in thin-but-present air to cut intake drag.
        [KSPField]
        public float autoCloseThreshold = 0.001f;

        // Thin-atmosphere guard: only auto-close once static pressure drops below this (kPa). This scopes
        // auto-close to the intended case - leaving the usable atmosphere on the way to/from space - and
        // prevents a false close when stationary in dense air (e.g. an intakeSpeed = 0 intake parked at sea
        // level, where airFlow is purely motion-driven). Negative (default) means "use the active intake's
        // own kPaThreshold", i.e. the pressure below which KSP itself produces no air.
        [KSPField]
        public float autoCloseMaxPressure = -1f;

        // Reopen when the computed potential airflow (units/s) rises to/above this. Must be >= the close
        // threshold to give hysteresis so the intake cannot flap open/closed at the boundary.
        [KSPField]
        public float autoOpenThreshold = 0.005f;

        // Seconds each condition must persist before acting (debounce against momentary dips/spikes,
        // e.g. a brief pitch that drops the velocity dot product to zero).
        [KSPField]
        public float autoCloseDelay = 2.0f;
        [KSPField]
        public float autoOpenDelay = 2.0f;

        // Main-thread evaluation cadence (seconds). Low cadence = low cost; this is NOT run every frame.
        [KSPField]
        public float autoCheckInterval = 0.25f;

        // Grace period (seconds) after a manual open/close before auto-manage may act again, so a manual
        // override is not immediately reverted. Auto stays armed - it just waits this long before resuming.
        [KSPField]
        public float autoManualCooldown = 10f;

        // Read-only status readout for the right-click menu.
        [KSPField(guiActive = true, guiActiveEditor = false, guiName = "Intake auto")]
        public string autoStatusGUI = "Idle";

        private bool _autoLoopRunning = false;
        private float _belowCloseTime = 0f;
        private float _aboveOpenTime = 0f;
        private float _manualCooldown = 0f;   // counts down in the auto loop after a manual open/close
        #endregion

        public List<ModuleResourceIntake> ModuleIntakes;

        protected override void initializeSettings()
        {
            GTIDebug.Log("GTI_MultiModeIntake --> initializeSettings()", iDebugLevel.DebugInfo);

            // NOTE: part.GetPartModuleConfig()/GetPartModuleConfigs() return null during part COMPILATION
            // (GetInfo() runs that early). These calls are debug-logging only - nothing here is functional -
            // so guard the null case; dereferencing it would NRE and stall the part loader.
            ConfigNode thisModuleConfig = part.GetPartModuleConfig("MODULE", "name", "GTI_MultiModeIntake");
            if (thisModuleConfig != null) GTIDebug.Log(thisModuleConfig.ToString(), iDebugLevel.DebugInfo);

            ConfigNode[] ResourceNodes = part.GetPartModuleConfigs("RESOURCE");
            if (ResourceNodes != null) GTIDebug.Log(ResourceNodes.ToStringExt(), iDebugLevel.DebugInfo);

            //Find resourceIntake modules
            ModuleIntakes = part.FindModulesImplementing<ModuleResourceIntake>();

            modes = new List<IntakeModes>(ModuleIntakes.Count);
            GTIDebug.Log(this.GetType().Name + " --> ModuleIntakes.Count; " + ModuleIntakes.Count, iDebugLevel.DebugInfo);
            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                GTIDebug.Log("for (int i = 0; i < ModuleIntakes.Count; i++): " + i, iDebugLevel.DebugInfo);
                modes.Add(new IntakeModes()
                {
                    moduleIndex = i,
                    ID = i.ToString(),
                    Name = ModuleIntakes[i].resourceName,
                    resourceName = ModuleIntakes[i].resourceName
                });
                GTIDebug.Log("modes[" + i + "].moduleIndex --> " + modes[i].moduleIndex, iDebugLevel.DebugInfo);
                GTIDebug.Log("modes[" + i + "].ID --> " + modes[i].ID, iDebugLevel.DebugInfo);
                GTIDebug.Log("modes[" + i + "].Name --> " + modes[i].Name, iDebugLevel.DebugInfo);
            }

            //Disable Events, as these should be handled by the multimode module
            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                ModuleIntakes[i].Events["Deactivate"].guiActive = false;
                ModuleIntakes[i].Events["Deactivate"].guiActiveEditor = false;
                ModuleIntakes[i].Events["Activate"].guiActive = false;
                ModuleIntakes[i].Events["Activate"].guiActiveEditor = false;

                ModuleIntakes[i].Actions["ToggleAction"].active = false;
            }
            
            //force no use af animation groups (it's not imlpemented, I don't believe it's relevant)
            useModuleAnimationGroup = false;
        }
        

        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log(this.GetType().Name + " --> GTI_MultiModeIntake: updateMultiMode() --> Begin", iDebugLevel.High);
            Part currentPart = this.part;

            if (silentUpdate == false) writeScreenMessage();

            for (int i = 0; i < ModuleIntakes.Count; i++)
            {
                if (i == modes[selectedMode].moduleIndex)
                {
                    GTIDebug.Log("GTI_MultiMode (" + (silentUpdate ? "silent" : "non-silent") + "): Activate Module [" + modes[i].moduleIndex + "] --> " + ModuleIntakes[modes[i].moduleIndex].resourceName, iDebugLevel.High);
                    GTIDebug.Log("selectedMode Intake: " + selectedMode + "\tselectedModeStatus: " + selectedModeStatus);
                    if (selectedModeStatus)
                    {
                        GTIDebug.Log("Activating Intake Mode: " + modes[selectedMode].resourceName);
                        ModuleIntakes[i].intakeEnabled = true;
                    }
                    else
                    {
                        ModuleIntakes[i].intakeEnabled = false;
                    }
                    ModuleIntakes[i].enabled = true;
                    ModuleIntakes[i].isEnabled = true;
                }
                else
                {
                    GTIDebug.Log("GTI_MultiMode (" + (silentUpdate ? "silent" : "non-silent") + "): Deactivate Module [" + modes[i].moduleIndex + "] --> " + ModuleIntakes[modes[i].moduleIndex].resourceName, iDebugLevel.High);
                    ModuleIntakes[i].enabled = false;
                    ModuleIntakes[i].isEnabled = false;         //=> FixedUpdate() will update status = "Closed" and exit
                    ModuleIntakes[i].intakeEnabled = false;
                }
            }
            this.Events["IntakeActivate"].active = !selectedModeStatus;
            this.Events["IntakeDeactivate"].active = selectedModeStatus;

            #region Create new Resource node
            //Only handle resources if preservation is not activated
            if (!preserveResourceNodes)
            {
                //List<ConfigNode> IntakeResources = new List<ConfigNode>();
                ConfigNode IntakeResource = new ConfigNode("RESOURCE");
                float resMaxAmount = (float)ModuleIntakes[modes[selectedMode].moduleIndex].res.maxAmount;
                if (resMaxAmount <= 0) resMaxAmount = 1f;
                float resIniAmount = HighLogic.LoadedSceneIsFlight ? 0f : resMaxAmount;

                //Create Resource node
                IntakeResource.AddValue("name", modes[selectedMode].resourceName);
                IntakeResource.AddValue("amount", resIniAmount);
                IntakeResource.AddValue("maxAmount", resMaxAmount);

                //Clear all resources since I get null ref error when I do not do this
                //currentPart.Resources.Clear();
                bool preserveResource;
                // remove all target resources
                List<PartResource> resourcesDeleteList = new List<PartResource>();
                foreach (PartResource resource in currentPart.Resources)
                {
                    preserveResource = true;
                    GTIDebug.Log("Check for resource removal: " + resource.resourceName, iDebugLevel.DebugInfo);
                    for (int j = 0; j < modes.Count; j++)
                    {
                        if (modes[j].resourceName == resource.resourceName)
                        {
                            //Remove resources managed by this mod
                            preserveResource = false;
                            break;
                        }
                    }
                    if (!preserveResource)
                    {
                        GTIDebug.Log("Removing Resource: " + resource.resourceName, iDebugLevel.DebugInfo);
                        resourcesDeleteList.Add(resource);
                    }
                }
                foreach (var resource in resourcesDeleteList)
                {
                    if (currentPart.Resources.Remove(resource)) GTIDebug.Log("Resource removed: " + GetResourceID(resource.resourceName), iDebugLevel.DebugInfo);
                }
                resourcesDeleteList = null;

                //Add the resources
                GTIDebug.Log("MultiModeIntake: Add Resource\n" + IntakeResource.ToString(), iDebugLevel.DebugInfo);
                currentPart.AddResource(IntakeResource);

                GTIDebug.Log("Listing resources defined in part", iDebugLevel.DebugInfo);
                if(DebugLevel == iDebugLevel.DebugInfo)
                {
                    for (int i = 0; i < currentPart.Resources.Count; i++)
                    {
                        GTIDebug.Log("currentPart.Resources[" + i + "].resourceName: " + currentPart.Resources[i].resourceName + " \t" + currentPart.Resources[i].maxAmount, iDebugLevel.DebugInfo);
                    }
                }
            }
            #endregion

            try
            { if (HighLogic.LoadedSceneIsFlight && !silentUpdate) { KSP.UI.Screens.ResourceDisplay.Instance.Refresh(); } }
            catch { GTIDebug.LogError("Update resource panel failed."); }
        }
        
        protected override void writeScreenMessage()
        {
            writeScreenMessage(
                Message: "Intake mode: " + modes[selectedMode].Name,
                messagePosition: messagePosition,
                duration: 3f
                );
        }

        protected override void ModuleAnimationGroupEvent_DisableModules() { throw new NotImplementedException(); }

        public List<GTI_MultiModeIntake> GetCounterPartModules(Part thispart)
        {
            List<Part> CounterParts = thispart.symmetryCounterparts;
            List<GTI_MultiModeIntake> modules = new List<GTI_MultiModeIntake>(CounterParts.Count);

            foreach (Part part in CounterParts)
            {
                modules.Add(part.FindModuleImplementing<GTI_MultiModeIntake>());
            }

            return modules;
        }

        [KSPEvent(name = "IntakeActivate", guiName = "Open Intake (GTI)", active = true, externalToEVAOnly = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActive = true, guiActiveEditor = true)]
        public void IntakeActivate()
        {
            // Manual interaction: hold auto-manage off for the cooldown so it does not instantly revert this.
            BeginManualCooldown(includeCounterparts: true);
            SetIntakeState(true, syncCounterparts: affectSymCounterpartsInFlight);
        }
        [KSPEvent(name = "IntakeDeactivate", guiName = "Close Intake (GTI)", active = true, externalToEVAOnly = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActive = true, guiActiveEditor = true)]
        public void IntakeDeactivate()
        {
            // Manual interaction: hold auto-manage off for the cooldown so it does not instantly revert this.
            BeginManualCooldown(includeCounterparts: true);
            SetIntakeState(false, syncCounterparts: affectSymCounterpartsInFlight);
        }

        /// <summary>
        /// Applies the open/closed state to this module (and optionally its symmetry counterparts).
        /// Shared by the manual events and the auto-manage loop - the difference is that the manual
        /// path starts the cooldown first and the auto path does not (and does not sync counterparts,
        /// since each counterpart runs its own loop and evaluates its own state independently).
        /// </summary>
        private void SetIntakeState(bool open, bool syncCounterparts)
        {
            selectedModeStatus = open;

            this.Events["IntakeActivate"].active = !open;
            this.Events["IntakeDeactivate"].active = open;

            InvokeOnUpdateMultiMode(true);

            if (syncCounterparts)
            {
                List<GTI_MultiModeIntake> CounterModules = GetCounterPartModules(this.part);
                foreach (GTI_MultiModeIntake module in CounterModules)
                {
                    module.selectedModeStatus = open;

                    module.Events["IntakeActivate"].active = !open;
                    module.Events["IntakeDeactivate"].active = open;

                    module.InvokeOnUpdateMultiMode(true);
                }
            }
        }

        /// <summary>
        /// Starts the post-manual grace period on this module (and counterparts that were synced with it),
        /// so the auto-manage loop waits autoManualCooldown seconds before it can act again.
        /// </summary>
        private void BeginManualCooldown(bool includeCounterparts)
        {
            _manualCooldown = autoManualCooldown;
            if (includeCounterparts && affectSymCounterpartsInFlight)
            {
                foreach (GTI_MultiModeIntake module in GetCounterPartModules(this.part))
                {
                    module._manualCooldown = module.autoManualCooldown;
                }
            }
        }

        #region Auto-manage loop
        [KSPAction("Toggle Auto-manage Intake")]
        public void ToggleAutoManageAction(KSPActionParam param) { autoManage = !autoManage; }

        public override void OnStartFinished(StartState state)
        {
            base.OnStartFinished(state);
            if (HighLogic.LoadedSceneIsFlight && !_autoLoopRunning)
            {
                StartCoroutine(AutoManageLoop());
            }
        }

        /// <summary>
        /// Low-cadence main-thread evaluator. Reuses the "wake, coalesce, act" shape of GTI_Events'
        /// ConsumeThrottleChanges coroutine - no worker thread, so all KSP/Unity reads are safe.
        /// </summary>
        private IEnumerator AutoManageLoop()
        {
            _autoLoopRunning = true;
            float interval = autoCheckInterval > 0f ? autoCheckInterval : 0.25f;
            WaitForSeconds wait = new WaitForSeconds(interval);
            GTIDebug.Log(this.GetType().Name + " --> AutoManageLoop started (interval " + interval + "s)", iDebugLevel.DebugInfo);

            while (HighLogic.LoadedSceneIsFlight)
            {
                yield return wait;

                if (!autoManage)
                {
                    _belowCloseTime = 0f;
                    _aboveOpenTime = 0f;
                    autoStatusGUI = "Off";
                    continue;
                }
                EvaluateAutoManage(interval);
            }
            _autoLoopRunning = false;
        }

        private void EvaluateAutoManage(float dt)
        {
            if (modes == null || ModuleIntakes == null || ModuleIntakes.Count == 0 || vessel == null) return;

            ModuleResourceIntake I = ModuleIntakes[modes[selectedMode].moduleIndex];
            if (I == null) return;

            // Hold off after a manual open/close so we don't immediately revert the player's choice.
            if (_manualCooldown > 0f)
            {
                _manualCooldown -= dt;
                _belowCloseTime = 0f;
                _aboveOpenTime = 0f;
                autoStatusGUI = "Manual hold " + Mathf.Ceil(_manualCooldown).ToString("F0") + "s";
                return;
            }

            if (selectedModeStatus)
            {
                // Currently OPEN -> close only when BOTH the live airFlow is at/below threshold AND the
                // atmosphere is thin (leaving usable air on the way to/from space). The pressure guard
                // stops a false close when stationary in dense air (e.g. an intakeSpeed = 0 intake).
                _aboveOpenTime = 0f;
                float closePressure = autoCloseMaxPressure >= 0f ? autoCloseMaxPressure : (float)I.kPaThreshold;
                bool lowAir = I.airFlow <= autoCloseThreshold;
                bool thinAtmosphere = vessel.staticPressurekPa < closePressure;
                if (lowAir && thinAtmosphere)
                {
                    _belowCloseTime += dt;
                    autoStatusGUI = "Open - thin air " + I.airFlow.ToString("F3");
                    if (_belowCloseTime >= autoCloseDelay)
                    {
                        GTIDebug.Log(this.GetType().Name + " auto-closing intake '" + modes[selectedMode].Name + "' (airFlow " + I.airFlow + ", " + vessel.staticPressurekPa + " kPa)", iDebugLevel.Low);
                        SetIntakeState(false, syncCounterparts: false);
                        _belowCloseTime = 0f;
                        autoStatusGUI = "Auto-closed";
                        writeScreenMessage("Intake auto-closed: " + modes[selectedMode].Name, messagePosition: messagePosition, duration: 3f);
                    }
                }
                else
                {
                    _belowCloseTime = 0f;
                    autoStatusGUI = "Open";
                }
            }
            else
            {
                // Currently CLOSED -> reopen on the computed potential airflow rising above threshold (sustained).
                _belowCloseTime = 0f;
                double potential = CalcPotentialAirflow(I);
                if (potential >= autoOpenThreshold)
                {
                    _aboveOpenTime += dt;
                    autoStatusGUI = "Closed - air " + potential.ToString("F3");
                    if (_aboveOpenTime >= autoOpenDelay)
                    {
                        GTIDebug.Log(this.GetType().Name + " auto-opening intake '" + modes[selectedMode].Name + "' (potential " + potential + ")", iDebugLevel.Low);
                        SetIntakeState(true, syncCounterparts: false);
                        _aboveOpenTime = 0f;
                        autoStatusGUI = "Auto-opened";
                        writeScreenMessage("Intake auto-opened: " + modes[selectedMode].Name, messagePosition: messagePosition, duration: 3f);
                    }
                }
                else
                {
                    _aboveOpenTime = 0f;
                    autoStatusGUI = "Closed - no air";
                }
            }
        }

        /// <summary>
        /// Mirrors ModuleResourceIntake.FixedUpdate's airflow calculation so we get a live value even while
        /// the intake is closed (the stock airFlow field is frozen then). Must run on the main thread -
        /// it reads the part Transform and FloatCurve, which are not thread-safe.
        /// </summary>
        private double CalcPotentialAirflow(ModuleResourceIntake I)
        {
            if (I == null || vessel == null || I.intakeTransform == null) return 0.0;
            if (part.ShieldedFromAirstream) return 0.0;
            if (I.checkNode && I.node != null && I.node.attachedPart != null) return 0.0;

            CelestialBody body = vessel.mainBody;
            if (I.checkForOxygen && !body.atmosphereContainsOxygen) return 0.0;
            if (vessel.staticPressurekPa < I.kPaThreshold) return 0.0;

            // Underwater gating - identical to the stock module's compound condition.
            bool envOK;
            if (!I.disableUnderwater && !I.underwaterOnly) envOK = true;
            else if (I.disableUnderwater) envOK = !body.ocean || FlightGlobals.getAltitudeAtPos((Vector3d)I.intakeTransform.position, body) >= 0.0;
            else envOK = body.ocean && FlightGlobals.getAltitudeAtPos((Vector3d)I.intakeTransform.position, body) < 0.0;
            if (!envOK) return 0.0;

            double num = UtilMath.Clamp01(Vector3.Dot(vessel.srf_vel_direction, I.intakeTransform.forward)) * vessel.srfSpeed + I.intakeSpeed;
            num *= I.unitScalar * I.area * (double)I.machCurve.Evaluate((float)vessel.mach);
            double density = I.underwaterOnly ? body.oceanDensity : vessel.atmDensity;
            double flow = num * density * I.densityRecip;
            return flow > 0.0 ? flow : 0.0;
        }
        #endregion

        [KSPAction("Toggle Intake")]
        public void ToggleAction(KSPActionParam param)
        {
            if (selectedModeStatus)
            {
                IntakeDeactivate();
            }
            else
            {
                IntakeActivate();
            }
        }

        [KSPEvent(name = "GetStatus", guiName = "GetStatus (GTI)", active = false, externalToEVAOnly = false, guiActiveUnfocused = false, unfocusedRange = 5f, guiActive = true, guiActiveEditor = true)]
        public void GetStatus()
        {
            GTIDebug.Log("\nGetStatus() of all ModuleResourceIntakes", iDebugLevel.DebugInfo);
            foreach (ModuleResourceIntake I in ModuleIntakes)
            {
                GTIDebug.Log("\nresourceName " + I.resourceName, iDebugLevel.DebugInfo);
                GTIDebug.Log("status " + I.status, iDebugLevel.DebugInfo);
                GTIDebug.Log("intakeEnabled " + I.intakeEnabled, iDebugLevel.DebugInfo);
                GTIDebug.Log("isActiveAndEnabled " + I.isActiveAndEnabled, iDebugLevel.DebugInfo);
                GTIDebug.Log("isEnabled " + I.isEnabled, iDebugLevel.DebugInfo);
                GTIDebug.Log("enabled " + I.enabled, iDebugLevel.DebugInfo);
                GTIDebug.Log("airFlow " + I.airFlow, iDebugLevel.DebugInfo);
                GTIDebug.Log("airSpeedGui " + I.airSpeedGui, iDebugLevel.DebugInfo);
                GTIDebug.Log("area " + I.area, iDebugLevel.DebugInfo);
                GTIDebug.Log("checkForOxygen " + I.checkForOxygen, iDebugLevel.DebugInfo);
                GTIDebug.Log("kPaThreshold " + I.kPaThreshold, iDebugLevel.DebugInfo);
                GTIDebug.Log("moduleIsEnabled " + I.moduleIsEnabled, iDebugLevel.DebugInfo);
            }
        }
    }
}
/* NOTES
.Events
GUIName: Close Intake
id: -3298156
name: Deactivate
active: True
assigned: False
category: 
externalToEVAOnly: True
guiActive: True
guiActiveEditor: True
guiActiveUncommand: False
guiActiveUnfocused: False
guiIcon: Close Intake
unfocusedRange: 2

[LOG 14:39:45.346]
[GTI] 
.Events
GUIName: Open Intake
id: -1591330541
name: Activate
active: False
assigned: False
category: 
externalToEVAOnly: True
guiActive: True
guiActiveEditor: True
guiActiveUncommand: False
guiActiveUnfocused: False
guiIcon: Open Intake
unfocusedRange: 2

[LOG 14:39:45.346]
[GTI] 
.Events
GUIName: Disable Staging
id: 1164541479
name: ToggleStaging
active: True
assigned: False
category: 
externalToEVAOnly: True
guiActive: False
guiActiveEditor: False
guiActiveUncommand: False
guiActiveUnfocused: False
guiIcon: Disable Staging
unfocusedRange: 2
*/