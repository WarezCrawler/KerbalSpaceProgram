using UnityEngine;
using ns9;

namespace CompoundParts;

public class CModuleFuelLine : CompoundPartModule, IModuleInfo
{
	private static string cacheAutoLOC_216750;

	private static string cacheAutoLOC_216757;

	public override void OnTargetSet(Part target)
	{
		target.fuelLookupTargets.Add(base.part);
		GameEvents.onPartFuelLookupStateChange.Fire(new GameEvents.HostedFromToAction<bool, Part>(host: false, target, base.part));
		base.part.stackIcon.SetIconColor(XKCDColors.BrightTeal);
	}

	public override void OnTargetLost()
	{
		CloseFuelLine();
	}

	public void CloseFuelLine()
	{
		if (base.target != null)
		{
			base.target.fuelLookupTargets.Remove(base.part);
			GameEvents.onPartFuelLookupStateChange.Fire(new GameEvents.HostedFromToAction<bool, Part>(host: false, base.target, base.part));
		}
		base.part.stackIcon.SetIconColor(XKCDColors.SlateGrey);
	}

	public override string GetInfo()
	{
		return Localizer.Format("#autoLOC_216749", base.compoundPart.maxLength.ToString("0.0")) + cacheAutoLOC_216750;
	}

	public string GetModuleTitle()
	{
		return "Fuel Line";
	}

	public Callback<Rect> GetDrawModulePanelCallback()
	{
		return null;
	}

	public string GetPrimaryField()
	{
		return "";
	}

	public override string GetModuleDisplayName()
	{
		return cacheAutoLOC_216757;
	}

	internal static void CacheLocalStrings()
	{
		cacheAutoLOC_216750 = Localizer.Format("#autoLOC_216750");
		cacheAutoLOC_216757 = Localizer.Format("#autoLOC_216757");
	}
}
