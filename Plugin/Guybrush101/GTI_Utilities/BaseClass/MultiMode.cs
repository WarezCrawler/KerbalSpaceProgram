using System;
using System.Collections.Generic;
using System.Text;
using static GTI.GTIConfig;
using static GTI.Utilities;


namespace GTI
{
    public interface IMultiMode
    {
        int moduleIndex { get; set; }
        string ID { get; set; }
        string Name { get; set; }

        string ToString();
    }

    //Default MultiMode object
    public class MultiMode : IMultiMode
    {
        public int moduleIndex { get; set; }
        public string ID { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return moduleIndex + "\t" + ID + "\t" + Name;
        }
    }


    public abstract class GTI_MultiMode<T> : PartModule where T : IMultiMode
    {
        protected bool _settingsInitialized = false;
        protected bool _GUIsettingsInitialized = false;
        protected bool _MAGsettingsInitialized = false;

        //public List<MultiMode> mode { get; protected set; }
        public List<T> modes { get; protected set; }

        //Availability of the functionality
        [KSPField]
        public bool availableInFlight = false;
        [KSPField]
        public bool availableInEditor = true;
        [KSPField]
        public bool externalToEVAOnly = false;

        [KSPField]
        public bool useModuleAnimationGroup = false;
        //protected Animation Anim;
        protected ModuleAnimationGroup MAG;

        [KSPField]
        public bool affectSymCounterpartsInFlight = false;

        [KSPField]
        public string messagePosition = string.Empty;

        #region Tech gating fields
        // Semicolon-separated list of tech-node IDs, one per mode (parallel to mode order). A blank
        // entry means that mode needs no tech. Mode 0 (the part's original behaviour) is ALWAYS
        // unlocked regardless of this list.
        [KSPField]
        public string techRequired = string.Empty;

        // Semicolon-separated list of tech-node IDs, one per mode (parallel to mode order). The INVERSE
        // of techRequired: once the listed tech is researched the mode is REMOVED (obsoleted) from the
        // selector - use it for "upgrade" behaviour where a basic mode is retired when a better one is
        // unlocked. A blank entry means the mode is never removed. This applies to mode 0 as well, so an
        // always-unlocked original behaviour can still be retired by an upgrade tech. Like techRequired,
        // the change only takes effect at editor build time or via the EVA service action - a flying
        // vessel keeps the modes it launched with until a Kerbal services it.
        [KSPField]
        public string techObsolete = string.Empty;

        // A single tech-node ID that gates the WHOLE selector; until it is researched the mode menu
        // is hidden entirely ("unlock the configuration" flavour).
        [KSPField]
        public string moduleTechRequired = string.Empty;

        // Per-part frozen snapshot of unlocked mode IDs (comma list). Persisted with the vessel so a
        // craft in flight keeps exactly the modes it launched with until a Kerbal services it on EVA.
        [KSPField(isPersistant = true)]
        public string unlockedModes = string.Empty;

        // Runtime: parsed unlock + obsolete tech per mode index, and the resulting available flag per
        // index (modeUnlocked[i] == "this mode is offered on this part instance").
        protected string[] modeTech;
        protected string[] modeTechObsolete;
        protected bool[] modeUnlocked;
        #endregion

        /// <summary>
        /// Currently selected mode in integer format for use in the arrays
        /// </summary>
        public int selectedMode { get; protected set; } = 0;

        public override void OnStart(PartModule.StartState state)
        {
            GTIDebug.Log("GTI_MultiMode baseclass --> OnStart()", iDebugLevel.DebugInfo);

            //Assign update method to delegate
            OnUpdateMultiMode = FindSelectedMode;
            OnUpdateMultiMode += updateMultiMode;
            
            initialize();

            this.Events["EVAChangeMode"].active = externalToEVAOnly;

            //Show the EVA "service / upgrade" button only when this part actually has tech-gated modes
            //AND the game has an R&D system (career/science). In sandbox everything is unlocked already.
            bool hasTechGating = !(string.IsNullOrEmpty(techRequired) && string.IsNullOrEmpty(techObsolete) && string.IsNullOrEmpty(moduleTechRequired));
            bool techGameMode = ResearchAndDevelopment.Instance != null;
            this.Events[nameof(EVAUpgradeModes)].active = hasTechGating && techGameMode;
        }

        /// <summary>
        /// After onstart, the mode can be updated. This cannot be done sooner, since all settings does not seem to be finished loading at that point.
        /// </summary>
        /// <param name="state"></param>
        public override void OnStartFinished(StartState state)
        {
            //updateMultiMode(silentUpdate: true);
            OnUpdateMultiMode.Invoke(silentUpdate: true);
        }
        protected void InvokeOnUpdateMultiMode(bool silentUpdate = false)
        {
            OnUpdateMultiMode.Invoke(silentUpdate);
        }

        /// <summary>
        /// Updates the module with new selections. Deactivating the inactive ones, and activating the selected one.
        /// </summary>
        public abstract void updateMultiMode(bool silentUpdate = false);

        public delegate void OnUpdateAction(bool silentUpdate = false);
        public event OnUpdateAction OnUpdateMultiMode;
        //public updateMultiModeModule updateMultiMode;

        /// <summary>
        /// initializeSettings() method is for custom settings initialization, and will automatically be called from the OnStart() method unless it is overridden.
        /// This method runs before the generic ones kicks in
        /// </summary>
        protected abstract void initializeSettings();


