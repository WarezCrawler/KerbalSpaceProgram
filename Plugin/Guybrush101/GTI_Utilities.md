# GTI_Utilities (plugin — shared library)

Shared base classes and utilities for the GTI plugin suite. Every other GTI MultiMode plugin
depends on this assembly.

- **Source:** `Plugin/Guybrush101/GTI_Utilities/`
- **Output DLL:** `GTI_Utilities.dll`
- **Namespace:** `GTI` (most types; `GTI_Events.cs` types are in `GTI.Events`)
- **Target framework:** .NET Framework 4.7.1
- **References:** Assembly-CSharp, Assembly-CSharp-firstpass, UnityEngine (Core/CoreModule/
  InputLegacyModule/PhysicsModule/UI), System

## The MultiMode base — `BaseClass/MultiMode.cs`

The heart of the suite. All MultiMode part modules derive from the generic abstract base.

```
PartModule
  └─ GTI_MultiMode<T> where T : IMultiMode   (abstract)
```

- `IMultiMode` — interface defining a mode's data (`moduleIndex`, `ID`, `Name`).
- `MultiMode : IMultiMode` — default concrete mode type (used by Converter/Engine/Harvester).
- `GTI_MultiMode<T>` — generic base: mode list, selection, GUI generation, action handling,
  EVA switching, symmetry sync, and optional `ModuleAnimationGroup` integration.

### Config-facing fields shared by all subclasses (KSPField)

| Field | Default | Meaning |
|-------|---------|---------|
| `availableInFlight` | false | Show mode selector in flight |
| `availableInEditor` | true | Show mode selector in VAB/SPH |
| `externalToEVAOnly` | false | Restrict mode changes to EVA crew |
| `useModuleAnimationGroup` | false | Gate mode availability on a `ModuleAnimationGroup` deploy state |
| `affectSymCounterpartsInFlight` | false | Sync symmetry counterparts when switching in flight |
| `messagePosition` | "" | On-screen message placement |
| `techRequired` | "" | Semicolon list of tech-node IDs, one per mode (parallel to mode order); blank entry = no tech. Gates which modes are selectable. Mode 0 is always available. |
| `moduleTechRequired` | "" | Single tech-node ID gating the **whole** selector; until researched the mode menu is hidden entirely. |
| `unlockedModes` | "" | **Persistent.** Per-part frozen snapshot of unlocked mode IDs (comma list). Saved with the vessel — do not set by hand. |
| `ChooseOption` | — | `UI_ChooseOption` field holding the selected mode ID |

### Tech-gated mode unlocking (shared, inherited by all subscribers)

Modes can be locked behind tech-tree nodes via `techRequired` (per-mode) and/or `moduleTechRequired`
(whole selector). Core rule: **a vessel already in flight keeps exactly the modes it launched with —
researching new tech never changes it.** Only the editor (designing a fresh craft) and an in-flight
EVA "service" action resync a part to the current tech tree.

- **Editor** — the unlocked set is recomputed live from current tech every time the part loads.
- **Flight load** — the persisted `unlockedModes` snapshot is trusted verbatim (the freeze). An empty
  snapshot (legacy/spawned craft) is seeded once from live tech.
