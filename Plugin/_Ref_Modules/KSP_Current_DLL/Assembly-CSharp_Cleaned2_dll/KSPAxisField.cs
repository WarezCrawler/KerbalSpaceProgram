public class KSPAxisField : KSPField
{
	public float minValue;

	public float maxValue;

	public float incrementalSpeed;

	public bool ignoreClampWhenIncremental;

	public bool ignoreIncrementByZero;

	public KSPAxisGroup axisGroup;

	public KSPAxisMode axisMode;

	public KSPAxisField()
	{
		minValue = 0f;
		maxValue = 1f;
		incrementalSpeed = 1f;
		axisGroup = KSPAxisGroup.None;
		axisMode = KSPAxisMode.Incremental;
		ignoreClampWhenIncremental = false;
		ignoreIncrementByZero = true;
	}
}
