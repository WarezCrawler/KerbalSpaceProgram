namespace KerbalEngineer.Extensions;

public static class PartResourceExtensions
{
	public static double GetCost(this PartResource resource)
	{
		return resource.amount * (double)resource.info.unitCost;
	}

	public static PartResourceDefinition GetDefinition(this PartResource resource)
	{
		return PartResourceLibrary.Instance.GetDefinition(resource.info.id);
	}

	public static double GetDensity(this PartResource resource)
	{
		return resource.GetDefinition().density;
	}

	public static double GetMass(this PartResource resource)
	{
		return resource.amount * resource.GetDensity();
	}
}
