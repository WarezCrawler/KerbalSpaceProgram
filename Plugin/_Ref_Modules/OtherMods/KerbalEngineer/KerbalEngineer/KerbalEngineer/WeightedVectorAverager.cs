namespace KerbalEngineer;

public class WeightedVectorAverager
{
	private Vector3d sum = Vector3d.zero;

	private double totalweight;

	public void Add(Vector3d v, double weight)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		sum += v * weight;
		totalweight += weight;
	}

	public Vector3d Get()
	{
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		if (totalweight > 0.0)
		{
			return sum / totalweight;
		}
		return Vector3d.zero;
	}

	public double GetTotalWeight()
	{
		return totalweight;
	}

	public void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		sum = Vector3d.zero;
		totalweight = 0.0;
	}
}
