# GTI_MultiModeHarvester (plugin)

Lets a single part switch between multiple `ModuleResourceHarvester` instances ("modes") — e.g. a
drill that can harvest different resources.

- **Source:** `Plugin/Guybrush101/GTI_MultiModeHarvester/GTI_MultiModeHarvester.cs`
- **Output DLL:** `GTI_MultiModeHarvester.dll`
- **Namespace:** `GTI`
- **Target framework:** .NET Framework 4.7.1
- **Depends on:** GTI_Utilities, Assembly-CSharp, UnityEngine
- **Module name (for .cfg):** `GTI_MultiModeHarvester`

## Class

```
GTI_MultiModeHarvester : GTI_MultiMode<MultiMode>
```

Targets the part's `List<ModuleResourceHarvester>`. On mode switch it enables the selected
harvester and disables/stops the others, and shows the current harvester's inputs and harvested
output resource on screen. Supports `ModuleAnimationGroup` gating.

The stock per-harvester actions (Toggle/Start/Stop Resource Converter) are disabled and replaced
by the module-level actions below, which route to the currently selected harvester (tracked via
`currentHarvester`) so a single action-group binding controls whichever mode is active.

## Config fields

Uses the shared `GTI_MultiMode<T>` fields only — see [GTI_Utilities.md](GTI_Utilities.md).
Modes map to the order of the `ModuleResourceHarvester` modules declared on the part.

## Actions

- `ActionActivate` ("Activate Harvester") — start the current harvester.
- `ActionShutdown` ("Shutdown Harvester") — stop the current harvester.
- `ActionToggle` ("Toggle Harvester") — toggle the current harvester.
- Inherited: `MultiModeAction_1…12`, `ActionNextMode`, `ActionPreviousMode`, `EVAChangeMode`.

## .cfg pattern

```
MODULE
{
    name = GTI_MultiModeHarvester
    availableInFlight = true
    // ... one ModuleResourceHarvester MODULE per mode declared on the part
}
```
