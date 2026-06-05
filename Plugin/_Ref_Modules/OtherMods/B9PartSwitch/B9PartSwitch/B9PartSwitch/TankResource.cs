using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch;

public class TankResource : IContextualNode
{
	[NodeData(name = "name")]
	public PartResourceDefinition resourceDefinition;

	[NodeData]
	public float unitsPerVolume = 1f;

	[NodeData]
	public float? percentFilled;

	public string ResourceName => resourceDefinition.name;

	public void Load(ConfigNode node, OperationContext context)
	{
		this.LoadFields(node, context);
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		this.SaveFields(node, context);
	}

	public override string ToString()
	{
		string text = "Tank Resource:";
		text = ((resourceDefinition == null) ? (text + " Null resource") : (text + " Resource Name = " + ResourceName));
		return text + $" unitsPerVolume = {unitsPerVolume}";
	}
}
