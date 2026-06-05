using System;
using System.Collections.Generic;
using System.Linq;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;
using B9PartSwitch.Utils;
using UnityEngine;

namespace B9PartSwitch;

public class MaterialModifierInfo : IContextualNode
{
	[NodeData]
	public string name;

	[NodeData(name = "baseTransform")]
	public List<IStringMatcher> baseTransformNames = new List<IStringMatcher>();

	[NodeData(name = "transform")]
	public List<IStringMatcher> transformNames = new List<IStringMatcher>();

	[NodeData(name = "FLOAT")]
	public List<FloatPropertyModifierInfo> floatPropertyModifierInfos = new List<FloatPropertyModifierInfo>();

	[NodeData(name = "COLOR")]
	public List<ColorPropertyModifierInfo> colorPropertyModifierInfos = new List<ColorPropertyModifierInfo>();

	[NodeData(name = "TEXTURE")]
	public List<TexturePropertyModifierInfo> texturePropertyModifierInfos = new List<TexturePropertyModifierInfo>();

	public void Load(ConfigNode node, OperationContext context)
	{
		this.LoadFields(node, context);
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		this.SaveFields(node, context);
	}

	public IEnumerable<IPartModifier> CreateModifiers(Transform rootTransform, Action<string> onError)
	{
		rootTransform.ThrowIfNullArgument("rootTransform");
		IEnumerable<Renderer> renderers;
		if (baseTransformNames.IsNullOrEmpty() && transformNames.IsNullOrEmpty())
		{
			renderers = rootTransform.GetComponentsInChildren<Renderer>(includeInactive: true);
		}
		else
		{
			renderers = GetBaseTransformRenderers(rootTransform, onError);
			renderers = renderers.Concat(GetTransformRenderers(rootTransform, onError));
			renderers = renderers.Distinct();
		}
		foreach (FloatPropertyModifierInfo floatPropertyModifierInfo in floatPropertyModifierInfos)
		{
			foreach (IPartModifier item in floatPropertyModifierInfo.CreateModifiers(renderers))
			{
				yield return item;
			}
		}
		foreach (ColorPropertyModifierInfo colorPropertyModifierInfo in colorPropertyModifierInfos)
		{
			foreach (IPartModifier item2 in colorPropertyModifierInfo.CreateModifiers(renderers))
			{
				yield return item2;
			}
		}
		foreach (TexturePropertyModifierInfo texturePropertyModifierInfo in texturePropertyModifierInfos)
		{
			foreach (IPartModifier item3 in texturePropertyModifierInfo.CreateModifiers(renderers, onError))
			{
				yield return item3;
			}
		}
	}

	private IEnumerable<Renderer> GetBaseTransformRenderers(Transform rootTransform, Action<string> onError)
	{
		IEnumerable<Renderer> enumerable = Enumerable.Empty<Renderer>();
		if (baseTransformNames == null)
		{
			return enumerable;
		}
		foreach (IStringMatcher baseTransformName in baseTransformNames)
		{
			bool flag = false;
			foreach (Transform item in from t in rootTransform.TraverseHierarchy()
				where baseTransformName.Match(t.name)
				select t)
			{
				flag = true;
				Renderer[] componentsInChildren = item.GetComponentsInChildren<Renderer>(includeInactive: true);
				if (componentsInChildren.Length == 0)
				{
					onError($"No renderers found on transform '{baseTransformName}'");
				}
				else
				{
					enumerable = enumerable.Concat(componentsInChildren);
				}
			}
			if (!flag)
			{
				onError($"No transforms matching '{baseTransformName}' found");
			}
		}
		return enumerable;
	}

	private IEnumerable<Renderer> GetTransformRenderers(Transform rootTransform, Action<string> onError)
	{
		IEnumerable<Renderer> enumerable = Enumerable.Empty<Renderer>();
		if (transformNames == null)
		{
			return enumerable;
		}
		foreach (IStringMatcher transformName in transformNames)
		{
			bool flag = false;
			foreach (Transform item in from t in rootTransform.TraverseHierarchy()
				where transformName.Match(t.name)
				select t)
			{
				flag = true;
				Renderer[] components = item.GetComponents<Renderer>();
				if (components.Length == 0)
				{
					onError($"No renderers found on transform '{transformName}'");
				}
				else
				{
					enumerable = enumerable.Concat(components);
				}
			}
			if (!flag)
			{
				onError($"No transforms matching '{transformName}' found");
			}
		}
		return enumerable;
	}
}
