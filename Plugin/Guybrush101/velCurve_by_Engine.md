# velCurve - Thrust Multiplier vs Speed, per Engine (stock + GTI + Solaris Hypernautics)

`velCurve` multiplies an air-breathing engine's thrust as a function of **Mach number**. It belongs to the *engine* (not the planet), so this doc is organised per engine. Native x-axis is Mach; shown here in **m/s** using Kerbin sea-level speed of sound **a0 = 340.3 m/s** (m/s = Mach * a0).

> **Speed-of-sound caveat:** the curve truly depends on Mach, so these m/s are *Kerbin-sea-level equivalent*. Local speed of sound `a = sqrt(gamma * R/M * T)` drops in colder/higher air and differs per planet. E.g. at 10 km on Kerbin (~217 K) a ~ 295 m/s, so a given Mach (hence a given multiplier) is reached at ~13% fewer m/s than shown. Jool's light H2 gives a very high a; Duna's cold CO2 a low one.

## Per-engine summary

| Source | Engine | Mode | Static (0 m/s) | Peak mult | Peak speed | Flameout speed |
|--------|--------|------|---------------:|----------:|-----------:|---------------:|
| Stock | J-20 "Juno" | jet | 1.00 | 1.03 | 442 m/s (Mach 1.30) | 817 m/s (Mach 2.40) |
| Stock | J-33 "Wheesley" | jet | 1.00 | 1.00 | 0 m/s (Mach 0.00) | 851 m/s (Mach 2.50) |
| Stock | J-404 "Panther" | dry | 1.00 | 1.50 | 596 m/s (Mach 1.75) | 851 m/s (Mach 2.50) |
| Stock | J-404 "Panther" | wet/AB | 1.00 | 3.63 | 851 m/s (Mach 2.50) | 1140 m/s (Mach 3.35) |
| Stock | J-X4 "Whiplash" | ramjet | 1.00 | 5.80 | 1021 m/s (Mach 3.00) | 1872 m/s (Mach 5.50) |
| Stock | J-90 "Goliath" | turbofan | 1.00 | 1.00 | 0 m/s (Mach 0.00) | 715 m/s (Mach 2.10) |
| Stock | CR-7 R.A.P.I.E.R. | air-breathing | 1.00 | 8.50 | 1276 m/s (Mach 3.75) | 2042 m/s (Mach 6.00) |
| GTI | CR-13 R.A.P.T.O.R. | Air Breathing | 1.00 | 8.50 | 1276 m/s (Mach 3.75) | 2042 m/s (Mach 6.00) |
| GTI | CR-13 R.A.P.T.O.R. | Cruise | 0.50 | 1.00 | 340 m/s (Mach 1.00) | none (flat) |
| GTI | CR-13 R.A.P.T.O.R. | Atmosphere Breathing | 1.00 | 8.50 | 1276 m/s (Mach 3.75) | 2042 m/s (Mach 6.00) |
| Solaris | CX-1000M "Sabetier" | Cycle/Filter | 1.00 | 5.80 | 1021 m/s (Mach 3.00) | 1872 m/s (Mach 5.50) |
| Solaris | CX-2500L "Plymouth" | Cycle/Filter | 1.00 | 5.80 | 1021 m/s (Mach 3.00) | 1872 m/s (Mach 5.50) |
| Solaris | JY-05 "Osiris" | scramjet | 0.00 | 6.20 | 5737 m/s (Mach 16.86) | 8507 m/s (Mach 25.00) |

Notes: Wheesley & Goliath never exceed their static thrust (peak = 1.0 at 0 m/s) - high-bypass turbofans, best down low and slow. Osiris 'peak' is a spline overshoot (see below).

## Stock engines

