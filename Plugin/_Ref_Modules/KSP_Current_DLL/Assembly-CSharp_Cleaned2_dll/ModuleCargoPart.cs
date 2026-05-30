using ns9;

public class ModuleCargoPart : PartModule
{
	[KSPField]
	public string inventoryTooltip = "";

	public virtual void OnDestroy()
	{
	}

	public override string GetInfo()
	{
		return Localizer.Format("#autoLOC_8002220");
	}

	public virtual string GetTooltip()
	{
		if (string.IsNullOrEmpty(inventoryTooltip))
		{
			return GetInfo();
		}
		return Localizer.Format(inventoryTooltip);
	}

	public override string GetModuleDisplayName()
	{
		return Localizer.Format("#autoLOC_8002221");
	}
}
