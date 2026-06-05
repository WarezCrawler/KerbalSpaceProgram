using System;
using System.Collections.Generic;
using System.Diagnostics;
using CompoundParts;
using KSP.UI.Screens;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.VesselSimulator;

public class Simulation
{
	private const double SECONDS_PER_DAY = 86400.0;

	private readonly Stopwatch _timer;

	private List<EngineSim> activeEngines;

	private List<EngineSim> allEngines;

	private List<RCSSim> allRCS;

	private List<PartSim> allFuelLines;

	private List<PartSim> allParts;

	private double atmosphere;

	private int currentStage;

	private double currentisp;

	private HashSet<PartSim> decoupledParts;

	private bool doingCurrent;

	private List<PartSim> dontStageParts;

	private List<List<PartSim>> dontStagePartsLists;

	private HashSet<PartSim> drainingParts;

	private HashSet<int> drainingResources;

	private double gravity;

	private Dictionary<Part, PartSim> partSimLookup;

	private LogMsg log;

	private int lastStage;

	private List<Part> partList;

	private double simpleTotalThrust;

	private double stageStartMass;

	private Vector3d stageStartCom;

	private double stageTime;

	private double stepEndMass;

	private double stepStartMass;

	private double totalStageActualThrust;

	private double totalStageFlowRate;

	private double totalStageIspFlowRate;

	private double totalStageThrust;

	private ForceAccumulator totalStageThrustForce;

	private Vector3 vecActualThrust;

	private Vector3 vecStageDeltaV;

	private Vector3 vecThrust;

	private double mach;

	private float maxMach;

	public string vesselName;

	public VesselType vesselType;

	private WeightedVectorAverager vectorAverager;

	private double RCSIsp;

	private double RCSThrust;

	private double RCSDeltaV;

	private double RCSTWR;

	private double RCSBurnTime;

	private double ShipMass
	{
		get
		{
			double num = 0.0;
			for (int i = 0; i < allParts.Count; i++)
			{
				num += allParts[i].GetMass(currentStage);
			}
			return num;
		}
	}

	private Vector3d ShipCom
	{
		get
		{
			//IL_0023: Unknown result type (might be due to invalid IL or missing references)
			//IL_0052: Unknown result type (might be due to invalid IL or missing references)
			vectorAverager.Reset();
			for (int i = 0; i < allParts.Count; i++)
			{
				PartSim partSim = allParts[i];
				vectorAverager.Add(partSim.centerOfMass, partSim.GetMass(currentStage, forCoM: true));
			}
			return vectorAverager.Get();
		}
	}

	public Simulation()
	{
		_timer = new Stopwatch();
		activeEngines = new List<EngineSim>();
		allEngines = new List<EngineSim>();
		allRCS = new List<RCSSim>();
		allFuelLines = new List<PartSim>();
		allParts = new List<PartSim>();
		decoupledParts = new HashSet<PartSim>();
		dontStagePartsLists = new List<List<PartSim>>();
		drainingParts = new HashSet<PartSim>();
		drainingResources = new HashSet<int>();
		partSimLookup = new Dictionary<Part, PartSim>();
		partList = new List<Part>();
		totalStageThrustForce = new ForceAccumulator();
		vectorAverager = new WeightedVectorAverager();
	}