| Speed (m/s) | Mach | J-20 "Juno" (jet) | J-33 "Wheesley" (jet) | J-404 "Panther" (dry) | J-404 "Panther" (wet/AB) | J-X4 "Whiplash" (ramjet) | J-90 "Goliath" (turbofan) | CR-7 R.A.P.I.E.R. (air-breathing) |
|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| 0 | 0.00 | 1 | 1 | 1 | 1 | 1 | 1 | 1 |
| 50 | 0.15 | 0.973 | 0.969 | 0.974 | 0.973 | 0.983 | 0.956 | 0.972 |
| 100 | 0.29 | 0.924 | 0.903 | 0.937 | 0.977 | 1.01 | 0.889 | 1.05 |
| 150 | 0.44 | 0.897 | 0.847 | 0.938 | 1 | 1.16 | 0.824 | 1.26 |
| 200 | 0.59 | 0.909 | 0.836 | 0.967 | 1.05 | 1.42 | 0.791 | 1.55 |
| 250 | 0.73 | 0.938 | 0.856 | 1.02 | 1.15 | 1.75 | 0.799 | 1.88 |
| 300 | 0.88 | 0.973 | 0.889 | 1.08 | 1.28 | 2.11 | 0.829 | 2.26 |
| 350 | 1.03 | 1.01 | 0.924 | 1.14 | 1.46 | 2.47 | 0.869 | 2.69 |
| 400 | 1.18 | 1.02 | 0.951 | 1.23 | 1.68 | 2.81 | 0.911 | 3.18 |
| 450 | 1.32 | 1.03 | 0.96 | 1.32 | 1.94 | 3.12 | 0.946 | 3.7 |
| 500 | 1.47 | 1.01 | 0.947 | 1.41 | 2.22 | 3.43 | 0.963 | 4.27 |
| 550 | 1.62 | 0.964 | 0.887 | 1.48 | 2.51 | 3.77 | 0.942 | 4.8 |
| 600 | 1.76 | 0.884 | 0.753 | 1.5 | 2.79 | 4.12 | 0.826 | 5.3 |
| 650 | 1.91 | 0.769 | 0.57 | 1.46 | 3.05 | 4.46 | 0.564 | 5.76 |
| 700 | 2.06 | 0.598 | 0.371 | 1.28 | 3.28 | 4.75 | 0.101 | 6.18 |
| 750 | 2.20 | 0.278 | 0.188 | 0.798 | 3.47 | 4.98 | 0 | 6.57 |
| 800 | 2.35 | 0.0226 | 0.0532 | 0.26 | 3.59 | 5.2 | 0 | 6.92 |
| 850 | 2.50 | 0 | 1.26e-05 | 6.79e-05 | 3.63 | 5.41 | 0 | 7.24 |
| 900 | 2.64 | 0 | 0 | 0 | 3.09 | 5.59 | 0 | 7.52 |
| 950 | 2.79 | 0 | 0 | 0 | 1.92 | 5.72 | 0 | 7.76 |
| 1000 | 2.94 | 0 | 0 | 0 | 0.835 | 5.79 | 0 | 7.97 |
| 1050 | 3.09 | 0 | 0 | 0 | 0.37 | 5.79 | 0 | 8.15 |
| 1100 | 3.23 | 0 | 0 | 0 | 0.113 | 5.75 | 0 | 8.29 |
| 1150 | 3.38 | 0 | 0 | 0 | 0 | 5.66 | 0 | 8.39 |
| 1200 | 3.53 | 0 | 0 | 0 | 0 | 5.52 | 0 | 8.46 |
| 1250 | 3.67 | 0 | 0 | 0 | 0 | 5.33 | 0 | 8.5 |
| 1300 | 3.82 | 0 | 0 | 0 | 0 | 5.07 | 0 | 8.49 |
| 1350 | 3.97 | 0 | 0 | 0 | 0 | 4.76 | 0 | 8.38 |
| 1400 | 4.11 | 0 | 0 | 0 | 0 | 4.37 | 0 | 8.18 |
| 1450 | 4.26 | 0 | 0 | 0 | 0 | 3.91 | 0 | 7.9 |
| 1500 | 4.41 | 0 | 0 | 0 | 0 | 3.38 | 0 | 7.55 |
| 1550 | 4.55 | 0 | 0 | 0 | 0 | 2.76 | 0 | 7.14 |
| 1600 | 4.70 | 0 | 0 | 0 | 0 | 2.13 | 0 | 6.65 |
| 1650 | 4.85 | 0 | 0 | 0 | 0 | 1.53 | 0 | 6.09 |
| 1700 | 5.00 | 0 | 0 | 0 | 0 | 0.983 | 0 | 5.47 |
| 1750 | 5.14 | 0 | 0 | 0 | 0 | 0.528 | 0 | 4.8 |
| 1800 | 5.29 | 0 | 0 | 0 | 0 | 0.196 | 0 | 4.08 |
| 1850 | 5.44 | 0 | 0 | 0 | 0 | 0.02 | 0 | 3.33 |
| 1900 | 5.58 | 0 | 0 | 0 | 0 | 0 | 0 | 2.47 |
| 1950 | 5.73 | 0 | 0 | 0 | 0 | 0 | 0 | 1.33 |
| 2000 | 5.88 | 0 | 0 | 0 | 0 | 0 | 0 | 0.336 |
| 2050 | 6.02 | 0 | 0 | 0 | 0 | 0 | 0 | 0 |

