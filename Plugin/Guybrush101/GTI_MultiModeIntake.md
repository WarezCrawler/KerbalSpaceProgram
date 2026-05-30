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

## Config fields

| Field | Default | Meaning |
|-------|---------|---------|
| `GUINames` | "" | Display names for the intake modes |
| `selectedModeStatus` | true | Current intake on/off state |
| `resMaxAmount` | "" | Reserved for future use |
| `preserveResourceNodes` | false | Keep existing RESOURCE nodes instead of auto-managing them |

Plus shared `GTI_MultiMode<T>` fields — see [GTI_Utilities.md](GTI_Utilities.md).

## Events / Actions

- `IntakeActivate()` — "Open Intake (GTI)".
- `IntakeDeactivate()` — "Close Intake (GTI)".
- `GetStatus()` — debug: list intake module properties.
- `ToggleAction` — toggle intake on/off.
- Inherited: `MultiModeAction_1…12`, `ActionNextMode`, `ActionPreviousMode`, `EVAChangeMode`.

## .cfg pattern

```
MODULE
{
    name = GTI_MultiModeIntake
    GUINames = Air;Karbonite Atm
    availableInFlight = true
    // ... one ModuleResourceIntake MODULE per intake mode declared on the part
}
```
