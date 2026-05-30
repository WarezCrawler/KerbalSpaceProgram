using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ns10;
using ns9;

[Serializable]
public class DeltaVStageInfo
{
	public int stage;

	public int separationIndex;

	[NonSerialized]
	public VesselDeltaV vesselDeltaV;

	public bool payloadStage;

	public List<DeltaVPartInfo> parts;

	public List<ModuleResourceIntake> airIntakeParts;

	public List<DeltaVEngineInfo> enginesActiveInStage;

	public List<DeltaVEngineInfo> enginesInStage;

	public float stageMass;

	public float dryMass;

	public float fuelMass;

	public float startMass;

	public float endMass;

	public float decoupledMass;

	public double ispVac;

	public double ispASL;

	public double ispActual;

	public float TWRVac;

	public float TWRASL;

	public float TWRActual;

	public float thrustVac;

	public float thrustASL;

	public float thrustActual;

	public float vectoredThrustVac;

	public float vectoredThrustASL;

	public float vectoredThrustActual;

	public float deltaVinVac;

	public float deltaVatASL;

	public float deltaVActual;

	public double stageBurnTime;

	public List<DeltaVCalc> deltaVCalcs;

	public float totalExhaustVelocityVAC;

	public float totalExhaustVelocityASL;

	public float totalExhaustVelocityActual;

	public Vector3 vectoredExhaustVelocityVAC;

	public Vector3 vectoredExhaustVelocityASL;

	public Vector3 vectoredExhaustVelocityActual;

	private bool partsDisplayListDirty;

	private StringBuilder partsDisplayList;

	private Vector3 enginesThrustVac = Vector3.zero;

	private Vector3 enginesThrustASL = Vector3.zero;

	private Vector3 enginesThrustActual = Vector3.zero;

	private HashSet<Part> cachedResourcePartSetParts;

	private List<DeltaVPartInfo> cachedActivatedParts;

	private float removedFuelMass;

	[SerializeField]
	private bool stageContainsOnlyLaunchClamps;

	public DeltaVStageInfo(ShipConstruct ship, int inStage, VesselDeltaV vesselDeltaV)
	{
		parts = new List<DeltaVPartInfo>();
		enginesActiveInStage = new List<DeltaVEngineInfo>();
		enginesInStage = new List<DeltaVEngineInfo>();
		deltaVCalcs = new List<DeltaVCalc>();
		airIntakeParts = new List<ModuleResourceIntake>();
		this.vesselDeltaV = vesselDeltaV;
		stage = inStage;
		if (ship != null)
		{
			ProcessParts(inStage);
		}
	}

	public DeltaVStageInfo(Vessel vessel, int inStage, VesselDeltaV vesselDeltaV)
	{
		parts = new List<DeltaVPartInfo>();
		enginesActiveInStage = new List<DeltaVEngineInfo>();
		enginesInStage = new List<DeltaVEngineInfo>();
		deltaVCalcs = new List<DeltaVCalc>();
		airIntakeParts = new List<ModuleResourceIntake>();
		this.vesselDeltaV = vesselDeltaV;
		stage = inStage;
		if (vessel != null)
		{
			ProcessParts(inStage);
		}
	}

	public double GetSituationISP(DeltaVSituationOptions situation)
	{
		return situation.GetSwitchedValue(ispASL, ispActual, ispVac);
	}

	public float GetSituationTWR(DeltaVSituationOptions situation)
	{
		return situation.GetSwitchedValue(TWRASL, TWRActual, TWRVac);
	}

	public float GetSituationThrust(DeltaVSituationOptions situation)
	{
		return situation.GetSwitchedValue(thrustASL, thrustActual, thrustVac);
	}

	public float GetSituationVectoredThrust(DeltaVSituationOptions situation)
	{
		return situation.GetSwitchedValue(vectoredThrustASL, vectoredThrustActual, vectoredThrustVac);
	}

	public float GetSituationDeltaV(DeltaVSituationOptions situation)
	{
		return situation.GetSwitchedValue(deltaVatASL, deltaVActual, deltaVinVac);
	}

	public float GetSituationTotalExhaustVelocity(DeltaVSituationOptions situation)
	{
		return situation.GetSwitchedValue(totalExhaustVelocityASL, totalExhaustVelocityActual, totalExhaustVelocityVAC);
	}

	public Vector3 GetSituationVectoredExhaustVelocity(DeltaVSituationOptions situation)
	{
		return situation.GetSwitchedValue(vectoredExhaustVelocityASL, vectoredExhaustVelocityActual, vectoredExhaustVelocityVAC);
	}

	public void Reset(int inStage, VesselDeltaV vesselDeltaV)
	{
		this.vesselDeltaV = vesselDeltaV;
		if (airIntakeParts == null)
		{
			airIntakeParts = new List<ModuleResourceIntake>();
		}
		if (parts == null)
		{
			parts = new List<DeltaVPartInfo>();
		}
		if (enginesActiveInStage == null)
		{
			enginesActiveInStage = new List<DeltaVEngineInfo>();
		}
		if (enginesInStage == null)
		{
			enginesInStage = new List<DeltaVEngineInfo>();
		}
		if (deltaVCalcs == null)
		{
			deltaVCalcs = new List<DeltaVCalc>();
		}
		stage = inStage;
		payloadStage = false;
		deltaVinVac = 0f;
		deltaVatASL = 0f;
		deltaVActual = 0f;
		stageMass = 0f;
		endMass = 0f;
		dryMass = 0f;
		fuelMass = 0f;
		decoupledMass = 0f;
		TWRActual = 0f;
		TWRASL = 0f;
		TWRVac = 0f;
		ispActual = 0.0;
		ispASL = 0.0;
		ispVac = 0.0;
		thrustActual = 0f;
		thrustASL = 0f;
		thrustVac = 0f;
		stageBurnTime = 0.0;
		vectoredThrustVac = 0f;
		vectoredThrustASL = 0f;
		vectoredThrustActual = 0f;
		totalExhaustVelocityVAC = 0f;
		totalExhaustVelocityASL = 0f;
		totalExhaustVelocityActual = 0f;
		ProcessParts(inStage);
	}

