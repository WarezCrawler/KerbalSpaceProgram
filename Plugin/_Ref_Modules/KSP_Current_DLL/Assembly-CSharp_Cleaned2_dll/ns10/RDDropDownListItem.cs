using KSP.UI;
using TMPro;
using UnityEngine;
using ns2;

namespace ns10;

[RequireComponent(typeof(UIListItem))]
public class RDDropDownListItem : MonoBehaviour
{
	public TextMeshProUGUI header;

	public TextMeshProUGUI description;

	public UIRadioButton radioButton;

	public void Setup(string header, string description)
	{
		this.header.text = header;
		this.description.text = description;
	}
}
