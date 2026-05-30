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

## Custom resources

GTIndustries adds a family of four "Goo" propellants used by its advanced engines. The
resource definitions live in `Resources/MM_Resources.cfg`; where each one is *found* (asteroid,
planetary, biome distributions for the Community Resource Pack) lives in the per-resource files
`Resources/DarkGoo.cfg`, `WhiteGoo.cfg`, and `ExoticGoo.cfg`. SCANsat scanner support is in
`Resources/MM_SCANresource.cfg`.

### Resource definitions (`Resources/MM_Resources.cfg`)

All four share the same physical definition — only `unitCost` differs:

| Resource | unitCost | density | flowMode | transfer | tweakable | visible | color |
|----------|---------:|---------|----------|----------|-----------|---------|-------|
| **DarkGoo** | 500 | 0.001 | ALL_VESSEL | PUMP | false | true | red (1,0,0) |
| **WhiteGoo** | 1500 | 0.001 | ALL_VESSEL | PUMP | false | true | red (1,0,0) |
| **PlasmaGoo** | 3000 | 0.001 | ALL_VESSEL | PUMP | false | true | red (1,0,0) |
| **ExoticGoo** | 5000 | 0.001 | ALL_VESSEL | PUMP | false | true | red (1,0,0) |

- `density = 0.001` → **1 kg per unit** (`volume = 1`, so 1 unit = 1 L).
- `flowMode = ALL_VESSEL` → drawn from anywhere on the vessel, like LiquidFuel.
- `transfer = PUMP` → crew can pump it between tanks.
- `isTweakable = false` → the per-tank fill slider is hidden in the editor.
- Cost climbs with capability: DarkGoo (cheapest) → WhiteGoo → PlasmaGoo → **ExoticGoo** (5000/u),
  the top-tier fuel produced by fusing DarkGoo + WhiteGoo for supreme ISP and thrust.

### Tier / progression

| Tier | Resource | How obtained | Role |
|------|----------|--------------|------|
| 1 | **DarkGoo** | Mined from asteroids; harvested from interplanetary/orbital bands | Base advanced propellant |
| 1 | **WhiteGoo** | Mined from asteroids; crustal deposits on low-grav bodies | Base advanced propellant |
| 2 | **PlasmaGoo** | **Not naturally occurring** — produced by conversion only | Mid-tier propellant |
| 3 | **ExoticGoo** | **Not naturally occurring** — fused from DarkGoo + WhiteGoo | Top-tier propellant |

> Note: `PlasmaGoo` has a resource definition but **no distribution config**, and `ExoticGoo`'s
> distributions are all explicitly zeroed — neither is found in the wild. Both must be
> manufactured by the mod's converters/engines.

### Where it's found (CRP distributions)

There are three placement mechanisms in play. **Asteroid** resources (`@PART[PotatoRoid]` →
`ModuleAsteroidResource`) ride inside captured asteroids. **Crustal / biome** resources
(`ResourceType = 0`) are drilled off a body's surface. **Interplanetary / exospheric** resources
(`ResourceType = 3`) are *scooped out of orbit* by flying through a spherical shell around the
body — these are detailed below.

#### How an interplanetary (Type-3) orbital band is configured

A `PLANETARY_RESOURCE { ResourceType = 3 }` block defines a hollow spherical shell of resource
around one body. The shell's edges are given **as multiples of that body's radius `R`** — the
config never states metres directly, so the same numbers mean very different altitudes at Kerbin
vs. tiny Gilly. Two coordinate systems describe the same shell:

| Field | Reference point | Convert to metres |
|-------|-----------------|-------------------|
| `MinAltitude` / `MaxAltitude` | **above the surface** | `value × R` = height above sea level |
| `MinRange` / `MaxRange` | **from the body's centre** | `value × R` = distance from centre |

Because the surface sits one radius out from the centre, the two are linked by:

```
Range = Altitude + 1        (in radius-multiples)
distance_from_centre = altitude_above_surface + R   (in metres)
```

Other fields:
- `PresenceChance` — % chance the band exists at all for this body (rolled once from the save seed).
- `MinAbundance` / `MaxAbundance` — harvested **concentration in %** when present; scales the scoop
  rate. (Note CRP reads these as percentages, 0–100, *not* 0–1.)
- `Variance` — % random spread applied when the band is generated.