	public bool PrepareSimulation(LogMsg _log, List<Part> parts, double theGravity, double theAtmosphere = 0.0, double theMach = 0.0, bool dumpTree = false, bool vectoredThrust = false, bool fullThrust = false)
	{
		//IL_012b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0130: Unknown result type (might be due to invalid IL or missing references)
		log = _log;
		if (log != null)
		{
			log.AppendLine("PrepareSimulation started");
		}
		_timer.Reset();
		_timer.Start();
		partList = parts;
		gravity = theGravity;
		atmosphere = theAtmosphere;
		mach = theMach;
		lastStage = StageManager.LastStage;
		maxMach = 1f;
		if (log != null)
		{
			log.AppendLine("lastStage = ", lastStage);
		}
		allParts.Clear();
		allFuelLines.Clear();
		drainingParts.Clear();
		allEngines.Clear();
		activeEngines.Clear();
		drainingResources.Clear();
		partSimLookup.Clear();
		if (partList.Count > 0 && (Object)(object)partList[0].vessel != (Object)null)
		{
			vesselName = partList[0].vessel.vesselName;
			vesselType = partList[0].vessel.vesselType;
		}
		int num = 1;
		for (int i = 0; i < partList.Count; i++)
		{
			Part val = partList[i];
			if (partSimLookup.ContainsKey(val))
			{
				if (log != null)
				{
					log.AppendLine("Part ", ((Object)val).name, " appears in vessel list more than once");
				}
				continue;
			}
			PartSim partSim = PartSim.New(val, num, atmosphere, log);
			partSimLookup.Add(val, partSim);
			allParts.Add(partSim);
			if (partSim.isFuelLine)
			{
				allFuelLines.Add(partSim);
			}
			if (partSim.isEngine)
			{
				partSim.CreateEngineSims(allEngines, atmosphere, mach, vectoredThrust, fullThrust, log);
			}
			if (partSim.isRCS)
			{
				partSim.CreateRCSSims(allRCS, atmosphere, mach, vectoredThrust, fullThrust, log);
			}
			num++;
		}
		for (int j = 0; j < allEngines.Count; j++)
		{
			maxMach = Mathf.Max(maxMach, allEngines[j].maxMach);
		}
		UpdateActiveEngines();
		for (int k = 0; k < allParts.Count; k++)
		{
			allParts[k].SetupParent(partSimLookup, log);
		}
		if (HighLogic.LoadedSceneIsEditor)
		{
			for (int l = 0; l < allFuelLines.Count; l++)
			{
				PartSim partSim2 = allFuelLines[l];
				CModuleFuelLine module = partSim2.part.GetModule<CModuleFuelLine>();
				if ((Object)(object)((CompoundPartModule)module).target != (Object)null)
				{
					if (partSimLookup.TryGetValue(((CompoundPartModule)module).target, out var value))
					{
						if (log != null)
						{
							log.AppendLine("Fuel line target is ", value.name, ":", value.partId);
						}
						value.fuelTargets.Add(partSim2.parent);
					}
					else if (log != null)
					{
						log.AppendLine("No PartSim for fuel line target (", partSim2.part.partInfo.name, ")");
					}
				}
				else if (log != null)
				{
					log.AppendLine("Fuel line target is null");
				}
			}
		}
		if (log != null)
		{
			log.AppendLine("SetupAttachNodes and count stages");
		}
		for (int m = 0; m < allParts.Count; m++)
		{
			PartSim partSim3 = allParts[m];
			partSim3.SetupAttachNodes(partSimLookup, log);
			if (partSim3.decoupledInStage >= lastStage)
			{
				lastStage = partSim3.decoupledInStage + 1;
			}
		}
		if (log != null)
		{
			log.AppendLine("ReleaseParts");
		}
		for (int n = 0; n < allParts.Count; n++)
		{
			allParts[n].ReleasePart();
		}
		partList = null;
		_timer.Stop();
		if (log != null)
		{
			log.AppendLine("PrepareSimulation: ", _timer.ElapsedMilliseconds, "ms");
			log.Flush();
		}
		Dump();
		log = null;
		return true;
	}

