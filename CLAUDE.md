# KSP Mod Development

Author: **WarezCrawler** / Guybrush Threepwood Industries (GTI). All mods MIT licensed.

## File locations

- **KSP install (1.9.1):** `T:\Kerbal Space Program\KSP1.9.1`
- **Installed mods (GameData):** `T:\Kerbal Space Program\KSP1.9.1\GameData`
- **Mod source / this repo:** `O:\Mod Development\KerbalSpaceProgram`
- **Plugin (C#) source:** `O:\Mod Development\KerbalSpaceProgram\Plugin\Guybrush101`

**My mods are prefixed with `GTI`** (e.g. `GTI_Utilities`, `GTIndustries`, `GTI_SimpleEPL`,
`GTI_SimpleKarbonite`). When working in GameData, `GTI*` folders are mine; other folders are
third-party dependencies (ModuleManager, UmbraSpaceIndustries, SmokeScreen, etc.).

## Documentation

Per-mod reference docs live in this root folder (`GTI_*.md`, `GTIndustries.md`). Per-plugin
developer reference docs live next to their source in `Plugin/Guybrush101/` (`*.md`).

## KSP API Reference (decompiled source)

The decompiled C# source for Kerbal Space Program's main assembly is available for
reference when coding mods:

- **Source DLL:** `Plugin/_Ref_Modules/KSP_Current_DLL/Assembly-CSharp.Cleaned2.dll`
- **Decompiled source:** `Plugin/_Ref_Modules/KSP_Current_DLL/Assembly-CSharp_Cleaned2_dll/`
  - 3,077 `.cs` files (one per type), organized into namespace folders
    (`CommNet`, `Contracts`, `Experience`, `FinePrint`, `UnityEngine`, `TMPro`, etc.)
  - Global-namespace KSP types (`Part`, `AddonLoader`, `AssemblyLoader`, ...) are at the root
  - `ns0`–`ns36` folders hold types whose namespaces were stripped/mangled in the
    "Cleaned2" DLL — code is fully readable, just bucketed under synthetic folder names

**Use this folder to look up KSP API signatures, types, and behavior when writing mod code.**
Grep/search it directly rather than guessing at the API.

### How it was generated
Decompiled with `ilspycmd` (ILSpy CLI). Notes for re-running:
- Use `ilspycmd` **v8.2.0.7535** — newer versions target .NET 9, but this machine only
  has the .NET 8 runtime.
- Use the `--nested-directories` flag — the default project mode crashes on KSP's
  global-namespace types (`ns0` directory bug).
- Command:
  ```
  ilspycmd <dll> -p --nested-directories -o <outdir> -r <dll-dir> --disable-updatecheck
  ```