**Worked example — DarkGoo mid band at Kerbin (`R = 600,000 m`):**

```
MinAltitude = 2      →  2.0  × 600 km = 1,200 km above surface
MaxAltitude = 2.1    →  2.1  × 600 km = 1,260 km above surface
MinRange    = 3.0    →  3.0  × 600 km = 1,800 km from centre  (= 1,200 km alt + 600 km R)
MaxRange    = 3.1    →  3.1  × 600 km = 1,860 km from centre
```

So a harvester scoops DarkGoo in this band while orbiting Kerbin between **1,200 km and 1,260 km
altitude**.

#### DarkGoo interplanetary bands — `Resources/DarkGoo.cfg`

| Body | `R` | Band | `MinAlt`→`MaxAlt` (×R) | **Altitude above surface** | Distance from centre | Chance | Abundance | Variance |
|------|----:|------|------------------------|----------------------------|----------------------|-------:|-----------|---------:|
| **Kerbin** | 600 km | Low | 0.1167 → 0.1667 | **70 → 100 km** (atmosphere edge) | 670 → 1,000 km | 30% | 0.5–1% | 20 |
| **Kerbin** | 600 km | Mid | 2.0 → 2.1 | **1,200 → 1,260 km** | 1,800 → 1,860 km | 60% | 3–5% | 30 |
| **Kerbin** | 600 km | High | 20.0 → 78.3 | **12,000 → 46,980 km** | 12,600 → 47,280 km | 100% | 6–10% | 50 |
| **Eve** | 700 km | — | 0.1286 → 0.1357 | **90.0 → 95.0 km** | 790 → 795 km | 90% | 12–16% | 70 |
| **Gilly** | 13 km | — | 3.8 → 3.9 | **49.4 → 50.7 km** | 62.4 → 63.7 km | 80% | 4–6% | 40 |

Reading the table: at Kerbin the three bands stack from the edge of space outward, getting both
more reliable (30→60→100% chance) and richer (≤1% → up to 10%) the higher you go — the High band
is a huge, guaranteed, high-yield shell spanning ~12,000–47,000 km. **Eve** is the single richest
spot (up to 16%) in a thin 90–95 km LEO-style band. **Gilly** is best harvested in a ~50 km orbit.

