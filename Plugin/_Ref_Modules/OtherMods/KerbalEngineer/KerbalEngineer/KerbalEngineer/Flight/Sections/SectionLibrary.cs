using System;
using System.Collections.Generic;
using System.Linq;
using KerbalEngineer.Flight.Presets;
using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.Settings;
using KerbalEngineer.TrackingStation;
using UnityEngine;

namespace KerbalEngineer.Flight.Sections;

public static class SectionLibrary
{
	public static SectionModuleTS TrackingStationSection { get; set; }

	public static List<SectionModule> CustomSections { get; set; }

	public static int NumberOfSections { get; private set; }

	public static int NumberOfStackSections { get; private set; }

	public static List<SectionModule> StockSections { get; set; }

	static SectionLibrary()
	{
		StockSections = new List<SectionModule>();
		CustomSections = new List<SectionModule>();
		SectionModule sectionModule = new SectionModule
		{
			Name = "ORBITAL",
			Abbreviation = "ORBT",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Orbital"))
				where r.IsDefault
				select r).ToList()
		};
		sectionModule.ApplyPreset(PresetLibrary.GetPreset(sectionModule.Abbreviation));
		StockSections.Add(sectionModule);
		SectionModule sectionModule2 = new SectionModule
		{
			Name = "SURFACE",
			Abbreviation = "SURF",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Surface"))
				where r.IsDefault
				select r).ToList()
		};
		sectionModule2.ApplyPreset(PresetLibrary.GetPreset(sectionModule2.Abbreviation));
		StockSections.Add(sectionModule2);
		SectionModule sectionModule3 = new SectionModule
		{
			Name = "VESSEL",
			Abbreviation = "VESL",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Vessel"))
				where r.IsDefault
				select r).ToList()
		};
		sectionModule3.ApplyPreset(PresetLibrary.GetPreset(sectionModule3.Abbreviation));
		StockSections.Add(sectionModule3);
		SectionModule sectionModule4 = new SectionModule
		{
			Name = "RENDEZVOUS",
			Abbreviation = "RDZV",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Rendezvous"))
				where r.IsDefault
				select r).ToList()
		};
		sectionModule4.ApplyPreset(PresetLibrary.GetPreset(sectionModule4.Abbreviation));
		StockSections.Add(sectionModule4);
		SectionModuleTS obj = new SectionModuleTS
		{
			Name = "TRACKING",
			Abbreviation = "TRCK",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Rendezvous"))
				where r.IsDefault
				select r).ToList(),
			IsVisible = true,
			showFloatButton = false
		};
		obj.ApplyPreset(PresetLibrary.GetPreset(obj.Abbreviation));
		TrackingStationSection = obj;
		SectionModule sectionModule5 = new SectionModule
		{
			Name = "THERMAL",
			Abbreviation = "HEAT",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Thermal"))
				where r.IsDefault
				select r).ToList()
		};
		sectionModule5.ApplyPreset(PresetLibrary.GetPreset(sectionModule5.Abbreviation));
		StockSections.Add(sectionModule5);
		SectionModule sectionModule6 = new SectionModule
		{
			Name = "BODY",
			Abbreviation = "BODY",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Body"))
				where r.IsDefault
				select r).ToList()
		};
		sectionModule6.ApplyPreset(PresetLibrary.GetPreset(sectionModule6.Abbreviation));
		StockSections.Add(sectionModule6);
		SectionModule sectionModule7 = new SectionModule
		{
			Name = "MANEUVER",
			Abbreviation = "BURN",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Orbital"))
				where r.Name.StartsWith("Node")
				select r).ToList()
		};
		sectionModule7.ApplyPreset(PresetLibrary.GetPreset(sectionModule7.Abbreviation));
		StockSections.Add(sectionModule7);
		SectionModule sectionModule8 = new SectionModule
		{
			Name = "LANDING",
			Abbreviation = "LAND",
			ReadoutModules = (from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Surface"))
				where r.Name.StartsWith("Impact")
				select r).ToList()
		};
		sectionModule8.ReadoutModules.AddRange((from r in ReadoutLibrary.GetCategory(ReadoutCategory.GetCategory("Vessel"))
			where r.Name.StartsWith("Suicide")
			select r).ToList());
		sectionModule8.ApplyPreset(PresetLibrary.GetPreset(sectionModule8.Abbreviation));
		StockSections.Add(sectionModule8);
		SectionModule sectionModule9 = new SectionModule
		{
			Name = "HUD 1",
			Abbreviation = "HUD 1",
			IsVisible = true,
			showButton = false,
			ReadoutModules = new List<ReadoutModule>
			{
				ReadoutLibrary.GetReadout("ApoapsisHeight"),
				ReadoutLibrary.GetReadout("TimeToApoapsis"),
				ReadoutLibrary.GetReadout("PeriapsisHeight"),
				ReadoutLibrary.GetReadout("TimeToPeriapsis")
			}
		};
		sectionModule9.FloatingPositionX = (float)Screen.width * 0.33f - sectionModule9.ReadoutModules.First().ContentWidth * 0.5f;
		sectionModule9.FloatingPositionY = 0f;
		sectionModule9.IsHud = true;
		sectionModule9.ApplyPreset(PresetLibrary.GetPreset(sectionModule9.Abbreviation));
		StockSections.Add(sectionModule9);
		SectionModule sectionModule10 = new SectionModule
		{
			Name = "HUD 2",
			Abbreviation = "HUD 2",
			IsVisible = true,
			showButton = false,
			ReadoutModules = new List<ReadoutModule>
			{
				ReadoutLibrary.GetReadout("AltitudeTerrain"),
				ReadoutLibrary.GetReadout("VerticalSpeed"),
				ReadoutLibrary.GetReadout("HorizontalSpeed"),
				ReadoutLibrary.GetReadout("Biome"),
				ReadoutLibrary.GetReadout("MachNumber")
			}
		};
		sectionModule10.FloatingPositionX = (float)Screen.width * 0.7f - sectionModule10.ReadoutModules.First().ContentWidth * 0.5f;
		sectionModule10.FloatingPositionY = 0f;
		sectionModule10.IsHud = true;
		sectionModule10.ApplyPreset(PresetLibrary.GetPreset(sectionModule10.Abbreviation));
		StockSections.Add(sectionModule10);
	}

	public static void FixedUpdate()
	{
		FixedUpdateSections(StockSections);
		FixedUpdateSections(CustomSections);
	}

	public static void Update()
	{
		NumberOfStackSections = 0;
		NumberOfSections = 0;
		UpdateSections(StockSections);
		UpdateSections(CustomSections);
	}

	private static void FixedUpdateSections(IEnumerable<SectionModule> sections)
	{
		foreach (SectionModule section in sections)
		{
			if (section.IsVisible)
			{
				section.FixedUpdate();
			}
		}
	}

	private static void UpdateSections(IEnumerable<SectionModule> sections)
	{
		foreach (SectionModule section in sections)
		{
			if (section.IsVisible)
			{
				if (!section.IsFloating)
				{
					foreach (ReadoutModule readoutModule in section.ReadoutModules)
					{
						if (readoutModule.ResizeRequested)
						{
							DisplayStack.Instance.RequestResize();
							readoutModule.ResizeRequested = false;
						}
					}
					NumberOfStackSections++;
				}
				else
				{
					foreach (ReadoutModule readoutModule2 in section.ReadoutModules)
					{
						if (readoutModule2.ResizeRequested)
						{
							section.Window.RequestResize();
							readoutModule2.ResizeRequested = false;
						}
					}
				}
				section.Update();
			}
			NumberOfSections++;
		}
	}

	public static void Load()
	{
		if (!SettingHandler.Exists("SectionLibrary.xml"))
		{
			return;
		}
		GetAllSections().ForEach(delegate(SectionModule s)
		{
			if ((Object)(object)s.Window != (Object)null)
			{
				Object.Destroy((Object)(object)s.Window);
			}
		});
		SettingHandler settingHandler = SettingHandler.Load("SectionLibrary.xml", new Type[2]
		{
			typeof(List<SectionModule>),
			typeof(SectionModuleTS)
		});
		StockSections = settingHandler.Get("StockSections", StockSections);
		CustomSections = settingHandler.Get("CustomSections", CustomSections);
		foreach (SectionModule allSection in GetAllSections())
		{
			allSection.ClearNullReadouts();
		}
	}

	public static void LoadTS()
	{
		if (!SettingHandler.Exists("SectionLibraryTS.xml"))
		{
			return;
		}
		GetAllSections().ForEach(delegate(SectionModule s)
		{
			if ((Object)(object)s.Window != (Object)null)
			{
				Object.Destroy((Object)(object)s.Window);
			}
		});
		TrackingStationSection = SettingHandler.Load("SectionLibraryTS.xml", new Type[2]
		{
			typeof(List<SectionModule>),
			typeof(SectionModuleTS)
		}).Get("TrackingStation", TrackingStationSection);
		foreach (SectionModule allSection in GetAllSections())
		{
			allSection.ClearNullReadouts();
		}
	}

	public static void Save()
	{
		SettingHandler settingHandler = new SettingHandler();
		settingHandler.Set("StockSections", StockSections);
		settingHandler.Set("CustomSections", CustomSections);
		settingHandler.Save("SectionLibrary.xml");
	}

	public static void SaveTS()
	{
		SettingHandler settingHandler = new SettingHandler();
		settingHandler.Set("TrackingStation", TrackingStationSection);
		settingHandler.Save("SectionLibraryTS.xml");
	}

	public static List<SectionModule> GetAllSections()
	{
		List<SectionModule> list = new List<SectionModule>();
		list.AddRange(StockSections);
		list.AddRange(CustomSections);
		return list;
	}

	public static SectionModule GetCustomSection(string name)
	{
		return CustomSections.FirstOrDefault((SectionModule s) => s.Name == name);
	}

	public static SectionModule GetSection(string name)
	{
		return GetStockSection(name) ?? GetCustomSection(name);
	}

	public static SectionModule GetStockSection(string name)
	{
		return StockSections.FirstOrDefault((SectionModule s) => s.Name == name);
	}

	public static bool RemoveCustomSection(string name)
	{
		return CustomSections.Remove(GetCustomSection(name));
	}

	public static bool RemoveSection(string name)
	{
		if (!RemoveStockSection(name))
		{
			return RemoveCustomSection(name);
		}
		return true;
	}

	public static bool RemoveStockSection(string name)
	{
		return StockSections.Remove(GetStockSection(name));
	}
}
