using System;
using System.Collections.Generic;
using CompoundParts;
using UnityEngine;

namespace KerbalEngineer.Extensions;

public static class PartExtensions
{
	public class ProtoModuleDecoupler
	{
		private readonly PartModule module;

		public double EjectionForce { get; private set; }

		public bool IsOmniDecoupler { get; private set; }

		public bool IsStageEnabled { get; private set; }

		public ProtoModuleDecoupler(PartModule module)
		{
			this.module = module;
			if (this.module is ModuleDecouple)
			{
				SetModuleDecouple();
			}
			else if (this.module is ModuleAnchoredDecoupler)
			{
				SetModuleAnchoredDecoupler();
			}
		}

		private void SetModuleAnchoredDecoupler()
		{
			PartModule obj = module;
			ModuleAnchoredDecoupler val = (ModuleAnchoredDecoupler)(object)((obj is ModuleAnchoredDecoupler) ? obj : null);
			if (!((Object)(object)val == (Object)null))
			{
				EjectionForce = ((ModuleDecouplerBase)val).ejectionForce;
				IsStageEnabled = ((PartModule)val).stagingEnabled;
			}
		}

		private void SetModuleDecouple()
		{
			PartModule obj = module;
			ModuleDecouple val = (ModuleDecouple)(object)((obj is ModuleDecouple) ? obj : null);
			if (!((Object)(object)val == (Object)null))
			{
				EjectionForce = ((ModuleDecouplerBase)val).ejectionForce;
				IsOmniDecoupler = ((ModuleDecouplerBase)val).isOmniDecoupler;
				IsStageEnabled = ((PartModule)val).stagingEnabled;
			}
		}
	}

	public class ProtoModuleEngine
	{
		private readonly PartModule module;

		public double MaximumThrust { get; private set; }

		public double MinimumThrust { get; private set; }

		public List<Propellant> Propellants { get; private set; }

		public ProtoModuleEngine(PartModule module)
		{
			this.module = module;
			if (module is ModuleEngines)
			{
				SetModuleEngines();
			}
		}

		public float GetSpecificImpulse(float atmosphere)
		{
			if (module is ModuleEngines)
			{
				PartModule obj = module;
				return ((ModuleEngines)((obj is ModuleEngines) ? obj : null)).atmosphereCurve.Evaluate(atmosphere);
			}
			return 0f;
		}

		private void SetModuleEngines()
		{
			PartModule obj = module;
			ModuleEngines val = (ModuleEngines)(object)((obj is ModuleEngines) ? obj : null);
			if (!((Object)(object)val == (Object)null))
			{
				MaximumThrust = (double)val.maxThrust * ((double)val.thrustPercentage * 0.01);
				MinimumThrust = val.minThrust;
				Propellants = val.propellants;
			}
		}
	}

	public static bool ContainsResource(this Part part, int resourceId)
	{
		return part.Resources.Contains(resourceId);
	}

	public static bool ContainsResources(this Part part)
	{
		for (int i = 0; i < part.Resources.dict.Count; i++)
		{
			if (part.Resources.dict.At(i).amount > 0.0)
			{
				return true;
			}
		}
		return false;
	}

	public static double GetCostDry(this Part part)
	{
		return (double)part.partInfo.cost - part.GetResourceCostMax() + (double)part.GetModuleCosts(0f, (ModifierStagingSituation)0);
	}

	public static double GetCostMax(this Part part)
	{
		return part.partInfo.cost + part.GetModuleCosts(0f, (ModifierStagingSituation)0);
	}

	public static double GetModuleCostsNoAlloc(this Part part, float defaultCost)
	{
		float num = 0f;
		for (int i = 0; i < part.Modules.Count; i++)
		{
			PartModule val = part.Modules[i];
			if (val is IPartCostModifier)
			{
				num += ((IPartCostModifier)((val is IPartCostModifier) ? val : null)).GetModuleCost(defaultCost, (ModifierStagingSituation)0);
			}
		}
		return num;
	}

	public static double GetCostWet(this Part part)
	{
		return (double)part.partInfo.cost - part.GetResourceCostInverted() + part.GetModuleCostsNoAlloc(0f);
	}

