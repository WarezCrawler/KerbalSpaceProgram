using System;
using System.Collections.Generic;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;
using UnityEngine;

namespace B9PartSwitch;

public class TexturePropertyModifierInfo : IContextualNode
{
	[NodeData(name = "currentTexture")]
	public string currentTextureName;

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

	public IEnumerable<IPartModifier> CreateModifiers(IEnumerable<Renderer> renderers, Action<string> onError)
	{
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
		foreach (Renderer renderer in renderers)
		{
			Texture texture = renderer.sharedMaterial.GetTexture(shaderProperty);
			if (!(texture == null) && (currentTextureName.IsNullOrEmpty() || !(texture.name.Substring(texture.name.LastIndexOf('/') + 1) != currentTextureName)))
			{
				yield return new TextureReplacement(renderer, shaderProperty, newTexture);
			}
		}
	}
}
