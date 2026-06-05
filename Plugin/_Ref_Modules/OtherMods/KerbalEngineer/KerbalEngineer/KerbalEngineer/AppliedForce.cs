namespace KerbalEngineer;

public class AppliedForce
{
	private static readonly Pool<AppliedForce> pool = new Pool<AppliedForce>(Create, Reset);

	public Vector3d vector;

	public Vector3d applicationPoint;

	private static AppliedForce Create()
	{
		return new AppliedForce();
	}

	private static void Reset(AppliedForce appliedForce)
	{
	}

	public static AppliedForce New(Vector3d vector, Vector3d applicationPoint)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		AppliedForce appliedForce = pool.Borrow();
		appliedForce.vector = vector;
		appliedForce.applicationPoint = applicationPoint;
		return appliedForce;
	}

	public void Release()
	{
		pool.Release(this);
	}
}