	public static double GetDryMass(this Part part)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)part.physicalSignificance != 0)
		{
			return 0.0;
		}
		return (double)part.mass + part.getCrewAdjustment();
	}

	public static double getCrewAdjustment(this Part part)
	{
		if (HighLogic.LoadedSceneIsEditor && PhysicsGlobals.KerbalCrewMass != 0f && ShipConstruction.ShipManifest != null)
		{
			List<ProtoCrewMember> allCrew = ShipConstruction.ShipManifest.GetAllCrew(false);
			int num = 0;
			foreach (ProtoCrewMember item in allCrew)
			{
				if (item != null)
				{
					num++;
				}
			}
			if (num > 0)
			{
				PartCrewManifest partCrewManifest = ShipConstruction.ShipManifest.GetPartCrewManifest(part.craftID);
				int num2 = 0;
				ProtoCrewMember[] partCrew = partCrewManifest.GetPartCrew();
				for (int i = 0; i < partCrew.Length; i++)
				{
					if (partCrew[i] != null)
					{
						num2++;
					}
				}
				if (num2 < num)
				{
					return (0f - PhysicsGlobals.KerbalCrewMass) * (float)(num - num2);
				}
			}
		}
		return 0.0;
	}

	public static T GetModule<T>(this Part part) where T : PartModule
	{
		for (int i = 0; i < part.Modules.Count; i++)
		{
			PartModule val = part.Modules[i];
			if (val is T)
			{
				return (T)(object)val;
			}
		}
		return default(T);
	}

	public static T GetModule<T>(this Part part, string className) where T : PartModule
	{
		PartModule obj = part.Modules[className];
		return (T)(object)((obj is T) ? obj : null);
	}

	public static T GetModule<T>(this Part part, int classId) where T : PartModule
	{
		PartModule obj = part.Modules[classId];
		return (T)(object)((obj is T) ? obj : null);
	}

	public static ModuleAlternator GetModuleAlternator(this Part part)
	{
		return part.GetModule<ModuleAlternator>();
	}

	public static ModuleDeployableSolarPanel GetModuleDeployableSolarPanel(this Part part)
	{
		return part.GetModule<ModuleDeployableSolarPanel>();
	}

	public static ModuleEngines GetModuleEngines(this Part part)
	{
		return part.GetModule<ModuleEngines>();
	}

	public static ModuleGenerator GetModuleGenerator(this Part part)
	{
		return part.GetModule<ModuleGenerator>();
	}

	public static ModuleGimbal GetModuleGimbal(this Part part)
	{
		return part.GetModule<ModuleGimbal>();
	}

	public static ModuleEngines GetModuleMultiModeEngine(this Part part)
	{
		MultiModeEngine module = part.GetModule<MultiModeEngine>();
		if ((Object)(object)module != (Object)null)
		{
			string mode = module.mode;
			for (int i = 0; i < part.Modules.Count; i++)
			{
				PartModule obj = part.Modules[i];
				ModuleEngines val = (ModuleEngines)(object)((obj is ModuleEngines) ? obj : null);
				if ((Object)(object)val != (Object)null && val.engineID == mode)
				{
					return val;
				}
			}
		}
		return null;
	}

	public static ModuleParachute GetModuleParachute(this Part part)
	{
		return part.GetModule<ModuleParachute>();
	}

	public static ModuleRCS GetModuleRcs(this Part part)
	{
		return part.GetModule<ModuleRCS>();
	}

	public static List<T> GetModules<T>(this Part part) where T : PartModule
	{
		List<T> list = new List<T>();
		for (int i = 0; i < part.Modules.Count; i++)
		{
			PartModule obj = part.Modules[i];
			T val = (T)(object)((obj is T) ? obj : null);
			if ((Object)(object)val != (Object)null)
			{
				list.Add(val);
			}
		}
		return list;
	}

	public static ProtoModuleDecoupler GetProtoModuleDecoupler(this Part part)
	{
		PartModule module = (PartModule)(object)part.GetModule<ModuleDecouple>();
		if ((Object)(object)module == (Object)null)
		{
			module = (PartModule)(object)part.GetModule<ModuleAnchoredDecoupler>();
		}
		if ((Object)(object)module != (Object)null)
		{
			return new ProtoModuleDecoupler(module);
		}
		return null;
	}

	public static ProtoModuleEngine GetProtoModuleEngine(this Part part)
	{
		PartModule module = (PartModule)(object)part.GetModule<ModuleEngines>();
		if ((Object)(object)module != (Object)null)
		{
			return new ProtoModuleEngine(module);
		}
		module = (PartModule)(((object)part.GetModuleMultiModeEngine()) ?? ((object)part.GetModule<ModuleEnginesFX>()));
		if ((Object)(object)module != (Object)null)
		{
			return new ProtoModuleEngine(module);
		}
		return null;
	}

	public static double GetResourceCost(this Part part)
	{
		double num = 0.0;
		for (int i = 0; i < part.Resources.dict.Count; i++)
		{
			PartResource val = part.Resources.dict.At(i);
			num += val.amount * (double)val.info.unitCost;
		}
		return num;
	}

	public static double GetResourceCostInverted(this Part part)
	{
		double num = 0.0;
		for (int i = 0; i < part.Resources.dict.Count; i++)
		{
			PartResource val = part.Resources.dict.At(i);
			num += (val.maxAmount - val.amount) * (double)val.info.unitCost;
		}
		return num;
	}

	public static double GetResourceCostMax(this Part part)
	{
		double num = 0.0;
		for (int i = 0; i < part.Resources.dict.Count; i++)
		{
			PartResource val = part.Resources.dict.At(i);
			num += val.maxAmount * (double)val.info.unitCost;
		}
		return num;
	}

	public static double GetWetMass(this Part part)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		if ((int)part.physicalSignificance != 0)
		{
			return part.GetResourceMass();
		}
		return (double)(part.mass + part.GetResourceMass()) + part.getCrewAdjustment();
	}

	public static bool HasModule<T>(this Part part) where T : PartModule
	{
		for (int i = 0; i < part.Modules.Count; i++)
		{
			if (part.Modules[i] is T)
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasModule<T>(this Part part, Func<T, bool> predicate) where T : PartModule
	{
		for (int i = 0; i < part.Modules.Count; i++)
		{
			PartModule val = part.Modules[i];
			if (val is T && predicate((T)(object)((val is T) ? val : null)))
			{
				return true;
			}
		}
		return false;
	}

	public static bool HasModule(this Part part, string className)
	{
		return part.Modules.Contains(className);
	}

	public static bool HasModule(this Part part, int moduleId)
	{
		return part.Modules.Contains(moduleId);
	}

	public static bool HasOneShotAnimation(this Part part)
	{
		PartModule module = (PartModule)(object)part.GetModule<ModuleAnimateGeneric>();
		if ((Object)(object)module != (Object)null)
		{
			return ((ModuleAnimateGeneric)((module is ModuleAnimateGeneric) ? module : null)).isOneShot;
		}
		return false;
	}

	public static bool IsCommandModule(this Part part)
	{
		return part.HasModule<ModuleCommand>();
	}

	public static bool IsDecoupledInStage(this Part part, int stage)
	{
		if ((part.IsDecoupler() || part.IsLaunchClamp()) && part.inverseStage == stage)
		{
			return true;
		}
		if ((Object)(object)part.parent == (Object)null)
		{
			return false;
		}
		return part.parent.IsDecoupledInStage(stage);
	}

	public static bool IsDecoupler(this Part part)
	{
		if (!part.HasModule<ModuleDecouple>())
		{
			return part.HasModule<ModuleAnchoredDecoupler>();
		}
		return true;
	}

	public static bool IsEngine(this Part part)
	{
		return part.HasModule<ModuleEngines>();
	}

	public static bool IsFuelLine(this Part part)
	{
		return part.HasModule<CModuleFuelLine>();
	}

	public static bool IsGenerator(this Part part)
	{
		return part.HasModule<ModuleGenerator>();
	}

	public static bool IsLaunchClamp(this Part part)
	{
		return part.HasModule<LaunchClamp>();
	}

	public static bool IsParachute(this Part part)
	{
		return part.HasModule<ModuleParachute>();
	}

	public static bool IsPrimary(this Part part, List<Part> partsList, PartModule module)
	{
		for (int i = 0; i < partsList.Count; i++)
		{
			Part val = partsList[i];
			if (val.HasModule(module.ClassID))
			{
				if (!((Object)(object)val == (Object)(object)part))
				{
					break;
				}
				return true;
			}
		}
		return false;
	}

	public static bool IsRcsModule(this Part part)
	{
		return part.HasModule<ModuleRCS>();
	}

	public static bool IsSepratron(this Part part)
	{
		for (int i = 0; i < part.Modules.Count; i++)
		{
			if (part.Modules[i] is ModuleEngines && ((ModuleEngines)/*isinst with value type is only supported in some contexts*/).throttleLocked)
			{
				return true;
			}
		}
		return false;
	}

	public static bool ContainedPart(this Part part, List<Part> chain)
	{
		for (int i = 0; i < chain.Count; i++)
		{
			if ((Object)(object)chain[i] == (Object)(object)part)
			{
				return true;
			}
		}
		return false;
	}

	public static bool IsSolarPanel(this Part part)
	{
		return part.HasModule<ModuleDeployableSolarPanel>();
	}

	public static bool IsSolidRocket(this Part part)
	{
		if (part.HasModule<ModuleEngines>())
		{
			return part.GetModuleEngines().throttleLocked;
		}
		return false;
	}
}
