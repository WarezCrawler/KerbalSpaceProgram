using UnityEngine;

namespace ns2;

public class Tooltip : MonoBehaviour
{
	private RectTransform rectTransform;

	public RectTransform RectTransform => rectTransform;

	protected void Awake()
	{
		rectTransform = base.transform as RectTransform;
	}
}
