# GTI_MultiModeEngine (plugin)

Lets a single part switch between multiple `ModuleEnginesFX` instances ("modes") — e.g. an
air-breathing/closed-cycle multi-mode engine — preserving ignition state across switches.

- **Source:** `Plugin/Guybrush101/GTI_MultiModeEngine/`
- **Output DLL:** `GTI_MultiModeEngine.dll`
- **Namespace:** `GTI`
- **Target framework:** .NET Framework 4.7.1
- **Depends on:** GTI_Utilities, Assembly-CSharp, UnityEngine
- **Primary module name (for .cfg):** `GTI_MultiModeEngineFX`

## Source files

- `MultiModeEngineFX.cs` — main module (`GTI_MultiModeEngineFX`).
- `MultiModeRCS.cs` — `GTI_MultiModeRCS` (**incomplete** — throws `NotImplementedException`).
- `MultiModeEngine.cs` — engine mode data class (incomplete).
- `DEBUG.cs` — debug helpers (not compiled).

## GTI_MultiModeEngineFX

```
GTI_MultiModeEngineFX : GTI_MultiMode<MultiMode>
```

Targets the part's `List<ModuleEnginesFX>`, matched by `engineID`. Stores ignition state and
re-ignites the newly selected engine if the previous one was running. Stock engine actions
(OnAction/Activate/Shutdown) are deactivated and managed internally.

### Engine-specific config fields

| Field | Meaning |
|-------|---------|
| `engineID` | Semicolon-separated list of `ModuleEnginesFX.engineID`s, one per mode |
| `GUIengineID` | Semicolon-separated display names, parallel to `engineID` |

Plus the shared `GTI_MultiMode<T>` fields — see [GTI_Utilities.md](GTI_Utilities.md).

### Actions

- `ActionActivate` — ignite current engine.
- `ActionShutdown` — shut down current engine.
- `ActionToggle` — toggle current engine ignition.
- Inherited: `MultiModeAction_1…12`, `ActionNextMode`, `ActionPreviousMode`, `EVAChangeMode`.

### .cfg pattern

```
MODULE
{
    name = GTI_MultiModeEngineFX
    engineID = airBreathing;closedCycle
    GUIengineID = Air-Breathing;Closed Cycle
    availableInFlight = true
    // ... one ModuleEnginesFX MODULE per engineID listed above
}
```

Used in GTIndustries by the CR-13 R.A.P.T.O.R. and NRX "KINKI" engines.

## GTI_MultiModeRCS (incomplete)

Intended `ModuleRCS` equivalent. Fields `RCSID` / `GUIRCSID` mirror the engine pattern, but the
implementation is unfinished — do not use in production configs.