- **EVA** — `EVAUpgradeModes()` resyncs that one part to current tech and rebuilds its menu.
- Sandbox (no R&D instance): everything is unlocked automatically.
- Locked modes stay in the `modes` list (indices/IDs stay stable for order-matched modules like RCS);
  they are simply filtered out of the selector. Mode 0 (the part's original behaviour) is always
  unlocked.

Example (RCS with 4 modes — original always free, the rest tech-gated):
```
techRequired = ;advFlightControl;ionPropulsion;experimentalScience
```

Internals: `ParseTechRequired()`, `ComputeLiveUnlockedSet()`, `ApplyTechUnlocks()` (called in the
init flow before `initializeGUI()`), `IsModeUnlocked(int)`, `ModuleTechLocked()`,
`BuildVisibleOptions(...)`, `RefreshModeOptions()`, and `TechResearched(string)` (static helper).

**Editor tooltips (`GetInfo`)** show each mode's tech requirement and a coloured researched/locked
status. Helpers: `ModeTechInfo(int)` (per-mode tag), `ModuleTechInfo()` (module-level gate tag), and
`BuildModesTechInfo(header)` (full mode listing + tags). The base `GetInfo()` uses
`BuildModesTechInfo` by default (Intake and any future subclass); RCS/Engine/EngineFX override
`GetInfo()` and call `ModeTechInfo`/`ModuleTechInfo` inline within their richer panels. Tags resolve
the readable tech title via `ResearchAndDevelopment.GetTechnologyTitle`.

### Shared actions/events

- `EVAChangeMode()` (KSPEvent) — change mode from EVA (cycles only unlocked modes).
- `EVAUpgradeModes()` (KSPEvent) — EVA "service / upgrade": resync this part to the current tech tree.
  Only active when the part has tech gating **and** the game has R&D (career/science).
- `MultiModeAction_1` … `MultiModeAction_12` — direct "set mode #N" actions (ignore locked modes).
- `ActionNextMode()` / `ActionPreviousMode()` — cycle modes (skip locked modes).

### Key overridable members (for subclasses)

`initializeSettings()`, `updateMultiMode(bool silentUpdate)`, `writeScreenMessage()`,
`ModuleAnimationGroupEvent_DisableModules()` (abstract); `initializeGUI()`, `selectMode()`,
`FindSelectedMode()`, `selModeFromChooseOption()` (virtual).

## Other components

- **`GTI_Events.cs`** (namespace `GTI.Events`)
  - `GTI_EventCreator` (`KSPAddon` MainMenu) — registers a custom `onThrottleChange<float,float>`
    (current, previous) game event.
  - `GTI_Events` (`KSPAddon` Flight) — background thread polling throttle; fires `onThrottleChange`;
    runs the scene-load "fixer" to prevent on-load explosions. Poll rate configurable
    (`GTIConfig.Event.CheckFreqIdle` ≈ 250 ms, `CheckFreqActive` ≈ 90 ms).
- **`BackgroundDetectors.cs`** — `BackgroundDetector_Flight` (`KSPAddon` Flight): NavBall docking
  alignment indicator (DAI) detection + double-tap brake lock. Gated by `GTIConfig`.
- **`PartModules/GTI_ModuleAsteroidDrill.cs`** — `GTI_ModuleAsteroidDrill : BaseDrill`. Fields:
  `DirectAttach` (require direct asteroid attach), `PowerConsumption` (EC/s, default 1),
  `RockOnly`. Raycasts for asteroid contact and validates situation before drilling.
- **`Utilities/`** — general functions, config loader (`GTIConfig`), debug/logging, settings, DAI,
  CameraFocusChanger.
- **`Utilities_CustomTypes/`** — PropellantList, EngineSwitchList, engineMultiModeList (not all
  compiled).

## Config file

Ships `GTI_Config.cfg` (deployed to `GameData/GTI_Utilities/Plugins/`). Sections: `EventConfig`,
`MISCELLANEOUS` (feature toggles), `Debug` (log level).

## Build output

DLLs/config are written to the live GameData plugins folder via the `.csproj` post-build path.

## Licensing

GTI code is MIT (see `Plugin/Guybrush101/Licence`), **except** for
`Utilities/DockingAlignmentIndicator/DAI.cs`, which is derived from the "NavBall Docking
Alignment Indicator" mod by mic-e (maintained by linuxgurugamer) and is **GPLv3**. Because
DAI.cs is compiled into `GTI_Utilities.dll`, distribution of that binary is subject to GPLv3
with respect to the derived component. See the DAI.cs header and the repo `LICENSE` for details.
