# GTI_MultiModeIntake (plugin)

Lets a single part switch between multiple `ModuleResourceIntake` instances ("modes"), each tied
to a different intake resource. Unlike the other MultiMode modules, it **dynamically rewrites the
part's RESOURCE nodes** to match the selected intake.

- **Source:** `Plugin/Guybrush101/GTI_MultiModeIntake/GTI_MultiModeIntake.cs`
- **Output DLL:** `GTI_MultiModeIntake.dll`
- **Namespace:** `GTI`
- **Target framework:** .NET Framework 4.7.1
- **Depends on:** GTI_Utilities, Assembly-CSharp, UnityEngine
- **Module name (for .cfg):** `GTI_MultiModeIntake`

## Classes

```
GTI_MultiModeIntake : GTI_MultiMode<IntakeModes>
IntakeModes : IMultiMode   // moduleIndex, ID, Name, resourceName
```

Targets the part's `List<ModuleResourceIntake>`. The mode type is the custom `IntakeModes`
(adds `resourceName`). The `ChooseOption` selector is disabled for intakes; switching is driven by
the activate/deactivate events instead.

### Behavior

- Enables/disables each intake independently.
- On switch, dynamically creates/updates the part `RESOURCE` node for the selected intake's
  `resourceName` (and removes the others unless `preserveResourceNodes = true`).
- Refreshes the KSP resource panel when switching in flight.
- Supports symmetry counterparts when `affectSymCounterpartsInFlight = true`.

### Auto-manage (auto open/close)

Optionally closes the intake automatically as air thins out and reopens it when air returns —
so the player no longer has to babysit intakes on ascent/descent. Armed per-part via the
`autoManage` toggle (right-click UI + action group), persistent and synced across symmetry.

- **Close** reads the active `ModuleResourceIntake.airFlow` (live, authoritative). The intake
  closes only when **both** the airflow stays at/below `autoCloseThreshold` **and** static
  pressure is below `autoCloseMaxPressure`, sustained for `autoCloseDelay` seconds. The pressure
  guard scopes auto-close to leaving the usable atmosphere (to/from space) and prevents a false
  close when stationary in dense air (e.g. an `intakeSpeed = 0` intake parked at sea level, where
  airflow is purely motion-driven).
- **Reopen** can't read the live field (KSP freezes `airFlow` once an intake is closed), so the
  module re-derives the *potential* airflow itself — same formula and gates as the stock
  `FixedUpdate` (shielding, oxygen, `kPaThreshold`, underwater, occlude-node). When it rises
  to/above `autoOpenThreshold` for `autoOpenDelay` seconds, the intake reopens.
- `autoOpenThreshold > autoCloseThreshold` gives hysteresis so it can't flap at the boundary.
- Evaluation runs on a low-cadence main-thread coroutine (`autoCheckInterval`), not every frame
  and not on a worker thread (the calc touches the part Transform / `FloatCurve`, which are
  main-thread only).
- **Manual override:** clicking Open/Close (or the toggle action) starts an `autoManualCooldown`
  grace period (default 10 s) during which auto-manage holds off, so it won't instantly revert
  the player. Auto stays armed and resumes once the cooldown elapses. To stop auto entirely,
  switch the `autoManage` toggle Off.
- The `autoCloseThreshold` default (~0) plus the `autoCloseMaxPressure` default (the part's own
  `kPaThreshold`) ties closing to the pressure below which KSP itself produces no air — universal
  across intakes, no per-part tuning. Raise `autoCloseThreshold` (and `autoCloseMaxPressure`) to
  close earlier in thin-but-present air to cut intake drag.

## Config fields

| Field | Default | Meaning |
|-------|---------|---------|
| `GUINames` | "" | Display names for the intake modes |
| `selectedModeStatus` | true | Current intake on/off state |
| `resMaxAmount` | "" | Reserved for future use |
| `preserveResourceNodes` | false | Keep existing RESOURCE nodes instead of auto-managing them |
| `autoManage` | false | Arm auto open/close (persistent; also a right-click toggle + action) |
| `autoCloseThreshold` | 0.001 | Live `airFlow` (units/s) at/below which the intake auto-closes |
| `autoCloseMaxPressure` | -1 | Only auto-close below this static pressure (kPa); -1 = use the intake's own `kPaThreshold` |
| `autoOpenThreshold` | 0.005 | Potential airflow (units/s) at/above which the intake auto-reopens (≥ close threshold) |
| `autoCloseDelay` | 2.0 | Seconds the close condition must persist before closing |
| `autoOpenDelay` | 2.0 | Seconds the open condition must persist before reopening |
| `autoCheckInterval` | 0.25 | Auto-manage evaluation cadence (s) |
| `autoManualCooldown` | 10 | Grace period (s) after a manual open/close before auto-manage resumes |

Plus shared `GTI_MultiMode<T>` fields — see [GTI_Utilities.md](GTI_Utilities.md).

## Events / Actions

- `IntakeActivate()` — "Open Intake (GTI)". Starts the `autoManualCooldown` grace period.
- `IntakeDeactivate()` — "Close Intake (GTI)". Starts the `autoManualCooldown` grace period.
- `GetStatus()` — debug: list intake module properties.
- `ToggleAction` ("Toggle Intake") — toggle intake on/off.
- `ToggleAutoManageAction` ("Toggle Auto-manage Intake") — arm/disarm auto open/close.
- `autoManage` — right-click toggle ("Auto-manage Intake (GTI)"); `autoStatusGUI` shows live state.
- Inherited: `MultiModeAction_1…12`, `ActionNextMode`, `ActionPreviousMode`, `EVAChangeMode`.

## .cfg pattern

```
MODULE
{
    name = GTI_MultiModeIntake
    GUINames = Air;Karbonite Atm
    availableInFlight = true
    autoManage = true          // arm auto open/close by default (optional)
    // autoCloseThreshold = 0.05   // raise to close earlier in thin air (cuts drag)

    // --- Tech gating (optional; semicolon list parallel to the intake MODULE order) ---
    // techRequired = ;advExploration   // unlock per mode; blank = always available (mode 0 always free)
    // techObsolete = ;                 // retire per mode once researched; blank = never removed

    // ... one ModuleResourceIntake MODULE per intake mode declared on the part
}
```

Tech gating is shared by all MultiMode modules — see the tech-gating section in
[GTI_Utilities.md](GTI_Utilities.md) for the freeze rules and the "upgrade" (`techObsolete`) pattern.
