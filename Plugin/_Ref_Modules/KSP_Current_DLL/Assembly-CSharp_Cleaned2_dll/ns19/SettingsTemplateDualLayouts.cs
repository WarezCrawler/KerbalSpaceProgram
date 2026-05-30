using UnityEngine;

namespace ns19;

public class SettingsTemplateDualLayouts : SettingsTemplate
{
	public RectTransform layout2;

	private bool flop;

	public RectTransform GetLayoutFlipFlop()
	{
		if (flop)
		{
			flop = !flop;
			return layout2;
		}
		flop = !flop;
		return layout;
	}
}
