using static GTI.GTIConfig;
using static GTI.Utilities;
using System.Collections.Generic;
using System;
using System.Text;

namespace GTI
{
    /// <summary>
    /// Lets a single part switch between several stock ModuleRCS thrusters (modes), the RCS
    /// equivalent of GTI_MultiModeEngineFX. Exactly one ModuleRCS is kept enabled at a time; the
    /// rest are fully disabled so they neither thrust nor clutter the right-click menu.
    ///
    /// ModuleRCS has no "engineID" like an engine, so modes are matched by their order on the part:
    /// mode i drives the i-th ModuleRCS. Optional GUIRCSID supplies display names; otherwise the
    /// thruster's resource name is used.
    /// </summary>
    public class GTI_MultiModeRCS : GTI_MultiMode<MultiMode>
    {
        // Reserved. RCS modules have no per-module id, so modes are matched by order, not by this value.
        [KSPField]
        public string RCSID = string.Empty;
        // Optional semicolon-separated display names, one per ModuleRCS on the part.
        [KSPField]
        public string GUIRCSID = string.Empty;

        // True when GUIRCSID was not supplied, so the thruster's resource name is used as the display name.
        protected bool GUIRCSIDEmpty = true;

        // The ModuleRCS instances on this part, located once during initialization.
        protected List<ModuleRCS> ModuleRCSs;

        // The RCS module matching the current selection, its index, and whether it was enabled.
        // currentRCSState lets us carry the enabled/disabled state across a mode switch.
        protected ModuleRCS currentModuleRCS;
        protected int currentModuleRCSindex;
        private bool currentRCSState = true;

        /// <summary>
        /// Builds one mode per ModuleRCS on the part and hides the stock per-thruster toggle action.
        /// Runs from the base OnStart flow, and again from GetInfo() in the editor; the
        /// _settingsInitialized guard makes sure the work only happens once.
        /// </summary>
        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeRCS() --> initializeSettings()", iDebugLevel.DebugInfo);

                string[] arrGUIRCSID;

                // GUIRCSID is optional; GUIRCSIDEmpty flags when it was left out.
                GUIRCSIDEmpty = ArraySplitEvaluate(GUIRCSID, out arrGUIRCSID, ';');

                // Find the RCS modules on the part. Single lookup, reused for the part's lifetime.
                GTIDebug.Log("Find modules RCS from part", iDebugLevel.DebugInfo);
                ModuleRCSs = part.FindModulesImplementing<ModuleRCS>();

                // If display names were supplied, their count must match the modules, otherwise the cfg is wrong.
                if (!GUIRCSIDEmpty && ModuleRCSs.Count != arrGUIRCSID.Length)
                    GTIDebug.LogError("GTI_MultiModeRCS: GUIRCSID has " + arrGUIRCSID.Length + " names but the part has " + ModuleRCSs.Count + " ModuleRCS - fix the cfg.");

                // Create one mode per RCS module, naming it from GUIRCSID when available, else its resource name.
                GTIDebug.Log("Create list of modes", iDebugLevel.DebugInfo);
                modes = new List<MultiMode>(ModuleRCSs.Count);
                for (int i = 0; i < ModuleRCSs.Count; i++)
                {
                    modes.Add(new MultiMode()
                    {
                        moduleIndex = i,
                        ID = i.ToString(),
                        Name = GUIRCSIDEmpty ? ModuleRCSs[i].resourceName : arrGUIRCSID[i]
                    });
                }

                // Hide the stock per-thruster toggle so this module is the only control point.
                for (int i = 0; i < ModuleRCSs.Count; i++)
                {
                    ModuleRCSs[i].Actions["ToggleAction"].active = false;
                }