	private void ProcessParts(int inStage)
	{
		partsDisplayListDirty = true;
		stageContainsOnlyLaunchClamps = false;
		airIntakeParts.Clear();
		bool flag = false;
		int count = parts.Count;
		while (count-- > 0)
		{
			bool flag2 = false;
			if (vesselDeltaV.ActiveMode == VesselDeltaV.Mode.Ship && vesselDeltaV.Ship != null)
			{
				for (int i = 0; i < vesselDeltaV.Ship.parts.Count; i++)
				{
					if (vesselDeltaV.Ship.parts[i].persistentId == parts[count].part.persistentId)
					{
						flag2 = true;
						break;
					}
				}
			}
			else if (vesselDeltaV.ActiveMode == VesselDeltaV.Mode.Vessel)
			{
				for (int j = 0; j < vesselDeltaV.Vessel.parts.Count; j++)
				{
					if (vesselDeltaV.Vessel.parts[j].persistentId == parts[count].part.persistentId)
					{
						flag2 = true;
						break;
					}
				}
			}
			if (!flag2)
			{
				parts.RemoveAt(count);
				flag = true;
			}
		}
		for (int k = 0; k < vesselDeltaV.PartInfo.Count; k++)
		{
			DeltaVPartInfo deltaVPartInfo = vesselDeltaV.PartInfo[k];
			if (deltaVPartInfo.decoupleStage <= inStage)
			{
				if (!parts.ContainsPart(deltaVPartInfo.part))
				{
					parts.Add(deltaVPartInfo);
					flag = true;
				}
				if (deltaVPartInfo.isIntake && deltaVPartInfo.moduleResourceIntake != null)
				{
					airIntakeParts.Add(deltaVPartInfo.moduleResourceIntake);
				}
				separationIndex = Math.Max(separationIndex, deltaVPartInfo.decoupleStage);
			}
			else if (parts.ContainsPart(vesselDeltaV.PartInfo[k].part))
			{
				parts.Remove(vesselDeltaV.PartInfo[k]);
				flag = true;
			}
		}
		ProcessActiveEngines();
		if (parts.Count == 0)
		{
			separationIndex = vesselDeltaV.GetHighestSeparationStage(inStage);
		}
		if (flag)
		{
			ResetPartCaches();
		}
		PartsActivateInStage(out cachedActivatedParts);
		bool flag3 = true;
		for (int l = 0; l < cachedActivatedParts.Count; l++)
		{
			if (!cachedActivatedParts[l].part.isLaunchClamp())
			{
				flag3 = false;
				break;
			}
		}
		stageContainsOnlyLaunchClamps = flag3;
	}

	private void CalculateStartMass()
	{
		float decoupledDryMass = 0f;
		float decoupledFuelMass = 0f;
		float jettisonedDryMass = 0f;
		int decoupledPartCount = 0;
		stageMass = GetStageMass(out dryMass, out fuelMass, out decoupledDryMass, out decoupledFuelMass, out jettisonedDryMass, out decoupledPartCount);
		decoupledMass = decoupledDryMass + decoupledFuelMass + jettisonedDryMass;
		dryMass = dryMass - decoupledDryMass - jettisonedDryMass;
		startMass = stageMass - decoupledMass;
		StoreStartFuelMass();
	}

	private void StoreInterimDeltaV(List<DeltaVEngineInfo> engines, List<DeltaVCalc> deltaVCalcsList, float startingMass, float currentMass, double simulationTime, bool logMsgs)
	{
		enginesThrustVac = Vector3.zero;
		enginesThrustASL = Vector3.zero;
		enginesThrustActual = Vector3.zero;
		for (int i = 0; i < engines.Count; i++)
		{
			engines[i].CalculateThrustVector(stage == StageManager.CurrentStage);
			enginesThrustVac += engines[i].thrustVectorVac;
			enginesThrustASL += engines[i].thrustVectorASL;
			enginesThrustActual += engines[i].thrustVectorActual;
		}
		Vector3 vector = enginesThrustVac * (float)(Math.Log(startingMass / currentMass) * ispVac * PhysicsGlobals.GravitationalAcceleration / (double)thrustVac);
		Vector3 vector2 = enginesThrustASL * (float)(Math.Log(startingMass / currentMass) * ispASL * PhysicsGlobals.GravitationalAcceleration / (double)thrustASL);
		Vector3 vector3 = enginesThrustActual * (float)(Math.Log(startingMass / currentMass) * ispActual * PhysicsGlobals.GravitationalAcceleration / (double)thrustActual);
		CalculateTWR(startingMass);
		deltaVCalcsList.Add(new DeltaVCalc(vector.magnitude, vector2.magnitude, vector3.magnitude, simulationTime, engines, ispVac, ispASL, ispActual, TWRVac, TWRASL, TWRActual, startingMass, currentMass, thrustVac, thrustASL, thrustActual));
		if (logMsgs)
		{
			Debug.LogFormat("[StageInfo]: Interim DeltaV Calc. Stage:{0} ThrustVector:{1} Simulation Time:{2:N3} Start Mass:{3} End Mass:{4} ISP:{5} Thrust:{6} ASL dV:{7} VAC dV:{8} Actual dV:{9}", stage, enginesThrustVac, simulationTime, startingMass, currentMass, ispVac, thrustVac, vector2.magnitude, vector.magnitude, vector3.magnitude);
		}
	}

