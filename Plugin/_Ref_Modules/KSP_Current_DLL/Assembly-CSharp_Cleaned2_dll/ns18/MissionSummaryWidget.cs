using TMPro;
using UnityEngine;
using ns10;

namespace ns18;

public class MissionSummaryWidget : MonoBehaviour
{
	private RectTransform rTrf;

	protected MissionRecoveryDialog host;

	[SerializeField]
	protected TextMeshProUGUI header;

	public RectTransform RTrf => rTrf;

	protected void Init(MissionRecoveryDialog host)
	{
		this.host = host;
		rTrf = GetComponent<RectTransform>();
	}
}
