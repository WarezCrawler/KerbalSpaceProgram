using System;
using System.Collections.Generic;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using UnityEngine;

namespace B9PartSwitch;

public static class B9TankSettings
{
	public const string structuralTankName = "Structural";

	private static Dictionary<string, TankType> tankTypes = new Dictionary<string, TankType>();

	public static bool LoadedTankDefs { get; private set; } = false;


	public static TankType StructuralTankType => new TankType
	{
		tankName = "Structural",
		tankMass = 0f,
		tankCost = 0f
	};

	public static void ModuleManagerPostLoad()
	{
		ReloadTankDefs();
	}

	public static void ReloadTankDefs()
	{
		tankTypes.Clear();
		tankTypes.Add("Structural", StructuralTankType);
		ConfigNode[] configNodes = GameDatabase.Instance.GetConfigNodes("B9_TANK_TYPE");
		foreach (ConfigNode node in configNodes)
		{
			TankType tankType = new TankType();
			OperationContext context = new OperationContext(Operation.LoadPrefab, tankType);
			try
			{
				tankType.Load(node, context);
			}
			catch (Exception innerException)
			{
				Exception ex = new Exception("Fatal exception while loading tank type " + (tankType.tankName ?? "<unknown>"), innerException);
				FatalErrorHandler.HandleFatalError(ex);
				throw ex;
			}
			if (tankTypes.ContainsKey(tankType.tankName))
			{
				Debug.LogError("B9TankSettings: The tank type " + tankType.tankName + " already exists");
				continue;
			}
			tankTypes.Add(tankType.tankName, tankType);
			Debug.Log("B9TankSettings: registered tank type " + tankType.tankName);
		}
		LoadedTankDefs = true;
	}

	public static TankType GetTankType(string name)
	{
		CheckTankDefs();
		if (name.IsNullOrEmpty())
		{
			return StructuralTankType;
		}
		if (!tankTypes.ContainsKey(name))
		{
			throw new KeyNotFoundException("No tank type named '" + name + "' exists");
		}
		return tankTypes[name].CloneUsingFields();
	}

	private static void CheckTankDefs()
	{
		if (!LoadedTankDefs)
		{
			throw new InvalidOperationException("The tank definitions have not been loaded yet (done after game database load).  This is likely caused by an earlier error or by ModuleManager being missing or out of date");
		}
	}
}
