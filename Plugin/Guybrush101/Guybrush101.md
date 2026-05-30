# Guybrush101 (legacy / reference project)

Historical/reference code showing the evolution of the GTI suite before the current
`GTI_MultiMode<T>` architecture. **Not compiled** — every file is marked `<None>` in the
`.csproj`, so it ships nothing.

- **Source:** `Plugin/Guybrush101/Guybrush101/`
- **Assembly name (legacy):** `MightyPirate`
- **Target framework:** .NET Framework 3.5

## Contents (reference only)

| Folder | What it shows |
|--------|---------------|
| `Core/` | `EngineClassSwitch`, `EngineClassSwitch_2`, `IntakeSwitch` — predecessors of the MultiMode engine/intake modules |
| `ISRU/` | `ISRUSwitch` — predecessor of the MultiMode converter |
| `Scenarios/` | `GTES_ScenarioModule` |
| `UI_Examples/` | `TEST_MODULE`, `UI_CHOOSEOPTION` — `UI_ChooseOption` usage examples |
| `Other/` | old `DockingAlignmentIndicator` (now in GTI_Utilities) |
| `Utilities/`, `Utilities_CustomTypes/` | legacy helpers |

## Why it's still here

Useful as a reference when extending the current modules — the older switch classes spell out the
raw KSP API interactions that `GTI_MultiMode<T>` now abstracts. For active development use the
current plugins instead; see the other `*.md` files in this folder.

## Solution

`GuybrushThreepwoodIndustries.sln` (parent folder) references 6 projects: this one (`MightyPirate`,
no compile) plus the 5 active plugins (`GTI_Utilities` and the four `GTI_MultiMode*`). See
`README.md` and `Licence` (MIT) in the parent folder.