	public Stage[] RunSimulation(LogMsg _log)
	{
		//IL_0235: Unknown result type (might be due to invalid IL or missing references)
		//IL_023a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0375: Unknown result type (might be due to invalid IL or missing references)
		//IL_037a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0724: Unknown result type (might be due to invalid IL or missing references)
		//IL_072a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075a: Unknown result type (might be due to invalid IL or missing references)
		//IL_075f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0764: Unknown result type (might be due to invalid IL or missing references)
		log = _log;
		if (log != null)
		{
			log.AppendLine("RunSimulation started");
		}
		_timer.Reset();
		_timer.Start();
		currentStage = lastStage;
		bool flag = false;
		for (int i = 0; i < allEngines.Count; i++)
		{
			EngineSim engineSim = allEngines[i];
			if (log != null)
			{
				log.AppendLine("Testing engine mod of ", engineSim.partSim.name, ":", engineSim.partSim.partId);
			}
			bool isActive = engineSim.isActive;
			bool flag2 = engineSim.partSim.inverseStage >= currentStage;
			if (log != null)
			{
				log.AppendLine("bActive = ", isActive, "   bStage = ", flag2);
			}
			if (HighLogic.LoadedSceneIsFlight)
			{
				if (isActive)
				{
					flag = true;
				}
				if (isActive != flag2)
				{
					if (log != null)
					{
						log.AppendLine("Need to do current active engines first");
					}
					doingCurrent = true;
				}
			}
			else if (flag2)
			{
				if (log != null)
				{
					log.AppendLine("Marking as active");
				}
				engineSim.isActive = true;
			}
		}
		if (doingCurrent && flag)
		{
			currentStage++;
		}
		else
		{
			ActivateStage();
			doingCurrent = false;
		}
		BuildDontStageLists(log);
		if (log != null)
		{
			log.Flush();
		}
		Stage[] array = new Stage[currentStage + 1];
		int num = currentStage;
		while (currentStage >= 0)
		{
			if (log != null)
			{
				log.AppendLine("Simulating stage ", currentStage);
				log.Flush();
				_timer.Reset();
				_timer.Start();
			}
			UpdateResourceDrains();
			stageStartMass = UpdatePartMasses();
			if (log != null)
			{
				allParts[0].DumpPartToLog(log, "", allParts);
			}
			Stage stage = new Stage();
			stageTime = 0.0;
			vecStageDeltaV = Vector3.zero;
			stageStartCom = ShipCom;
			stepStartMass = stageStartMass;
			stepEndMass = 0.0;
			CalculateThrustAndISP();
			stage.thrust = totalStageThrust;
			stage.thrustToWeight = totalStageThrust / (stageStartMass * gravity);
			stage.maxThrustToWeight = stage.thrustToWeight;
			stage.actualThrust = totalStageActualThrust;
			stage.actualThrustToWeight = totalStageActualThrust / (stageStartMass * gravity);
			CalculateRCS(gravity, final: false);
			stage.RCSIsp = RCSIsp;
			stage.RCSThrust = RCSThrust;
			stage.RCSdeltaVStart = RCSDeltaV;
			stage.RCSTWRStart = RCSTWR;
			stage.RCSBurnTime = RCSBurnTime;
			if (log != null)
			{
				log.AppendLine("stage.thrust = ", stage.thrust);
				log.AppendLine("StageMass = ", stageStartMass);
				log.AppendLine("Initial maxTWR = ", stage.maxThrustToWeight);
			}
			Vector3d val = totalStageThrustForce.TorqueAt(stageStartCom);
			stage.maxThrustTorque = ((Vector3d)(ref val)).magnitude;
			double num2 = ((stage.thrust <= 0.0) ? 0.0 : (stage.maxThrustTorque / stage.thrust));
			val = stageStartCom - totalStageThrustForce.GetAverageForceApplicationPoint();
			double magnitude = ((Vector3d)(ref val)).magnitude;
			double num3 = 0.0;
			if (magnitude > 1E-07)
			{
				num3 = num2 / magnitude;
				if (num3 > 1.0)
				{
					num3 = 1.0;
				}
			}
			stage.thrustOffsetAngle = Math.Asin(num3) * 180.0 / Math.PI;
			stage.totalCost = 0.0;
			for (int j = 0; j < allParts.Count; j++)
			{
				if (currentStage > allParts[j].decoupledInStage)
				{
					stage.totalCost += allParts[j].GetCost(currentStage);
				}
			}
			stage.totalMass = stageStartMass;
			if (currentStage < num)
			{
				Stage obj = array[currentStage + 1];
				obj.cost = obj.totalCost - stage.totalCost;
				obj.mass = obj.totalMass - stage.totalMass;
			}
			if (currentStage == 0)
			{
				stage.cost = stage.totalCost;
				stage.mass = stage.totalMass;
			}
			dontStageParts = dontStagePartsLists[currentStage];
			if (log != null)
			{
				log.AppendLine("Stage setup took ", _timer.ElapsedMilliseconds, "ms");
				if (dontStageParts.Count > 0)
				{
					log.AppendLine("Parts preventing staging:");
					for (int k = 0; k < dontStageParts.Count; k++)
					{
						dontStageParts[k].DumpPartToLog(log, "");
					}
				}
				else
				{
					log.AppendLine("No parts preventing staging");
				}
				log.Flush();
			}
			int num4 = 0;
			while (!AllowedToStage())
			{
				num4++;
				double num5 = double.MaxValue;
				PartSim partSim = null;
				foreach (PartSim drainingPart in drainingParts)
				{
					double num6 = drainingPart.TimeToDrainResource(log);
					if (num6 < num5)
					{
						num5 = num6;
						partSim = drainingPart;
					}
				}
				if (log != null)
				{
					log.Append("Drain time = ", num5, " (", partSim.name).AppendLine(":", partSim.partId, ")");
				}
				foreach (PartSim drainingPart2 in drainingParts)
				{
					drainingPart2.DrainResources(num5, log);
				}
				stepEndMass = ShipMass;
				stageTime += num5;
				double num7 = totalStageThrust / (stepEndMass * gravity);
				if (num7 > stage.maxThrustToWeight)
				{
					stage.maxThrustToWeight = num7;
				}
				if (num5 > 0.0 && stepStartMass > stepEndMass && stepStartMass > 0.0 && stepEndMass > 0.0)
				{
					vecStageDeltaV += vecThrust * (float)(currentisp * 9.80665 * Math.Log(stepStartMass / stepEndMass) / simpleTotalThrust);
				}
				UpdateResourceDrains();
				CalculateThrustAndISP();
				if (stepStartMass == stepEndMass)
				{
					break;
				}
				if (num4 == 1000)
				{
					if (log != null)
					{
						log.AppendLine("exceeded loop count");
						log.AppendLine("stageStartMass = " + stageStartMass);
						log.AppendLine("stepStartMass = " + stepStartMass);
						log.AppendLine("StepEndMass   = " + stepEndMass);
					}
					break;
				}
				stepStartMass = stepEndMass;
			}
			stage.deltaV = ((Vector3)(ref vecStageDeltaV)).magnitude;
			stage.resourceMass = stageStartMass - stepEndMass;
			if (HighLogic.LoadedSceneIsEditor)
			{
				CalculateRCS(gravity, final: true);
			}
			stage.RCSdeltaVEnd = RCSDeltaV;
			stage.RCSTWREnd = RCSTWR;
			if (stageStartMass != stepStartMass)
			{
				stage.isp = stage.deltaV / (9.80665 * Math.Log(stageStartMass / stepStartMass));
			}
			else
			{
				stage.isp = 0.0;
			}
			stage.time = ((stageTime < 86400.0) ? stageTime : 0.0);
			stage.number = (doingCurrent ? (-1) : currentStage);
			stage.totalPartCount = allParts.Count;
			stage.maxMach = maxMach;
			array[currentStage] = stage;
			currentStage--;
			doingCurrent = false;
			if (log != null)
			{
				_timer.Stop();
				log.AppendLine("Simulating stage took ", _timer.ElapsedMilliseconds, "ms");
				stage.Dump(log);
				_timer.Reset();
				_timer.Start();
			}
			ActivateStage();
			if (log != null)
			{
				_timer.Stop();
				log.AppendLine("ActivateStage took ", _timer.ElapsedMilliseconds, "ms");
			}
		}
		for (int l = 0; l < array.Length; l++)
		{
			for (int num8 = l; num8 >= 0; num8--)
			{
				array[l].totalDeltaV += array[num8].deltaV;
				array[l].totalTime += array[num8].time;
				array[l].partCount = ((l > 0) ? (array[l].totalPartCount - array[l - 1].totalPartCount) : array[l].totalPartCount);
			}
			for (int m = l; m < array.Length; m++)
			{
				array[l].inverseTotalDeltaV += array[m].deltaV;
			}
			if (array[l].totalTime > 86400.0)
			{
				array[l].totalTime = 0.0;
			}
		}
		FreePooledObject();
		_timer.Stop();
		if (log != null)
		{
			log.AppendLine("RunSimulation: ", _timer.ElapsedMilliseconds, "ms");
			log.Flush();
		}
		log = null;
		return array;
	}

