using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CompoundParts;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.VesselSimulator;

public class PartSim
{
	private static readonly Pool<PartSim> pool = new Pool<PartSim>(Create, Reset);

	private readonly List<AttachNodeSim> attachNodes = new List<AttachNodeSim>();

	public double realMass;

	public double baseMass;

	public double baseMassForCoM;

	public Vector3d centerOfMass;

	public double baseCost;

	public int decoupledInStage;

	public bool fuelCrossFeed;

	public List<PartSim> fuelTargets = new List<PartSim>();

	public List<PartSim> surfaceMountFuelTargets = new List<PartSim>();

	public bool hasModuleEngines;

	public bool hasMultiModeEngine;

	private List<Part> chain = new List<Part>();

	public bool hasVessel;

	public string initialVesselName;

	public int inverseStage;

	public int resPriorityOffset;

	public bool resPriorityUseParentInverseStage;

	public double resRequestRemainingThreshold;

	public bool isEngine;

	public bool isRCS;

	public bool isFuelLine;

	public bool isFuelTank;

	public bool isLanded;

	public bool isNoPhysics;

	public bool isSepratron;

	public float postStageMassAdjust;

	public int stageIndex;

	public string name;

	public string noCrossFeedNodeKey;

	public PartSim parent;

	public AttachModes parentAttach;

	public Part part;

	public int partId;

	public ResourceContainer resourceDrains = new ResourceContainer();

	public ResourceContainer resourceFlowStates = new ResourceContainer();

	public ResourceContainer resources = new ResourceContainer();

	public double startMass;

	public double crewMassOffset;

	public string vesselName;

	public VesselType vesselType;

	public bool isEnginePlate;

	private static PartSim Create()
	{
		return new PartSim();
	}

	private static void Reset(PartSim partSim)
	{
		for (int i = 0; i < partSim.attachNodes.Count; i++)
		{
			partSim.attachNodes[i].Release();
		}
		partSim.attachNodes.Clear();
		partSim.fuelTargets.Clear();
		partSim.surfaceMountFuelTargets.Clear();
		partSim.resourceDrains.Reset();
		partSim.resourceFlowStates.Reset();
		partSim.resources.Reset();
		partSim.parent = null;
		partSim.baseCost = 0.0;
		partSim.baseMass = 0.0;
		partSim.baseMassForCoM = 0.0;
		partSim.startMass = 0.0;
		partSim.crewMassOffset = 0.0;
	}

	public void Release()
	{
		pool.Release(this);
	}