        /// <summary>
        /// initialize() is the internal method for loading all initialization methods
        /// </summary>
        private void initialize()
        {
            if (!_settingsInitialized)
            {
                initializeSettings();
                //Handle ModuleAnimationGroup
                initializeModuleAnimationGroup();
                //Resolve which modes are unlocked on this part (tech gating) before the selector is built.
                ApplyTechUnlocks();
                initializeGUI();
                //initializeEventSubscriptions();       //UI_CHOOSEOPTION is not visible on EVA :o(

                _settingsInitialized = true;
            }
        }

        #region Tech unlocking
        // ---------------------------------------------------------------------------------------------
        //  Tech-gated modes (inherited by every GTI_MultiMode subscriber).
        //
        //  Driven by three optional cfg fields:
        //    techRequired       - semicolon list of tech-node IDs, one per mode (parallel to mode order).
        //                         A blank entry = no tech needed. Mode 0 (the part's original behaviour)
        //                         is ALWAYS unlocked regardless of this list.
        //    techObsolete       - semicolon list of tech-node IDs, one per mode (the INVERSE of the
        //                         above): once researched the mode is REMOVED from the selector. A blank
        //                         entry = never removed. Applies to mode 0 too (an upgrade can retire the
        //                         original behaviour). A mode is offered only when it is unlocked AND not
        //                         yet obsoleted.
        //    moduleTechRequired - a single tech-node ID gating the WHOLE selector; until researched the
        //                         mode menu is hidden entirely.
        //
        //  The crucial rule: a vessel already in flight keeps exactly the modes it launched with.
        //  Researching a new tech never changes it. Only the editor (designing a fresh craft) and the
        //  in-flight EVA "service" action are allowed to resync a part to the current tech tree. This is
        //  achieved by persisting the unlocked set per-part in 'unlockedModes' and only recomputing it
        //  from live tech in those two situations.
        // ---------------------------------------------------------------------------------------------

        // Parse techRequired + techObsolete into one entry per mode (idempotent; safe to call repeatedly).
        protected void ParseTechRequired()
        {
            if (modeTech != null && modeTech.Length == modes.Count) return;

            ArraySplitEvaluate(techRequired, out string[] arr, ';');
            ArraySplitEvaluate(techObsolete, out string[] arrObsolete, ';');
            modeTech = new string[modes.Count];
            modeTechObsolete = new string[modes.Count];
            for (int i = 0; i < modes.Count; i++)
            {
                modeTech[i] = (arr.Length > i) ? arr[i].Trim() : string.Empty;
                modeTechObsolete[i] = (arrObsolete.Length > i) ? arrObsolete[i].Trim() : string.Empty;
            }
        }

        // Is a mode available against the LIVE tech tree right now? A mode is available when its unlock
        // requirement is met (mode 0 / a blank entry is always unlocked) AND it has not been obsoleted by
        // a researched techObsolete entry. This is the single rule used to (re)build the frozen snapshot.
        protected bool ModeAvailableLive(int i)
        {
            ParseTechRequired();
            bool unlocked = (i == 0) || TechResearched(modeTech[i]);
            bool obsoleted = !string.IsNullOrEmpty(modeTechObsolete[i]) && TechResearched(modeTechObsolete[i]);
            return unlocked && !obsoleted;
        }

        // Is a tech node researched? Empty id = yes. In a game without R&D (sandbox) KSP's
        // GetTechnologyState returns Available, so everything is unlocked automatically.
        public static bool TechResearched(string techID)
        {
            if (string.IsNullOrEmpty(techID)) return true;
            return ResearchAndDevelopment.GetTechnologyState(techID) == RDTech.State.Available;
        }

        // Comma-separated list of mode IDs that are available against the live tech tree right now
        // (unlocked AND not obsoleted). Mode 0 is included unless an upgrade tech has obsoleted it.
        protected string ComputeLiveUnlockedSet()
        {
            ParseTechRequired();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < modes.Count; i++)
            {
                if (ModeAvailableLive(i))
                {
                    if (sb.Length > 0) sb.Append(',');
                    sb.Append(modes[i].ID);
                }
            }
            return sb.ToString();
        }

        // Decide which modes are unlocked on THIS part and fill modeUnlocked[].
        protected void ApplyTechUnlocks()
        {
            ParseTechRequired();
            modeUnlocked = new bool[modes.Count];

            if (HighLogic.LoadedSceneIsEditor)
            {
                //Editor == always live: a craft is (re)designed against the current tech tree.
                unlockedModes = ComputeLiveUnlockedSet();
            }
            else if (string.IsNullOrEmpty(unlockedModes))
            {
                //Flight, but no snapshot yet (a craft saved before this feature existed, or a vessel
                //spawned directly into the world). Seed it once from live tech, then it stays frozen.
                unlockedModes = ComputeLiveUnlockedSet();
            }
            //else: flight WITH a snapshot -> trust it verbatim. This is the freeze that keeps a flying
            //vessel unchanged when the player researches new tech.

            HashSet<string> allowed = new HashSet<string>(unlockedModes.Split(','));
            for (int i = 0; i < modes.Count; i++)
            {
                //A mode is offered iff it appears in this part's frozen snapshot. The snapshot was built
                //by ComputeLiveUnlockedSet, which already applied BOTH rules (unlock + obsolete), so we
                //must NOT re-add mode 0 / no-tech modes here - doing so would resurrect a mode the upgrade
                //(techObsolete) was supposed to remove.
                modeUnlocked[i] = allowed.Contains(modes[i].ID);
            }
        }

