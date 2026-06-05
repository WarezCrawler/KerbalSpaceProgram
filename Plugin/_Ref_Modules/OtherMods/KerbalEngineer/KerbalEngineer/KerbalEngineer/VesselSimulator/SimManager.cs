using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using UnityEngine;

namespace KerbalEngineer.VesselSimulator;

public class SimManager
{
	public delegate void ReadyEvent();

	public const double RESOURCE_MIN = 0.0001;

	public const double RESOURCE_PART_EMPTY_THRESH = 0.01;

	public static bool dumpTree = false;

	public static bool logOutput = false;

	public static LogMsg log = new LogMsg();

	public static TimeSpan minSimTime = new TimeSpan(0, 0, 0, 0, 150);

	public static bool vectoredThrust = false;

	private static readonly object locker = new object();

	private static readonly Stopwatch timer = new Stopwatch();

	private static bool bRequested;

	private static bool bRunning;

	private static TimeSpan delayBetweenSims;

	private static bool hasCheckedForMods;

	public static bool hasInstalledRealFuels;

	private static FieldInfo RF_ModuleEngineConfigs_localCorrectThrust;

	private static FieldInfo RF_ModuleHybridEngine_localCorrectThrust;

	private static FieldInfo RF_ModuleHybridEngines_localCorrectThrust;

	private static bool hasInstalledKIDS;

	private static MethodInfo KIDS_Utils_GetIspMultiplier;

	private static bool bKIDSThrustISP = false;

	private static object[] KIDSparameters;

	private static List<Part> parts = new List<Part>();

	private static Simulation simulation = new Simulation();

	public static double Atmosphere { get; set; }

	public static double Gravity { get; set; }

	public static Stage LastStage { get; private set; }

	public static Stage[] Stages { get; private set; }

	public static double Mach { get; set; }

	public static string failMessage { get; private set; }

	public static event ReadyEvent OnReady;

	private static void CheckForMods()
	{
		hasCheckedForMods = true;
		foreach (LoadedAssembly loadedAssembly in AssemblyLoader.loadedAssemblies)
		{
			log.AppendLine("Assembly: ", loadedAssembly.assembly);
			string text = loadedAssembly.assembly.ToString().Split(',')[0];
			if (text == "RealFuels")
			{
				log.AppendLine("Found RealFuels mod");
				Type type = loadedAssembly.assembly.GetType("RealFuels.ModuleEngineConfigs");
				if (type != null)
				{
					RF_ModuleEngineConfigs_localCorrectThrust = type.GetField("localCorrectThrust");
				}
				Type type2 = loadedAssembly.assembly.GetType("RealFuels.ModuleHybridEngine");
				if (type2 != null)
				{
					RF_ModuleHybridEngine_localCorrectThrust = type2.GetField("localCorrectThrust");
				}
				Type type3 = loadedAssembly.assembly.GetType("RealFuels.ModuleHybridEngines");
				if (type3 != null)
				{
					RF_ModuleHybridEngines_localCorrectThrust = type3.GetField("localCorrectThrust");
				}
				hasInstalledRealFuels = true;
				break;
			}
			if (text == "KerbalIspDifficultyScaler")
			{
				log.AppendLine("Found KIDS mod");
				Type type4 = loadedAssembly.assembly.GetType("KerbalIspDifficultyScaler.KerbalIspDifficultyScalerUtils");
				if (type4 != null)
				{
					KIDS_Utils_GetIspMultiplier = type4.GetMethod("GetIspMultiplier");
				}
				KIDSparameters = new object[6];
				hasInstalledKIDS = true;
			}
		}
		log.Flush();
	}

	public static bool DoesEngineUseCorrectedThrust(Part theEngine)
	{
		if (hasInstalledRealFuels)
		{
			if (RF_ModuleEngineConfigs_localCorrectThrust != null && theEngine.Modules.Contains("ModuleEngineConfigs"))
			{
				PartModule val = theEngine.Modules["ModuleEngineConfigs"];
				if ((Object)(object)val != (Object)null)
				{
					return (bool)RF_ModuleEngineConfigs_localCorrectThrust.GetValue(val);
				}
			}
			if (RF_ModuleHybridEngine_localCorrectThrust != null && theEngine.Modules.Contains("ModuleHybridEngine"))
			{
				PartModule val2 = theEngine.Modules["ModuleHybridEngine"];
				if ((Object)(object)val2 != (Object)null)
				{
					return (bool)RF_ModuleHybridEngine_localCorrectThrust.GetValue(val2);
				}
			}
			if (RF_ModuleHybridEngines_localCorrectThrust != null && theEngine.Modules.Contains("ModuleHybridEngines"))
			{
				PartModule val3 = theEngine.Modules["ModuleHybridEngines"];
				if ((Object)(object)val3 != (Object)null)
				{
					return (bool)RF_ModuleHybridEngines_localCorrectThrust.GetValue(val3);
				}
			}
		}
		if (hasInstalledKIDS && HighLogic.LoadedSceneIsEditor)
		{
			return bKIDSThrustISP;
		}
		return false;
	}