	public double UpdatePartMasses()
	{
		for (int i = 0; i < allParts.Count; i++)
		{
			allParts[i].baseMass = allParts[i].realMass;
			allParts[i].baseMassForCoM = allParts[i].realMass;
		}
		for (int j = 0; j < allParts.Count; j++)
		{
			PartSim partSim = allParts[j];
			if (!partSim.isNoPhysics || partSim.parent == null)
			{
				continue;
			}
			for (PartSim parent = partSim.parent; parent != null; parent = parent.parent)
			{
				if (!parent.isNoPhysics)
				{
					parent.baseMassForCoM += partSim.baseMassForCoM;
					partSim.baseMassForCoM = 0.0;
					break;
				}
			}
		}
		double num = 0.0;
		for (int k = 0; k < allParts.Count; k++)
		{
			num += (allParts[k].startMass = allParts[k].GetMass(currentStage));
		}
		return num;
	}

	public void FreePooledObject()
	{
		foreach (PartSim allPart in allParts)
		{
			allPart.Release();
		}
		foreach (EngineSim allEngine in allEngines)
		{
			allEngine.Release();
		}
		foreach (RCSSim allRC in allRCS)
		{
			allRC.Release();
		}
	}

	private void BuildDontStageLists(LogMsg log)
	{
		log?.AppendLine("Creating list with capacity of ", currentStage + 1);
		dontStagePartsLists.Clear();
		for (int i = 0; i <= currentStage; i++)
		{
			if (i < dontStagePartsLists.Count)
			{
				dontStagePartsLists[i].Clear();
			}
			else
			{
				dontStagePartsLists.Add(new List<PartSim>());
			}
		}
		for (int j = 0; j < allParts.Count; j++)
		{
			PartSim partSim = allParts[j];
			if (partSim.isEngine || !partSim.resources.Empty)
			{
				log?.AppendLine(partSim.name, ":", partSim.partId, " is engine or tank, decoupled = ", partSim.decoupledInStage);
				if (partSim.decoupledInStage < -1 || partSim.decoupledInStage > currentStage - 1)
				{
					log?.AppendLine("decoupledInStage out of range");
				}
				else
				{
					dontStagePartsLists[partSim.decoupledInStage + 1].Add(partSim);
				}
			}
		}
		for (int k = 1; k <= lastStage; k++)
		{
			if (dontStagePartsLists[k].Count == 0)
			{
				dontStagePartsLists[k] = dontStagePartsLists[k - 1];
			}
		}
	}