        protected bool IsModeUnlocked(int index)
        {
            if (modeUnlocked == null || index < 0 || index >= modeUnlocked.Length) return true;
            return modeUnlocked[index];
        }

        // Index of the first mode offered on this part (unlocked and not obsoleted). Falls back to 0 if -
        // through a config error - nothing is available, so the part is never left without a selection.
        protected int FirstAvailableMode()
        {
            for (int i = 0; i < modes.Count; i++)
                if (IsModeUnlocked(i)) return i;
            return 0;
        }

        // True when a module-level tech gate has not been researched (hides the whole selector).
        protected bool ModuleTechLocked()
        {
            return !TechResearched(moduleTechRequired);
        }

        // Build the selector option/display arrays from the unlocked modes only. Returns the count.
        protected int BuildVisibleOptions(out string[] options, out string[] display)
        {
            List<string> ids = new List<string>(modes.Count);
            List<string> names = new List<string>(modes.Count);
            for (int i = 0; i < modes.Count; i++)
            {
                if (IsModeUnlocked(i))
                {
                    ids.Add(modes[i].ID);
                    names.Add(modes[i].Name);
                }
            }
            options = ids.ToArray();
            display = names.ToArray();
            return ids.Count;
        }

        // Rebuild the live selector after the unlocked set changes (used by the EVA service action).
        protected void RefreshModeOptions()
        {
            BaseField chooseField = Fields[nameof(ChooseOption)];
            int visibleCount = BuildVisibleOptions(out string[] options, out string[] display);

            UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight
                ? chooseField.uiControlFlight as UI_ChooseOption
                : chooseField.uiControlEditor as UI_ChooseOption;
            if (chooseOption != null)
            {
                chooseOption.options = options;
                chooseOption.display = display;
            }

            bool show = visibleCount >= 2 && !ModuleTechLocked();
            chooseField.guiActive = show && availableInFlight;
            chooseField.guiActiveEditor = show && availableInEditor;

            MonoUtilities.RefreshPartContextWindow(part);
        }

        // EVA "service" action: a Kerbal physically resyncs this part to the current tech tree. This is
        // the ONLY way a flying vessel gains modes unlocked after it launched - or loses modes an upgrade
        // tech (techObsolete) has since retired.
        [KSPEvent(name = "EVAUpgradeModes", guiName = "Service / upgrade modes", active = false,
                  externalToEVAOnly = true, guiActiveUnfocused = true, unfocusedRange = 5f,
                  guiActive = false, guiActiveEditor = false)]
        public void EVAUpgradeModes()
        {
            string before = unlockedModes;
            unlockedModes = ComputeLiveUnlockedSet();   //resync this part to current tech
            ApplyTechUnlocks();                          //recompute modeUnlocked from the new snapshot

            //The service may have REMOVED the mode currently in use (an upgrade tech obsoleted it). Move
            //to the first still-available mode and apply it before refreshing the menu, so the part never
            //keeps running a mode that is no longer offered.
            if (!IsModeUnlocked(selectedMode))
            {
                selectedMode = FirstAvailableMode();
                ChooseOption = modes[selectedMode].ID;
                InvokeOnUpdateMultiMode(silentUpdate: false);
            }

            RefreshModeOptions();                        //rebuild the selector + right-click menu

            string pos = (messagePosition == string.Empty) ? "UPPER_CENTER" : messagePosition;
            if (unlockedModes != before)
                writeScreenMessage("Modes updated on " + part.partInfo.title, 4f, pos);
            else
                writeScreenMessage("No mode changes available", 3f, pos);
        }

        // ---- GetInfo() helpers (editor tooltip) -----------------------------------------------------
        // These are safe to call from a subclass GetInfo(): they parse 'techRequired'/'moduleTechRequired'
        // directly and do not depend on the modes list having been built.

        // Tech tag for a single mode index, e.g. "Requires tech: Ion Propulsion (locked)".
        // Returns string.Empty when that mode has no tech requirement.
        protected string ModeTechInfo(int modeIndex)
        {
            ArraySplitEvaluate(techRequired, out string[] arr, ';');
            if (modeIndex < 0 || modeIndex >= arr.Length) return string.Empty;
            string tech = arr[modeIndex].Trim();
            if (string.IsNullOrEmpty(tech)) return string.Empty;
            return TechTag("Requires", tech);
        }

        // Obsolete tag for a single mode index, e.g. "Removed by tech: Heavy Rocketry (locked)". Returns
        // string.Empty when that mode has no obsolete requirement.
        protected string ModeObsoleteInfo(int modeIndex)
        {
            ArraySplitEvaluate(techObsolete, out string[] arr, ';');
            if (modeIndex < 0 || modeIndex >= arr.Length) return string.Empty;
            string tech = arr[modeIndex].Trim();
            if (string.IsNullOrEmpty(tech)) return string.Empty;
            return TechTag("Removed by", tech);
        }

        // Module-level tech tag (the gate that hides the whole selector). Empty when none configured.
        protected string ModuleTechInfo()
        {
            if (string.IsNullOrEmpty(moduleTechRequired)) return string.Empty;
            return TechTag("Module requires", moduleTechRequired);
        }

