# GTI_MultiModeConverter (plugin)

Lets a single part switch between multiple `ModuleResourceConverter` instances ("modes"),
exposing one selector instead of separate converters.

- **Source:** `Plugin/Guybrush101/GTI_MultiModeConverter/GTI_MultiModeConverter.cs`
- **Output DLL:** `GTI_MultiModeConverter.dll`
- **Namespace:** `GTI`
- **Target framework:** .NET Framework 4.7.1
- **Depends on:** GTI_Utilities, Assembly-CSharp, UnityEngine
- **Module name (for .cfg):** `GTI_MultiModeConverter`

## Class

```
GTI_MultiModeConverter : GTI_MultiMode<MultiMode>
```

Targets the part's `List<ModuleResourceConverter>`. On mode switch it activates the selected
converter and stops the others, and shows the selected mode's name plus its recipe inputs/outputs
on screen. The stock converter action buttons (Toggle/Start/Stop) are hidden and driven internally
via the module-level actions below (routed through `currentConverter`). Supports
`ModuleAnimationGroup` gating.

## Config fields

Uses the shared `GTI_MultiMode<T>` fields — see [GTI_Utilities.md](GTI_Utilities.md)
(`availableInFlight`, `availableInEditor`, `externalToEVAOnly`, `useModuleAnimationGroup`,
`affectSymCounterpartsInFlight`, `ChooseOption`). No converter-specific fields; modes map to the
order of the `ModuleResourceConverter` modules in the part.

## Actions

- `ActionActivate` — start the current converter.
- `ActionShutdown` — stop the current converter.
- `ActionToggle` — toggle the current converter.
- Inherited: `MultiModeAction_1…12`, `ActionNextMode`, `ActionPreviousMode`, `EVAChangeMode`.

## .cfg pattern

```
MODULE
{
    name = GTI_MultiModeConverter
    availableInFlight = true
    availableInEditor = true

    // --- Tech gating (optional; semicolon list parallel to the converter MODULE order) ---
    techRequired = ;advConstruction    // unlock per mode; blank = always available (mode 0 always free)
    techObsolete = ;                   // retire per mode once researched; blank = never removed
    // moduleTechRequired = ...         // hides the whole selector until researched

    // ... one ModuleResourceConverter MODULE per mode declared on the part
}
```

See the tech-gating section in [GTI_Utilities.md](GTI_Utilities.md) for the freeze rules and the
"upgrade" (`techObsolete`) pattern.