	public static PartSim New(Part p, int id, double atmosphere, LogMsg log)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Invalid comparison between Unknown and I4
		//IL_016b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Invalid comparison between Unknown and I4
		//IL_027a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0409: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		PartSim partSim = pool.Borrow();
		partSim.part = p;
		partSim.centerOfMass = Vector3d.op_Implicit(((Component)p).transform.TransformPoint(p.CoMOffset));
		partSim.partId = id;
		partSim.name = p.partInfo.name;
		log?.AppendLine("Create PartSim for ", partSim.name);
		partSim.parent = null;
		partSim.parentAttach = p.attachMode;
		partSim.fuelCrossFeed = p.fuelCrossFeed;
		partSim.noCrossFeedNodeKey = p.NoCrossFeedNodeKey;
		partSim.isEnginePlate = IsEnginePlate(p);
		if (partSim.isEnginePlate)
		{
			partSim.noCrossFeedNodeKey = "bottom";
		}
		partSim.decoupledInStage = partSim.DecoupledInStage(p);
		partSim.isFuelLine = p.HasModule<CModuleFuelLine>();
		partSim.isRCS = p.HasModule<ModuleRCS>() || p.HasModule<ModuleRCSFX>();
		partSim.isSepratron = p.IsSepratron();
		partSim.inverseStage = p.inverseStage;
		log?.AppendLine("inverseStage = ", partSim.inverseStage);
		partSim.resPriorityOffset = p.resourcePriorityOffset;
		partSim.resPriorityUseParentInverseStage = p.resourcePriorityUseParentInverseStage;
		partSim.resRequestRemainingThreshold = p.resourceRequestRemainingThreshold;
		partSim.baseCost = p.GetCostDry();
		log?.AppendLine("Parent part = ", ((Object)(object)p.parent == (Object)null) ? "null" : p.parent.partInfo.name).AppendLine<string, PhysicalSignificance>("physicalSignificance = ", p.physicalSignificance).AppendLine("PhysicsSignificance = ", p.PhysicsSignificance);
		partSim.isNoPhysics = (int)p.physicalSignificance == 1 || p.PhysicsSignificance == 1;
		if (p.HasModule<LaunchClamp>())
		{
			partSim.realMass = 0.0;
			log?.AppendLine("Ignoring mass of launch clamp");
		}
		else
		{
			partSim.crewMassOffset = p.getCrewAdjustment();
			partSim.realMass = (double)p.mass + partSim.crewMassOffset;
			log?.AppendLine("Using part.mass of " + partSim.realMass);
		}
		partSim.postStageMassAdjust = 0f;
		log?.AppendLine("Calculating postStageMassAdjust, prefabMass = ", p.prefabMass);
		int count = p.Modules.Count;
		for (int i = 0; i < count; i++)
		{
			log?.AppendLine("Module: ", p.Modules[i].moduleName);
			PartModule obj = p.Modules[i];
			IPartMassModifier val = (IPartMassModifier)(object)((obj is IPartMassModifier) ? obj : null);
			if (val != null)
			{
				log?.AppendLine<string, ModifierChangeWhen>("ChangeWhen = ", val.GetModuleMassChangeWhen());
				if ((int)val.GetModuleMassChangeWhen() == 1)
				{
					float moduleMass = val.GetModuleMass(p.prefabMass, (ModifierStagingSituation)1);
					float moduleMass2 = val.GetModuleMass(p.prefabMass, (ModifierStagingSituation)2);
					log?.AppendLine("preStage = ", moduleMass, "   postStage = ", moduleMass2);
					partSim.postStageMassAdjust += moduleMass2 - moduleMass;
				}
			}
		}
		log?.AppendLine("postStageMassAdjust = ", partSim.postStageMassAdjust);
		log?.AppendLine("crewMassOffset = ", partSim.crewMassOffset);
		for (int j = 0; j < p.Resources.Count; j++)
		{
			PartResource val2 = p.Resources[j];
			if (!double.IsNaN(val2.amount))
			{
				log?.AppendLine(val2.resourceName, " = ", val2.amount);
				partSim.resources.Add(val2.info.id, val2.amount);
				partSim.resourceFlowStates.Add(val2.info.id, val2.flowState ? 1 : 0);
			}
			else
			{
				log?.AppendLine(val2.resourceName, " is NaN. Skipping.");
			}
		}
		partSim.hasVessel = (Object)(object)p.vessel != (Object)null;
		partSim.isLanded = partSim.hasVessel && p.vessel.Landed;
		if (partSim.hasVessel)
		{
			partSim.vesselName = p.vessel.vesselName;
			partSim.vesselType = p.vesselType;
		}
		partSim.initialVesselName = p.initialVesselName;
		partSim.hasMultiModeEngine = p.HasModule<MultiModeEngine>();
		partSim.hasModuleEngines = p.HasModule<ModuleEngines>();
		partSim.isEngine = partSim.hasMultiModeEngine || partSim.hasModuleEngines;
		log?.AppendLine("Created ", partSim.name, ". Decoupled in stage ", partSim.decoupledInStage);
		return partSim;
	}

	public void CreateEngineSims(List<EngineSim> allEngines, double atmosphere, double mach, bool vectoredThrust, bool fullThrust, LogMsg log)
	{
		log?.AppendLine("CreateEngineSims for ", name);
		List<ModuleEngines> list = part.FindModulesImplementing<ModuleEngines>();
		try
		{
			if (list.Count <= 0)
			{
				return;
			}
			foreach (ModuleEngines item2 in list)
			{
				if (((PartModule)item2).isEnabled)
				{
					log?.AppendLine("Module: ", ((PartModule)item2).moduleName);
					EngineSim item = EngineSim.New(this, item2, atmosphere, (float)mach, vectoredThrust, fullThrust, log);
					allEngines.Add(item);
				}
			}
		}
		catch
		{
			Debug.Log((object)"[KER] Error Catch in CreateEngineSims");
		}
	}

	public void CreateRCSSims(List<RCSSim> allRCS, double atmosphere, double mach, bool vectoredThrust, bool fullThrust, LogMsg log)
	{
		log?.AppendLine("CreateRCSSims for ", name);
		List<ModuleRCS> list = part.FindModulesImplementing<ModuleRCS>();
		try
		{
			if (list.Count <= 0)
			{
				return;
			}
			foreach (ModuleRCS item2 in list)
			{
				if (((PartModule)item2).isEnabled)
				{
					log?.AppendLine("Module: ", ((PartModule)item2).moduleName);
					RCSSim item = RCSSim.New(this, item2, atmosphere, (float)mach, vectoredThrust, fullThrust, log);
					allRCS.Add(item);
				}
			}
		}
		catch
		{
			Debug.Log((object)"[KER] Error Catch in CreateRCSSims");
		}
	}

	public void DrainResources(double time, LogMsg log)
	{
		for (int i = 0; i < resourceDrains.Types.Count; i++)
		{
			int type = resourceDrains.Types[i];
			resources.Add(type, (0.0 - time) * resourceDrains[type]);
		}
	}

	public string DumpPartAndParentsToLog(LogMsg log, string prefix)
	{
		if (log != null)
		{
			if (parent != null)
			{
				prefix = parent.DumpPartAndParentsToLog(log, prefix) + " ";
			}
			DumpPartToLog(log, prefix);
		}
		return prefix;
	}

	public void DumpPartToLog(LogMsg log, string prefix, List<PartSim> allParts = null)
	{
		if (log == null)
		{
			return;
		}
		log.Append(prefix);
		log.Append(name);
		log.Append(":[id = ", partId, ", decouple = ", decoupledInStage);
		log.Append(", invstage = ", inverseStage);
		log.Append(", isNoPhys = ", isNoPhysics);
		log.buf.AppendFormat(", baseMass = {0}", baseMass);
		log.buf.AppendFormat(", baseMassForCoM = {0}", baseMassForCoM);
		log.Append(", fuelCF = {0}", fuelCrossFeed);
		log.Append(", noCFNKey = '{0}'", noCrossFeedNodeKey);
		log.Append(", isSep = {0}", isSepratron);
		try
		{
			for (int i = 0; i < resources.Types.Count; i++)
			{
				int type = resources.Types[i];
				log.buf.AppendFormat(", {0} = {1:g6}", ResourceContainer.GetResourceName(type), resources[type]);
			}
		}
		catch (Exception ex)
		{
			log.Append("error dumping part resources " + ex.ToString());
		}
		try
		{
			if (attachNodes.Count > 0)
			{
				log.Append(", attached = <");
				attachNodes[0].DumpToLog(log);
				for (int j = 1; j < attachNodes.Count; j++)
				{
					log.Append(", ");
					if (attachNodes[j] != null)
					{
						attachNodes[j].DumpToLog(log);
					}
				}
				log.Append(">");
			}
		}
		catch (Exception ex2)
		{
			log.Append("error dumping part nodes" + ex2.ToString());
		}
		try
		{
			if (surfaceMountFuelTargets.Count > 0)
			{
				log.Append(", surface = <");
				if (surfaceMountFuelTargets[0] != null)
				{
					log.Append(surfaceMountFuelTargets[0].name, ":", surfaceMountFuelTargets[0].partId);
				}
				for (int k = 1; k < surfaceMountFuelTargets.Count; k++)
				{
					if (surfaceMountFuelTargets[k] != null)
					{
						log.Append(", ", surfaceMountFuelTargets[k].name, ":", surfaceMountFuelTargets[k].partId);
					}
				}
				log.Append(">");
			}
		}
		catch (Exception ex3)
		{
			log.Append("error dumping part surface fuels " + ex3.ToString());
		}
		log.AppendLine("]");
		if (allParts == null)
		{
			return;
		}
		string prefix2 = prefix + " ";
		for (int l = 0; l < allParts.Count; l++)
		{
			PartSim partSim = allParts[l];
			if (partSim.parent == this)
			{
				partSim.DumpPartToLog(log, prefix2, allParts);
			}
		}
	}

	public bool EmptyOf(HashSet<int> types)
	{
		foreach (int type in types)
		{
			if (resources.HasType(type) && resourceFlowStates[type] != 0.0 && resources[type] > 0.01)
			{
				return false;
			}
		}
		return true;
	}

	public double GetMass(int currentStage, bool forCoM = false)
	{
		if (decoupledInStage >= currentStage)
		{
			return 0.0;
		}
		double num = (forCoM ? baseMassForCoM : baseMass);
		for (int i = 0; i < resources.Types.Count; i++)
		{
			num += resources.GetResourceMass(resources.Types[i]);
		}
		if ((double)postStageMassAdjust != 0.0 && currentStage <= inverseStage)
		{
			num += (double)postStageMassAdjust;
		}
		return num;
	}

	public double GetCost(int currentStage)
	{
		if (decoupledInStage >= currentStage)
		{
			return 0.0;
		}
		double num = baseCost;
		for (int i = 0; i < resources.Types.Count; i++)
		{
			num += resources.GetResourceCost(resources.Types[i]);
		}
		return num;
	}

	public void ReleasePart()
	{
		part = null;
	}

	public int GetResourcePriority()
	{
		return ((!resPriorityUseParentInverseStage || parent == null) ? inverseStage : parent.inverseStage) * 10 + resPriorityOffset;
	}

	public void GetSourceSet(int type, List<PartSim> allParts, HashSet<PartSim> visited, HashSet<PartSim> allSources, LogMsg log, string indent)
	{
		int priMax = int.MinValue;
		GetSourceSet_Internal(type, allParts, visited, allSources, ref priMax, log, indent);
		log?.AppendLine(allSources.Count, " parts with priority of ", priMax);
	}

	public void GetSourceSet_Internal(int type, List<PartSim> allParts, HashSet<PartSim> visited, HashSet<PartSim> allSources, ref int priMax, LogMsg log, string indent)
	{
		//IL_0227: Unknown result type (might be due to invalid IL or missing references)
		if (log != null)
		{
			log.Append(indent, "GetSourceSet_Internal(", ResourceContainer.GetResourceName(type), ") for ").AppendLine(name, ":", partId);
			indent += "  ";
		}
		if (visited.Contains(this))
		{
			log?.Append(indent, "Nothing added, already visited (", name, ":").AppendLine(partId + ")");
			return;
		}
		log?.AppendLine(indent, "Adding this to visited");
		visited.Add(this);
		_ = allSources.Count;
		for (int i = 0; i < fuelTargets.Count; i++)
		{
			PartSim partSim = fuelTargets[i];
			if (partSim != null)
			{
				if (visited.Contains(partSim))
				{
					log?.Append(indent, "Fuel target already visited, skipping (", partSim.name, ":").AppendLine(partSim.partId, ")");
					continue;
				}
				log?.Append(indent, "Adding fuel target as source (", partSim.name, ":").AppendLine(partSim.partId, ")");
				partSim.GetSourceSet_Internal(type, allParts, visited, allSources, ref priMax, log, indent);
			}
		}
		if (fuelCrossFeed)
		{
			for (int j = 0; j < surfaceMountFuelTargets.Count; j++)
			{
				PartSim partSim2 = surfaceMountFuelTargets[j];
				if (partSim2 != null)
				{
					if (visited.Contains(partSim2))
					{
						log?.Append(indent, "Surface part already visited, skipping (", partSim2.name, ":").AppendLine(partSim2.partId, ")");
						continue;
					}
					log?.Append(indent, "Adding surface part as source (", partSim2.name, ":").AppendLine(partSim2.partId, ")");
					partSim2.GetSourceSet_Internal(type, allParts, visited, allSources, ref priMax, log, indent);
				}
			}
			_ = allSources.Count;
			for (int k = 0; k < attachNodes.Count; k++)
			{
				AttachNodeSim attachNodeSim = attachNodes[k];
				if (attachNodeSim.attachedPartSim == null || (int)attachNodeSim.nodeType != 0 || (!string.IsNullOrEmpty(noCrossFeedNodeKey) && attachNodeSim.id.Contains(noCrossFeedNodeKey)))
				{
					continue;
				}
				if (visited.Contains(attachNodeSim.attachedPartSim))
				{
					log?.Append(indent, "Attached part already visited, skipping (", attachNodeSim.attachedPartSim.name, ":").AppendLine(attachNodeSim.attachedPartSim.partId, ")");
					continue;
				}
				bool flag = true;
				if (attachNodeSim.attachedPartSim.isEnginePlate)
				{
					foreach (AttachNodeSim attachNode in attachNodeSim.attachedPartSim.attachNodes)
					{
						if (attachNode.attachedPartSim == this && attachNode.id == "bottom")
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					log?.Append(indent, "Adding attached part as source  (", attachNodeSim.attachedPartSim.name, ":").AppendLine(attachNodeSim.attachedPartSim.partId, ")");
					attachNodeSim.attachedPartSim.GetSourceSet_Internal(type, allParts, visited, allSources, ref priMax, log, indent);
				}
			}
		}
		if (resources.HasType(type) && resourceFlowStates[type] > 0.0)
		{
			if (resources[type] > resRequestRemainingThreshold)
			{
				int resourcePriority = GetResourcePriority();
				if (resourcePriority > priMax)
				{
					allSources.Clear();
					priMax = resourcePriority;
				}
				if (resourcePriority == priMax)
				{
					log?.Append(indent, "Adding enabled tank as source (", name, ":").AppendLine(partId, ")");
					allSources.Add(this);
				}
			}
			else
			{
				log?.Append(indent, name + " not enough " + ResourceContainer.GetResourceName(type)).AppendLine("  Requested = " + resRequestRemainingThreshold + " actual " + resources[type]);
			}
		}
		else
		{
			log?.Append(indent, name + " not fuel tank or disabled. HasType = ", resources.HasType(type)).AppendLine("  FlowState = " + resourceFlowStates[type]);
		}
	}

	public double GetStartMass()
	{
		return startMass;
	}

	public void RemoveAttachedParts(HashSet<PartSim> partSims)
	{
		for (int i = 0; i < attachNodes.Count; i++)
		{
			AttachNodeSim attachNodeSim = attachNodes[i];
			if (partSims.Contains(attachNodeSim.attachedPartSim))
			{
				attachNodeSim.attachedPartSim = null;
			}
		}
		for (int j = 0; j < fuelTargets.Count; j++)
		{
			PartSim partSim = fuelTargets[j];
			if (partSim != null && partSims.Contains(partSim))
			{
				fuelTargets[j] = null;
			}
		}
		for (int k = 0; k < surfaceMountFuelTargets.Count; k++)
		{
			PartSim partSim2 = surfaceMountFuelTargets[k];
			if (partSim2 != null && partSims.Contains(partSim2))
			{
				surfaceMountFuelTargets[k] = null;
			}
		}
	}

	public void SetupAttachNodes(Dictionary<Part, PartSim> partSimLookup, LogMsg log)
	{
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		log?.AppendLine("SetupAttachNodes for ", name, ":", partId);
		attachNodes.Clear();
		for (int i = 0; i < part.attachNodes.Count; i++)
		{
			AttachNode val = part.attachNodes[i];
			log?.AppendLine("AttachNode ", val.id, " = ", ((Object)(object)val.attachedPart != (Object)null) ? val.attachedPart.partInfo.name : "null");
			if ((Object)(object)val.attachedPart != (Object)null && val.id != "Strut")
			{
				if (partSimLookup.TryGetValue(val.attachedPart, out var value))
				{
					log?.AppendLine("Adding attached node ", value.name, ":", value.partId);
					attachNodes.Add(AttachNodeSim.New(value, val.id, val.nodeType));
				}
				else
				{
					log?.AppendLine("No PartSim for attached part (", val.attachedPart.partInfo.name, ")");
				}
			}
		}
		for (int j = 0; j < part.fuelLookupTargets.Count; j++)
		{
			Part val2 = part.fuelLookupTargets[j];
			if ((Object)(object)val2 != (Object)null)
			{
				if (partSimLookup.TryGetValue(val2, out var value2))
				{
					log?.AppendLine("Fuel target: ", value2.name, ":", value2.partId);
					fuelTargets.Add(value2);
				}
				else
				{
					log?.AppendLine("No PartSim for fuel target (", ((Object)val2).name, ")");
				}
			}
		}
	}

	public void SetupParent(Dictionary<Part, PartSim> partSimLookup, LogMsg log)
	{
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Invalid comparison between Unknown and I4
		if (!((Object)(object)part.parent != (Object)null))
		{
			return;
		}
		parent = null;
		if (partSimLookup.TryGetValue(part.parent, out parent))
		{
			log?.AppendLine("Parent part is ", parent.name, ":", parent.partId);
			if ((int)part.attachMode == 1 && part.attachRules.srfAttach && part.fuelCrossFeed && part.parent.fuelCrossFeed)
			{
				log?.Append("Added (", name, ":", partId).AppendLine(", ", parent.name, ":", parent.partId, ") to surface mounted fuel targets.");
				parent.surfaceMountFuelTargets.Add(this);
				surfaceMountFuelTargets.Add(parent);
			}
		}
		else
		{
			log?.AppendLine("No PartSim for parent part (", part.parent.partInfo.name, ")");
		}
	}

	public double TimeToDrainResource(LogMsg log)
	{
		double num = double.MaxValue;
		for (int i = 0; i < resourceDrains.Types.Count; i++)
		{
			int type = resourceDrains.Types[i];
			if (resourceDrains[type] > 0.0)
			{
				num = Math.Min(num, resources[type] / resourceDrains[type]);
			}
		}
		return num;
	}

	public double CalcTimeToDrainResource(double consumptionRate, int resource, LogMsg log)
	{
		double num = double.MaxValue;
		if (consumptionRate > 0.0)
		{
			num = Math.Min(num, resources[resource] / consumptionRate);
		}
		return num;
	}

	private Vector3 CalculateThrustVector(List<Transform> thrustTransforms, LogMsg log)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		if (thrustTransforms == null)
		{
			return Vector3.forward;
		}
		Vector3 val = Vector3.zero;
		for (int i = 0; i < thrustTransforms.Count; i++)
		{
			Transform val2 = thrustTransforms[i];
			if (log != null)
			{
				StringBuilder buf = log.buf;
				object[] obj = new object[4]
				{
					val2.forward.x,
					val2.forward.y,
					val2.forward.z,
					null
				};
				Vector3 forward = val2.forward;
				obj[3] = ((Vector3)(ref forward)).magnitude;
				buf.AppendFormat("Transform = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", obj);
			}
			val -= val2.forward;
		}
		log?.buf.AppendFormat("ThrustVec  = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", val.x, val.y, val.z, ((Vector3)(ref val)).magnitude);
		((Vector3)(ref val)).Normalize();
		log?.buf.AppendFormat("ThrustVecN = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", val.x, val.y, val.z, ((Vector3)(ref val)).magnitude);
		return val;
	}

	private int DecoupledInStage(Part thePart)
	{
		int num = -1;
		Part val = thePart;
		if ((Object)(object)val.parent == (Object)null)
		{
			return num;
		}
		chain.Clear();
		while ((Object)(object)thePart != (Object)null)
		{
			chain.Add(thePart);
			if (thePart.inverseStage > num)
			{
				ModuleDecouple module = thePart.GetModule<ModuleDecouple>();
				ModuleDockingNode module2 = thePart.GetModule<ModuleDockingNode>();
				ModuleAnchoredDecoupler module3 = thePart.GetModule<ModuleAnchoredDecoupler>();
				if ((Object)(object)module != (Object)null)
				{
					AttachNode val2 = thePart.FindAttachNode(((ModuleDecouplerBase)module).explosiveNodeID);
					if (((ModuleDecouplerBase)module).isOmniDecoupler)
					{
						num = thePart.inverseStage;
					}
					else if (val2 != null)
					{
						if (((Object)(object)thePart.parent != (Object)null && (Object)(object)val2.attachedPart == (Object)(object)thePart.parent) || val2.attachedPart.ContainedPart(chain))
						{
							num = thePart.inverseStage;
						}
					}
					else
					{
						num = thePart.inverseStage;
					}
				}
				if ((Object)(object)module3 != (Object)null)
				{
					AttachNode val3 = thePart.FindAttachNode(((ModuleDecouplerBase)module3).explosiveNodeID);
					if (val3 != null)
					{
						if (((Object)(object)thePart.parent != (Object)null && (Object)(object)val3.attachedPart == (Object)(object)thePart.parent) || val3.attachedPart.ContainedPart(chain))
						{
							num = thePart.inverseStage;
						}
					}
					else
					{
						num = thePart.inverseStage;
					}
				}
				if ((Object)(object)module2 != (Object)null && !((Object)(object)val == (Object)(object)thePart))
				{
					num = thePart.inverseStage;
				}
			}
			thePart = thePart.parent;
		}
		return num;
	}

	private static bool IsEnginePlate(Part thePart)
	{
		ModuleDecouple module = thePart.GetModule<ModuleDecouple>();
		if ((Object)(object)module != (Object)null && ((PartModule)module).IsStageable() && (Object)(object)thePart.GetModule<ModuleDynamicNodes>() != (Object)null)
		{
			return true;
		}
		return false;
	}

	private bool IsSepratron()
	{
		if (!part.ActivatesEvenIfDisconnected)
		{
			return false;
		}
		return ((IEnumerable)part.Modules).OfType<ModuleEngines>().Any((ModuleEngines module) => module.throttleLocked);
	}
}
