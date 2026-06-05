using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace KerbalEngineer.VesselSimulator;

public class RCSSim
{
	private static readonly Pool<RCSSim> pool = new Pool<RCSSim>(Create, Reset);

	public readonly ResourceContainer resourceConsumptions = new ResourceContainer();

	public readonly ResourceContainer resourceFlowModes = new ResourceContainer();

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

	private static RCSSim Create()
	{
		return new RCSSim();
	}

	private static void Reset(RCSSim engineSim)
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

	public static RCSSim New(PartSim theEngine, ModuleRCS engineMod, double atmosphere, float machNumber, bool vectoredThrust, bool fullThrust, LogMsg log)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0096: Unknown result type (might be due to invalid IL or missing references)
		//IL_0097: Unknown result type (might be due to invalid IL or missing references)
		//IL_040d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0412: Unknown result type (might be due to invalid IL or missing references)
		//IL_0416: Unknown result type (might be due to invalid IL or missing references)
		//IL_041b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0420: Unknown result type (might be due to invalid IL or missing references)
		//IL_0422: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_042c: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_043c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		double maxFuelFlow = engineMod.maxFuelFlow;
		_ = engineMod.thrustPercentage;
		List<Transform> thrusterTransforms = engineMod.thrusterTransforms;
		Vector3 val = CalculateThrustVector(vectoredThrust ? thrusterTransforms : null, log);
		FloatCurve atmosphereCurve = engineMod.atmosphereCurve;
		_ = engineMod.G;
		List<Propellant> propellants = engineMod.propellants;
		bool moduleIsEnabled = ((PartModule)engineMod).moduleIsEnabled;
		bool flameout = engineMod.flameout;
		RCSSim rCSSim = pool.Borrow();
		rCSSim.isp = 0.0;
		rCSSim.maxMach = 0f;
		rCSSim.actualThrust = 0.0;
		rCSSim.partSim = theEngine;
		rCSSim.isActive = moduleIsEnabled;
		rCSSim.thrustVec = val;
		rCSSim.isFlamedOut = flameout;
		rCSSim.resourceConsumptions.Reset();
		rCSSim.resourceFlowModes.Reset();
		rCSSim.appliedForces.Clear();
		double num = 0.0;
		if (rCSSim.partSim.hasVessel)
		{
			log?.AppendLine("hasVessel is true");
			rCSSim.isp = atmosphereCurve.Evaluate((float)atmosphere);
			rCSSim.thrust = GetThrust(maxFuelFlow, rCSSim.isp);
			rCSSim.actualThrust = (rCSSim.isActive ? rCSSim.thrust : 0.0);
			if (log != null)
			{
				log.buf.AppendFormat("isp     = {0:g6}\n", rCSSim.isp);
				log.buf.AppendFormat("thrust  = {0:g6}\n", rCSSim.thrust);
				log.buf.AppendFormat("actual  = {0:g6}\n", rCSSim.actualThrust);
			}
			log?.AppendLine("throttleLocked is true, using thrust for flowRate");
			num = GetFlowRate(rCSSim.thrust, rCSSim.isp);
		}
		else
		{
			log?.buf.AppendLine("hasVessel is false");
			rCSSim.isp = atmosphereCurve.Evaluate((float)atmosphere);
			rCSSim.thrust = GetThrust(maxFuelFlow, rCSSim.isp);
			rCSSim.actualThrust = 0.0;
			if (log != null)
			{
				log.buf.AppendFormat("isp     = {0:g6}\n", rCSSim.isp);
				log.buf.AppendFormat("thrust  = {0:g6}\n", rCSSim.thrust);
				log.buf.AppendFormat("actual  = {0:g6}\n", rCSSim.actualThrust);
				log.AppendLine("no vessel, using thrust for flowRate");
			}
			num = GetFlowRate(rCSSim.thrust, rCSSim.isp);
		}
		log?.buf.AppendFormat("flowRate = {0:g6}\n", num);
		float num2 = 0f;
		for (int i = 0; i < propellants.Count; i++)
		{
			Propellant val2 = propellants[i];
			if (!val2.ignoreForIsp)
			{
				num2 += val2.ratio * ResourceContainer.GetResourceDensity(val2.id);
			}
		}
		log?.buf.AppendFormat("flowMass = {0:g6}\n", num2);
		for (int j = 0; j < propellants.Count; j++)
		{
			Propellant val3 = propellants[j];
			if (!val3.ignoreForIsp && !(val3.name == "ElectricCharge") && !(val3.name == "IntakeAir"))
			{
				double num3 = (double)val3.ratio * num / (double)num2;
				log?.buf.AppendFormat("Add consumption({0}, {1}:{2:d}) = {3:g6}\n", ResourceContainer.GetResourceName(val3.id), theEngine.name, theEngine.partId, num3);
				rCSSim.resourceConsumptions.Add(val3.id, num3);
				rCSSim.resourceFlowModes.Add(val3.id, (double)val3.GetFlowMode());
			}
		}
		for (int k = 0; k < thrusterTransforms.Count; k++)
		{
			Transform obj = thrusterTransforms[k];
			Vector3 forward = obj.forward;
			Vector3d val4 = Vector3d.op_Implicit(((Vector3)(ref forward)).normalized);
			Vector3d applicationPoint = Vector3d.op_Implicit(obj.position);
			AppliedForce item = AppliedForce.New(val4 * rCSSim.thrust, applicationPoint);
			rCSSim.appliedForces.Add(item);
		}
		return rCSSim;
	}

	private static Vector3 CalculateThrustVector(List<Transform> thrustTransforms, LogMsg log)
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

	public static double GetExhaustVelocity(double isp)
	{
		return isp * 9.80665;
	}

	public static float GetFlowModifier(bool atmChangeFlow, FloatCurve atmCurve, double atmDensity, FloatCurve velCurve, float machNumber, ref float maxMach)
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
		log?.buf.AppendFormat("RCS: [thrust = {0:g6}, actual = {1:g6}, isp = {2:g6}\n", thrust, actualThrust, isp);
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
}
