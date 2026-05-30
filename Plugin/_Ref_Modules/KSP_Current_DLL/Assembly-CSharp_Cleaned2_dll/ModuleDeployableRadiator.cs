using ns9;

public class ModuleDeployableRadiator : ModuleDeployablePart
{
	private static string cacheAutoLOC_7000030;

	public override void OnLoad(ConfigNode node)
	{
		base.OnLoad(node);
		subPartName = Localizer.Format("#autoLOC_234369");
		partType = Localizer.Format("#autoLOC_232117");
	}

	public override string GetModuleDisplayName()
	{
		return cacheAutoLOC_7000030;
	}

	internal new static void CacheLocalStrings()
	{
		cacheAutoLOC_7000030 = Localizer.Format("#autoLOC_7000030");
	}
}
