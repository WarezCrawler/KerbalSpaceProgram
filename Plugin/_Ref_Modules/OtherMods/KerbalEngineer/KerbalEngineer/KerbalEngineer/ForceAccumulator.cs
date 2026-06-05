namespace KerbalEngineer;

public class ForceAccumulator
{
	private Vector3d totalForce = Vector3d.zero;

	private Vector3d totalZeroOriginTorque = Vector3d.zero;

	private WeightedVectorAverager avgApplicationPoint = new WeightedVectorAverager();

	public void AddForce(Vector3d applicationPoint, Vector3d force)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		totalForce += force;
		totalZeroOriginTorque += Vector3d.Cross(applicationPoint, force);
		avgApplicationPoint.Add(applicationPoint, ((Vector3d)(ref force)).magnitude);
	}

	public Vector3d GetAverageForceApplicationPoint()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return avgApplicationPoint.Get();
	}

	public void AddForce(AppliedForce force)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		AddForce(force.applicationPoint, force.vector);
	}

	public Vector3d TorqueAt(Vector3d origin)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		return totalZeroOriginTorque - Vector3d.Cross(origin, totalForce);
	}

	public Vector3d GetTotalForce()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		return totalForce;
	}

	public Vector3d GetMinTorqueForceApplicationPoint(Vector3d origin)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		double sqrMagnitude = ((Vector3d)(ref totalForce)).sqrMagnitude;
		if (sqrMagnitude <= 0.0)
		{
			return origin;
		}
		return origin + Vector3d.Cross(totalForce, TorqueAt(origin)) / sqrMagnitude;
	}

	public Vector3d GetMinTorqueForceApplicationPoint()
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		return GetMinTorqueForceApplicationPoint(avgApplicationPoint.Get());
	}

	public void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		totalForce = Vector3d.zero;
		totalZeroOriginTorque = Vector3d.zero;
		avgApplicationPoint.Reset();
	}
}