        // Format a "<prefix> tech: <Title> (<status>)" tag, resolving the readable tech title and
        // colouring the researched/locked status. Falls back to the raw id if no title is found.
        private string TechTag(string prefix, string techID)
        {
            string title = ResearchAndDevelopment.GetTechnologyTitle(techID);
            if (string.IsNullOrEmpty(title)) title = techID;
            string status = TechResearched(techID)
                ? "<color=#44ff44>researched</color>"
                : "<color=#ffaa00>locked</color>";
            return prefix + " tech: " + title + " (" + status + ")";
        }

        // Convenience block for subclasses with no detailed GetInfo of their own (Converter/Harvester):
        // lists every mode and its tech tag.
        //
        // GetInfo() runs during part COMPILATION (PartLoader.CompilePartInfo), where some subclasses'
        // initializeSettings() cannot run yet - e.g. GTI_MultiModeIntake calls part.GetPartModuleConfig(),
        // which returns null that early and throws. A throw here aborts the CompileParts coroutine and
        // hangs loading, so we build the mode list defensively: try to initialise, swallow any failure,
        // and fall back to a header-only tooltip if the mode list is still unavailable.
        protected string BuildModesTechInfo(string header)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<color=yellow>" + header + "</color>");

            if (modes == null)
            {
                try { if (!_settingsInitialized) initializeSettings(); }
                catch (Exception e)
                {
                    GTIDebug.Log("BuildModesTechInfo: initializeSettings() unavailable during GetInfo (" + e.Message + ")", iDebugLevel.DebugInfo);
                }
            }

            //Module-level tech gate reads straight from the cfg field, so it is always safe to show.
            string moduleTag = ModuleTechInfo();
            if (moduleTag != string.Empty) sb.AppendLine("<i>" + moduleTag + "</i>");

            //If the mode list could not be built this early, return the header (+ module gate) only.
            if (modes == null) return sb.ToString();

            for (int i = 0; i < modes.Count; i++)
            {
                sb.Append("• ").Append(modes[i].Name);
                string tag = ModeTechInfo(i);
                if (tag != string.Empty) sb.Append("  <i>").Append(tag).Append("</i>");
                string obsoleteTag = ModeObsoleteInfo(i);
                if (obsoleteTag != string.Empty) sb.Append("  <i>").Append(obsoleteTag).Append("</i>");
                sb.AppendLine();
            }

