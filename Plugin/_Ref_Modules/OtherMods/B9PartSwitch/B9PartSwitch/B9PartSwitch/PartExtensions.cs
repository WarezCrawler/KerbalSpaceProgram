using UniLinq;
using UnityEngine;

namespace B9PartSwitch;

public static class PartExtensions
{
	public static Part GetPrefab(this Part part)
	{
		return part.partInfo?.partPrefab;
	}

	public static PartResource AddResource(this Part part, PartResourceDefinition info, float maxAmount, float amount)
	{
		PartResource partResource = new PartResource(part);
		partResource.SetInfo(info);
		partResource.maxAmount = maxAmount;
		partResource.amount = amount;
		partResource.flowState = true;
		partResource.isTweakable = info.isTweakable;
		partResource.isVisible = info.isVisible;
		partResource.hideFlow = false;
		partResource.flowMode = PartResource.FlowMode.Both;
		part.Resources.dict.Add(info.name.GetHashCode(), partResource);
		PartResource partResource2 = new PartResource(partResource);
		partResource2.simulationResource = true;
		part.SimulationResources.dict.Add(info.name.GetHashCode(), partResource2);
		GameEvents.onPartResourceListChange.Fire(part);
		return partResource;
	}

	public static PartResource AddOrCreateResource(this Part part, PartResourceDefinition info, float maxAmount, float amount, bool modifyAmountIfPresent)
	{
		if (amount > maxAmount)
		{
			part.LogWarning($"Cannot add resource '{info.name}' with amount > maxAmount, will use maxAmount (amount = {amount}, maxAmount = {maxAmount})");
			amount = maxAmount;
		}
		else if (amount < 0f)
		{
			part.LogWarning($"Cannot add resource '{info.name}' with amount < 0, will use 0 (amount = {amount})");
			amount = 0f;
		}
		PartResource partResource = part.Resources[info.name];
		if (partResource == null)
		{
			partResource = part.AddResource(info, maxAmount, amount);
		}
		else
		{
			partResource.maxAmount = maxAmount;
			PartResource partResource2 = part.SimulationResources?[info.name];
			if (partResource2.IsNotNull())
			{
				partResource2.maxAmount = maxAmount;
			}
			if (modifyAmountIfPresent)
			{
				partResource.amount = amount;
			}
		}
		return partResource;
	}

	public static float GetResourceMassMax(this Part part)
	{
		return part.Resources.Sum((PartResource resource) => (float)resource.maxAmount * resource.info.density);
	}

	public static float GetResourceCostMax(this Part part)
	{
		return part.Resources.Sum((PartResource resource) => (float)resource.maxAmount * resource.info.unitCost);
	}

	public static float GetResourceCostOffset(this Part part)
	{
		return part.Resources.Sum((PartResource resource) => (float)(resource.amount - resource.maxAmount) * resource.info.unitCost);
	}

	public static void UpdateTransformEnabled(this Part part, Transform t)
	{
		bool flag = true;
		foreach (ModuleB9PartSwitch item in part.Modules.OfType<ModuleB9PartSwitch>())
		{
			if (!item.TransformShouldBeEnabled(t))
			{
				flag = false;
				break;
			}
		}
		t.gameObject.SetActive(flag);
		if (part.partRendererBoundsIgnore.Contains(t.name))
		{
			if (flag)
			{
				part.partRendererBoundsIgnore.Remove(t.name);
			}
		}
		else if (!flag)
		{
			part.partRendererBoundsIgnore.Add(t.name);
		}
	}

	public static void UpdateNodeEnabled(this Part part, AttachNode node)
	{
		bool flag = true;
		foreach (ModuleB9PartSwitch item in part.Modules.OfType<ModuleB9PartSwitch>())
		{
			if (!item.NodeShouldBeEnabled(node))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			node.Unhide();
		}
		else
		{
			node.Hide();
		}
	}

	public static Transform GetModelRoot(this Part part)
	{
		part.ThrowIfNullArgument("part");
		return part.partTransform.Find("model");
	}

	public static void FixModuleJettison(this Part part)
	{
		if (!HighLogic.LoadedSceneIsFlight)
		{
			return;
		}
		foreach (ModuleJettison item in part.Modules.OfType<ModuleJettison>())
		{
			if (item.useMultipleDragCubes && item.isFairing && item.decoupleEnabled && !item.isJettisoned && !(item.jettisonTransform == null) && !(item.jettisonTransform.root == part.gameObject.transform.root))
			{
				Object.Instantiate(item.jettisonTransform, item.jettisonTransform.parent);
				item.jettisonTransform.parent = part.GetModelRoot();
				item.jettisonTransform.gameObject.SetActive(value: false);
			}
		}
	}

	public static void LogInfo(this Part part, object message)
	{
		Debug.Log($"[Part {part.name}] {message}");
	}

	public static void LogWarning(this Part part, object message)
	{
		Debug.LogWarning($"[WARNING] [Part {part.name}] {message}");
	}

	public static void LogError(this Part part, object message)
	{
		Debug.LogError($"[ERROR] [Part {part.name}] {message}");
	}
}
