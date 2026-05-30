using static GTI.GTIConfig;
using static GTI.Utilities;
using System.Collections.Generic;
using System;
using System.Text;

namespace GTI
{
    /// <summary>
    /// WORK IN PROGRESS - not finished and not usable yet. Intended to switch a part between
    /// several stock ModuleRCS thrusters (modes), mirroring the engine module. updateMultiMode()
    /// still ends in NotImplementedException, so do not reference this module from a part cfg.
    /// </summary>
    class GTI_MultiModeRCS : GTI_MultiMode<MultiMode>
    {
        // Reserved for an RCS-id list (not yet used). GUIRCSID holds optional display names.
        [KSPField]
        public string RCSID = string.Empty;
        [KSPField]
        public string GUIRCSID = string.Empty;

        // True when GUIRCSID was not supplied, so the RCS resource name is used as the display name.
        protected bool GUIRCSIDEmpty = true;

        // The ModuleRCS instances on this part, located once during initialization.
        protected List<ModuleRCS> ModuleRCSs;

        protected ModuleRCS currentModuleRCS;
        protected int currentModuleRCSindex;

        /// <summary>
        /// Builds one mode per ModuleRCS on the part and hides the stock RCS toggle action.
        /// </summary>
        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeRCS() --> initializeSettings()", iDebugLevel.DebugInfo);

                string[] arrGUIRCSID;

                // GUIRCSID is optional; GUIRCSIDEmpty flags when it was left out.
                GUIRCSIDEmpty = ArraySplitEvaluate(GUIRCSID, out arrGUIRCSID, ';');

                // Find the RCS modules on the part.
                GTIDebug.Log("Find modules RCS from part", iDebugLevel.DebugInfo);
                ModuleRCSs = part.FindModulesImplementing<ModuleRCS>();

                // The cfg is wrong if display names were given but their count doesn't match the modules.
                modes = new List<MultiMode>(ModuleRCSs.Count);
                if (ModuleRCSs.Count != arrGUIRCSID.Length || GUIRCSIDEmpty)
                    GTIDebug.LogError("GTI_MultiModeRCS Error in CFG configuration detected");

                // Create one mode per RCS module, naming it from GUIRCSID when available, else its resource name.
                GTIDebug.Log("Create list of modes", iDebugLevel.DebugInfo);
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

                // Animation-group gating could be added later; disabled for now.
                useModuleAnimationGroup = false;
            }
        }

        /// <summary>
        /// Intended to enable the selected RCS module and disable the rest. NOT FINISHED -
        /// the selection loop is incomplete and the method still throws NotImplementedException.
        /// </summary>
        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log("GTI_MultiModeRCS: updateMultiMode() --> Begin", iDebugLevel.High);
            if (!silentUpdate) writeScreenMessage();

            for (int i = 0; i < modes.Count; i++)
            {
                if (i == modes[currentModuleRCSindex].moduleIndex)
                {
                    currentModuleRCS = ModuleRCSs[i];
                    currentModuleRCSindex = i;
                    ModuleRCSs[i].rcsEnabled = true;
                }
            }

            throw new NotImplementedException();
        }

        // Animation-group gating is not supported (and the module itself is unfinished).
        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// On-screen message shown when the mode changes, defaulting to the upper-centre position.
        /// </summary>
        protected override void writeScreenMessage()
        {
            if (messagePosition == string.Empty)
                messagePosition = "UPPER_CENTER";

            writeScreenMessage(
                Message: "Changing PRCS propulsion to: " + modes[selectedMode].Name,
                messagePosition: messagePosition
                );
        }
    }
}
