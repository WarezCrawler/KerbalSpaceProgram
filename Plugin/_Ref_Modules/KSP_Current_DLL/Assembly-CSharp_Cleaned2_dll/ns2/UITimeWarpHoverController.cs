namespace ns2;

public class UITimeWarpHoverController : UIHoverSlidePanel
{
	private int currentRateIndex;

	private void Update()
	{
		if (currentRateIndex == TimeWarp.CurrentRateIndex || coroutine != null)
		{
			return;
		}
		currentRateIndex = TimeWarp.CurrentRateIndex;
		if (TimeWarp.CurrentRateIndex > 0)
		{
			locked = true;
			coroutine = StartCoroutine(MoveToState(0f, newState: true));
			return;
		}
		locked = false;
		if (!pointOver)
		{
			coroutine = StartCoroutine(MoveToState(0f, newState: false));
		}
	}
}
