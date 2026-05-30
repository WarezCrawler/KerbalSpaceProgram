using TMPro;
using UnityEngine.UI;
using ns2;

namespace ns10;

public class RDPartListItem : EditorPartIcon
{
	public UIStateButton button;

	public TextMeshProUGUI label;

	public Image searchHighlight;

	public AvailablePart myPart;

	public void Setup(string label, string buttonState, AvailablePart availablePart, PartUpgradeHandler.Upgrade upgrade)
	{
		if (upgrade != null)
		{
			base.upgrade = upgrade;
			isPart = false;
		}
		Create(null, availablePart, iconSize, iconOverScale, iconOverSpin);
		this.label.text = label;
		myPart = availablePart;
		button.SetState(buttonState);
	}
}
