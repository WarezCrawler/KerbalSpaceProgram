public class ProtoPartResourceSnapshot
{
	protected ConfigNode resourceValues;

	public string resourceName;

	public PartResource resourceRef;

	public double amount;

	public double maxAmount;

	public bool flowState;

	public PartResourceDefinition definition { get; private set; }

	public ProtoPartResourceSnapshot(PartResource resource)
	{
		resourceRef = resource;
		resourceName = resource.info.name;
		resourceValues = new ConfigNode("RESOURCE");
		resource.Save(resourceValues);
		amount = resource.amount;
		maxAmount = resource.maxAmount;
		flowState = resource.flowState;
		definition = PartResourceLibrary.Instance.GetDefinition(resourceName);
	}

	public ProtoPartResourceSnapshot(ConfigNode node)
	{
		resourceValues = new ConfigNode("RESOURCE");
		node.CopyTo(resourceValues);
		string text = null;
		if ((text = node.GetValue("name")) != null)
		{
			resourceName = text;
		}
		if ((text = node.GetValue("amount")) != null)
		{
			amount = double.Parse(text);
		}
		if ((text = node.GetValue("maxAmount")) != null)
		{
			maxAmount = double.Parse(text);
		}
		if ((text = node.GetValue("flowState")) != null)
		{
			flowState = bool.Parse(text);
		}
		if (!string.IsNullOrEmpty(resourceName))
		{
			definition = PartResourceLibrary.Instance.GetDefinition(resourceName);
		}
	}

	public void Save(ConfigNode node)
	{
		resourceValues.CopyTo(node);
		node.SetValue("flowState", flowState, createIfNotFound: true);
		node.SetValue("amount", amount, createIfNotFound: true);
		node.SetValue("maxAmount", maxAmount, createIfNotFound: true);
	}

	public void Load(Part hostPart)
	{
		resourceValues.SetValue("flowState", flowState, createIfNotFound: true);
		resourceValues.SetValue("amount", amount, createIfNotFound: true);
		resourceValues.SetValue("maxAmount", maxAmount, createIfNotFound: true);
		hostPart.SetResource(resourceValues);
	}
}
