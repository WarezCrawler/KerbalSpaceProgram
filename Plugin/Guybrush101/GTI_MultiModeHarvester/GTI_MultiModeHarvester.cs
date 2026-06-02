using System;
using static GTI.GTIConfig;
using System.Collections.Generic;
using System.Text;

namespace GTI
{
    public class GTI_MultiModeHarvester : GTI_MultiMode<MultiMode>
    {
        // Every ModuleResourceHarvester found on the part. Each one is exposed as one selectable "mode".
        protected List<ModuleResourceHarvester> MRH;
        // The harvester matching the currently selected mode; target for the action-group actions below.
        private ModuleResourceHarvester currentHarvester;

        #region Initialization
        /// <summary>
        /// Discovers every ModuleResourceHarvester on the part, registers one MultiMode entry per harvester,
        /// and disables the stock per-harvester actions so that mode switching is driven solely by this module.
        /// Called once from the base-class OnStart() via initialize().
        /// </summary>
        protected override void initializeSettings()
        {
            if (!_settingsInitialized)
            {
                GTIDebug.Log("GTI_MultiModeHarvester --> initializeSettings()", iDebugLevel.DebugInfo);

                // Find all harvesters on the part and register each as a selectable mode.
                MRH = part.FindModulesImplementing<ModuleResourceHarvester>();

                modes = new List<MultiMode>(MRH.Count);
                for (int i = 0; i < MRH.Count; i++)
                {
                    modes.Add(new MultiMode()
                    {
                        moduleIndex = i,
                        ID          = i.ToString(),
                        Name        = MRH[i].ConverterName
                    });
                    GTIDebug.Log("mode[" + i + "].ID --> " + modes[i].ID, iDebugLevel.DebugInfo);
                    GTIDebug.Log("mode[" + i + "].Name --> " + modes[i].Name, iDebugLevel.DebugInfo);
                }

                // Disable the stock harvester actions; the replacement actions on this module (see Action groups
                // region) route to the active mode so action groups keep working across all modes.
                for (int i = 0; i < MRH.Count; i++)
                {
                    MRH[i].Actions["ToggleResourceConverterAction"].active = false;
                    MRH[i].Actions["StartResourceConverterAction"].active = false;
                    MRH[i].Actions["StopResourceConverterAction"].active = false;
                }
            }
        }
        #endregion

        #region Mode switching
        /// <summary>
        /// Enables the harvester for the selected mode (only when the optional ModuleAnimationGroup is deployed)
        /// and disables/stops every other harvester. Also caches the selected harvester for the action-group actions.
        /// </summary>
        public override void updateMultiMode(bool silentUpdate = false)
        {
            GTIDebug.Log("GTI_MultiModeHarvester: updateMultiMode() --> Begin", iDebugLevel.High);

            if (silentUpdate == false) writeScreenMessage();

            // Track the selected harvester so the action-group actions always target the active mode.
            currentHarvester = MRH[modes[selectedMode].moduleIndex];

            // Honour the optional ModuleAnimationGroup: a converter may only run while the group is deployed.
            bool MAG_isDeployed = true;
            if (MAG != null && useModuleAnimationGroup == true)
                MAG_isDeployed = MAG.isDeployed;

            for (int i = 0; i < modes.Count; i++)
            {
                if (i == selectedMode && MAG_isDeployed)
                {
                    // Selected (and deployed) mode --> enable so it can run and show in the PAW.
                    GTIDebug.Log("GTI_MultiModeHarvester (" + (silentUpdate ? "silent" : "non-silent") + "): Activate Converter Module [" + modes[i].moduleIndex + "] --> " + MRH[modes[i].moduleIndex].ConverterName, iDebugLevel.High);
                    MRH[modes[i].moduleIndex].EnableModule();
                }
                else
                {
                    // Every other mode --> deactivate then stop so it neither runs nor clutters the PAW.
                    GTIDebug.Log("GTI_MultiModeHarvester (" + (silentUpdate ? "silent" : "non-silent") + "): Deactivate Converter Module [" + modes[i].moduleIndex + "] --> " + MRH[modes[i].moduleIndex].ConverterName, iDebugLevel.High);

                    MRH[modes[i].moduleIndex].DisableModule();
                    MRH[modes[i].moduleIndex].StopResourceConverter();
                }
            }
            MonoUtilities.RefreshContextWindows(part);
            GTIDebug.Log("GTI_MultiModeHarvester: updateMultiMode() --> Finished", iDebugLevel.DebugInfo);
        }

        /// <summary>
        /// Called by the base class when the ModuleAnimationGroup retracts: stops and disables every harvester.
        /// </summary>
        protected override void ModuleAnimationGroupEvent_DisableModules()
        {
            for (int i = 0; i < MRH.Count; i++)     // Disable all modules
            {
                GTIDebug.Log("GTI_MultiModeHarvester: Deactivate Converter Module [" + modes[i].moduleIndex + "] --> " + MRH[modes[i].moduleIndex].ConverterName, iDebugLevel.High);
                MRH[modes[i].moduleIndex].DirtyFlag = false;    // Fix for KSP1.3.1
                MRH[modes[i].moduleIndex].DisableModule();
                MRH[modes[i].moduleIndex].StopResourceConverter();
            }
        }
        #endregion

        #region Screen message
        /// <summary>
        /// Posts a summary of the newly selected mode (its inputs and harvested output resource) to the screen.
        /// </summary>
        protected override void writeScreenMessage()
        {
            // Resolve the selected harvester once via its moduleIndex (selectedMode is an index into modes,
            // not necessarily into MRH).
            ModuleResourceHarvester selected = MRH[modes[selectedMode].moduleIndex];

            StringBuilder strOutInfo = new StringBuilder();
            strOutInfo.AppendLine("Converter mode changed to " + modes[selectedMode].Name);
            strOutInfo.AppendLine("Inputs:");
            foreach (ResourceRatio input in selected.inputList)
            {
                strOutInfo.AppendLine(input.ResourceName + " (" + input.Ratio + ")");
            }
            // A harvester has a single harvested output resource (ResourceName), unlike a converter recipe.
            strOutInfo.AppendLine("Outputs:");
            strOutInfo.AppendLine(selected.ResourceName);

            GTIDebug.Log("\nGTI_MultiModeHarvester:\n" + strOutInfo.ToString(), iDebugLevel.DebugInfo);

            writeScreenMessage(
                Message: strOutInfo.ToString(),
                messagePosition: messagePosition,
                duration: 3f
                );
        }
        #endregion

        #region Action groups
        // Replacements for the stock harvester actions (disabled in initializeSettings). Each routes to the
        // currently selected harvester so a single action-group binding controls whichever mode is active.
        [KSPAction("Activate Harvester")]
        public void ActionActivate(KSPActionParam param)
        {
            if (currentHarvester != null) currentHarvester.StartResourceConverterAction(param);
        }
        [KSPAction("Shutdown Harvester")]
        public void ActionShutdown(KSPActionParam param)
        {
            if (currentHarvester != null) currentHarvester.StopResourceConverterAction(param);
        }
        [KSPAction("Toggle Harvester")]
        public void ActionToggle(KSPActionParam param)
        {
            if (currentHarvester != null) currentHarvester.ToggleResourceConverterAction(param);
        }
        #endregion

        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                return BuildModesTechInfo("GTI Multi Mode Harvester");
            }
            catch (Exception e)
            {
                GTIDebug.LogError(this.GetType().Name + " GetInfo() Error " + e.Message);
                throw;
            }
        }
        #endregion
    }
}
