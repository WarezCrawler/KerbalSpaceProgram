# GTI_Utilities (mod)

Core plugin framework for the GTI mod suite. Provides the "MultiMode" part modules plus a set
of optional quality-of-life utilities. This is the deployed GameData mod; its C# source lives in
`Plugin/Guybrush101/` (see the per-plugin docs there).

- **GameData:** `T:\Kerbal Space Program\KSP1.9.1\GameData\GTI_Utilities`
- **Author / License:** WarezCrawler, MIT (Copyright 2016)
- **Hard dependency:** ModuleManager

## Folder structure

| Folder | Contents |
|--------|----------|
| `Plugins/` | The 5 GTI DLLs + `GTI_Config.cfg` + `PluginData/` |
| `Patches/` | ModuleManager patches wiring the MultiMode modules onto stock/other parts |
| `Resources/` | `MM_IntakeAtm.cfg` — defines the `IntakeAtm` resource for air-breathing engines |

## Shipped DLLs

- `GTI_Utilities.dll` — shared base classes + utilities (dependency for the others)
- `GTI_MultiModeConverter.dll`
- `GTI_MultiModeEngine.dll`
- `GTI_MultiModeHarvester.dll`
- `GTI_MultiModeIntake.dll`

## Patches (`Patches/`)

- `GTI_MultiModeIntake__MM.cfg`
- `GTI_MultiModeHarvester__MM.cfg`
- `GTI_MultiModeConverter__MM.cfg`

These apply the GTI MultiMode modules to applicable parts.

## Configuration — `Plugins/GTI_Config.cfg`

- **EventConfig** — event system polling frequencies (idle vs. active throttle-change detection).
- **MISCELLANEOUS** — feature toggles: `LoadFixer`, `DAI` (Docking Alignment Indicator),
  `DoubleTabForBrakeLock`, `CameraFocusChanger`, `CrowdSourcedScienceFixer`, `ProjectManager`.
- **Debug** — log verbosity (Low / Medium / High / VeryHigh / DebugInfo).

## Provided part modules

`GTI_MultiModeConverter`, `GTI_MultiModeEngineFX`, `GTI_MultiModeHarvester`,
`GTI_MultiModeIntake`. See `Plugin/Guybrush101/*.md` for each module's config-facing API.

## Notes

- A `Plugins_20210725.7z` archive sits in the mod root (backup of an earlier plugin build).