            sb.AppendLine("\nIn Flight switching is <color=yellow>" + (availableInFlight ? "available" : "not available") + "</color>");
            return sb.ToString();
        }
        #endregion

        protected void initializeEventSubscriptions()
        {
            //if (externalToEVAOnly)
            //{
                if (GameEvents.onCrewOnEva != null) GameEvents.onCrewOnEva.Add(onEVA);
                if (GameEvents.onCrewBoardVessel != null) GameEvents.onCrewBoardVessel.Add(onEVABoard);
                if (GameEvents.onVesselSwitching != null) GameEvents.onVesselSwitching.Add(onVesselSwitching);
            //}
        }

        #region UI
        [KSPField(guiActive = true, guiActiveEditor = true, isPersistant = true, guiName = "MultiMode")]
        [UI_ChooseOption(affectSymCounterparts = UI_Scene.Editor, scene = UI_Scene.All, suppressEditorShipModified = false, options = new[] { "None" }, display = new[] { "None" })]
        public string ChooseOption = string.Empty;

        [KSPEvent(name = "EVAChangeMode", guiName = "Change mode", active = true, externalToEVAOnly = true, guiActiveUnfocused = true, unfocusedRange = 5f, guiActive = false, guiActiveEditor = false)]
        public void EVAChangeMode()
        {
            ActionNextMode();
        }

        protected virtual void initializeGUI()
        {
            GTIDebug.Log("GTI_MultiMode: initializeGUI() --> start", iDebugLevel.DebugInfo);
            if (!_GUIsettingsInitialized)
            {
                BaseField chooseField;
                string[] Options;
                string[] OptionsDisplay;

                GTIDebug.Log("GTI_MultiMode: chooseField", iDebugLevel.DebugInfo);
                chooseField = Fields[nameof(ChooseOption)];
                chooseField.guiName = "Mode";
                chooseField.guiActiveEditor = availableInEditor;
                chooseField.guiActive = availableInFlight;

                //Detect state of ModuleAnimationGroup and set UI accordingly
                if (useModuleAnimationGroup && _MAGsettingsInitialized)
                {
                    GTIDebug.Log("Detect ModuleAnimationGroup state, and adjust UI accordingly", iDebugLevel.DebugInfo);
                    //Override the converter selection UI is useModuleAnimationGroup is true
                    chooseField.guiActiveEditor = MAG.isDeployed ? availableInEditor : false;
                    chooseField.guiActive = MAG.isDeployed ? availableInFlight : false;
                }

                //Extract options from the mode list, including ONLY modes unlocked on this part instance
                //(tech gating). Locked modes stay in 'modes' so indices/IDs remain stable - they are
                //simply not offered in the selector.
                GTIDebug.Log("GTI_MultiMode: Set Options & OptionsDisplay", iDebugLevel.DebugInfo);
                int visibleCount = BuildVisibleOptions(out Options, out OptionsDisplay);

                //Hide the selector when fewer than two modes are selectable (a single option yields
                //null ref errors in flight) or when a module-level tech gate is not yet researched.
                if (visibleCount < 2 || ModuleTechLocked())
                {
                    chooseField.guiActive = false;
                    chooseField.guiActiveEditor = false;
                }

                GTIDebug.Log("GTI_MultiMode: Set .options & .display & .onFieldChanged", iDebugLevel.DebugInfo);
                UI_ChooseOption chooseOption = HighLogic.LoadedSceneIsFlight ? chooseField.uiControlFlight as UI_ChooseOption : chooseField.uiControlEditor as UI_ChooseOption;
                chooseOption.options = Options;
                chooseOption.display = OptionsDisplay;
                chooseOption.onFieldChanged = selectMode;
                // Also run the switch when a symmetry counterpart's value changes. KSP copies the chosen
                // value to counterparts but fires onSymmetryFieldChanged (NOT onFieldChanged) on them - so
                // without this line a counterpart keeps its previous mode's modules active (e.g. an RCS block
                // left on the old propellant), making the vessel behave as if two modes are live at once.
                chooseOption.onSymmetryFieldChanged = selectMode;
                chooseOption.affectSymCounterparts = affectSymCounterpartsInFlight ? UI_Scene.All : UI_Scene.Editor;

                //Update Actions GUI texts and hide the ones not applicable
                initializeActions();

                //Load the previous selected Option, and sync up with the selectedConverter number
                //ChooseOption = selectedChooseOption;
                GTIDebug.Log("Find selected mode from ChooseOption", iDebugLevel.DebugInfo);
                selModeFromChooseOption();

                //If it's possible to switch mode in flight, then it natural that a Kerbal can do it manually as well.
                externalToEVAOnly = availableInFlight ? true : externalToEVAOnly;

                _GUIsettingsInitialized = true;

                GTIDebug.Log("GTI_MultiMode: initializeGUI() --> end", iDebugLevel.DebugInfo);
            }
        }

        /// <summary>
        /// Updates the selection the user uses the right click UI
        /// </summary>
        /// <param name="field"></param>
        /// <param name="oldValueObj"></param>
        protected virtual void selectMode(BaseField field, object oldValueObj)
        {
            //FindSelectedMode();
            
            /*updateMultiMode();*/
            

            foreach (Delegate d in OnUpdateMultiMode.GetInvocationList())
            {
                GTIDebug.Log(d.Method.ToString());
            }
            OnUpdateMultiMode.Invoke();
        }


        //public virtual List<T> GetCounterPartModules(Part thispart)
        //{
        //    List<Part> CounterParts = thispart.symmetryCounterparts;
        //    List<T> modules = new List<T>(CounterParts.Count);

        //    foreach (Part part in CounterParts)
        //    {
        //        modules.Add(part.FindModuleImplementing<T>());
        //    }

        //    return modules;
        //}

        /// <summary>
        /// Derives the selected converter as integer from the ChooseOption value
        /// </summary>
        protected virtual void FindSelectedMode()
        {
            for (int i = 0; i < modes.Count; i++)
            {
                if (modes[i].ID == ChooseOption)
                {
                    selectedMode = i;
                    //this.Fields["EVAChangeMode"].guiName = "Change mode: " + mode[selectedMode].Name;
                    this.Events[nameof(EVAChangeMode)].guiName = "Change mode: " + modes[selectedMode].Name;

                    ////Sync up all counterparts modules
                    //if (affectSymCounterpartsInFlight)
                    //{
                    //    List<GTI_MultiMode<T>> CounterModules = CounterPartModules(this.part);
                    //    foreach (GTI_MultiMode<T> module in CounterModules)
                    //    {
                    //        module.ChooseOption = ChooseOption;
                    //    }
                    //}

                    return;
                }
            }
            GTIDebug.Log("FindSelectedMode() was unsuccessful in locating current mode: " + ChooseOption, iDebugLevel.DebugInfo);
        }
        private void FindSelectedMode(bool boo = false) { FindSelectedMode(); }

        /// <summary>
        /// selModeFromChooseOption set selectedMode from ChooseOption.
        /// If ChooseOption is empty, then the first mode in moduleList is returned.
        /// Dependent on: ChooseOption, selectedMode, mode.ID
        /// </summary>
        protected virtual void selModeFromChooseOption()
        {
            GTIDebug.Log("selModeFromChooseOption() start", iDebugLevel.DebugInfo);
            if (ChooseOption == string.Empty)
            {
                GTIDebug.Log("selModeFromChooseOption() --> ChooseOption == string.Empty", iDebugLevel.DebugInfo);
                selectedMode = FirstAvailableMode();
                ChooseOption = modes[selectedMode].ID;
                return;
            }
            else
            {
                for (int i = 0; i < modes.Count; i++)
                {
                    if (ChooseOption == modes[i].ID)
                    {
                        //A persisted selection pointing at a mode that is locked OR has been obsoleted by
                        //an upgrade falls through to the default below (the first available mode), so a
                        //part can never boot up sitting on a mode the player is not allowed to use.
                        if (!IsModeUnlocked(i)) break;

                        GTIDebug.Log("selModeFromChooseOption() --> ChooseOption == mode[i].ID", iDebugLevel.DebugInfo);
                        selectedMode = i;
                        return;
                    }
                }
                //If mode not found or no longer available, revert to the first available mode.
                GTIDebug.Log("selModeFromChooseOption() --> Default", iDebugLevel.DebugInfo);
                selectedMode = FirstAvailableMode();
                ChooseOption = modes[selectedMode].ID;
            }
        }


        /// <summary>
        /// This is where you implement the on screen message function which you can then call in you updateMultiMode method like "if (silentUpdate == false) writeScreenMessage();" 
        /// </summary>
        protected abstract void writeScreenMessage();
        protected virtual void writeScreenMessage(string Message, float duration = 3f, string messagePosition = "UPPER_RIGHT")
        {
            //Default position and switch to user defined position
            ScreenMessageStyle position = ScreenMessageStyle.UPPER_RIGHT;
            switch (messagePosition)
            {
                case "UPPER_CENTER":
                    position = ScreenMessageStyle.UPPER_CENTER;
                    break;
                case "UPPER_RIGHT":
                    position = ScreenMessageStyle.UPPER_RIGHT;
                    break;
                case "UPPER_LEFT":
                    position = ScreenMessageStyle.UPPER_LEFT;
                    break;
                case "LOWER_CENTER":
                    position = ScreenMessageStyle.LOWER_CENTER;
                    break;
            }
            writeScreenMessage(Message, duration, position);
        }
        protected void writeScreenMessage(string Message, float duration = 3f, ScreenMessageStyle messagePosition = ScreenMessageStyle.UPPER_RIGHT)
        {
            ScreenMessages.PostScreenMessage(Message, duration, messagePosition);
        }
        #endregion

        #region Actions
        public virtual void initializeActions()
        {
            for (int i = 1; i <= numberOfSpecificActions; i++)
            {
                //if (MultiModeID.Length != MultiModeNames.Length) GTIDebug.LogError("MultiMode class implementation failed. MultiModeID and MultiModeNames arrays are inconsistent. Please fix the issue.");
                if (modes.Count < i)
                { this.Actions["MultiModeAction_" + i].active = false; }
                else
                { this.Actions["MultiModeAction_" + i].guiName = "Activate: " + modes[i - 1].Name; }
            }
        }

        //Specific actions
        protected const int numberOfSpecificActions = 12;
        [KSPAction("Set #1")]
        public void MultiModeAction_1(KSPActionParam param) { MultiModeAction(0); }

        [KSPAction("Set #2")]
        public void MultiModeAction_2(KSPActionParam param) { MultiModeAction(1); }

        [KSPAction("Set #3")]
        public void MultiModeAction_3(KSPActionParam param) { MultiModeAction(2); }

        [KSPAction("Set #4")]
        public void MultiModeAction_4(KSPActionParam param) { MultiModeAction(3); }

        [KSPAction("Set #5")]
        public void MultiModeAction_5(KSPActionParam param) { MultiModeAction(4); }

        [KSPAction("Set #6")]
        public void MultiModeAction_6(KSPActionParam param) { MultiModeAction(5); }

        [KSPAction("Set #7")]
        public void MultiModeAction_7(KSPActionParam param) { MultiModeAction(6); }

        [KSPAction("Set #8")]
        public void MultiModeAction_8(KSPActionParam param) { MultiModeAction(7); }

        [KSPAction("Set #9")]
        public void MultiModeAction_9(KSPActionParam param) { MultiModeAction(8); }

        [KSPAction("Set #10")]
        public void MultiModeAction_10(KSPActionParam param) { MultiModeAction(9); }

        [KSPAction("Set #11")]
        public void MultiModeAction_11(KSPActionParam param) { MultiModeAction(10); }

        [KSPAction("Set #12")]
        public void MultiModeAction_12(KSPActionParam param) { MultiModeAction(11); }

        protected virtual void MultiModeAction(int inActionSelect)
        {
            GTIDebug.Log("Action ActionPropulsion_" + inActionSelect + " (before): " + ChooseOption, iDebugLevel.Medium);

            //Check if the selected mode is possible (exists and is unlocked on this part)
            if (inActionSelect < modes.Count && IsModeUnlocked(inActionSelect))
            {
                //Check if the selected mode is a change
                if (!(selectedMode == inActionSelect))
                {
                    selectedMode = inActionSelect;

                    ChooseOption = modes[selectedMode].ID;        //converterNames[selectedConverter];
                    GTIDebug.Log("MultiModeAction_" + inActionSelect + " Executed", iDebugLevel.DebugInfo);

                    //updateMultiMode();
                    OnUpdateMultiMode();
                }
            }
            GTIDebug.Log("Action MultiModeAction_" + inActionSelect + " (after): " + ChooseOption, iDebugLevel.Medium);
        }

        [KSPAction("Next mode")]
        public void ActionNextMode(KSPActionParam param) { ActionNextMode(); }
        public virtual void ActionNextMode()
        {
            //Advance to the next UNLOCKED mode, wrapping around. The 'start' guard prevents an infinite
            //loop in the (config-error) case where no other mode is unlocked.
            int start = selectedMode;
            do
            {
                selectedMode++;
                if (selectedMode > modes.Count - 1) { selectedMode = 0; }
            } while (!IsModeUnlocked(selectedMode) && selectedMode != start);

            ChooseOption = modes[selectedMode].ID;
            //updateMultiMode();
            OnUpdateMultiMode();
        }
        [KSPAction("Previous mode")]
        public void ActionPreviousMode(KSPActionParam param) { ActionPreviousMode(); }
        public virtual void ActionPreviousMode()
        {
            //Step back to the previous UNLOCKED mode, wrapping around. The 'start' guard prevents an
            //infinite loop in the (config-error) case where no other mode is unlocked.
            int start = selectedMode;
            do
            {
                selectedMode--;
                //Check if selected proplusion was the first one, and return the last one instead
                if (selectedMode < 0) { selectedMode = modes.Count - 1; }
            } while (!IsModeUnlocked(selectedMode) && selectedMode != start);

            ChooseOption = modes[selectedMode].ID;
            //updateMultiMode();
            OnUpdateMultiMode();
        }
        #endregion

        #region ModuleAnimationGroup
        //Comment: Using "OnAnimationGroupStateChanged" event, I could probably further simplify the logics and remove 
        //the overriding of the "ModuleAnimationGroup" functionality
        protected virtual void initializeModuleAnimationGroup()
        {
            //Debug.Log("useModuleAnimationGroup: " + useModuleAnimationGroup);
            if (!_MAGsettingsInitialized && useModuleAnimationGroup == true)
            {
                GTIDebug.Log("useModuleAnimationGroup: Evaluated as true", iDebugLevel.DebugInfo);
                //Animation Anim = new Animation();
                //ModuleAnimationGroup MAG = new ModuleAnimationGroup();

                MAG = part.FindModuleImplementing<ModuleAnimationGroup>();

                //Anim = part.FindModelAnimator(MAG.deployAnimationName);

                //foreach (BaseEvent e in MAG.Events)
                //{
                //    e.active = false;
                //    e.guiActive = false;
                //    e.guiActiveEditor = false;
                //}
                //foreach (BaseAction a in MAG.Actions)
                //{
                //    a.active = false;
                //}

                //GTIDebug.Log("Activate 'ModuleAnimationGroupEvent'", iDebugLevel.DebugInfo);

                //Activate event in this module to trigger animations instead
                //this.Events["ModuleAnimationGroupEvent"].active = true;
                //this.Events["ModuleAnimationGroupEvent"].guiActive = true;
                //this.Events["ModuleAnimationGroupEvent"].guiActiveEditor = true;
                //Patch 2018-03-14 to fix the button showing even when module is not deployed.
                GTIDebug.Log("initializeModuleAnimationGroup: !MAG.isDeployed " + !MAG.isDeployed, iDebugLevel.DebugInfo);
                if (!MAG.isDeployed)
                {
                    ModuleAnimationGroupEvent_DisableModules();
                }
                    
                //Register the event of Animation Groups
                if (GameEvents.OnAnimationGroupStateChanged != null && MAG != null)
                {
                    GTIDebug.Log("Subscribing to OnAnimationGroupStateChanged", iDebugLevel.DebugInfo);
                    GameEvents.OnAnimationGroupStateChanged.Add(OnModuleAnimationGroupStateChanged);
                }

                //?? Add check if the MAG isDeployed? Adjust UI accordingly?

                _MAGsettingsInitialized = true;
            }
        }

        /*[KSPEvent(active = false, guiActive = false, guiActiveEditor = false, guiName = "Deploy")]
        public void ModuleAnimationGroupEvent()
        {
            float AnimLength;

            #region tests
            //Debug.Log("ModuleAnimationGroupEvent fired");

            //Debug.Log("Anim.isPlaying " + Anim.isPlaying.ToString());
            #endregion

            try { AnimLength = Anim.clip.length; } catch { AnimLength = 1f; }

            BaseField chooseField = Fields[nameof(ChooseOption)];
            try  //We only handle the  first module
            {
                if (MAG.isDeployed)
                {
                    MAG.RetractModule();
                    chooseField.guiActive = false;
                    chooseField.guiActiveEditor = false;

                    this.Events["ModuleAnimationGroupEvent"].guiName = txtDeploy;
                    this.Events["ModuleAnimationGroupEvent"].guiActive = false;
                    //StartCoroutine(ModuleAnimationGroupEventCoroutine(AnimLength, 0.001f, Deploying: false));
                }
                else
                {
                    MAG.DeployModule();

                    //Update UI
                    //chooseField.guiActiveEditor = availableInEditor;
                    //chooseField.guiActive = availableInFlight;

                    //Debug.Log("updateConverter in 'ModuleAnimationGroupEvent'");
                    this.Events["ModuleAnimationGroupEvent"].guiName = txtRetract;
                    this.Events["ModuleAnimationGroupEvent"].guiActive = false;
                    //StartCoroutine(ModuleAnimationGroupEventCoroutine(AnimLength, 0.001f, Deploying: true));
                }
            }
            catch
            {
                //... do nothing
                GTIDebug.LogError(this.GetType().Name  + " -- ModuleAnimationGroup --- Error when handling animations");
            }
        }*/

        /*protected virtual IEnumerator ModuleAnimationGroupEventCoroutine(float starttime, float waitingtime, bool Deploying)
        {
            GTIDebug.Log("Start of ModuleAnimationGroupEventCoroutine()", iDebugLevel.High);
            yield return new WaitForSeconds(starttime);
            int _InvokeCounter = 0;
            while (_InvokeCounter++ < 600)
            {
                GTIDebug.Log("Coroutine Looping while animation is playing: " + _InvokeCounter, iDebugLevel.DebugInfo);
                if (Anim.isPlaying == false)
                {
                    GTIDebug.Log("'Animation finished playing' --> update Mode");
                    if (Deploying)
                    {
                        this.Fields["ChooseOption"].guiActiveEditor = availableInEditor;
                        this.Fields["ChooseOption"].guiActive = availableInFlight;
                        updateMultiMode(silentUpdate: true);
                    }
                    else
                    {
                        ModuleAnimationGroupEvent_DisableModules();
                    }

                    //Reactivate the gui button
                    this.Events["ModuleAnimationGroupEvent"].guiActive = true;
                    GTIDebug.Log("Coroutine should stop here", iDebugLevel.DebugInfo);
                    break;
                }
                else
                {
                    GTIDebug.Log("Coroutine will wait for " + waitingtime + " sec and run again", iDebugLevel.DebugInfo);
                    yield return new WaitForSeconds(waitingtime);
                    GTIDebug.Log(this.GetType().Name + " -- Coroutine have waited for " + waitingtime + " sec and continue", iDebugLevel.DebugInfo);
                }
            }
            //Reactivate the gui button if waiting failed
            if (_InvokeCounter >= 600)
            {
                this.Fields["ChooseOption"].guiActive = true;
                this.Events["ModuleAnimationGroupEvent"].guiActive = true;
                GTIDebug.LogError("ModuleAnimationGroupEventCoroutine failed to finish successfully");
            }
        }*/

        protected virtual void OnModuleAnimationGroupStateChanged(ModuleAnimationGroup module, bool Deploying)
        {
            GTIDebug.Log("Animation finished 'OnAnimationGroupStateChanged' --> update Mode", iDebugLevel.High);

            if (module != null && module.part != part) { GTIDebug.Log("triggering part is not this part", iDebugLevel.DebugInfo); return; }
            
            if (Deploying)
            {
                this.Fields["ChooseOption"].guiActiveEditor = availableInEditor;
                this.Fields["ChooseOption"].guiActive = availableInFlight;
                //updateMultiMode(silentUpdate: true);
                OnUpdateMultiMode(silentUpdate: true);
            }
            else
            {
                this.Fields["ChooseOption"].guiActiveEditor = false;
                this.Fields["ChooseOption"].guiActive = false;
                //externalToEVAOnly
                ModuleAnimationGroupEvent_DisableModules();
            }

            //Reactivate the gui button
            //this.Events["ModuleAnimationGroupEvent"].guiActive = true;
            GTIDebug.Log("OnAnimationGroupStateChanged executed", iDebugLevel.DebugInfo);
        }

        protected abstract void ModuleAnimationGroupEvent_DisableModules();
        #endregion

        #region VAB Information
        public override string GetInfo()
        {
            try
            {
                //Default editor tooltip: list the modes and any tech requirements. Subclasses with richer
                //info (RCS/Engine/EngineFX) override this and call ModeTechInfo()/ModuleTechInfo() inline.
                return BuildModesTechInfo(this.GetType().Name);
            }
            catch (Exception e)
            {
                GTIDebug.LogError(this.GetType().Name + " GetInfo() Error " + e.Message);
                throw;
            }
        }
        #endregion

        protected void onEVA(GameEvents.FromToAction<Part, Part> FromToData)
        {
            //Consistency Check
            GTIDebug.Log("onEVA", iDebugLevel.DebugInfo);
            if (FromToData.to == null || FromToData.from == null) return;
            GTIDebug.Log("Vessel: " + this.vessel.vesselName + "fromVessel: " + FromToData.from.vessel.vesselName + "toVessel: " + FromToData.to.vessel.vesselName, iDebugLevel.DebugInfo);
            GTIDebug.Log("isEVA: " + FromToData.to.vessel.isEVA, iDebugLevel.DebugInfo);

        }

        protected void onEVABoard(GameEvents.FromToAction<Part, Part> FromToData)
        {
            //Consistency Check
            GTIDebug.Log("onEVABoard", iDebugLevel.DebugInfo);
            if (FromToData.to == null || FromToData.from == null) return;
            GTIDebug.Log("Vessel: " + this.vessel.vesselName + "fromVessel: " + FromToData.from.vessel.vesselName + "toVessel: " + FromToData.to.vessel.vesselName, iDebugLevel.DebugInfo);
            GTIDebug.Log("isEVA: " + FromToData.to.vessel.isEVA, iDebugLevel.DebugInfo);
        }

        private void onVesselSwitching(Vessel fromVessel, Vessel toVessel)
        {
            //Consistency Check
            GTIDebug.Log("onVesselSwitching", iDebugLevel.DebugInfo);
            if (toVessel == null || fromVessel == null) return;
            GTIDebug.Log("isEVA: " + toVessel.isEVA, iDebugLevel.DebugInfo);
        }

        protected virtual void OnDestroy()
        {
            try
            {
                if (this.vessel.vesselName != null)
                    GTIDebug.Log(this.vessel.vesselName + " --> " + this.GetType().Name + " --> OnDestroy()");
            }
            catch { /* DO NOTHING */ }
            

            if (GameEvents.OnAnimationGroupStateChanged != null) GameEvents.OnAnimationGroupStateChanged.Remove(OnModuleAnimationGroupStateChanged);
            if (GameEvents.onCrewOnEva != null) GameEvents.onCrewOnEva.Remove(onEVA);
            if (GameEvents.onCrewBoardVessel != null) GameEvents.onCrewBoardVessel.Remove(onEVABoard);
            if (GameEvents.onVesselSwitching != null) GameEvents.onVesselSwitching.Remove(onVesselSwitching);
        }
    }
}