	private void UpdateActiveEngines()
	{
		activeEngines.Clear();
		for (int i = 0; i < allEngines.Count; i++)
		{
			EngineSim engineSim = allEngines[i];
			if (engineSim.isActive && !engineSim.isFlamedOut)
			{
				activeEngines.Add(engineSim);
			}
		}
	}

	private void CalculateThrustAndISP()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		vecThrust = Vector3.zero;
		vecActualThrust = Vector3.zero;
		simpleTotalThrust = 0.0;
		totalStageThrust = 0.0;
		totalStageActualThrust = 0.0;
		totalStageFlowRate = 0.0;
		totalStageIspFlowRate = 0.0;
		totalStageThrustForce.Reset();
		for (int i = 0; i < activeEngines.Count; i++)
		{
			EngineSim engineSim = activeEngines[i];
			simpleTotalThrust += engineSim.thrust;
			vecThrust += (float)engineSim.thrust * engineSim.thrustVec;
			vecActualThrust += (float)engineSim.actualThrust * engineSim.thrustVec;
			totalStageFlowRate += engineSim.ResourceConsumptions.Mass;
			totalStageIspFlowRate += engineSim.ResourceConsumptions.Mass * engineSim.isp;
			for (int j = 0; j < engineSim.appliedForces.Count; j++)
			{
				totalStageThrustForce.AddForce(engineSim.appliedForces[j]);
			}
		}
		if (log != null)
		{
			log.AppendLine("vecThrust = ", ((object)(Vector3)(ref vecThrust)).ToString(), "   magnitude = ", ((Vector3)(ref vecThrust)).magnitude);
		}
		totalStageThrust = ((Vector3)(ref vecThrust)).magnitude;
		totalStageActualThrust = ((Vector3)(ref vecActualThrust)).magnitude;
		if (totalStageFlowRate > 0.0 && totalStageIspFlowRate > 0.0)
		{
			currentisp = totalStageIspFlowRate / totalStageFlowRate;
		}
		else
		{
			currentisp = 0.0;
		}
	}

	private void CalculateRCS(double localGravity, bool final)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		//IL_0266: Unknown result type (might be due to invalid IL or missing references)
		//IL_026b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = Vector3.zero;
		_ = Vector3.zero;
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		Dictionary<int, double> dictionary = new Dictionary<int, double>();
		Dictionary<int, double> dictionary2 = new Dictionary<int, double>();
		Dictionary<int, double> dictionary3 = new Dictionary<int, double>();
		Dictionary<int, double> dictionary4 = new Dictionary<int, double>();
		HashSet<int> hashSet = new HashSet<int>();
		foreach (RCSSim allRC in allRCS)
		{
			allRC.DumpEngineToLog(log);
			foreach (int type in allRC.resourceConsumptions.Types)
			{
				if (!hashSet.Contains(type))
				{
					hashSet.Add(type);
					dictionary.Add(type, 0.0);
					dictionary2.Add(type, 0.0);
					dictionary4.Add(type, 0.0);
					dictionary3.Add(type, 0.0);
				}
			}
		}
		foreach (int item in hashSet)
		{
			foreach (PartSim allPart in allParts)
			{
				if (allPart.resources.HasType(item) && allPart.resourceFlowStates[item] > 0.0)
				{
					dictionary3[item] += allPart.resources[item];
				}
			}
		}
		foreach (RCSSim allRC2 in allRCS)
		{
			bool flag = allRC2.isActive;
			foreach (int type2 in allRC2.resourceConsumptions.Types)
			{
				if (dictionary3[type2] < 0.0001)
				{
					flag = false;
				}
			}
			if (flag)
			{
				num += allRC2.thrust;
				val += (float)allRC2.thrust * allRC2.thrustVec;
				num2 += allRC2.resourceConsumptions.Mass;
				num3 += allRC2.resourceConsumptions.Mass * allRC2.isp;
			}
		}
		double shipMass = ShipMass;
		double num4 = shipMass;
		double num5 = 0.0;
		RCSBurnTime = 0.0;
		int num6 = 0;
		if (log != null)
		{
			log.AppendLine("**RCS PRE ");
			log.AppendLine("   StartingMss = ", num4);
			foreach (int item2 in hashSet)
			{
				log.AppendLine("   **Fuel " + item2);
				log.AppendLine("      FuelMass = ", dictionary3[item2]);
			}
		}
		while (true)
		{
			foreach (int item3 in hashSet)
			{
				dictionary4[item3] = 0.0;
				dictionary[item3] = 0.0;
				dictionary2[item3] = 0.0;
			}
			double num7 = 0.0;
			double num8 = 0.0;
			foreach (RCSSim allRC3 in allRCS)
			{
				bool flag2 = allRC3.isActive;
				foreach (int type3 in allRC3.resourceConsumptions.Types)
				{
					if (dictionary3[type3] < 0.0001)
					{
						flag2 = false;
					}
				}
				if (!flag2)
				{
					continue;
				}
				foreach (int type4 in allRC3.resourceConsumptions.Types)
				{
					dictionary4[type4] += allRC3.thrust;
					dictionary[type4] += allRC3.resourceConsumptions[type4];
					dictionary2[type4] += allRC3.resourceConsumptions[type4] * allRC3.isp;
					num7 += allRC3.resourceConsumptions[type4];
					num8 += allRC3.resourceConsumptions[type4] * allRC3.isp;
				}
			}
			double num9 = num4;
			double num10 = double.MaxValue;
			foreach (int item4 in hashSet)
			{
				if (dictionary3[item4] > 0.0001 && dictionary[item4] > 0.0)
				{
					double num11 = dictionary3[item4] / dictionary[item4];
					if (num11 < num10)
					{
						num10 = num11;
					}
				}
			}
			if (num10 > 0.0 && num10 != double.MaxValue)
			{
				foreach (int item5 in hashSet)
				{
					if (dictionary3[item5] > 0.0001)
					{
						double num12 = num10 * dictionary[item5];
						num4 -= num12 * (double)PartResourceLibrary.Instance.GetDefinition(item5).density;
						dictionary3[item5] -= num12;
					}
				}
			}
			if (log != null)
			{
				log.AppendLine("**RCS STEP " + num6);
				log.AppendLine("   burnTime = ", num10);
				log.AppendLine("   StartMass = ", num9);
				log.AppendLine("   StepEndMass = ", num4);
				foreach (int item6 in hashSet)
				{
					log.AppendLine("   **Fuel " + item6);
					log.AppendLine("      FuelMass = ", dictionary3[item6]);
					log.AppendLine("      FlowRateByFuel = ", dictionary[item6]);
					log.AppendLine("      IspFlowRateByFuel = ", dictionary2[item6]);
					log.AppendLine("      thrustByFuel = ", dictionary4[item6]);
					log.AppendLine("      fuelDensity = ", PartResourceLibrary.Instance.GetDefinition(item6).density);
				}
			}
			if (num9 == num4)
			{
				break;
			}
			double num13 = ((num8 > 0.0 && num7 > 0.0) ? (num8 / num7) : 0.0);
			num5 += (double)(float)(num13 * 9.80665 * Math.Log(num9 / num4));
			RCSBurnTime += num10;
			num6++;
			if (num6 == 1000)
			{
				Debug.Log((object)"RCS exceeded loop count");
				Debug.Log((object)("stageStartMass = " + shipMass));
				Debug.Log((object)("burnTime = " + RCSBurnTime));
				Debug.Log((object)("StepEndMass   = " + num4));
				break;
			}
		}
		RCSThrust = num;
		RCSDeltaV = num5;
		if (final)
		{
			RCSTWR = RCSThrust / num4 / localGravity;
		}
		else
		{
			RCSTWR = RCSThrust / ShipMass / localGravity;
		}
		if (num2 > 0.0 && num3 > 0.0)
		{
			RCSIsp = num3 / num2;
		}
		else
		{
			RCSIsp = 0.0;
		}
	}

	private void UpdateResourceDrains()
	{
		UpdateActiveEngines();
		drainingResources.Clear();
		foreach (PartSim drainingPart in drainingParts)
		{
			drainingPart.resourceDrains.Reset();
		}
		drainingParts.Clear();
		for (int i = 0; i < activeEngines.Count; i++)
		{
			EngineSim engineSim = activeEngines[i];
			if (engineSim.SetResourceDrains(log, allParts, allFuelLines, drainingParts))
			{
				for (int j = 0; j < engineSim.ResourceConsumptions.Types.Count; j++)
				{
					drainingResources.Add(engineSim.ResourceConsumptions.Types[j]);
				}
			}
		}
		UpdateActiveEngines();
		if (log != null)
		{
			log.AppendLine("Active engines = ", activeEngines.Count);
			int num = 0;
			for (int k = 0; k < activeEngines.Count; k++)
			{
				EngineSim engineSim2 = activeEngines[k];
				log.Append("Engine " + num++ + ":");
				engineSim2.DumpEngineToLog(log);
			}
			log.Flush();
		}
	}

	private bool AllowedToStage()
	{
		if (log != null)
		{
			log.AppendLine("AllowedToStage").AppendLine("currentStage = ", currentStage);
		}
		if (activeEngines.Count > 0)
		{
			for (int i = 0; i < dontStageParts.Count; i++)
			{
				PartSim partSim = dontStageParts[i];
				if (log != null)
				{
					partSim.DumpPartToLog(log, "Testing: ");
				}
				if (!partSim.isSepratron && !partSim.EmptyOf(drainingResources))
				{
					if (log != null)
					{
						partSim.DumpPartToLog(log, "Decoupled part not empty => false: ");
					}
					return false;
				}
				if (!partSim.isEngine)
				{
					continue;
				}
				for (int j = 0; j < activeEngines.Count; j++)
				{
					EngineSim engineSim = activeEngines[j];
					if (engineSim.dontDecoupleActive && engineSim.partSim == partSim)
					{
						if (log != null)
						{
							partSim.DumpPartToLog(log, "Decoupled part is active engine => false: ");
						}
						return false;
					}
				}
			}
		}
		if (currentStage == 0 && doingCurrent)
		{
			if (log != null)
			{
				log.AppendLine("Current stage == 0 && doingCurrent => false");
			}
			return false;
		}
		if (log != null)
		{
			log.AppendLine("Returning true");
		}
		return true;
	}

	private void ActivateStage()
	{
		decoupledParts.Clear();
		for (int i = 0; i < allParts.Count; i++)
		{
			PartSim partSim = allParts[i];
			if (partSim.decoupledInStage >= currentStage)
			{
				decoupledParts.Add(partSim);
			}
		}
		foreach (PartSim decoupledPart in decoupledParts)
		{
			allParts.Remove(decoupledPart);
			decoupledPart.Release();
			if (decoupledPart.isEngine)
			{
				for (int num = allEngines.Count - 1; num >= 0; num--)
				{
					EngineSim engineSim = allEngines[num];
					if (engineSim.partSim == decoupledPart)
					{
						allEngines.RemoveAt(num);
						engineSim.Release();
					}
				}
			}
			if (decoupledPart.isRCS)
			{
				for (int num2 = allRCS.Count - 1; num2 >= 0; num2--)
				{
					RCSSim rCSSim = allRCS[num2];
					if (rCSSim.partSim == decoupledPart)
					{
						allRCS.RemoveAt(num2);
						rCSSim.Release();
					}
				}
			}
			if (decoupledPart.isFuelLine)
			{
				allFuelLines.Remove(decoupledPart);
			}
		}
		for (int j = 0; j < allParts.Count; j++)
		{
			allParts[j].RemoveAttachedParts(decoupledParts);
		}
		for (int k = 0; k < allEngines.Count; k++)
		{
			EngineSim engineSim2 = allEngines[k];
			if (engineSim2.partSim.inverseStage == currentStage)
			{
				engineSim2.isActive = true;
			}
		}
	}

	public void Dump()
	{
		//IL_0070: Unknown result type (might be due to invalid IL or missing references)
		if (log == null)
		{
			return;
		}
		log.AppendLine("Part count = ", allParts.Count);
		if (allParts.Count > 0)
		{
			PartSim partSim = allParts[0];
			while (partSim.parent != null)
			{
				partSim = partSim.parent;
			}
			if (partSim.hasVessel)
			{
				log.Append("vesselName = '", vesselName, "'  vesselType = ", SimManager.GetVesselTypeString(vesselType));
			}
			partSim.DumpPartToLog(log, "", allParts);
		}
		log.Flush();
	}
}