	private int GetHighestSeparationIndex(List<DeltaVEngineInfo> engines)
	{
		int num = int.MinValue;
		for (int i = 0; i < engines.Count; i++)
		{
			num = Math.Max(num, engines[i].partInfo.decoupleStage);
		}
		return num;
	}

	private List<DeltaVEngineInfo> MatchingSeparationIndex(List<DeltaVEngineInfo> engines, int index)
	{
		List<DeltaVEngineInfo> list = new List<DeltaVEngineInfo>();
		for (int i = 0; i < engines.Count; i++)
		{
			if (engines[i].partInfo.decoupleStage == index)
			{
				list.Add(engines[i]);
			}
		}
		if (list.Count == 0)
		{
			return engines;
		}
		return list;
	}

	private List<DeltaVEngineResourcePartInfo> GetEngineResourceParts(List<DeltaVEngineInfo> engines)
	{
		List<DeltaVEngineResourcePartInfo> list = new List<DeltaVEngineResourcePartInfo>();
		for (int i = 0; i < engines.Count; i++)
		{
			HashSet<Part>.Enumerator enumerator = engines[i].partInfo.part.simulationCrossfeedPartSet.GetParts().GetEnumerator();
			while (enumerator.MoveNext())
			{
				DeltaVPartInfo deltaVPartInfo = parts.Get(enumerator.Current);
				if (deltaVPartInfo == null)
				{
					continue;
				}
				if (deltaVPartInfo.decoupleStage == stage)
				{
					for (int j = 0; j < deltaVPartInfo.part.SimulationResources.Count; j++)
					{
						deltaVPartInfo.part.SimulationResources[j].amount = 0.0;
					}
					continue;
				}
				for (int k = 0; k < engines[i].propellantInfo.Count; k++)
				{
					if (deltaVPartInfo.part.Resources.Contains(engines[i].propellantInfo[k].propellant.id))
					{
						DeltaVEngineResourcePartInfo deltaVEngineResourcePartInfo = list.Get(deltaVPartInfo);
						if (deltaVEngineResourcePartInfo == null)
						{
							deltaVEngineResourcePartInfo = new DeltaVEngineResourcePartInfo(deltaVPartInfo);
							list.Add(deltaVEngineResourcePartInfo);
						}
						deltaVEngineResourcePartInfo.resourceIdsUsed.AddUnique(engines[i].propellantInfo[k].propellant.id);
					}
				}
			}
			enumerator.Dispose();
		}
		return list;
	}

	private void CalculateEngineResourceFuelMass(List<DeltaVEngineResourcePartInfo> resourceParts)
	{
		for (int i = 0; i < resourceParts.Count; i++)
		{
			DeltaVEngineResourcePartInfo deltaVEngineResourcePartInfo = resourceParts[i];
			for (int j = 0; j < deltaVEngineResourcePartInfo.resourceIdsUsed.Count; j++)
			{
				PartResource partResource = deltaVEngineResourcePartInfo.resourcePart.part.SimulationResources.Get(deltaVEngineResourcePartInfo.resourceIdsUsed[j]);
				if (partResource != null)
				{
					deltaVEngineResourcePartInfo.fuelMassUsed += partResource.info.density * (float)partResource.amount;
				}
			}
		}
	}

	private bool EnginesDeprived(List<DeltaVEngineInfo> enginesToStageNext, bool allEngines)
	{
		int num = 0;
		if (enginesToStageNext.Count == 0)
		{
			return true;
		}
		int num2 = 0;
		while (true)
		{
			if (num2 < enginesToStageNext.Count)
			{
				if (enginesToStageNext[num2].deprived)
				{
					num++;
					if (!allEngines)
					{
						break;
					}
				}
				num2++;
				continue;
			}
			if (allEngines && num == enginesToStageNext.Count)
			{
				return true;
			}
			return false;
		}
		return true;
	}

