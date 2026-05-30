using TMPro;
using UnityEngine;

namespace ns31;

public class ScreenDeltaVPartsListInfo : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI partsListText;

	public void UpdateData(string partsText)
	{
		partsListText.text = partsText;
	}
}
