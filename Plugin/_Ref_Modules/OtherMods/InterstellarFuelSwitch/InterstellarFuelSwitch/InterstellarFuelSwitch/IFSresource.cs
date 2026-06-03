namespace InterstellarFuelSwitch;

public class IFSresource
{
	public string name;

	public double amount;

	public double maxAmount;

	public double density;

	public double FullMass => maxAmount * density;

	public IFSresource(string name)
	{
		this.name = name;
		PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition(name);
		if (definition != null)
		{
			density = definition.density;
		}
	}
}
