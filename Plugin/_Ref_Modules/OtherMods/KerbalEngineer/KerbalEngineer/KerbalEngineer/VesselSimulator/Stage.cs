namespace KerbalEngineer.VesselSimulator;

public class Stage
{
	public double actualThrust;

	public double actualThrustToWeight;

	public double cost;

	public double deltaV;

	public double inverseTotalDeltaV;

	public double isp;

	public double mass;

	public double rcsMass;

	public double maxThrustToWeight;

	public int number;

	public double thrust;

	public double thrustToWeight;

	public double time;

	public double totalCost;

	public double totalDeltaV;

	public double totalMass;

	public double totalTime;

	public int totalPartCount;

	public int partCount;

	public double resourceMass;

	public double maxThrustTorque;

	public double thrustOffsetAngle;

	public float maxMach;

	public double RCSIsp;

	public double RCSThrust;

	public double RCSdeltaVStart;

	public double RCSTWRStart;

	public double RCSdeltaVEnd;

	public double RCSTWREnd;

	public double RCSBurnTime;

	public void Dump(LogMsg log)
	{
		log.buf.AppendFormat("number        : {0:d}\n", number);
		log.buf.AppendFormat("cost          : {0:g6}\n", cost);
		log.buf.AppendFormat("totalCost     : {0:g6}\n", totalCost);
		log.buf.AppendFormat("time          : {0:g6}\n", time);
		log.buf.AppendFormat("totalTime     : {0:g6}\n", totalTime);
		log.buf.AppendFormat("mass          : {0:g6}\n", mass);
		log.buf.AppendFormat("totalMass     : {0:g6}\n", totalMass);
		log.buf.AppendFormat("isp           : {0:g6}\n", isp);
		log.buf.AppendFormat("thrust        : {0:g6}\n", thrust);
		log.buf.AppendFormat("actualThrust  : {0:g6}\n", actualThrust);
		log.buf.AppendFormat("thrustToWeight: {0:g6}\n", thrustToWeight);
		log.buf.AppendFormat("maxTWR        : {0:g6}\n", maxThrustToWeight);
		log.buf.AppendFormat("actualTWR     : {0:g6}\n", actualThrustToWeight);
		log.buf.AppendFormat("ThrustTorque  : {0:g6}\n", maxThrustTorque);
		log.buf.AppendFormat("ThrustOffset  : {0:g6}\n", thrustOffsetAngle);
		log.buf.AppendFormat("deltaV        : {0:g6}\n", deltaV);
		log.buf.AppendFormat("totalDeltaV   : {0:g6}\n", totalDeltaV);
		log.buf.AppendFormat("invTotDeltaV  : {0:g6}\n", inverseTotalDeltaV);
		log.buf.AppendFormat("RCSdeltaVStart        : {0:g6}\n", RCSdeltaVStart);
		log.buf.AppendFormat("RCSIsp   : {0:g6}\n", RCSIsp);
		log.buf.AppendFormat("RCSThrust  : {0:g6}\n", RCSThrust);
		log.buf.AppendFormat("RCSTWRStart        : {0:g6}\n", RCSTWRStart);
		log.buf.AppendFormat("RCSdeltaVEnd   : {0:g6}\n", RCSdeltaVEnd);
		log.buf.AppendFormat("RCSTWREnd  : {0:g6}\n", RCSTWREnd);
		log.Flush();
	}
}
