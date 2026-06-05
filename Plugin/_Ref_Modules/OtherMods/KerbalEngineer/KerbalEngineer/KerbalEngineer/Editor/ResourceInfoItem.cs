using KerbalEngineer.Extensions;

namespace KerbalEngineer.Editor;

public class ResourceInfoItem
{
	public double Amount { get; set; }

	public PartResourceDefinition Definition { get; set; }

	public double Mass => Amount * (double)Definition.density;

	public string Name { get; set; }

	public ResourceInfoItem(PartResource resource)
	{
		Definition = resource.GetDefinition();
		Name = Definition.name;
		Amount = resource.amount;
	}
}
