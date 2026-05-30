# GTI_MultiModeHarvester (plugin)

Lets a single part switch between multiple `ModuleResourceHarvester` instances ("modes") — e.g. a
drill that can harvest different resources.

- **Source:** `Plugin/Guybrush101/GTI_MultiModeHarvester/GTI_MultiModeHarvester.cs`
- **Output DLL:** `GTI_MultiModeHarvester.dll`
- **Namespace:** `GTI_MultiModeHarvester`
- **Target framework:** .NET Framework 4.7.1
- **Depends on:** GTI_Utilities, Assembly-CSharp, UnityEngine
- **Module name (for .cfg):** `GTI_MultiModeHarvester`

## Class

```
GTI_MultiModeHarvester : GTI_MultiMode<MultiMode>
```

Targets the part's `List<ModuleResourceHarvester>`. On mode switch it enables the selected
harvester and disables the others, hides the stock harvester action buttons, and shows the
current harvester's inputs/outputs on screen. Supports `ModuleAnimationGroup` gating.

## Config fields

Uses the shared `GTI_MultiMode<T>` fields only — see [GTI_Utilities.md](GTI_Utilities.md).
Modes map to the order of the `ModuleResourceHarvester` modules declared on the part.

## Actions

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
