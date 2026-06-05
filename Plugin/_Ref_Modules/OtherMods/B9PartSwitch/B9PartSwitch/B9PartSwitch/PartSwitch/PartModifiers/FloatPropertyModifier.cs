using System;
using UnityEngine;

namespace B9PartSwitch.PartSwitch.PartModifiers;

public class FloatPropertyModifier : PartModifierBase, IPartAspectLock
{
	private readonly Renderer renderer;

	private readonly string shaderProperty;

	private readonly float originalValue;

	private readonly float newValue;

	public object PartAspectLock => renderer.GetInstanceID() + "---" + shaderProperty;

	public override string Description => "object " + renderer.name + " shader property " + shaderProperty;

	public FloatPropertyModifier(Renderer renderer, string shaderProperty, float newValue)
	{
		renderer.ThrowIfNullArgument("renderer");
		shaderProperty.ThrowIfNullOrEmpty("shaderProperty");
		newValue.ThrowIfNullArgument("newValue");
		this.renderer = renderer;
		this.shaderProperty = shaderProperty;
		this.newValue = newValue;
		if (!renderer.sharedMaterial.HasProperty(shaderProperty))
		{
			throw new ArgumentException(renderer.sharedMaterial.name + " has no property " + shaderProperty);
		}
		originalValue = renderer.sharedMaterial.GetFloat(shaderProperty);
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
		renderer.material.SetFloat(shaderProperty, newValue);
	}

	private void Deactivate()
	{
		renderer.material.SetFloat(shaderProperty, originalValue);
	}
}
