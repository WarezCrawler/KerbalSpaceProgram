using System.Collections.Generic;
using System.Text;
using KerbalEngineer.Editor;
using UnityEngine;

namespace KerbalEngineer.VesselSimulator;

public class EngineSim
{
	private static readonly Pool<EngineSim> pool = new Pool<EngineSim>(Create, Reset);

	private readonly ResourceContainer resourceConsumptions = new ResourceContainer();

	private readonly ResourceContainer resourceFlowModes = new ResourceContainer();

	public double actualThrust;

	public bool isActive;

	public double isp;

	public PartSim partSim;

	public List<AppliedForce> appliedForces = new List<AppliedForce>();

	public float maxMach;

	public bool isFlamedOut;

	public bool dontDecoupleActive = true;

	public double thrust;

	public Vector3 thrustVec;

	private Dictionary<int, HashSet<PartSim>> sourcePartSets = new Dictionary<int, HashSet<PartSim>>();

	private Dictionary<int, HashSet<PartSim>> stagePartSets = new Dictionary<int, HashSet<PartSim>>();

	private HashSet<PartSim> visited = new HashSet<PartSim>();

	public ResourceContainer ResourceConsumptions => resourceConsumptions;

	private static EngineSim Create()
	{
		return new EngineSim();
	}

	private static void Reset(EngineSim engineSim)
	{
		engineSim.resourceConsumptions.Reset();
		engineSim.resourceFlowModes.Reset();
		engineSim.partSim = null;
		engineSim.actualThrust = 0.0;
		engineSim.isActive = false;
		engineSim.isp = 0.0;
		for (int i = 0; i < engineSim.appliedForces.Count; i++)
		{
			engineSim.appliedForces[i].Release();
		}
		engineSim.appliedForces.Clear();
		engineSim.thrust = 0.0;
		engineSim.maxMach = 0f;
		engineSim.isFlamedOut = false;
	}

	public void Release()
	{
		pool.Release(this);
	}

