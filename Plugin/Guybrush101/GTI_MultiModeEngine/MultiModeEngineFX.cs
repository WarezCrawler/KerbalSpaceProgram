using static GTI.GTIConfig;
using static GTI.Utilities;
using System.Collections.Generic;
using System;
using System.Text;

namespace GTI
{
    /// <summary>
    /// Lets a single part switch between several stock ModuleEnginesFX "engines" (modes).
    /// The part declares the engines through the engineID list; this module shows one mode
    /// selector and keeps exactly one underlying engine active at a time.
    /// </summary>
    public class GTI_MultiModeEngineFX : GTI_MultiMode<MultiMode>
    {
        // Semicolon-separated list of ModuleEnginesFX.engineID values, one per mode (set in the part cfg).
        [KSPField]
        public string engineID = string.Empty;
        // Optional semicolon-separated display names, parallel to engineID. Falls back to engineID when omitted.
        [KSPField]
        public string GUIengineID = string.Empty;

        // Optional semicolon-separated list, parallel to engineID: the mode to auto-switch to when that
        // mode's engine flames out. A mode mapping to itself (or omitted/unmatched) means "no switch" - i.e.
        // a plain flameout, the behaviour when this field is absent. Empty = auto-switch feature off.
        [KSPField]
        public string engineID_onFlameout = string.Empty;

        // Player toggle for auto-switch; only shown when engineID_onFlameout is configured (see initializeGUI).
        [KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "Auto-switch on flameout")]
        [UI_Toggle(enabledText = "On", disabledText = "Off")]
        public bool autoSwitchEnabled = true;

        // True when GUIengineID was not supplied, so the raw engineID is used as the display name.
        private bool GUIengineIDEmpty = true;

        // Per-mode auto-switch target (index into modes), built from engineID_onFlameout.
        // null when the feature is not configured for this part.
        private int[] onFlameoutTarget;

        // The ModuleEnginesFX instances on this part, located once during initialization.
        protected List<ModuleEnginesFX> ModuleEngines;

        // The engine matching the current selection, and whether it was running.
        // currentEngineState lets us carry the on/off state across a mode switch.
        private ModuleEnginesFX currentModuleEngine;
        private bool currentEngineState;

        /// <summary>
        /// Builds the mode list from the cfg and links each mode to its ModuleEnginesFX on the part.
        /// Runs from the base OnStart flow, and again from GetInfo() in the editor; the
        /// _settingsInitialized guard makes sure the work only happens once.
        /// </summary>
        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeEngineFX() --> initializeSettings()", iDebugLevel.DebugInfo);

                // Split the cfg lists into arrays. GUIengineID is optional - GUIengineIDEmpty flags that case.
                string[] arrEngineID = engineID.Trim().Split(';');
                GUIengineIDEmpty = ArraySplitEvaluate(GUIengineID, out string[] arrGUIengineID, ';');

                // Create one mode per declared engineID, naming it from GUIengineID when one was given.
                modes = new List<MultiMode>(arrEngineID.Length);
                GTIDebug.Log("Create list of modes", iDebugLevel.DebugInfo);
                for (int i = 0; i < arrEngineID.Length; i++)
                {
                    modes.Add(new MultiMode()
                    {
                        ID = arrEngineID[i],
                        Name = GUIengineIDEmpty ? arrEngineID[i] : arrGUIengineID[i]
                    });
                }

                // Parse the optional flameout-target list (parallel to engineID). Each entry is resolved to a
                // mode index; an entry that names no engineID, or names its own mode, means "no switch".
                bool flameoutEmpty = ArraySplitEvaluate(engineID_onFlameout, out string[] arrFlameout, ';');
                if (!flameoutEmpty)
                {
                    if (arrFlameout.Length != arrEngineID.Length)
                        throw new Exception("GTI_MultiModeEngineFX: engineID_onFlameout has " + arrFlameout.Length + " entries but engineID has " + arrEngineID.Length + " on part '" + part?.name + "'. The two lists must be the same length.");

                    onFlameoutTarget = new int[modes.Count];
                    for (int i = 0; i < modes.Count; i++)
                    {
                        int targetIndex = i;   // default: map to self => no switch (plain flameout)
                        for (int j = 0; j < modes.Count; j++)
                        {
                            if (modes[j].ID == arrFlameout[i]) { targetIndex = j; break; }
                        }
                        if (targetIndex == i && modes[i].ID != arrFlameout[i])
                            GTIDebug.LogWarning("GTI_MultiModeEngineFX: engineID_onFlameout entry '" + arrFlameout[i] + "' matched no engineID; mode '" + modes[i].ID + "' will not auto-switch.", iDebugLevel.Low);
                        onFlameoutTarget[i] = targetIndex;
                    }
                }

