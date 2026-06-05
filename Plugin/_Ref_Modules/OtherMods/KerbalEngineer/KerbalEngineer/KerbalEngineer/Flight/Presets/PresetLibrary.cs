using System;
using System.Collections.Generic;
using System.IO;
using KerbalEngineer.Settings;

namespace KerbalEngineer.Flight.Presets;

public class PresetLibrary
{
	private static readonly List<Preset> presets;

	private static readonly string rootPath;

	public static List<Preset> Presets => presets;

	static PresetLibrary()
	{
		presets = new List<Preset>();
		rootPath = Path.Combine(EngineerGlobals.AssemblyPath, "Presets");
		Load();
	}

	public static void Add(Preset preset)
	{
		Presets.Add(preset);
	}

	public static Preset GetPreset(string abbrev)
	{
		foreach (Preset preset in presets)
		{
			if (preset.Abbreviation == abbrev)
			{
				return preset;
			}
		}
		return null;
	}

	public static void Load()
	{
		if (!Directory.Exists(rootPath))
		{
			Directory.CreateDirectory(rootPath);
		}
		string[] files = Directory.GetFiles(rootPath);
		for (int i = 0; i < files.Length; i++)
		{
			SettingHandler settingHandler = SettingHandler.Load(files[i], new Type[1] { typeof(Preset) });
			presets.Add(settingHandler.Get<Preset>("preset", null));
		}
	}

	public static bool Remove(Preset preset)
	{
		if (File.Exists(Path.Combine(rootPath, preset.FileName)))
		{
			File.Delete(Path.Combine(rootPath, preset.FileName));
		}
		return Presets.Remove(preset);
	}

	public static void Save()
	{
		Presets.ForEach(Save);
	}

	public static void Save(Preset preset)
	{
		if (!Directory.Exists(rootPath))
		{
			Directory.CreateDirectory(rootPath);
		}
		if (!Presets.Contains(preset))
		{
			Presets.Add(preset);
		}
		SettingHandler settingHandler = new SettingHandler();
		settingHandler.Set("preset", preset);
		settingHandler.Save(Path.Combine("../Presets", preset.FileName));
		ScreenMessages.PostScreenMessage("Saved Preset: " + preset.Name, 2f, (ScreenMessageStyle)0);
	}
}
