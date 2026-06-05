using System;

namespace KerbalEngineer.Flight.Readouts.Thermal;

public class ThermalProcessor : IUpdatable, IUpdateRequest
{
	private static readonly ThermalProcessor instance;

	public static double ConvectionFlux { get; private set; }

	public static string CoolestPartName { get; private set; }

	public static double CoolestSkinTemperature { get; private set; }

	public static double CoolestSkinTemperatureMax { get; private set; }

	public static double CoolestTemperature { get; private set; }

	public static double CoolestTemperatureMax { get; private set; }

	public static string CriticalPartName { get; private set; }

	public static double CriticalSkinTemperature { get; private set; }

	public static double CriticalSkinTemperatureMax { get; private set; }

	public static double CriticalTemperature { get; private set; }

	public static double CriticalTemperatureMax { get; private set; }

	public static double CriticalTemperaturePercentage { get; private set; }

	public static string HottestPartName { get; private set; }

	public static double HottestSkinTemperature { get; private set; }

	public static double HottestSkinTemperatureMax { get; private set; }

	public static double HottestTemperature { get; private set; }

	public static double HottestTemperatureMax { get; private set; }

	public static ThermalProcessor Instance => instance;

	public static double InternalFlux { get; private set; }

	public static double RadiationFlux { get; private set; }

	public static bool ShowDetails { get; private set; }

	public bool UpdateRequested { get; set; }

	static ThermalProcessor()
	{
		instance = new ThermalProcessor();
		HottestTemperature = 0.0;
		HottestTemperatureMax = 0.0;
		HottestSkinTemperature = 0.0;
		HottestSkinTemperatureMax = 0.0;
		CoolestTemperature = 0.0;
		CoolestTemperatureMax = 0.0;
		CoolestSkinTemperature = 0.0;
		CoolestSkinTemperatureMax = 0.0;
		CriticalTemperature = 0.0;
		CriticalTemperatureMax = 0.0;
		CriticalSkinTemperature = 0.0;
		CriticalSkinTemperatureMax = 0.0;
		HottestPartName = string.Empty;
		CoolestPartName = string.Empty;
		CriticalPartName = string.Empty;
	}

	public void Update()
	{
		if (FlightGlobals.ActiveVessel.parts.Count == 0)
		{
			ShowDetails = false;
			return;
		}
		ShowDetails = true;
		ConvectionFlux = 0.0;
		RadiationFlux = 0.0;
		InternalFlux = 0.0;
		HottestTemperature = 0.0;
		HottestSkinTemperature = 0.0;
		CoolestTemperature = double.MaxValue;
		CoolestSkinTemperature = double.MaxValue;
		CriticalTemperature = double.MaxValue;
		CriticalSkinTemperature = double.MaxValue;
		CriticalTemperaturePercentage = 0.0;
		HottestPartName = string.Empty;
		CoolestPartName = string.Empty;
		CriticalPartName = string.Empty;
		for (int i = 0; i < FlightGlobals.ActiveVessel.parts.Count; i++)
		{
			Part val = FlightGlobals.ActiveVessel.parts[i];
			ConvectionFlux += val.thermalConvectionFlux;
			RadiationFlux += val.thermalRadiationFlux;
			InternalFlux += val.thermalInternalFluxPrevious;
			if (val.temperature > HottestTemperature || val.skinTemperature > HottestSkinTemperature)
			{
				HottestTemperature = val.temperature;
				HottestTemperatureMax = val.maxTemp;
				HottestSkinTemperature = val.skinTemperature;
				HottestSkinTemperatureMax = val.skinMaxTemp;
				HottestPartName = val.partInfo.title;
			}
			if (val.temperature < CoolestTemperature || val.skinTemperature < CoolestSkinTemperature)
			{
				CoolestTemperature = val.temperature;
				CoolestTemperatureMax = val.maxTemp;
				CoolestSkinTemperature = val.skinTemperature;
				CoolestSkinTemperatureMax = val.skinMaxTemp;
				CoolestPartName = val.partInfo.title;
			}
			if (val.temperature / val.maxTemp > CriticalTemperaturePercentage || val.skinTemperature / val.skinMaxTemp > CriticalTemperaturePercentage)
			{
				CriticalTemperature = val.temperature;
				CriticalTemperatureMax = val.maxTemp;
				CriticalSkinTemperature = val.skinTemperature;
				CriticalSkinTemperatureMax = val.skinMaxTemp;
				CriticalTemperaturePercentage = Math.Max(val.temperature / val.maxTemp, val.skinTemperature / val.skinMaxTemp);
				CriticalPartName = val.partInfo.title;
			}
		}
	}

	public static void RequestUpdate()
	{
		instance.UpdateRequested = true;
	}
}
