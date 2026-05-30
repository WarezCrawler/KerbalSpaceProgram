using KSP.UI;
using TMPro;
using UnityEngine;

namespace ns10;

[RequireComponent(typeof(UIListItem))]
public class RDNodeListItem : MonoBehaviour
{
	public RDNode node;

	public TextMeshProUGUI text;
}
