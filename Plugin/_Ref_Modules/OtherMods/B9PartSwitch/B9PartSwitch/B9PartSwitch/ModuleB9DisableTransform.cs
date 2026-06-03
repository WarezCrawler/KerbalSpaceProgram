using UnityEngine;

namespace B9PartSwitch;

public class ModuleB9DisableTransform : PartModule
{
	public override void OnLoad(ConfigNode node)
	{
		base.OnLoad(node);
		string[] values = node.GetValues("transform");
		foreach (string text in values)
		{
			Transform[] array = base.part.FindModelTransforms(text);
			if (array.Length == 0)
			{
				this.LogError("No transforms named '" + text + "' found in model");
				continue;
			}
			Transform[] array2 = array;
			for (int j = 0; j < array2.Length; j++)
			{
				array2[j].gameObject.SetActive(value: false);
			}
		}
		base.part.RemoveModule(this);
	}
}
