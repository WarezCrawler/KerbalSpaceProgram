using KSP.UI.Screens;
using UnityEngine;

namespace InterstellarFuelSwitch;

[KSPAddon(KSPAddon.Startup.EditorAny, false)]
internal class HideHiddenPartsFilter : MonoBehaviour
{
	private void Start()
	{
		EditorPartList.Instance.ExcludeFilters.AddFilter(new EditorPartListFilter<AvailablePart>("FuelSwitch Parts Filter", (AvailablePart p) => p.TechRequired != "hidden"));
		EditorPartList.Instance.Refresh();
	}
}
