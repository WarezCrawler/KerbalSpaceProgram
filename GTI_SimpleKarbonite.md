# GTI_SimpleKarbonite (mod)

Minimal ModuleManager patch layer that extends **UmbraSpaceIndustries (USI)** Karbonite parts with
additional resource-conversion recipes. Not standalone — it only patches parts from USI.

- **GameData:** `T:\Kerbal Space Program\KSP1.9.1\GameData\GTI_SimpleKarbonite`
- **Author / License:** WarezCrawler, MIT

## Dependencies

- **Hard:** UmbraSpaceIndustries (patches use `:AFTER[UmbraSpaceIndustries]`), ModuleManager

## Folder structure

| Folder | Contents |
|--------|----------|
| `Parts/` | 4 `.cfg` files (patch layer only — no models/textures) |

## Files

- `KA_Distiller_125_01.cfg` — `@PART` patch adding `ModuleResourceConverter_USI` recipes to the
  distiller (Dust → Dirt → Ore, consuming ElectricCharge). Applied `:AFTER[UmbraSpaceIndustries]`.
- `KA_Distiller_250_01.cfg` — 2.5 m variant of the above.
- `KA_Converter_125_02.cfg`, `KA_Converter_250_01.cfg` — currently commented out / disabled.

## Resources involved

Dust, Dirt, Ore, ElectricCharge (all from USI / Community Resource Pack).