                // Grab the engine modules on the part. Single lookup, reused for the part's lifetime.
                GTIDebug.Log("Find module engines from part", iDebugLevel.DebugInfo);
                ModuleEngines = part.FindModulesImplementing<ModuleEnginesFX>();

                for (int i = 0; i < ModuleEngines.Count; i++)
                {
                    // Hide the stock per-engine actions so this module is the only control point.
                    // Otherwise a player could toggle an underlying engine directly and desync the selection.
                    ModuleEngines[i].Actions["OnAction"].active = false;
                    ModuleEngines[i].Actions["ShutdownAction"].active = false;
                    ModuleEngines[i].Actions["ActivateAction"].active = false;

                    // Map each mode to the index of its matching engine, so a switch knows which engine to drive.
                    GTIDebug.Log(ModuleEngines[i].engineID + " - Collect module engines index's", this.GetType().Name, iDebugLevel.DebugInfo);
                    for (int j = 0; j < modes.Count; j++)
                    {
                        if (ModuleEngines[i].engineID == modes[j].ID)
                        {
                            GTIDebug.Log("Engine index found: " + i, this.GetType().Name, iDebugLevel.DebugInfo);
                            modes[j].moduleIndex = i;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Applies the current selection: activates the chosen engine and shuts down / hides the rest.
        /// </summary>
        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log("GTI_MultiModeEngine: updatePropulsion() --> ChooseOption = " + ChooseOption, iDebugLevel.High);

            // Remember whether the engine we are switching away from was running, so we can match that state below.
            if (currentModuleEngine != null)
            {
                currentEngineState = currentModuleEngine.getIgnitionState;
            } else GTIDebug.Log("updateMultiMode() --> currentModuleEngine is null", iDebugLevel.Low);

            writeScreenMessage();

            // initializeSettings() must have populated this list. A null here means a setup bug, so fail loudly.
            if (ModuleEngines == null)
                throw new Exception("GTI_MultiModeEngineFX.updateMultiMode(): ModuleEngines list is null on part '" + part?.name + "' - initializeSettings() did not run before updateMultiMode().");

            // Walk every engine: enable the selected one, disable all the others.
            foreach (ModuleEnginesFX moduleEngine in ModuleEngines)
            {
                if (moduleEngine.engineID == ChooseOption)
                {
                    GTIDebug.Log("GTI_MultiModeEngine: Set currentModuleEngine " + moduleEngine.engineID, iDebugLevel.High);
                    currentModuleEngine = moduleEngine;

                    // Re-ignite only if the previous engine was running, so a switch doesn't silently kill thrust.
                    if (currentEngineState)
                    {
                        GTIDebug.Log("GTI_MultiModeEngine: Activate() " + moduleEngine.engineID, iDebugLevel.High);
                        moduleEngine.Activate();
                    }
                    // manuallyOverridden=false + isEnabled=true make this the live, visible engine.
                    moduleEngine.manuallyOverridden = false;
                    moduleEngine.isEnabled = true;
                }
                else
                {
                    // Shut down and hide the unselected engines (manuallyOverridden/isEnabled remove them from the UI).
                    GTIDebug.Log("GTI_MultiModeEngine: Shutdown() " + moduleEngine.engineID, iDebugLevel.High);
                    moduleEngine.Shutdown();
                    moduleEngine.manuallyOverridden = true;
                    moduleEngine.isEnabled = false;
                }
            }
        }

        /// <summary>
        /// Auto-switch on flameout. If the active engine flames out, jump to its mapped fallback mode
        /// (engineID_onFlameout) - but only if that target engine can actually start, so we don't flip to
        /// a mode that is also dead. A mode mapped to itself is a no-op (ordinary flameout). The whole
        /// feature is off unless engineID_onFlameout is configured and the player toggle is on.
        /// </summary>
        public override void OnUpdate()
        {
            base.OnUpdate();

            if (onFlameoutTarget == null || !autoSwitchEnabled) return;
            if (!HighLogic.LoadedSceneIsFlight) return;
            if (currentModuleEngine == null || !currentModuleEngine.flameout) return;

            int target = onFlameoutTarget[selectedMode];
            if (target == selectedMode) return;   // maps to itself -> plain flameout, nothing to do

            // Don't switch to a mode that can't run either (e.g. no propellant) - avoids a pointless flip.
            if (!ModuleEngines[modes[target].moduleIndex].CanStart()) return;

            GTIDebug.Log("Auto-switch on flameout: " + modes[selectedMode].ID + " -> " + modes[target].ID, iDebugLevel.Medium);
            AutoSwitchToMode(target);
        }

        /// <summary>
        /// Switches to the given mode in response to a flameout and makes sure the new engine fires
        /// (the old one flamed out under throttle, so the player still wants thrust).
        /// </summary>
        private void AutoSwitchToMode(int target)
        {
            selectedMode = target;
            ChooseOption = modes[target].ID;
            updateMultiMode();

            if (currentModuleEngine != null && !currentModuleEngine.getIgnitionState)
                currentModuleEngine.Activate();
            currentEngineState = currentModuleEngine != null && currentModuleEngine.getIgnitionState;
        }

        /// <summary>
        /// After the base class builds the selector UI, resolve currentModuleEngine from the persisted
        /// selection so the action handlers have a valid engine before the first manual switch.
        /// </summary>
        protected override void initializeGUI()
        {
            GTIDebug.Log("GTI_MultiModeEngine() --> override initializeGUI()", iDebugLevel.High);
            base.initializeGUI();

            // Find the engine whose ID matches the saved ChooseOption value.
            GTIDebug.Log("override initializeGUI() --> Get currently activated engine module", iDebugLevel.High);
            for (int i = 0; i < ModuleEngines.Count; i++)
            {
                if (ModuleEngines[i].engineID == ChooseOption) currentModuleEngine = ModuleEngines[i];
            }

            // Only expose the auto-switch toggle when the feature is actually configured for this part.
            bool autoSwitchConfigured = onFlameoutTarget != null;
            Fields[nameof(autoSwitchEnabled)].guiActive = autoSwitchConfigured;
            Fields[nameof(autoSwitchEnabled)].guiActiveEditor = autoSwitchConfigured;
        }

        /// <summary>
        /// On-screen message shown when the mode changes.
        /// </summary>
        protected override void writeScreenMessage()
        {
            writeScreenMessage(
                Message: "Changing Propulsion to: " + modes[selectedMode].ID,
                messagePosition: ScreenMessageStyle.UPPER_CENTER
                );
        }

        /// <summary>
        /// Fail fast with a descriptive message if no engine is currently selected (indicates a bug or a bad cfg).
        /// </summary>
        private void EnsureCurrentEngine()
        {
            if (currentModuleEngine == null)
                throw new Exception("GTI_MultiModeEngineFX: currentModuleEngine is null on part '" + part?.name + "' - ChooseOption '" + ChooseOption + "' matched no ModuleEnginesFX.");
        }

        // Action-group hook: ignite the selected engine.
        [KSPAction("Activate Engine")]
        public void ActionActivate(KSPActionParam param)
        {
            EnsureCurrentEngine();
            if (!currentModuleEngine.getIgnitionState) { currentModuleEngine.Activate(); }

            currentEngineState = currentModuleEngine.getIgnitionState;
        }
        // Action-group hook: shut down the selected engine.
        [KSPAction("Shutdown Engine")]
        public void ActionShutdown(KSPActionParam param)
        {
            EnsureCurrentEngine();
            if (currentModuleEngine.getIgnitionState) { currentModuleEngine.Shutdown(); }

            currentEngineState = currentModuleEngine.getIgnitionState;
        }
        // Action-group hook: toggle the selected engine on/off.
        [KSPAction("Toggle Engine")]
        public void ActionToggle(KSPActionParam param)
        {
            EnsureCurrentEngine();
            if (currentModuleEngine.getIgnitionState)
            {
                currentModuleEngine.Shutdown();
            }
            else
            {
                currentModuleEngine.Activate();
            }

            currentEngineState = currentModuleEngine.getIgnitionState;
        }

        // Animation-group gating is not supported by this engine module.
        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Editor part-info text. The editor requests this before OnStart runs, so we initialize the
        /// settings here when needed to make sure the mode list is available for the info panel.
        /// </summary>
        public override string GetInfo()
        {
            StringBuilder Info = new StringBuilder();
            GTIDebug.Log("GTI_MultiModeEngineFX GetInfo");
            try
            {
                if (!_settingsInitialized)
                {
                    initializeSettings();
                }

                Info.AppendLine("<color=yellow>Engine Modes Available:</color>");

                // Module-level tech gate (hides the whole selector until researched), if configured.
                string moduleTag = ModuleTechInfo();
                if (moduleTag != string.Empty) Info.AppendLine("<i>" + moduleTag + "</i>");

                for (int i = 0; i < modes.Count; i++)
                {
                    Info.Append(modes[i].Name);
                    // Per-mode tech requirement (blank for always-available modes).
                    string tag = ModeTechInfo(i);
                    if (tag != string.Empty) { Info.AppendLine(); Info.Append("  <i>" + tag + "</i>"); }
                    Info.AppendLine();
                }
                Info.AppendLine("\nIn Flight switching is <color=yellow>" + (availableInFlight ? "available" : "not available") + "</color>");

                return Info.ToString();
            }
            catch (Exception e)
            {
                GTIDebug.LogError("GTI_MultiModeEngineFX GetInfo Error " + e.Message);
                throw;
            }
        }
    }
}
