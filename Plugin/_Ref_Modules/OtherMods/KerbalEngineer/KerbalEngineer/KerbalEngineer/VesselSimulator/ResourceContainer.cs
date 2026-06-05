using System.Collections.Generic;
using UnityEngine;

namespace KerbalEngineer.VesselSimulator;

public class ResourceContainer
{
	private Dictionary<int, double> resources = new Dictionary<int, double>();

	private List<int> types = new List<int>();

	public double this[int type]
	{
		get
		{
			if (resources.TryGetValue(type, out var value))
			{
				return value;
			}
			return 0.0;
		}
		set
		{
			if (resources.ContainsKey(type))
			{
				resources[type] = value;
				return;
			}
			resources.Add(type, value);
			types.Add(type);
		}
	}

	public List<int> Types => types;

	public double Mass
	{
		get
		{
			double num = 0.0;
			foreach (double value in resources.Values)
			{
				num += value;
			}
			return num;
		}
	}

	public bool Empty
	{
		get
		{
			foreach (int key in resources.Keys)
			{
				if (resources[key] > 0.0001)
				{
					return false;
				}
			}
			return true;
		}
	}

	public bool HasType(int type)
	{
		return resources.ContainsKey(type);
	}

	public bool EmptyOf(HashSet<int> types)
	{
		foreach (int type in types)
		{
			if (HasType(type) && resources[type] > 0.0001)
			{
				return false;
			}
		}
		return true;
	}

	public void Add(int type, double amount)
	{
		if (resources.ContainsKey(type))
		{
			resources[type] += amount;
			return;
		}
		resources.Add(type, amount);
		types.Add(type);
	}

	public void Reset()
	{
		resources.Clear();
		types.Clear();
	}

	public void Debug()
	{
		foreach (int key in resources.Keys)
		{
			MonoBehaviour.print((object)(" -> " + GetResourceName(key) + " = " + resources[key]));
		}
	}

	public double GetResourceMass(int type)
	{
		double num = GetResourceDensity(type);
		if (num != 0.0)
		{
			return resources[type] * num;
		}
		return 0.0;
	}

	public double GetResourceCost(int type)
	{
		double resourceUnitCost = GetResourceUnitCost(type);
		if (resourceUnitCost != 0.0)
		{
			return resources[type] * resourceUnitCost;
		}
		return 0.0;
	}

	public static ResourceFlowMode GetResourceFlowMode(int type)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return PartResourceLibrary.Instance.GetDefinition(type).resourceFlowMode;
	}

	public static ResourceTransferMode GetResourceTransferMode(int type)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		return PartResourceLibrary.Instance.GetDefinition(type).resourceTransferMode;
	}

	public static float GetResourceDensity(int type)
	{
		return PartResourceLibrary.Instance.GetDefinition(type).density;
	}

	public static string GetResourceName(int type)
	{
		return PartResourceLibrary.Instance.GetDefinition(type).name;
	}

	public static double GetResourceUnitCost(int type)
	{
		return PartResourceLibrary.Instance.GetDefinition(type).unitCost;
	}
}