## GTI engines

| Speed (m/s) | Mach | CR-13 R.A.P.T.O.R. (Air Breathing) | CR-13 R.A.P.T.O.R. (Cruise) | CR-13 R.A.P.T.O.R. (Atmosphere Breathing) |
|---:|---:|---:|---:|---:|
| 0 | 0.00 | 1 | 0.5 | 1 |
| 50 | 0.15 | 0.972 | 0.529 | 0.972 |
| 100 | 0.29 | 1.05 | 0.604 | 1.05 |
| 150 | 0.44 | 1.26 | 0.706 | 1.26 |
| 200 | 0.59 | 1.55 | 0.815 | 1.55 |
| 250 | 0.73 | 1.88 | 0.913 | 1.88 |
| 300 | 0.88 | 2.26 | 0.981 | 2.26 |
| 350 | 1.03 | 2.69 | 1 | 2.69 |
| 400 | 1.18 | 3.18 | 1 | 3.18 |
| 450 | 1.32 | 3.7 | 1 | 3.7 |
| 500 | 1.47 | 4.27 | 1 | 4.27 |
| 550 | 1.62 | 4.8 | 1 | 4.8 |
| 600 | 1.76 | 5.3 | 1 | 5.3 |
| 650 | 1.91 | 5.76 | 1 | 5.76 |
| 700 | 2.06 | 6.18 | 1 | 6.18 |
| 750 | 2.20 | 6.57 | 1 | 6.57 |
| 800 | 2.35 | 6.92 | 1 | 6.92 |
| 850 | 2.50 | 7.24 | 1 | 7.24 |
| 900 | 2.64 | 7.52 | 1 | 7.52 |
| 950 | 2.79 | 7.76 | 1 | 7.76 |
| 1000 | 2.94 | 7.97 | 1 | 7.97 |
| 1050 | 3.09 | 8.15 | 1 | 8.15 |
| 1100 | 3.23 | 8.29 | 1 | 8.29 |
| 1150 | 3.38 | 8.39 | 1 | 8.39 |
| 1200 | 3.53 | 8.46 | 1 | 8.46 |
| 1250 | 3.67 | 8.5 | 1 | 8.5 |
| 1300 | 3.82 | 8.49 | 1 | 8.49 |
| 1350 | 3.97 | 8.38 | 1 | 8.38 |
| 1400 | 4.11 | 8.18 | 1 | 8.18 |
| 1450 | 4.26 | 7.9 | 1 | 7.9 |
| 1500 | 4.41 | 7.55 | 1 | 7.55 |
| 1550 | 4.55 | 7.14 | 1 | 7.14 |
| 1600 | 4.70 | 6.65 | 1 | 6.65 |
| 1650 | 4.85 | 6.09 | 1 | 6.09 |
| 1700 | 5.00 | 5.47 | 1 | 5.47 |
| 1750 | 5.14 | 4.8 | 1 | 4.8 |
| 1800 | 5.29 | 4.08 | 1 | 4.08 |
| 1850 | 5.44 | 3.33 | 1 | 3.33 |
| 1900 | 5.58 | 2.47 | 1 | 2.47 |
| 1950 | 5.73 | 1.33 | 1 | 1.33 |
| 2000 | 5.88 | 0.336 | 1 | 0.336 |
| 2050 | 6.02 | 0 | 1 | 0 |
GTI also patches the stock **Whiplash** into a multi-mode engine, adding a *Cruise* mode that reuses the Whiplash velCurve (identical shape). The R.A.P.T.O.R. *Cruise* mode holds multiplier 1.0 at all speeds above Mach 1 (never flames out on velCurve); its two other air-breathing modes use the RAPIER curve.

