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
| `ChooseOption` | — | `UI_ChooseOption` field holding the selected mode ID |

### Shared actions/events

- `EVAChangeMode()` (KSPEvent) — change mode from EVA.
- `MultiModeAction_1` … `MultiModeAction_12` — direct "set mode #N" actions.
- `ActionNextMode()` / `ActionPreviousMode()` — cycle modes.

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