	public static void UpdateModSettings()
	{
		if (!hasCheckedForMods)
		{
			CheckForMods();
		}
		if (hasInstalledKIDS)
		{
			KIDSparameters.Initialize();
			KIDS_Utils_GetIspMultiplier.Invoke(null, KIDSparameters);
			bKIDSThrustISP = (bool)KIDSparameters[3];
		}
	}

	public static string GetVesselTypeString(VesselType vesselType)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Expected I4, but got Unknown
		return (int)vesselType switch
		{
			0 => "Debris", 
			1 => "SpaceObject", 
			2 => "Unknown", 
			3 => "Probe", 
			5 => "Rover", 
			6 => "Lander", 
			7 => "Ship", 
			9 => "Station", 
			10 => "Base", 
			11 => "EVA", 
			12 => "Flag", 
			_ => "Undefined", 
		};
	}

	public static void RequestSimulation()
	{
		if (!hasCheckedForMods)
		{
			CheckForMods();
		}
		lock (locker)
		{
			bRequested = true;
			if (!timer.IsRunning)
			{
				timer.Start();
			}
		}
	}

	public static bool ResultsReady()
	{
		lock (locker)
		{
			return !bRunning;
		}
	}

	public static void TryStartSimulation()
	{
		lock (locker)
		{
			if (!bRequested || bRunning || (timer.Elapsed < delayBetweenSims && timer.Elapsed >= TimeSpan.Zero) || (!HighLogic.LoadedSceneIsEditor && (Object)(object)FlightGlobals.ActiveVessel == (Object)null))
			{
				return;
			}
			bRequested = false;
			timer.Reset();
		}
		StartSimulation();
	}

	private static void ClearResults()
	{
		failMessage = "";
		Stages = null;
		LastStage = null;
	}

	private static void RunSimulation(object simObject)
	{
		try
		{
			Stages = (simObject as Simulation).RunSimulation(logOutput ? log : null);
			if (Stages != null && Stages.Length != 0)
			{
				if (logOutput)
				{
					Stage[] stages = Stages;
					for (int i = 0; i < stages.Length; i++)
					{
						stages[i].Dump(log);
					}
				}
				LastStage = Stages[Stages.Length - 1];
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "SimManager.RunSimulation()");
			Stages = null;
			LastStage = null;
			failMessage = ex.ToString();
		}
		lock (locker)
		{
			timer.Stop();
			if (logOutput)
			{
				log.AppendLine("Total simulation time: ", timer.ElapsedMilliseconds, "ms");
			}
			delayBetweenSims = minSimTime - timer.Elapsed;
			if (delayBetweenSims < TimeSpan.Zero)
			{
				delayBetweenSims = TimeSpan.Zero;
			}
			timer.Reset();
			timer.Start();
			bRunning = false;
			if (SimManager.OnReady != null)
			{
				SimManager.OnReady();
			}
		}
		logOutput = false;
	}

	private static void StartSimulation()
	{
		try
		{
			lock (locker)
			{
				bRunning = true;
			}
			ClearResults();
			lock (locker)
			{
				timer.Start();
			}
			if (HighLogic.LoadedSceneIsEditor)
			{
				parts = EditorLogic.fetch.ship.parts;
			}
			else
			{
				parts = FlightGlobals.ActiveVessel.Parts;
				Atmosphere = FlightGlobals.ActiveVessel.staticPressurekPa * PhysicsGlobals.KpaToAtmospheres;
			}
			if (simulation.PrepareSimulation(logOutput ? log : null, parts, Gravity, Atmosphere, Mach, dumpTree, vectoredThrust))
			{
				ThreadPool.QueueUserWorkItem(RunSimulation, simulation);
			}
			else
			{
				failMessage = "PrepareSimulation failed";
				lock (locker)
				{
					bRunning = false;
				}
				logOutput = false;
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "SimManager.StartSimulation()");
			failMessage = ex.ToString();
			lock (locker)
			{
				bRunning = false;
			}
			logOutput = false;
		}
		dumpTree = false;
	}
}
