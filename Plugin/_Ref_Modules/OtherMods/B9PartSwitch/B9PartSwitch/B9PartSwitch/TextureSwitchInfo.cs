using System;
using System.Collections.Generic;
using System.Linq;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;
using B9PartSwitch.Utils;
using UnityEngine;

namespace B9PartSwitch;

public class TextureSwitchInfo : IContextualNode
{
	[NodeData(name = "currentTexture")]
	public string currentTextureName;

	[NodeData(name = "baseTransform")]
	public List<IStringMatcher> baseTransformNames;

	[NodeData(name = "transform")]
	public List<IStringMatcher> transformNames;

	[NodeData(name = "texture")]
	public string newTexturePath;

	[NodeData]
	public bool isNormalMap;

	[NodeData(name = "shaderProperty")]
	public string shaderPropName;

	public void Load(ConfigNode node, OperationContext context)
	{
		this.LoadFields(node, context);
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		this.SaveFields(node, context);
	}

	public IEnumerable<TextureReplacement> CreateTextureReplacements(Part part, Action<string> onError)
	{
		part.ThrowIfNullArgument("part");
		if (string.IsNullOrEmpty(newTexturePath))
		{
			onError("texture name is empty");
			yield break;
		}
		Texture newTexture = GameDatabase.Instance.GetTexture(newTexturePath, isNormalMap);
		if (newTexture == null)
		{
			onError("Texture '" + newTexturePath + "' not found!");
			yield break;
		}
		string shaderProperty = shaderPropName;
		if (string.IsNullOrEmpty(shaderProperty))
		{
			shaderProperty = ((!isNormalMap) ? "_MainTex" : "_BumpMap");
		}
		IEnumerable<Renderer> enumerable;
		if (baseTransformNames.IsNullOrEmpty() && transformNames.IsNullOrEmpty())
		{
			enumerable = part.GetModelRoot().GetComponentsInChildren<Renderer>(includeInactive: true);
		}
		else
		{
			enumerable = GetBaseTransformRenderers(part, onError);
			enumerable = enumerable.Concat(GetTransformRenderers(part, onError));
			enumerable = enumerable.Distinct();
		}
		foreach (Renderer item in enumerable)
		{
			Texture texture = item.sharedMaterial.GetTexture(shaderProperty);
			if (!(texture == null) && (currentTextureName.IsNullOrEmpty() || !(texture.name.Substring(texture.name.LastIndexOf('/') + 1) != currentTextureName)))
			{
				yield return new TextureReplacement(item, shaderProperty, newTexture);
			}
		}
	}

	private IEnumerable<Renderer> GetBaseTransformRenderers(Part part, Action<string> onError)
	{
		IEnumerable<Renderer> enumerable = Enumerable.Empty<Renderer>();
		if (baseTransformNames == null)
		{
			return enumerable;
		}
		foreach (IStringMatcher baseTransformName in baseTransformNames)
		{
			bool flag = false;
			foreach (Transform item in from t in part.GetModelRoot().TraverseHierarchy()
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

	private IEnumerable<Renderer> GetTransformRenderers(Part part, Action<string> onError)
	{
		IEnumerable<Renderer> enumerable = Enumerable.Empty<Renderer>();
		if (transformNames == null)
		{
			return enumerable;
		}
		foreach (IStringMatcher transformName in transformNames)
		{
			bool flag = false;
			foreach (Transform item in from t in part.GetModelRoot().TraverseHierarchy()
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