## Solaris Hypernautics engines

| Speed (m/s) | Mach | CX-1000M "Sabetier" (Cycle/Filter) | CX-2500L "Plymouth" (Cycle/Filter) | JY-05 "Osiris" (scramjet) |
|---:|---:|---:|---:|---:|
| 0 | 0.00 | 1 | 1 | 0 |
| 50 | 0.15 | 0.983 | 0.983 | 0 |
| 100 | 0.29 | 1.01 | 1.01 | 0 |
| 150 | 0.44 | 1.16 | 1.16 | 0 |
| 200 | 0.59 | 1.42 | 1.42 | 0 |
| 250 | 0.73 | 1.75 | 1.75 | 0 |
| 300 | 0.88 | 2.11 | 2.11 | 0 |
| 350 | 1.03 | 2.47 | 2.47 | 0.000273 |
| 400 | 1.18 | 2.81 | 2.81 | 0.0107 |
| 450 | 1.32 | 3.12 | 3.12 | 0.0371 |
| 500 | 1.47 | 3.43 | 3.43 | 0.0811 |
| 550 | 1.62 | 3.77 | 3.77 | 0.144 |
| 600 | 1.76 | 4.12 | 4.12 | 0.227 |
| 650 | 1.91 | 4.46 | 4.46 | 0.332 |
| 700 | 2.06 | 4.75 | 4.75 | 0.46 |
| 750 | 2.20 | 4.98 | 4.98 | 0.612 |
| 800 | 2.35 | 5.2 | 5.2 | 0.791 |
| 850 | 2.50 | 5.41 | 5.41 | 0.997 |
| 900 | 2.64 | 5.59 | 5.59 | 1.13 |
| 950 | 2.79 | 5.72 | 5.72 | 1.21 |
| 1000 | 2.94 | 5.79 | 5.79 | 1.38 |
| 1050 | 3.09 | 5.79 | 5.79 | 1.58 |
| 1100 | 3.23 | 5.75 | 5.75 | 1.7 |
| 1150 | 3.38 | 5.66 | 5.66 | 1.82 |
| 1200 | 3.53 | 5.52 | 5.52 | 1.93 |
| 1250 | 3.67 | 5.33 | 5.33 | 2.04 |
| 1300 | 3.82 | 5.07 | 5.07 | 2.14 |
| 1350 | 3.97 | 4.76 | 4.76 | 2.23 |
| 1400 | 4.11 | 4.37 | 4.37 | 2.32 |
| 1450 | 4.26 | 3.91 | 3.91 | 2.4 |
| 1500 | 4.41 | 3.38 | 3.38 | 2.47 |
| 1550 | 4.55 | 2.76 | 2.76 | 2.54 |
| 1600 | 4.70 | 2.13 | 2.13 | 2.61 |
| 1650 | 4.85 | 1.53 | 1.53 | 2.67 |
| 1700 | 5.00 | 0.983 | 0.983 | 2.73 |
| 1750 | 5.14 | 0.528 | 0.528 | 2.78 |
| 1800 | 5.29 | 0.196 | 0.196 | 2.82 |
| 1850 | 5.44 | 0.02 | 0.02 | 2.87 |
| 1900 | 5.58 | 0 | 0 | 2.91 |
| 1950 | 5.73 | 0 | 0 | 2.94 |
| 2000 | 5.88 | 0 | 0 | 2.98 |
| 2050 | 6.02 | 0 | 0 | 3 |
CX-1000M and CX-2500L share the stock Whiplash velCurve. The JY-05 "Osiris" is a **scramjet**: zero thrust below ~Mach 2.3, x1 at Mach 2.5, x3 at Mach 6 (~2042 m/s, the table's end); keyframes run to Mach 25 (~8508 m/s) flameout. The spline between the Mach-6 (x3) and Mach-25 (x0) keys **bulges above 3 (to ~x6 near Mach 17 / ~5700 m/s)** - a long-interval Hermite overshoot baked into the stock curve.

> Excluded (matched 'velCurve' text but have none): Skipper (GTI LFO patch), Solaris Magnapulse & Q-1RKannae - rockets/ion drives with `useVelCurve = false`.