                // Animation-group gating is not supported by this module.
                useModuleAnimationGroup = false;
            }
        }

        /// <summary>
        /// After the base class builds the selector UI, resolve currentModuleRCS from the persisted
        /// selection so the action handlers have a valid module before the first manual switch.
        /// </summary>
        protected override void initializeGUI()
        {
            GTIDebug.Log("GTI_MultiModeRCS() --> override initializeGUI()", iDebugLevel.High);
            base.initializeGUI();

            if (ModuleRCSs.Count == 0) return;
            currentModuleRCSindex = modes[selectedMode].moduleIndex;
            currentModuleRCS = ModuleRCSs[currentModuleRCSindex];
        }

        /// <summary>
        /// Applies the current selection: enables the chosen RCS module and fully disables the rest.
        /// </summary>
        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log("GTI_MultiModeRCS: updateMultiMode() --> ChooseOption = " + ChooseOption, iDebugLevel.High);

            // Remember whether the module we are switching away from was enabled, so we can match that below.
            if (currentModuleRCS != null)
            {
                currentRCSState = currentModuleRCS.rcsEnabled;
            }
            else GTIDebug.Log("updateMultiMode() --> currentModuleRCS is null", iDebugLevel.Low);

            if (!silentUpdate) writeScreenMessage();

            // initializeSettings() must have populated this list. A null here means a setup bug, so fail loudly.
            if (ModuleRCSs == null)
                throw new Exception("GTI_MultiModeRCS.updateMultiMode(): ModuleRCSs list is null on part '" + part?.name + "' - initializeSettings() did not run before updateMultiMode().");

            // The selected mode maps directly to one ModuleRCS by index.
            int targetIndex = modes[selectedMode].moduleIndex;

            // Walk every RCS module: enable the selected one, disable all the others.
            for (int i = 0; i < ModuleRCSs.Count; i++)
            {
                ModuleRCS moduleRCS = ModuleRCSs[i];
                if (i == targetIndex)
                {
                    GTIDebug.Log("GTI_MultiModeRCS: Enable index " + i, iDebugLevel.High);
                    currentModuleRCS = moduleRCS;
                    currentModuleRCSindex = i;

                    // moduleIsEnabled lets it thrust (FixedUpdate gate); isEnabled shows its right-click UI.
                    moduleRCS.moduleIsEnabled = true;
                    moduleRCS.isEnabled = true;
                    // Carry the enabled/disabled state from the previous mode so a switch doesn't silently change it.
                    moduleRCS.rcsEnabled = currentRCSState;
                }
                else
                {
                    // Disable thrust (moduleIsEnabled) and hide the module's UI (isEnabled); rcsEnabled off for good measure.
                    GTIDebug.Log("GTI_MultiModeRCS: Disable index " + i, iDebugLevel.High);
                    moduleRCS.rcsEnabled = false;
                    moduleRCS.moduleIsEnabled = false;
                    moduleRCS.isEnabled = false;
                }
            }
        }

        /// <summary>
        /// On-screen message shown when the mode changes, defaulting to the upper-centre position.
        /// </summary>
        protected override void writeScreenMessage()
        {
            if (messagePosition == string.Empty)
                messagePosition = "UPPER_CENTER";

            writeScreenMessage(
                Message: "Changing RCS to: " + modes[selectedMode].Name,
                messagePosition: messagePosition
                );
        }

        /// <summary>
        /// Fail fast with a descriptive message if no RCS module is currently selected (a bug or a bad cfg).
        /// </summary>
        private void EnsureCurrentRCS()
        {
            if (currentModuleRCS == null)
                throw new Exception("GTI_MultiModeRCS: currentModuleRCS is null on part '" + part?.name + "' - selection '" + ChooseOption + "' matched no ModuleRCS.");
        }

        // Action-group hook: enable the selected RCS module.
        [KSPAction("Enable RCS")]
        public void ActionActivate(KSPActionParam param)
        {
            EnsureCurrentRCS();
            currentModuleRCS.rcsEnabled = true;
            currentRCSState = currentModuleRCS.rcsEnabled;
        }
        // Action-group hook: disable the selected RCS module.
        [KSPAction("Disable RCS")]
        public void ActionShutdown(KSPActionParam param)
        {
            EnsureCurrentRCS();
            currentModuleRCS.rcsEnabled = false;
            currentRCSState = currentModuleRCS.rcsEnabled;
        }
        // Action-group hook: toggle the selected RCS module on/off.
        [KSPAction("Toggle RCS")]
        public void ActionToggle(KSPActionParam param)
        {
            EnsureCurrentRCS();
            currentModuleRCS.rcsEnabled = !currentModuleRCS.rcsEnabled;
            currentRCSState = currentModuleRCS.rcsEnabled;
        }

        // Animation-group gating is not supported (useModuleAnimationGroup is forced false, so this never runs).
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
            GTIDebug.Log("GTI_MultiModeRCS GetInfo");
            try
            {
                if (!_settingsInitialized)
                {
                    initializeSettings();
                }

                Info.AppendLine("<color=yellow>RCS Modes Available:</color>");
                for (int i = 0; i < modes.Count; i++)
                {
                    Info.AppendLine(modes[i].Name);
                }
                Info.AppendLine("\nIn Flight switching is <color=yellow>" + (availableInFlight ? "available" : "not available") + "</color>");

                return Info.ToString();
            }
            catch (Exception e)
            {
                GTIDebug.LogError("GTI_MultiModeRCS GetInfo Error " + e.Message);
                throw;
            }
        }
    }
}
