namespace InterstellarFuelSwitch;

internal class ResourceStats
{
	public PartResourceDefinition definition;

	public double maxAmount;

	public double currentAmount;

	public double amountRatio;

	public double retrieveAmount;

	public double transferRate = 1.0;

	public double normalizedDensity;

	public double conversionRatio;
}
