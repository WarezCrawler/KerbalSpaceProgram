using System;
using UnityEngine;

namespace B9PartSwitch.PartSwitch.PartModifiers;

public class ColorPropertyModifier : PartModifierBase, IPartAspectLock
{
	private readonly Renderer renderer;

	private readonly string shaderProperty;

	private readonly Color originalColor;

	private readonly Color newColor;

	public object PartAspectLock => renderer.GetInstanceID() + "---" + shaderProperty;

	public override string Description => "object " + renderer.name + " shader property " + shaderProperty;

	public ColorPropertyModifier(Renderer renderer, string shaderProperty, Color newColor)
	{
		renderer.ThrowIfNullArgument("renderer");
		shaderProperty.ThrowIfNullOrEmpty("shaderProperty");
		newColor.ThrowIfNullArgument("newColor");
		this.renderer = renderer;
		this.shaderProperty = shaderProperty;
		this.newColor = newColor;
		if (!renderer.sharedMaterial.HasProperty(shaderProperty))
		{
			throw new ArgumentException(renderer.sharedMaterial.name + " has no property " + shaderProperty);
		}
		originalColor = renderer.sharedMaterial.GetColor(shaderProperty);
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
		renderer.material.SetColor(shaderProperty, newColor);
	}

	private void Deactivate()
	{
		renderer.material.SetColor(shaderProperty, originalColor);
	}
}
