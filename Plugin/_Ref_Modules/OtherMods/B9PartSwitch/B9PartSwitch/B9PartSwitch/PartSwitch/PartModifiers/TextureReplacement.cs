using System;
using UnityEngine;

namespace B9PartSwitch.PartSwitch.PartModifiers;

public class TextureReplacement : PartModifierBase, IPartAspectLock
{
	private readonly Renderer renderer;

	private readonly string shaderProperty;

	private readonly Texture oldTexture;

	private readonly Texture newTexture;

	public object PartAspectLock => renderer.GetInstanceID() + "---" + shaderProperty;

	public override string Description => "object " + renderer.name + " shader property " + shaderProperty;

	public TextureReplacement(Renderer renderer, string shaderProperty, Texture newTexture)
	{
		renderer.ThrowIfNullArgument("renderer");
		shaderProperty.ThrowIfNullOrEmpty("shaderProperty");
		newTexture.ThrowIfNullArgument("newTexture");
		this.renderer = renderer;
		this.shaderProperty = shaderProperty;
		this.newTexture = newTexture;
		oldTexture = renderer.sharedMaterial.GetTexture(shaderProperty);
		if (oldTexture == null)
		{
			throw new ArgumentException(renderer.sharedMaterial.name + " has no texture on the property " + shaderProperty);
		}
	}

	public override void ActivateOnStartEditor()
	{
		Activate();
	}

	public override void ActivateOnStartFlight()
	{
		Activate();
	}

	public override void ActivateOnSwitchEditor()
	{
		Activate();
	}

	public override void ActivateOnSwitchFlight()
	{
		Activate();
	}

	public override void DeactivateOnSwitchEditor()
	{
		Deactivate();
	}

	public override void DeactivateOnSwitchFlight()
	{
		Deactivate();
	}

	public override void OnIconCreateActiveSubtype()
	{
		Activate();
	}

	public override void OnWillBeCopiedActiveSubtype()
	{
		Deactivate();
	}

	public override void OnWasCopiedActiveSubtype()
	{
		renderer.material = new Material(renderer.material);
		Activate();
	}

	public override void OnBeforeReinitializeActiveSubtype()
	{
		Deactivate();
	}

	public override void OnAfterReinitializeActiveSubtype()
	{
		Activate();
	}

	private void Activate()
	{
		renderer.material.SetTexture(shaderProperty, newTexture);
	}

	private void Deactivate()
	{
		renderer.material.SetTexture(shaderProperty, oldTexture);
	}
}