	private void StoreStartFuelMass()
	{
		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].StageStartFuelMass(stage);
		}
	}

	private void StoreEndFuelMass()
	{
		for (int i = 0; i < parts.Count; i++)
		{
			parts[i].StageEndFuelMass(stage);
		}
	}

	private float CheckTimeStep(float timeStep, float simulationBurnTime, float fuelMassRemoved, List<DeltaVEngineInfo> enginesStillActive, List<DeltaVEngineResourcePartInfo> resourcePartsToStageNext, bool logMsgs)
	{
		float num = 0f;
		float num2 = 0f;
		if (resourcePartsToStageNext.Count != 0 && enginesStillActive.Count != 1)
		{
			for (int i = 0; i < resourcePartsToStageNext.Count; i++)
			{
				num += resourcePartsToStageNext[i].fuelMassUsed;
			}
			for (int j = 0; j < enginesStillActive.Count; j++)
			{
				bool flag = false;
				for (int k = 0; k < resourcePartsToStageNext.Count; k++)
				{
					if (enginesStillActive[j].partInfo.part.simulationCrossfeedPartSet.ContainsPart(resourcePartsToStageNext[k].resourcePart.part))
					{
						flag = true;
						break;
					}
				}
				if (!(enginesStillActive[j].maxTimeStep <= 0f) && flag)
				{
					float throttle = enginesStillActive[j].engine.thrustPercentage * 0.01f;
					throttle = enginesStillActive[j].engine.ApplyThrottleAdjustments(throttle);
					num2 += (float)((double)enginesStillActive[j].thrustActual / enginesStillActive[j].ispActual / PhysicsGlobals.GravitationalAcceleration * (double)throttle);
				}
			}
			float num3 = (fuelMassRemoved + num) / num2;
			float num4 = num3 - simulationBurnTime;
			if (logMsgs)
			{
				Debug.LogFormat("[StageInfo]: CheckTimeStep Fuel Mass To burn:{0} Engines Fuel Flow:{1} Burn Time:{2} Remaining Time:{3}", num, num2, num3, num4);
			}
			if (num4 >= timeStep)
			{
				return timeStep;
			}
			if (logMsgs)
			{
				Debug.LogFormat("[StageInfo]: Minimum Timestep changed from {0} to {1}", timeStep, num4);
			}
			return num4;
		}
		return timeStep;
	}

	internal void SimulateDeltaV(bool runningActive, bool infiniteFuel, float timeStep = 0.2f, bool logMsgs = true, bool thisStageActive = false)
	{
		if (logMsgs)
		{
			Debug.LogFormat("[StageInfo]: DeltaV Simulation starting stage {0} Running Active:{1}", stage, runningActive);
		}
		CalculateStartMass();
		if (enginesActiveInStage != null && enginesActiveInStage.Count != 0 && (thisStageActive || !stageContainsOnlyLaunchClamps))
		{
			deltaVCalcs.Clear();
			List<Part> list = parts.PartsInStage(stage);
			if (cachedResourcePartSetParts == null || !cachedResourcePartSetParts.SetEquals(list))
			{
				PartSet.BuildPartSimulationSets(list);
				cachedResourcePartSetParts = new HashSet<Part>(list);
			}
			if (HighLogic.LoadedSceneIsEditor && vesselDeltaV != null && vesselDeltaV.Ship != null)
			{
				vesselDeltaV.Ship.UpdateResourceSets(new HashSet<Part>(list));
			}
			int count = enginesActiveInStage.Count;
			while (count-- > 0)
			{
				if (!enginesActiveInStage[count].deprived && !enginesActiveInStage[count].PropellantStarved())
				{
					if (enginesActiveInStage[count].partInfo.decoupleBeforeBurn)
					{
						if (logMsgs)
						{
							Debug.LogFormat("[StageInfo]: Engine {0}  {1} Removed from Stage {2} as it decouples before activating.", enginesActiveInStage[count].engine.part.persistentId, enginesActiveInStage[count].engine.part.partInfo.title, stage);
						}
						enginesActiveInStage.RemoveAt(count);
					}
					else if (HighLogic.LoadedSceneIsFlight && StageManager.CurrentStage == stage && enginesActiveInStage[count].RequiresAir() && enginesActiveInStage[count].engine.part.staticPressureAtm <= 0.0)
					{
						if (logMsgs)
						{
							Debug.LogFormat("[StageInfo]: Engine {0}  {1} Removed from Stage {2} as it is deprived of Air in a Vacuum.", enginesActiveInStage[count].engine.part.persistentId, enginesActiveInStage[count].engine.part.partInfo.title, stage);
						}
						enginesActiveInStage.RemoveAt(count);
					}
				}
				else
				{
					if (logMsgs)
					{
						Debug.LogFormat("[StageInfo]: Engine {0}  {1} Removed from Stage {2} as it is already deprived.", enginesActiveInStage[count].engine.part.persistentId, enginesActiveInStage[count].engine.part.partInfo.title, stage);
					}
					enginesActiveInStage.RemoveAt(count);
				}
			}
			if (payloadStage && enginesActiveInStage.Count > 0)
			{
				payloadStage = false;
				if (logMsgs)
				{
					Debug.LogFormat("[StageInfo]: Reset PayLoad Stage as there are still engines active in the stage that are not deprived. Stage {0}", stage);
				}
			}
			int highestSeparationIndex = GetHighestSeparationIndex(enginesActiveInStage);
			List<DeltaVEngineInfo> list2 = MatchingSeparationIndex(enginesActiveInStage, highestSeparationIndex);
			if (logMsgs)
			{
				Debug.LogFormat("[StageInfo]: Resource Tanks:");
			}
			List<DeltaVEngineResourcePartInfo> engineResourceParts = GetEngineResourceParts(list2);
			CalculateEngineResourceFuelMass(engineResourceParts);
			int highestPartSeparationIndex = engineResourceParts.GetHighestPartSeparationIndex(stage);
			List<DeltaVEngineResourcePartInfo> list3 = engineResourceParts.PartsMatchingSeparationIndex(highestPartSeparationIndex);
			if (logMsgs)
			{
				for (int i = 0; i < engineResourceParts.Count; i++)
				{
					Debug.LogFormat("[StageInfo]: Resource Parts in Stage {0} - {1}", engineResourceParts[i].resourcePart.part.persistentId, engineResourceParts[i].resourcePart.part.partInfo.title);
				}
				for (int j = 0; j < list3.Count; j++)
				{
					Debug.LogFormat("[StageInfo]: Resource Part to Stage Next {0} - {1}", list3[j].resourcePart.part.persistentId, list3[j].resourcePart.part.partInfo.title);
				}
			}
			if (logMsgs)
			{
				Debug.LogFormat("[StageInfo]: Engines Active in Stage:");
			}
			for (int k = 0; k < enginesActiveInStage.Count; k++)
			{
				enginesActiveInStage[k].ResetCalcVariables();
				enginesActiveInStage[k].CalculateFuelTime(stage);
				if (enginesActiveInStage[k].requiresAir)
				{
					timeStep = GameSettings.DELTAV_CALCULATIONS_BIGTIMESTEP;
					if (logMsgs)
					{
						Debug.Log("[StageInfo]: TimeStep changed as Engines requiring Air in stage");
					}
				}
				if (logMsgs)
				{
					Debug.LogFormat("[StageInfo]: Engine {0} - {1} Is In StageNext List? {2}", enginesActiveInStage[k].engine.part.persistentId, enginesActiveInStage[k].engine.part.partInfo.title, list2.Contains(enginesActiveInStage[k]) ? "Yes" : "No");
				}
			}
			List<DeltaVEngineInfo> list4 = new List<DeltaVEngineInfo>(enginesActiveInStage);
			CalculateISP();
			CalculateTWR();
			float startingMass = startMass;
			float num = 0f;
			removedFuelMass = 0f;
			List<DeltaVEngineInfo> list5 = new List<DeltaVEngineInfo>();
			do
			{
				bool flag = false;
				bool flag2 = false;
				float num2 = timeStep;
				for (int l = 0; l < list4.Count; l++)
				{
					if (!list4[l].deprived)
					{
						bool checkDeprived = false;
						float minTimeStep = timeStep;
						list4[l].CalculateBurn(this, list4, timeStep, runningActive, logMsgs, infiniteFuel, reCalc: false, out checkDeprived, out minTimeStep);
						if (flag = flag || checkDeprived)
						{
							num2 = Mathf.Min(num2, minTimeStep);
						}
					}
				}
				if (list4.Count > 1 || list3.Count > 1)
				{
					float num3 = CheckTimeStep(timeStep, num, removedFuelMass, list4, list3, logMsgs);
					if (num3 < num2)
					{
						flag = true;
						num2 = num3;
					}
					if (num2 <= 0f)
					{
						for (int m = 0; m < list2.Count; m++)
						{
							list2[m].deprived = true;
							list5.Add(list2[m]);
						}
						if (logMsgs)
						{
							Debug.Log("[StageInfo]: TimeStep Zero. Engines deprived.");
						}
					}
				}
				if (flag && list4.Count > 1)
				{
					for (int n = 0; n < list4.Count; n++)
					{
						if (!list4[n].deprived)
						{
							bool checkDeprived2 = false;
							float minTimeStep2 = num2;
							list4[n].CalculateBurn(this, list4, num2, runningActive, logMsgs, infiniteFuel, reCalc: true, out checkDeprived2, out minTimeStep2);
							if (logMsgs)
							{
								Debug.LogFormat("[StageInfo]: Recalc Minimum Time Step {0} For Engine {1} - {2}", num2, list4[n].engine.part.partInfo.title, list4[n].engine.part.persistentId);
							}
						}
					}
					if (logMsgs)
					{
						Debug.Log("[StageInfo]: Recalc Minimum Time Step For Engines Completed");
					}
				}
				if (!EnginesDeprived(list2, allEngines: true))
				{
					int count2 = list4.Count;
					while (count2-- > 0)
					{
						if (list4[count2].deprived)
						{
							if (logMsgs)
							{
								Debug.LogFormat("[StageInfo]: Removed Deprived Engine {0} - {1} from list. TimeStep for engine was {2}", list4[count2].engine.part.partInfo.title, list4[count2].engine.part.persistentId, list4[count2].maxTimeStep);
							}
							list5.Add(list4[count2]);
							continue;
						}
						list4[count2].ApplyBurn(this, num2, runningActive, logMsgs, infiniteFuel);
						if (list4[count2].deprived)
						{
							if (logMsgs)
							{
								Debug.LogFormat("[StageInfo]: Removed Deprived Engine {0} - {1} from list. TimeStep for engine was {2}", list4[count2].engine.part.partInfo.title, list4[count2].engine.part.persistentId, list4[count2].maxTimeStep);
							}
							list5.Add(list4[count2]);
						}
					}
				}
				if (list3.Count > 0)
				{
					int count3 = list3.Count;
					while (count3-- > 0)
					{
						bool flag3 = false;
						for (int num4 = 0; num4 < list3[count3].resourcePart.part.SimulationResources.Count; num4++)
						{
							flag3 = ((list3[count3].resourceIdsUsed.Contains(list3[count3].resourcePart.part.SimulationResources[num4].info.id) && list3[count3].resourcePart.part.SimulationResources[num4].amount <= 0.0) ? true : false);
						}
						if (flag3)
						{
							if (logMsgs)
							{
								Debug.LogFormat("[StageInfo]: Removed Empty Tank {0} - {1} from list.", list3[count3].resourcePart.part.partInfo.title, list3[count3].resourcePart.part.persistentId);
							}
							removedFuelMass += list3[count3].fuelMassUsed;
							list3.RemoveAt(count3);
						}
					}
					if (list3.Count == 0)
					{
						flag2 = true;
					}
				}
				if (list5.Count > 0 || num2 < timeStep || flag2)
				{
					CalculateISP(list4);
					float currentStageMass = GetCurrentStageMass();
					StoreInterimDeltaV(list4, deltaVCalcs, startingMass, currentStageMass, num + num2, logMsgs);
					startingMass = currentStageMass;
					for (int num5 = 0; num5 < list5.Count; num5++)
					{
						list4.Remove(list5[num5]);
					}
					list5.Clear();
				}
				if (EnginesDeprived(list2, allEngines: true) || flag2)
				{
					list4.Clear();
					if (logMsgs)
					{
						Debug.LogFormat("[StageInfo]: DeltaV Simulation All engines to stage next are deprived. Stage {0} Last Timestep = {1}", stage, num2);
					}
				}
				num += num2;
				if (num > 1000f && timeStep < GameSettings.DELTAV_CALCULATIONS_BIGTIMESTEP)
				{
					if (logMsgs)
					{
						Debug.Log("[StageInfo]: Simulation Running too slow. Time Step increased");
					}
					timeStep = GameSettings.DELTAV_CALCULATIONS_BIGTIMESTEP;
				}
			}
			while (list4.Count != 0 && num < 100000f);
			if (logMsgs && num >= 100000f)
			{
				Debug.LogWarning("[StageInfo]: Simulation Time exceeded!");
			}
			stageBurnTime = num;
			endMass = GetCurrentStageMass();
			StoreEndFuelMass();
			CalculateISP();
			CalculateTWR();
			deltaVinVac = 0f;
			deltaVatASL = 0f;
			deltaVActual = 0f;
			for (int num6 = 0; num6 < deltaVCalcs.Count; num6++)
			{
				if (!double.IsNaN(deltaVCalcs[num6].dVinVac))
				{
					deltaVinVac += (float)deltaVCalcs[num6].dVinVac;
				}
				if (!double.IsNaN(deltaVCalcs[num6].dVatASL))
				{
					deltaVatASL += (float)deltaVCalcs[num6].dVatASL;
				}
				if (!double.IsNaN(deltaVCalcs[num6].dVActual))
				{
					deltaVActual += (float)deltaVCalcs[num6].dVActual;
				}
			}
			float num7 = 0f;
			totalExhaustVelocityActual = 0f;
			float num8 = num7;
			num7 = 0f;
			totalExhaustVelocityASL = num8;
			totalExhaustVelocityVAC = num7;
			vectoredExhaustVelocityVAC = (vectoredExhaustVelocityASL = (vectoredExhaustVelocityActual = Vector3.zero));
			num7 = 0f;
			vectoredThrustActual = 0f;
			float num9 = num7;
			num7 = 0f;
			vectoredThrustASL = num9;
			vectoredThrustVac = num7;
			enginesThrustVac = (enginesThrustASL = (enginesThrustActual = Vector3.zero));
			for (int num10 = 0; num10 < enginesActiveInStage.Count; num10++)
			{
				if (enginesActiveInStage[num10].thrustVac > 0f)
				{
					totalExhaustVelocityVAC += (enginesActiveInStage[num10].thrustVectorVac * ((float)enginesActiveInStage[num10].ispVac * (float)PhysicsGlobals.GravitationalAcceleration) / enginesActiveInStage[num10].thrustVac).magnitude;
					vectoredExhaustVelocityVAC += enginesActiveInStage[num10].thrustVectorVac * ((float)enginesActiveInStage[num10].ispVac * (float)PhysicsGlobals.GravitationalAcceleration) / enginesActiveInStage[num10].thrustVac;
				}
				if (enginesActiveInStage[num10].thrustASL > 0f)
				{
					vectoredExhaustVelocityASL += enginesActiveInStage[num10].thrustVectorASL * ((float)enginesActiveInStage[num10].ispASL * (float)PhysicsGlobals.GravitationalAcceleration) / enginesActiveInStage[num10].thrustASL;
				}
				if (enginesActiveInStage[num10].thrustActual > 0f)
				{
					vectoredExhaustVelocityActual += enginesActiveInStage[num10].thrustVectorActual * ((float)enginesActiveInStage[num10].ispActual * (float)PhysicsGlobals.GravitationalAcceleration) / enginesActiveInStage[num10].thrustActual;
				}
				enginesThrustVac += enginesActiveInStage[num10].thrustVectorVac;
				enginesThrustASL += enginesActiveInStage[num10].thrustVectorASL;
				enginesThrustActual += enginesActiveInStage[num10].thrustVectorActual;
			}
			vectoredThrustVac = enginesThrustVac.magnitude;
			vectoredThrustASL = enginesThrustASL.magnitude;
			vectoredThrustActual = enginesThrustActual.magnitude;
			if (logMsgs)
			{
				Debug.LogFormat("[StageInfo]: Final DeltaV Calc. Stage:{0} Start Mass:{1} End Mass:{2} ASL ISP:{3} VAC ISP:{4} Actual ISP:{5} ASL dV:{6} VAC dV:{7} Actual dV:{8} BurnTime:{9}", stage, startMass, endMass, ispASL, ispVac, ispActual, deltaVatASL, deltaVinVac, deltaVActual, stageBurnTime);
				Debug.LogFormat("[StageInfo]: DeltaV Simulation ended stage {0}", stage);
			}
			return;
		}
		endMass = startMass;
		StoreEndFuelMass();
		if (logMsgs)
		{
			if (enginesActiveInStage == null || enginesActiveInStage.Count == 0)
			{
				Debug.LogFormat("[StageInfo]: No Active Engines in Stage. DeltaV Simulation skipped for stage {0}", stage);
			}
			if (!thisStageActive && stageContainsOnlyLaunchClamps)
			{
				Debug.LogFormat("[StageInfo]: Stage Not active && Stage Contains Only LaunchClamps for stage {0}", stage);
			}
		}
	}

	internal double CalculateTimeRequiredDV(bool runningActive, float deltaVRequested)
	{
		double num = 0.0;
		float num2 = (runningActive ? vectoredThrustActual : ((!(FlightGlobals.ActiveVessel != null)) ? vectoredThrustVac : ((FlightGlobals.ActiveVessel.atmDensity > 0.0) ? vectoredThrustASL : vectoredThrustVac)));
		float num3 = (runningActive ? ((float)ispActual) : ((!(FlightGlobals.ActiveVessel != null)) ? ((float)ispVac) : ((FlightGlobals.ActiveVessel.atmDensity > 0.0) ? ((float)ispASL) : ((float)ispVac))));
		float num4 = (runningActive ? vectoredExhaustVelocityActual.magnitude : ((!(FlightGlobals.ActiveVessel != null)) ? vectoredExhaustVelocityVAC.magnitude : ((FlightGlobals.ActiveVessel.atmDensity > 0.0) ? vectoredExhaustVelocityASL.magnitude : vectoredExhaustVelocityVAC.magnitude)));
		float num5 = (runningActive ? totalExhaustVelocityActual : ((!(FlightGlobals.ActiveVessel != null)) ? totalExhaustVelocityVAC : ((FlightGlobals.ActiveVessel.atmDensity > 0.0) ? totalExhaustVelocityASL : totalExhaustVelocityVAC)));
		float num6 = 1f;
		if (FlightGlobals.ActiveVessel != null && num5 > 0f)
		{
			num6 = num4 / num5;
			Mathf.Clamp(num6, -1f, 1f);
		}
		float num7 = startMass / Mathf.Exp(deltaVRequested / (float)PhysicsGlobals.GravitationalAcceleration / num3 / num6);
		float num8 = num2 / startMass;
		float num9 = num2 / num7;
		num = deltaVRequested / Mathf.Sqrt(num8 * num9);
		if (GameSettings.LOG_DELTAV_VERBOSE)
		{
			Debug.LogFormat("[DeltaVStageInfo]: Calculated Time for Required DV. Stage {0} DeltaV Requested:{1} Time Required:{2}", stage, deltaVRequested, num);
		}
		return num;
	}

	internal void CalcLerpDeltaV()
	{
		for (int i = 0; i < enginesActiveInStage.Count; i++)
		{
			enginesActiveInStage[i].CalcThrustActual();
			enginesActiveInStage[i].CalculateISP();
		}
		CalculateISP();
		CalculateTWR();
		if (vesselDeltaV.Vessel != null)
		{
			float num = (float)vesselDeltaV.Vessel.rootPart.staticPressureAtm;
			if (num > 0f)
			{
				num /= (float)vesselDeltaV.Vessel.mainBody.atmPressureASL;
			}
			deltaVActual = Mathf.Lerp(deltaVinVac, deltaVatASL, num);
		}
	}

	internal string GetPartDisplayInfo()
	{
		if (!partsDisplayListDirty && partsDisplayList != null)
		{
			return partsDisplayList.ToString();
		}
		if (partsDisplayList == null)
		{
			partsDisplayList = StringBuilderCache.Acquire();
		}
		else
		{
			partsDisplayList.Release();
			partsDisplayList = StringBuilderCache.Acquire();
		}
		if (parts != null && (parts == null || parts.Count != 0))
		{
			for (int i = 0; i < parts.Count; i++)
			{
				if (parts[i].decoupleStage == stage)
				{
					partsDisplayList.Append("<color=red>");
				}
				else
				{
					partsDisplayList.Append("<color=green>");
				}
				float stageStartMass = parts[i].GetStageStartMass(stage);
				partsDisplayList.Append(Localizer.Format("#autoLOC_8002202", parts[i].part.partInfo.title, parts[i].dryMass.ToString("N3"), stageStartMass.ToString("N3")) + "</color>");
				if (parts[i].JettisonInStage(stage))
				{
					partsDisplayList.Append("<color=yellow>  " + Localizer.Format("#autoLOC_8002203", parts[i].jettisonMass.ToString("N3")) + "</color>");
				}
				if (i < parts.Count - 1)
				{
					partsDisplayList.Append("\n");
				}
			}
			partsDisplayListDirty = false;
			return partsDisplayList.ToString();
		}
		partsDisplayList.Append(Localizer.Format("#autoLOC_6003083"));
		partsDisplayListDirty = false;
		return partsDisplayList.ToString();
	}

	internal void ProcessActiveEngines()
	{
		enginesActiveInStage.Clear();
		enginesInStage.Clear();
		List<DeltaVEngineInfo> workingEngineInfo = vesselDeltaV.WorkingEngineInfo;
		for (int i = 0; i < workingEngineInfo.Count; i++)
		{
			if (workingEngineInfo[i].partInfo == null)
			{
				vesselDeltaV.SetCalcsDirty(resetPartCaches: true);
			}
			else if (stage <= workingEngineInfo[i].startBurnStage)
			{
				enginesInStage.Add(workingEngineInfo[i]);
				if (parts.ContainsPart(workingEngineInfo[i].engine.part) && workingEngineInfo[i].partInfo.decoupleStage < stage && (!HighLogic.LoadedSceneIsFlight || StageManager.CurrentStage != stage || workingEngineInfo[i].engine.EngineIgnited))
				{
					enginesActiveInStage.Add(workingEngineInfo[i]);
				}
			}
		}
	}

	internal void ResetPartCaches()
	{
		cachedResourcePartSetParts = null;
	}

	public float GetStageMass(out float dryMass, out float fuelMass, out float decoupledDryMass, out float decoupledFuelMass, out float jettisonedDryMass, out int decoupledPartCount)
	{
		dryMass = 0f;
		fuelMass = 0f;
		decoupledDryMass = 0f;
		decoupledFuelMass = 0f;
		jettisonedDryMass = 0f;
		decoupledPartCount = 0;
		int count = parts.Count;
		while (count-- > 0)
		{
			DeltaVPartInfo deltaVPartInfo = parts[count];
			dryMass += deltaVPartInfo.dryMass;
			fuelMass += deltaVPartInfo.GetCurrentFuelMass();
			if (deltaVPartInfo.decoupleStage == stage)
			{
				decoupledPartCount++;
				decoupledDryMass += deltaVPartInfo.dryMass;
				decoupledFuelMass += deltaVPartInfo.GetCurrentFuelMass();
			}
			if (deltaVPartInfo.JettisonInStage(stage))
			{
				jettisonedDryMass += deltaVPartInfo.jettisonMass;
			}
		}
		return dryMass + fuelMass;
	}

	public float GetCurrentStageMass()
	{
		float num = startMass;
		float num2 = 0f;
		for (int i = 0; i < enginesActiveInStage.Count; i++)
		{
			num2 += enginesActiveInStage[i].PropellantMassBurnt();
		}
		num -= num2;
		if (GameSettings.LOG_DELTAV_VERBOSE)
		{
			Debug.LogFormat("[StageInfo]: End of Stage {0} Mass. Start Mass: {1} Fuel Mass Lost = {2} Final Mass {3}", stage, startMass, num2, num);
		}
		return num;
	}

	public bool ContainsAnchoredDecoupler()
	{
		int num = 0;
		while (true)
		{
			if (num < parts.Count)
			{
				if (parts[num].moduleAnchoredDecoupler != null)
				{
					break;
				}
				num++;
				continue;
			}
			return false;
		}
		return true;
	}

	public bool ContainsDecoupler()
	{
		int num = 0;
		while (true)
		{
			if (num < parts.Count)
			{
				if (parts[num].isDecoupler)
				{
					break;
				}
				num++;
				continue;
			}
			return false;
		}
		return true;
	}

	public Ray CoTForStage(bool activeOnly = false)
	{
		float num = 0f;
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		CenterOfThrustQuery centerOfThrustQuery = new CenterOfThrustQuery();
		int count = enginesActiveInStage.Count;
		while (count-- > 0)
		{
			if (!activeOnly || enginesActiveInStage[count].engine.EngineIgnited)
			{
				centerOfThrustQuery.Reset();
				enginesActiveInStage[count].engine.OnCenterOfThrustQuery(centerOfThrustQuery);
				zero += centerOfThrustQuery.pos * centerOfThrustQuery.thrust;
				zero2 += centerOfThrustQuery.dir * centerOfThrustQuery.thrust;
				num += centerOfThrustQuery.thrust;
			}
		}
		if (num != 0f)
		{
			float num2 = 1f / num;
			zero *= num2;
			zero2 *= num2;
			return new Ray(zero, zero2);
		}
		return new Ray(Vector3.zero, Vector3.zero);
	}

	public void CalculateISP(List<DeltaVEngineInfo> engines = null)
	{
		bool flag = true;
		ispVac = 0.0;
		ispASL = 0.0;
		ispActual = 0.0;
		thrustVac = 0f;
		thrustASL = 0f;
		thrustActual = 0f;
		if (engines == null)
		{
			engines = enginesActiveInStage;
		}
		for (int i = 0; i < engines.Count; i++)
		{
			DeltaVEngineInfo deltaVEngineInfo = engines[i];
			deltaVEngineInfo.CalculateThrustVector(stage == StageManager.CurrentStage);
			thrustVac += deltaVEngineInfo.thrustVac;
			thrustASL += deltaVEngineInfo.thrustASL;
			thrustActual += deltaVEngineInfo.thrustActual;
			deltaVEngineInfo.CalculateISP(stage == StageManager.CurrentStage);
			if (deltaVEngineInfo.ispVac != ispVac)
			{
				if (ispVac == 0.0)
				{
					ispVac = deltaVEngineInfo.ispVac;
					ispASL = deltaVEngineInfo.ispASL;
					ispActual = deltaVEngineInfo.ispActual;
				}
				else
				{
					flag = false;
				}
			}
		}
		if (!flag)
		{
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			for (int j = 0; j < engines.Count; j++)
			{
				DeltaVEngineInfo deltaVEngineInfo2 = engines[j];
				num += (double)deltaVEngineInfo2.thrustVac / deltaVEngineInfo2.ispVac;
				num2 += (double)deltaVEngineInfo2.thrustASL / deltaVEngineInfo2.ispASL;
				num3 += (double)deltaVEngineInfo2.thrustActual / deltaVEngineInfo2.ispActual;
			}
			ispVac = (double)thrustVac / num;
			ispASL = (double)thrustASL / num2;
			ispActual = (double)thrustActual / num3;
		}
	}

	public void CalculateTWR(float mass = -1f)
	{
		float num = (float)FlightGlobals.GetHomeBody().GeeASL;
		if (HighLogic.LoadedSceneIsFlight)
		{
			if (vesselDeltaV != null && vesselDeltaV.Vessel != null && vesselDeltaV.Vessel.mainBody != null && vesselDeltaV.Vessel.mainBody != null)
			{
				num = (float)vesselDeltaV.Vessel.mainBody.GeeASL;
			}
		}
		else if (HighLogic.LoadedSceneIsEditor)
		{
			DeltaVAppValues deltaVAppValues = DeltaVGlobals.DeltaVAppValues;
			if (deltaVAppValues != null && deltaVAppValues.body != null)
			{
				num = (float)deltaVAppValues.body.GeeASL;
			}
		}
		TWRVac = thrustVac / (startMass * ((float)PhysicsGlobals.GravitationalAcceleration * num));
		TWRASL = thrustASL / (startMass * ((float)PhysicsGlobals.GravitationalAcceleration * num));
		TWRActual = thrustActual / (((mass == -1f) ? startMass : mass) * ((float)PhysicsGlobals.GravitationalAcceleration * num));
	}

	public int PartsActiveInStage()
	{
		int num = 0;
		for (int i = 0; i < parts.Count; i++)
		{
			if (parts[i].decoupleStage != stage)
			{
				num++;
			}
		}
		return num;
	}

	public int PartsActivateInStage(out List<DeltaVPartInfo> activeParts)
	{
		int num = 0;
		activeParts = new List<DeltaVPartInfo>();
		for (int i = 0; i < parts.Count; i++)
		{
			if (parts[i].activationStage == stage)
			{
				num++;
				activeParts.Add(parts[i]);
			}
		}
		return num;
	}

	public int PartsDecoupledInStage()
	{
		int num = 0;
		for (int i = 0; i < parts.Count; i++)
		{
			if (parts[i].decoupleStage == stage)
			{
				num++;
			}
		}
		return num;
	}
}
