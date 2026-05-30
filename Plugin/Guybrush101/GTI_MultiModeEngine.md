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
- `MultiModeRCS.cs` — `GTI_MultiModeRCS` (switchable `ModuleRCS`; see its section below).
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
| `engineID_onFlameout` | Optional. Semicolon-separated list parallel to `engineID`: the mode to auto-switch **to** when that mode's engine flames out. Omit for no auto-switch. |
| `autoSwitchEnabled` | Persistent on/off toggle for auto-switch (default on). Only shown when `engineID_onFlameout` is set. |

Plus the shared `GTI_MultiMode<T>` fields — see [GTI_Utilities.md](GTI_Utilities.md).

### Auto-switch on flameout

When `engineID_onFlameout` is supplied, the module watches the active engine each frame; if it
flames out, it switches to that mode's mapped fallback and re-ignites it — the generalisation of
stock `MultiModeEngine`'s air-breathing→closed-cycle behaviour to N modes.

- The list is **parallel to `engineID`**: entry *i* is the mode to jump to when mode *i* flames out.
- A mode that **maps to itself** (or names an unknown engineID) does **not** switch — it just flames
  out as normal. This is also the behaviour when `engineID_onFlameout` is omitted entirely.
- A switch only happens if the **target engine can actually start** (`CanStart()` — i.e. has
  propellant/conditions), so it won't flip to a mode that is also dead.
- Chains (`A→B→C`) and bidirectional maps (`A↔B`, e.g. relight air-breathing on descent) are both
  expressible.
- The player can disable it in flight via the **Auto-switch on flameout** toggle.

Example — air-breathing falls back to closed-cycle, closed-cycle stays put:
```
engineID            = AirBreathing;ClosedCycle
engineID_onFlameout = ClosedCycle;ClosedCycle
//                    ^AirBreathing flames out -> ClosedCycle
//                                 ^ClosedCycle maps to itself -> no switch (plain flameout)
```

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
    engineID_onFlameout = closedCycle;closedCycle   // optional: air-breathing falls back to closed-cycle
    availableInFlight = true
    // ... one ModuleEnginesFX MODULE per engineID listed above
}
```

Used in GTIndustries by the CR-13 R.A.P.T.O.R. and NRX "KINKI" engines.

## GTI_MultiModeRCS

The RCS counterpart of `GTI_MultiModeEngineFX`: switches a part between several stock `ModuleRCS`
thrusters, keeping exactly one enabled at a time.

```
GTI_MultiModeRCS : GTI_MultiMode<MultiMode>
```

Targets the part's `List<ModuleRCS>`. Since `ModuleRCS` has no `engineID`, **modes are matched by
order** — mode *i* drives the *i*-th `ModuleRCS` on the part. On a switch, the selected module is
enabled (`moduleIsEnabled` + `isEnabled` = true, so it thrusts and shows its right-click UI) and
every other module is disabled (`rcsEnabled`/`moduleIsEnabled`/`isEnabled` = false). `rcsEnabled =
false` is the key one — it's the stock toggle's flag, and the thruster's `FixedUpdate` gate is
`moduleIsEnabled && rcsEnabled`, so clearing it stops the inactive thruster from thrusting **and
from consuming fuel**. The previous mode's enabled/disabled state is carried to the new one, and the
stock per-thruster `ToggleAction` is hidden so this module is the single control point.

> **Each mode must use its own `runningEffectName`** (the demo patch gives the LiquidFuel mode a
> `running_lf` effect). Every `ModuleRCSFX` runs `FixedUpdate` each frame and the inactive one calls
> `part.Effect(runningEffectName, 0)`; if two modes shared an effect, the one declared last would
> zero it every frame and the other's jet would never appear. (Same reason each stock engine mode
> uses a distinct effect name.)
>
> **Symmetry:** flight switching of symmetric blocks is handled by the base class wiring
> `onSymmetryFieldChanged` (KSP otherwise copies the value to counterparts without running their
> switch, leaving them on the old mode). So all symmetric blocks change together — set
> `affectSymCounterpartsInFlight = true`.

### Config-specific field

| Field | Meaning |
|-------|---------|
| `GUIRCSID` | Optional semicolon-separated display names, one per `ModuleRCS`. Defaults to each thruster's `resourceName`. |
| `RCSID` | Reserved/unused — kept for symmetry with the engine module; RCS has no per-module id. |

Plus the shared `GTI_MultiMode<T>` fields — see [GTI_Utilities.md](GTI_Utilities.md).

### Actions

- `ActionActivate` ("Enable RCS"), `ActionShutdown` ("Disable RCS"), `ActionToggle` ("Toggle RCS")
  — act on the currently selected thruster's `rcsEnabled`.
- Inherited: `MultiModeAction_1…12`, `ActionNextMode`, `ActionPreviousMode`, `EVAChangeMode`.

### .cfg pattern

```
MODULE
{
    name = GTI_MultiModeRCS
    GUIRCSID = Monoprop;Cold Gas
    availableInFlight = true
    // ... one ModuleRCS MODULE per mode declared on the part, in matching order
}
```
