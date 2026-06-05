namespace KerbalEngineer;

public class VectorAverager
{
	private Vector3d sum = Vector3d.zero;

	private uint count;

	public void Add(Vector3d v)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		sum += v;
		count++;
	}

	public Vector3d Get()
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		if (count != 0)
		{
			return sum / (double)count;
		}
		return Vector3d.zero;
	}

	public void Reset()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		sum = Vector3d.zero;
		count = 0u;
	}
}