DarkGoo is also on **asteroids** (`presenceChance = 100`, 5–20%) and its Crustal/Oceanic/Atmospheric
distributions are all zeroed (can't be drilled or air-scooped).

#### WhiteGoo — `Resources/WhiteGoo.cfg`

- **Asteroids** (`PotatoRoid`): `presenceChance = 100`, 1–20%.
- **Crustal deposits** (`ResourceType = 0`, drilled off the surface):
  - Minmus **Poles** biome — chance 80%, abundance ~1% (sparse)
  - Gilly — chance 70%, abundance 10–30%
  - Bob — chance 90%, abundance 40–50% (richest crustal source)
- **Interplanetary band** (`ResourceType = 3`):

  | Body | `R` | `MinAlt`→`MaxAlt` (×R) | **Altitude above surface** | Distance from centre | Chance | Abundance | Variance |
  |------|----:|------------------------|----------------------------|----------------------|-------:|-----------|---------:|
  | **Gilly** | 13 km | 3.8 → 3.9 | **49.4 → 50.7 km** | 62.4 → 63.7 km | 70% | 2–4% | 40 |

  (Same ~50 km Gilly shell as DarkGoo, slightly leaner — so a single Gilly orbit can scoop both.)

#### ExoticGoo — `Resources/ExoticGoo.cfg`

All Crustal/Oceanic/Atmospheric **and** the Kerbin interplanetary distribution are set to **0**
(disabled), and the asteroid block is commented out. ExoticGoo is therefore **never found in the
wild** — it exists only as a fusion product of DarkGoo + WhiteGoo.

### Asteroid patch (`Resources/MM_Asteroid_Patch.cfg`)

Intends to make **Karborundum** (USI's rare exospheric fuel) reliably minable from asteroids by
forcing `presenceChance = 100`, range 98–100% on the `ModuleAsteroidResource`. Author note in the
file flags this patch as **"Does not work?!?!"** — treat it as unverified/possibly inactive.

### SCANsat support (`Resources/MM_SCANresource.cfg`)

Adds resource-scanner sensor types so SCANsat can map the Goo:

| Resource | SCANtype |
|----------|----------|
| WhiteGoo | `1073741824` (2³⁰) |
| DarkGoo | `2147483648` (2³¹) |

PlasmaGoo and ExoticGoo are not scannable (consistent with being manufactured, not mined).
SCANtype bits must stay unique and ≥ 64 (values below 64 are reserved by SCANsat).

## Notable parts

- **Engines** — see the dedicated **[Engines](#engines)** section below for the full list, modes,
  and (where relevant) the Goo propellant mixes.
- **Collectors** — GT Exospheric Generator (Karbonite/Karborundum/DarkMatter/Dust); particle
  collectors (1.25 m / 2.5 m).
- **Science** — Science Experiment Box A (basic) and B (advanced) bundling stock experiments.
- **Solids** — KW-style SRBs (0.25/0.75/1.5/2.5 m), ullage boosters.
- **FTL** — VX 666 Hyperdrive (needs SmokeScreen + FTLDriveContinued + SolarisHypernautics; uses
  DarkGoo).
- **Structures** — SS-Structural-X girders, heat-pipe radiators, FASA-style launch clamps.

## Engines

GTIndustries adds a spread of engines from air-breathing jets to fusion drives. The multi-mode
ones use the `GTI_MultiModeEngineFX` part module (one switchable part wrapping several
`ModuleEnginesFX` — see [Plugin/Guybrush101/GTI_MultiModeEngine.md](Plugin/Guybrush101/GTI_MultiModeEngine.md)).

| Engine (part name) | Module | Modes | Needs | Goo? |
|--------------------|--------|-------|-------|------|
| **Icarus 1.25 m** (`Icarus_125`) | `ModuleEngines` | single (heat-exchanger turbojet, `IntakeAtm`, 300 kN) | — | no |
| **CR-13 R.A.P.T.O.R.** (`GTI_RAPTOR`) | `GTI_MultiModeEngineFX` | Cruise / Air Breathing / Atmosphere Breathing / Rocket / **Dark Goo Fusion** | — | **DarkGoo** (one mode) |
| **NRX "KINKI"** (`GTI_nuclearEngine`) | `GTI_MultiModeEngineFX` | Normal / High Velocity / Overdrive | SmokeScreen | no (LiquidFuel NTR) |
| **Dark Goo Fusion Drive** (`DarkGooFusionDrive_625/_125/_250`) | `ModuleEngines` | single | UmbraSpaceIndustries | **DarkGoo** |
| **GTI-2021 "GooCicle" Ion** (`GTI_ionEngine_WG`) | `GTI_MultiModeEngineFX` | Normal / High Velocity / Overdrive | — | **WhiteGoo** |
| Base ion (`GTI_ionEngine`), Ducted Fan (`DuctedFan`) | various | — | — | no |

The **VX 666 Hyperdrive** (`Parts/FTL/`) also draws DarkGoo, but it's a jump drive rather than a
thrust engine — not detailed here.

### Goo consumption mixes

Only three engines actually burn Goo. Their `PROPELLANT` ratios (ratio numbers are by *volume/unit*;
`ElectricCharge` carries `ignoreForIsp = true`, so it's a power draw, not reaction mass):

| Engine / mode | Propellant mix (`ratio`) | Goo : partner | Notes |
|---------------|--------------------------|---------------|-------|
| **Dark Goo Fusion Drive** (all sizes) | DarkGoo `1` : LiquidFuel `8` : ElectricCharge `55` | **DarkGoo : LiquidFuel = 1 : 8** | Vacuum ISP ~10,000; needs heavy EC supply |
| **R.A.P.T.O.R.** → *Dark Goo Fusion* mode | LiquidFuel `8` : DarkGoo `1` | **DarkGoo : LiquidFuel = 1 : 8** | `STAGE_PRIORITY_FLOW`; falls back to `Ore 1` if GTIndustries is absent |
| **GooCicle Ion** → Normal / High-Velocity | ElectricCharge `1.8` : XenonGas `0.1` : WhiteGoo `0.01` | **WhiteGoo : XenonGas = 1 : 10** | WhiteGoo is a trace additive |
| **GooCicle Ion** → Overdrive | EC `0.8` : LiquidFuel `0.2` : XenonGas `0.1` : WhiteGoo `0.01` | **WhiteGoo : XenonGas = 1 : 10** | Adds LiquidFuel afterburner |

So there are exactly **two mixes to feed**: `DarkGoo:LiquidFuel = 1:8` (both DarkGoo engines agree),
and `WhiteGoo:XenonGas = 1:10` (the ion engine).

> **Note — no engine burns PlasmaGoo or ExoticGoo.** Despite ExoticGoo being described as the
> "supreme ISP" fusion product, no engine config references PlasmaGoo or ExoticGoo as a propellant.
> They are currently defined and storable but unusable as fuel.

### Does tank storage match the mixes?

Goo is stored via **InterstellarFuelSwitch (IFS)** tank options, applied automatically by
`Patches/Fuelswitch/iFSPatch_Auto.cfg` to stock tanks, plus the dedicated `GTI_SphereTank` parts
(`Patches/Parts/Tanks/SphereTanks.cfg`). Checking each against the consumption mixes:

| Tank option | Stored ratio | Target engine mix | Verdict |
|-------------|-------------|-------------------|---------|
| LF+OX tank → **"LiquidFuel, Dark Goo"** | LiquidFuel `totalCap` : DarkGoo `totalCap/8` = **8 : 1** | DarkGoo engines want LF:DarkGoo `8:1` | ✅ **correct** |
| Xenon tank → **"XenonGas, WhiteGoo"** | XenonGas `0.9·cap` : WhiteGoo `0.1·cap` = **9 : 1** | GooCicle wants XenonGas:WhiteGoo `10:1` | ⚠️ **slightly off** (see below) |
| LF+OX tank → **"LiquidFuel, White Goo"** | LiquidFuel : WhiteGoo = **8 : 1** | *no engine burns WhiteGoo with LiquidFuel* | ⚠️ **serves no engine** |
| `GTI_SphereTank` → DarkGoo / WhiteGoo / ExoticGoo | pure single resource (250 u each) | — (pair with separate LF/Xe tanks) | ✅ ok as pure storage |

**Findings:**

1. **DarkGoo is correct.** The auto-patch computes the DarkGoo tank amount as `totalCap / 8`
   against full LiquidFuel, i.e. exactly the `1:8` the Fusion drives and R.A.P.T.O.R. consume — they
   drain in step.

2. **WhiteGoo + Xenon is ~10 % off.** The patch sets XenonGas = `cap·0.9` and WhiteGoo = `cap·0.1`
   (9:1), and its own comment even says *"1/10 white goo to xenon gas"* — but the engine consumes
   `10:1`. Storing 9:1 means Xenon runs dry first, stranding ~10 % of the WhiteGoo. To match, split
   the capacity 10/11 : 1/11 (or set WhiteGoo = Xenon/10).

3. **WhiteGoo + LiquidFuel matches no engine.** WhiteGoo's only consumer (GooCicle) pairs it with
   XenonGas, never LiquidFuel — so the `LiquidFuel, White Goo` tank option can't actually feed
   anything. Likely a copy of the DarkGoo option that should have been XenonGas-based.

4. **Mixed Goo tanks start empty of Goo.** `initialResourceAmounts` gives `LiquidFuel` full but
   `0` Goo — intended, since Goo is filled by mining/conversion, not bought at the pad.

5. **`GTI_SphereTank` has a field-count bug.** Its IFS lists **7** resources
   (`Karbonite;Karborundum;Ore;DarkGoo;WhiteGoo;ExoticGoo;Dust`) but **8** entries in both
   `resourceAmounts` (`250×7;900`) and `initialResourceAmounts` (`0×8`). The trailing `900`/extra
   `0` are orphaned — trim the amount lists to 7 to match the names.

6. **Description vs config mismatch.** The 2.5 m Dark Goo Fusion Drive's description says *"Dark Goo
   and Rocket Fuel in the relation 1:5"*, but the `PROPELLANT` block is `1:8`. The config (1:8) is
   what actually burns; the description text is stale.

## Key features

1. Custom Goo propellant economy (Dark/White/Plasma/Exotic).
2. Multi-mode engines via `GTI_MultiModeEngineFX`.
3. ~80 ModuleManager compatibility patches across the mod ecosystem.
4. FTL travel via the VX 666 Hyperdrive.
5. FilterExtensions manufacturer category.
