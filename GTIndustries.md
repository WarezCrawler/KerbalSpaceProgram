# GTIndustries (mod)

The flagship GTI content mod: parts, engines, custom resources, science, FTL, and a large set of
compatibility patches for other mods. Establishes the "Guybrush Threepwood Industries" /
"Threepwood Consortium" manufacturer category.

- **GameData:** `T:\Kerbal Space Program\KSP1.9.1\GameData\GTIndustries`
- **Author / License:** WarezCrawler, MIT (Copyright 2016)

## Dependencies

**Hard:** ModuleManager, SmokeScreen (NRX nuclear engine + FTL drives).

**Major mod integrations:** UmbraSpaceIndustries (USI — extensive container/fuel-switch/resource
integration, Karbonite/Karborundum), Community Resource Pack resources, Interstellar Fuel Switch,
B9 Part Switch, Solaris Hypernautics (FTL model/reactors), FTL Drive Continued.

**Optional / conditional:** MechJeb (patches provided), KW Rocketry (`!KWRocketry` fallback SRBs),
DMagic, SCANsat, FilterExtensions (category filtering). See `Dependencies.txt` in the mod root.

## Folder structure

| Folder | Contents |
|--------|----------|
| `Parts/` | ~50+ parts across Collectors, Engines, FASA_Launch_Clamp_125, FTL, Heat_Handling, Science, Solids, Weldings |
| `Patches/` | ~70+ ModuleManager patches (USI, MechJeb, fuel switch, drills, ISRU, Solaris, FTL, etc.) |
| `Patches_FX/` | FX/effect patches |
| `Resources/` | Custom resource definitions + asteroid/SCAN patches |
| `FX/` | CryoEngines + HotRockets effect packs (see `_readme.txt` files) |
| `Category/` | `Category.cfg` — FilterExtensions manufacturer category |
| `Contracts/` | `MM_Contracts.cfg` — custom contracts |
| `Soundbank/` | Audio assets |
| `Plugins/` | DLLs |

## Custom resources (`Resources/MM_Resources.cfg`)

| Resource | density | unitCost | Notes |
|----------|---------|----------|-------|
| DarkGoo | 0.001 | 500 | High-ISP propellant |
| WhiteGoo | 0.001 | 1500 | |
| PlasmaGoo | — | — | |
| ExoticGoo | — | 5000 | Produced by fusing DarkGoo + WhiteGoo |

## Notable parts

- **Engines** — Icarus heat-exchanger turbojet (`IntakeAtm`, 300 kN); CR-13 R.A.P.T.O.R.
  multi-mode engine (`GTI_MultiModeEngineFX`: AirBreathingCruise/AirBreathing/AtmBreathing/
  ClosedCycle/ClosedCycleDarkGoo); NRX "KINKI" atomic rocket (modes Normal/HighVelocity/Overdrive,
  needs SmokeScreen); Goo fusion engines; ion engine; ducted fan.
- **Collectors** — GT Exospheric Generator (Karbonite/Karborundum/DarkMatter/Dust); particle
  collectors (1.25 m / 2.5 m).
- **Science** — Science Experiment Box A (basic) and B (advanced) bundling stock experiments.
- **Solids** — KW-style SRBs (0.25/0.75/1.5/2.5 m), ullage boosters.
- **FTL** — VX 666 Hyperdrive (needs SmokeScreen + FTLDriveContinued + SolarisHypernautics; uses
  DarkGoo).
- **Structures** — SS-Structural-X girders, heat-pipe radiators, FASA-style launch clamps.

## Key features

1. Custom Goo propellant economy (Dark/White/Plasma/Exotic).
2. Multi-mode engines via `GTI_MultiModeEngineFX`.
3. ~80 ModuleManager compatibility patches across the mod ecosystem.
4. FTL travel via the VX 666 Hyperdrive.
5. FilterExtensions manufacturer category.
