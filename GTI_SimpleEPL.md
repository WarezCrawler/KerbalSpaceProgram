# GTI_SimpleEPL (mod)

Construction/infrastructure mod built around **Extraplanetary Launchpads (EL)**. Adds workshop
capability to command pods/labs and provides construction parts and tooling.

- **GameData:** `T:\Kerbal Space Program\KSP1.9.1\GameData\GTI_SimpleEPL`
- **Author / License:** WarezCrawler, MIT (`Licence.txt`)

## Dependencies

- **Hard:** Extraplanetary Launchpads (EL), ModuleManager
- **Optional:** KIS (Kerbal Inventory System) — required for the Mallet tool

## Folder structure

| Folder | Contents |
|--------|----------|
| `Agencies/` | `Agents.cfg` — "Kairyuu Shipping" agency |
| `MM_Patches/` | Patches adding `ELWorkshop` + `ELSurveyStation` to command pods/labs (ProductivityFactor 4–7) |
| `Parts/` | HexCanRocketParts, launchpad2, Mallet, MicroPad, OrbitalDock, SurveyStake |
| `Resources/` | `Recipes.cfg` (EL workshop recipes), `Experience_Traits.cfg` |
| `Plugins/` | `Launchpad.dll` |
| `Textures/` | Part textures |

## Parts

- **HexCanRocketParts** — `RocketParts` resource containers (Small/Normal/Large/Huge, 1.5 m form).
- **launchpad2** — off-planet launchpad (CrewCapacity 2, vesselType Base).
- **MicroPad** — small launchpad.
- **OrbitalDock** — docking port.
- **SurveyStake** — EL survey marker.
- **Mallet** — KIS-compatible construction tool (needs KIS).

## Patches

`MM_Patches/MM_MainPatch.cfg` injects `ELWorkshop` and `ELSurveyStation` into stock command
modules and labs. `MM_Workshop.cfg` holds workshop config. `Resources/Recipes.cfg` defines EL
workshop crafting recipes (MaterialKits, SolidFuel, EVA Propellant, etc.).