	public static EngineSim New(PartSim theEngine, ModuleEngines engineMod, double atmosphere, float machNumber, bool vectoredThrust, bool fullThrust, LogMsg log)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_0642: Unknown result type (might be due to invalid IL or missing references)
		//IL_0647: Unknown result type (might be due to invalid IL or missing references)
		//IL_064b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0650: Unknown result type (might be due to invalid IL or missing references)
		//IL_0655: Unknown result type (might be due to invalid IL or missing references)
		//IL_0657: Unknown result type (might be due to invalid IL or missing references)
		//IL_065c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0661: Unknown result type (might be due to invalid IL or missing references)
		//IL_0663: Unknown result type (might be due to invalid IL or missing references)
		//IL_066c: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0680: Unknown result type (might be due to invalid IL or missing references)
		//IL_0615: Unknown result type (might be due to invalid IL or missing references)
		float maxFuelFlow = engineMod.maxFuelFlow;
		float minFuelFlow = engineMod.minFuelFlow;
		float thrustPercentage = engineMod.thrustPercentage;
		List<Transform> thrustTransforms = engineMod.thrustTransforms;
		List<float> thrustTransformMultipliers = engineMod.thrustTransformMultipliers;
		Vector3 val = CalculateThrustVector(vectoredThrust ? thrustTransforms : null, vectoredThrust ? thrustTransformMultipliers : null, log);
		FloatCurve atmosphereCurve = engineMod.atmosphereCurve;
		bool atmChangeFlow = engineMod.atmChangeFlow;
		FloatCurve atmCurve = (engineMod.useAtmCurve ? engineMod.atmCurve : null);
		FloatCurve velCurve = (engineMod.useVelCurve ? engineMod.velCurve : null);
		FloatCurve thrustCurve = (engineMod.useThrustCurve ? engineMod.thrustCurve : null);
		float currentThrottle = engineMod.currentThrottle;
		_ = engineMod.g;
		bool flag = engineMod.throttleLocked || fullThrust;
		List<Propellant> propellants = engineMod.propellants;
		float num = engineMod.thrustCurveRatio;
		bool isOperational = engineMod.isOperational;
		float num2 = (SimManager.hasInstalledRealFuels ? engineMod.finalThrust : engineMod.resultingThrust);
		bool flameout = engineMod.flameout;
		EngineSim engineSim = pool.Borrow();
		engineSim.isp = 0.0;
		engineSim.maxMach = 0f;
		engineSim.actualThrust = 0.0;
		engineSim.partSim = theEngine;
		engineSim.isActive = isOperational;
		engineSim.thrustVec = val;
		engineSim.isFlamedOut = flameout;
		engineSim.resourceConsumptions.Reset();
		engineSim.resourceFlowModes.Reset();
		engineSim.appliedForces.Clear();
		double num3 = 0.0;
		if (engineSim.partSim.hasVessel)
		{
			log?.AppendLine("hasVessel is true");
			foreach (Propellant item2 in propellants)
			{
				if (!item2.ignoreForThrustCurve)
				{
					double num4 = item2.totalResourceAvailable / item2.totalResourceCapacity;
					if (num4 < (double)num)
					{
						num = (float)num4;
					}
				}
			}
			float flowModifier = GetFlowModifier(atmChangeFlow, atmCurve, engineSim.partSim.part.atmDensity, velCurve, machNumber, thrustCurve, num, ref engineSim.maxMach);
			engineSim.isp = atmosphereCurve.Evaluate((float)atmosphere);
			engineSim.thrust = GetThrust(Mathf.Lerp(minFuelFlow, maxFuelFlow, GetThrustPercent(thrustPercentage)) * flowModifier, engineSim.isp);
			engineSim.actualThrust = (engineSim.isActive ? ((double)num2) : 0.0);
			if (log != null)
			{
				log.buf.AppendFormat("flowMod = {0:g6}\n", flowModifier);
				log.buf.AppendFormat("isp     = {0:g6}\n", engineSim.isp);
				log.buf.AppendFormat("thrust  = {0:g6}\n", engineSim.thrust);
				log.buf.AppendFormat("actual  = {0:g6}\n", engineSim.actualThrust);
				log.buf.AppendFormat("final  = {0:g6}\n", engineMod.finalThrust);
				log.buf.AppendFormat("resulting  = {0:g6}\n", engineMod.resultingThrust);
			}
			if (flag)
			{
				log?.AppendLine("throttleLocked is true, using thrust for flowRate");
				num3 = GetFlowRate(engineSim.thrust, engineSim.isp);
			}
			else if (currentThrottle > 0f && !engineSim.partSim.isLanded)
			{
				log?.AppendLine("throttled up and not landed, using actualThrust for flowRate");
				num3 = GetFlowRate(engineSim.actualThrust, engineSim.isp);
			}
			else
			{
				log?.AppendLine("throttled down or landed, using thrust for flowRate");
				num3 = GetFlowRate(engineSim.thrust, engineSim.isp);
			}
		}
		else
		{
			log?.buf.AppendLine("hasVessel is false");
			float flowModifier2 = GetFlowModifier(atmChangeFlow, atmCurve, CelestialBodies.SelectedBody.GetDensity(BuildAdvanced.Altitude), velCurve, machNumber, thrustCurve, num, ref engineSim.maxMach);
			engineSim.isp = atmosphereCurve.Evaluate((float)atmosphere);
			engineSim.thrust = GetThrust(Mathf.Lerp(minFuelFlow, maxFuelFlow, GetThrustPercent(thrustPercentage)) * flowModifier2, engineSim.isp);
			engineSim.actualThrust = 0.0;
			if (log != null)
			{
				log.buf.AppendFormat("flowMod = {0:g6}\n", flowModifier2);
				log.buf.AppendFormat("isp     = {0:g6}\n", engineSim.isp);
				log.buf.AppendFormat("thrust  = {0:g6}\n", engineSim.thrust);
				log.buf.AppendFormat("actual  = {0:g6}\n", engineSim.actualThrust);
				log.AppendLine("no vessel, using thrust for flowRate");
			}
			num3 = GetFlowRate(engineSim.thrust, engineSim.isp);
		}
		log?.buf.AppendFormat("flowRate = {0:g6}\n", num3);
		float num5 = 0f;
		for (int i = 0; i < propellants.Count; i++)
		{
			Propellant val2 = propellants[i];
			if (!val2.ignoreForIsp)
			{
				num5 += val2.ratio * ResourceContainer.GetResourceDensity(val2.id);
			}
		}
		log?.buf.AppendFormat("flowMass = {0:g6}\n", num5);
		for (int j = 0; j < propellants.Count; j++)
		{
			Propellant val3 = propellants[j];
			if (!val3.ignoreForIsp && !(val3.name == "ElectricCharge") && !(val3.name == "IntakeAir"))
			{
				double num6 = (double)val3.ratio * num3 / (double)num5;
				log?.buf.AppendFormat("Add consumption({0}, {1}:{2:d}) = {3:g6}\n", ResourceContainer.GetResourceName(val3.id), theEngine.name, theEngine.partId, num6);
				engineSim.resourceConsumptions.Add(val3.id, num6);
				engineSim.resourceFlowModes.Add(val3.id, (double)val3.GetFlowMode());
			}
		}
		for (int k = 0; k < thrustTransforms.Count; k++)
		{
			Transform obj = thrustTransforms[k];
			Vector3 forward = obj.forward;
			Vector3d val4 = Vector3d.op_Implicit(((Vector3)(ref forward)).normalized);
			Vector3d applicationPoint = Vector3d.op_Implicit(obj.position);
			AppliedForce item = AppliedForce.New(val4 * engineSim.thrust * (double)thrustTransformMultipliers[k], applicationPoint);
			engineSim.appliedForces.Add(item);
		}
		return engineSim;
	}

	private static Vector3 CalculateThrustVector(List<Transform> thrustTransforms, List<float> thrustTransformMultipliers, LogMsg log)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0087: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_015d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
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
			val -= val2.forward * thrustTransformMultipliers[i];
		}
		log?.buf.AppendFormat("ThrustVec  = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", val.x, val.y, val.z, ((Vector3)(ref val)).magnitude);
		((Vector3)(ref val)).Normalize();
		log?.buf.AppendFormat("ThrustVecN = ({0:g6}, {1:g6}, {2:g6})   length = {3:g6}\n", val.x, val.y, val.z, ((Vector3)(ref val)).magnitude);
		return val;
	}

	public static double GetExhaustVelocity(double isp)
	{
		return isp * 9.80665;
	}

	public static float GetFlowModifier(bool atmChangeFlow, FloatCurve atmCurve, double atmDensity, FloatCurve velCurve, float machNumber, FloatCurve thrustCurve, float thrustCurveRatio, ref float maxMach)
	{
		float num = 1f;
		if (atmChangeFlow)
		{
			num = (float)(atmDensity / 1.225);
			if (atmCurve != null)
			{
				num = atmCurve.Evaluate(num);
			}
		}
		if (velCurve != null)
		{
			num *= velCurve.Evaluate(machNumber);
			maxMach = velCurve.maxTime;
		}
		if (thrustCurve != null)
		{
			num *= thrustCurve.Evaluate(thrustCurveRatio);
		}
		if (num < float.Epsilon)
		{
			num = float.Epsilon;
		}
		return num;
	}

	public static double GetFlowRate(double thrust, double isp)
	{
		return thrust / GetExhaustVelocity(isp);
	}

	public static float GetThrottlePercent(float currentThrottle, float thrustPercentage)
	{
		return currentThrottle * GetThrustPercent(thrustPercentage);
	}

	public static double GetThrust(double flowRate, double isp)
	{
		return flowRate * GetExhaustVelocity(isp);
	}

	public static float GetThrustPercent(float thrustPercentage)
	{
		return thrustPercentage * 0.01f;
	}

	public void DumpEngineToLog(LogMsg log)
	{
		log?.buf.AppendFormat("[thrust = {0:g6}, actual = {1:g6}, isp = {2:g6}\n", thrust, actualThrust, isp);
	}

	public void DumpSourcePartSets(LogMsg log, string msg)
	{
		if (log == null)
		{
			return;
		}
		log.AppendLine("DumpSourcePartSets ", msg);
		foreach (int key in sourcePartSets.Keys)
		{
			log.AppendLine("SourcePartSet for ", ResourceContainer.GetResourceName(key));
			HashSet<PartSim> hashSet = sourcePartSets[key];
			if (hashSet.Count > 0)
			{
				foreach (PartSim item in hashSet)
				{
					log.AppendLine("Part ", item.name, ":", item.partId);
				}
			}
			else
			{
				log.AppendLine("No parts");
			}
		}
	}

	public bool SetResourceDrains(LogMsg log, List<PartSim> allParts, List<PartSim> allFuelLines, HashSet<PartSim> drainingParts)
	{
		//IL_0081: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_00aa: Expected I4, but got Unknown
		//IL_049c: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c6: Invalid comparison between Unknown and I4
		//IL_04c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cb: Invalid comparison between Unknown and I4
		//IL_04cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d0: Invalid comparison between Unknown and I4
		//IL_04d2: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d5: Invalid comparison between Unknown and I4
		foreach (HashSet<PartSim> value5 in sourcePartSets.Values)
		{
			value5.Clear();
		}
		for (int i = 0; i < resourceConsumptions.Types.Count; i++)
		{
			int num = resourceConsumptions.Types[i];
			if (!sourcePartSets.TryGetValue(num, out var value))
			{
				value = new HashSet<PartSim>();
				sourcePartSets.Add(num, value);
			}
			ResourceFlowMode val = (ResourceFlowMode)(int)resourceFlowModes[num];
			switch ((int)val)
			{
			case 0:
				if (this.partSim.resources[num] > 0.0001 && this.partSim.resourceFlowStates[num] != 0.0)
				{
					value.Add(this.partSim);
				}
				break;
			case 1:
			case 4:
			{
				for (int k = 0; k < allParts.Count; k++)
				{
					PartSim partSim2 = allParts[k];
					if (partSim2.resources[num] > 0.0001 && partSim2.resourceFlowStates[num] != 0.0)
					{
						value.Add(partSim2);
					}
				}
				break;
			}
			case 2:
			case 5:
			{
				log?.Append("Find ", ResourceContainer.GetResourceName(num), " sources for ", this.partSim.name).AppendLine(":", this.partSim.partId);
				foreach (HashSet<PartSim> value6 in stagePartSets.Values)
				{
					value6.Clear();
				}
				int num2 = -1;
				for (int j = 0; j < allParts.Count; j++)
				{
					PartSim partSim = allParts[j];
					if (!(partSim.resources[num] <= 0.0001) && partSim.resourceFlowStates[num] != 0.0)
					{
						int inverseStage = partSim.inverseStage;
						if (inverseStage > num2)
						{
							num2 = inverseStage;
						}
						if (!stagePartSets.TryGetValue(inverseStage, out var value2))
						{
							value2 = new HashSet<PartSim>();
							stagePartSets.Add(inverseStage, value2);
						}
						value2.Add(partSim);
					}
				}
				for (int num3 = num2; num3 >= -1; num3--)
				{
					if (stagePartSets.TryGetValue(num3, out var value3) && value3.Count > 0)
					{
						foreach (PartSim item in value3)
						{
							value.Add(item);
						}
						break;
					}
				}
				break;
			}
			case 3:
			case 6:
			case 7:
				visited.Clear();
				log?.Append("Find ", ResourceContainer.GetResourceName(num), " sources for ", this.partSim.name).AppendLine(":", this.partSim.partId);
				this.partSim.GetSourceSet(num, allParts, visited, value, log, "");
				break;
			default:
				log?.Append("SetResourceDrains(", this.partSim.name, ":", this.partSim.partId).AppendLine(") Unexpected flow type for ", ResourceContainer.GetResourceName(num), ")");
				break;
			}
			if (log == null || value.Count <= 0)
			{
				continue;
			}
			log.AppendLine("Source parts for ", ResourceContainer.GetResourceName(num), ":");
			foreach (PartSim item2 in value)
			{
				log.AppendLine(item2.name, ":", item2.partId);
			}
		}
		for (int l = 0; l < resourceConsumptions.Types.Count; l++)
		{
			int num4 = resourceConsumptions.Types[l];
			if (!sourcePartSets.TryGetValue(num4, out var value4) || value4.Count == 0)
			{
				log?.AppendLine("No source of ", ResourceContainer.GetResourceName(num4));
				isActive = false;
				return false;
			}
		}
		for (int m = 0; m < resourceConsumptions.Types.Count; m++)
		{
			int num5 = resourceConsumptions.Types[m];
			HashSet<PartSim> hashSet = sourcePartSets[num5];
			ResourceFlowMode val2 = (ResourceFlowMode)(int)resourceFlowModes[num5];
			double num6 = resourceConsumptions[num5];
			double num7 = 0.0;
			double num8 = 0.0;
			if ((int)val2 == 4 || (int)val2 == 5 || (int)val2 == 7 || (int)val2 == 3)
			{
				foreach (PartSim item3 in hashSet)
				{
					num8 += item3.resources[num5];
				}
			}
			else
			{
				num7 = num6 / (double)hashSet.Count;
			}
			foreach (PartSim item4 in hashSet)
			{
				if (num8 != 0.0)
				{
					num7 = num6 * item4.resources[num5] / num8;
				}
				log?.Append("Adding drain of ", num7, " ", ResourceContainer.GetResourceName(num5)).AppendLine(" to ", item4.name, ":", item4.partId);
				item4.resourceDrains.Add(num5, num7);
				drainingParts.Add(item4);
			}
		}
		return true;
	}
}
