using B9PartSwitch.Fishbones.Context;

namespace B9PartSwitch.Fishbones;

public interface INodeDataField
{
	string Name { get; }

	void Load(ConfigNode node, OperationContext context);

	void Save(ConfigNode node, OperationContext context);
}
