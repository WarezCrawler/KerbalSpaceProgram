using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FNPlugin;

[KSPModule("Animated Container")]
public class AnimatedContainerContent : PartModule
{
	[KSPField(isPersistant = false)]
	public string animationName;

	[KSPField(isPersistant = false)]
	public string resourceName;

	[KSPField(isPersistant = false)]
	public double animationExponent = 1.0;

	[KSPField(isPersistant = false)]
	public double maximumRatio = 1.0;

	[KSPField(isPersistant = false, guiName = "Animation Ratio", guiActiveEditor = false, guiActive = false, guiFormat = "F3")]
	public float animationRatio;

	private AnimationState[] containerStates;

	public override void OnStart(StartState state)
	{
		containerStates = SetUpAnimation(animationName, base.part);
	}

	private void Update()
	{
		double num = -1.0;
		if (!string.IsNullOrEmpty(resourceName))
		{
			PartResource partResource = base.part.Resources[resourceName];
			if (partResource != null)
			{
				num = ((partResource.maxAmount > 0.0) ? (partResource.amount / partResource.maxAmount) : 0.0);
			}
		}
		if (num == -1.0)
		{
			List<PartResource> list = base.part.Resources.Where((PartResource m) => m.info.density > 0f).ToList();
			if (list.Count == 0)
			{
				list = base.part.Resources.ToList();
			}
			double num2 = list.Sum((PartResource m) => m.maxAmount);
			double num3 = list.Sum((PartResource m) => m.amount);
			num = ((num2 > 0.0) ? (num3 / num2) : 0.0);
		}
		double num4 = ((maximumRatio == 1.0) ? 1.0 : ((maximumRatio > 0.0) ? (1.0 / maximumRatio) : 1.0));
		double num5 = ((num4 == 1.0) ? num : Math.Min(num4 * num, 1.0));
		double value = ((animationExponent == 1.0) ? num5 : Math.Pow(num5, animationExponent));
		animationRatio = (float)Math.Round(value, 3);
		AnimationState[] array = containerStates;
		foreach (AnimationState animationState in array)
		{
			animationState.normalizedTime = animationRatio;
		}
	}

	private static AnimationState[] SetUpAnimation(string animationName, Part part)
	{
		List<AnimationState> list = new List<AnimationState>();
		Animation[] array = part.FindModelAnimators(animationName);
		foreach (Animation animation in array)
		{
			AnimationState animationState = animation[animationName];
			animationState.speed = 0f;
			animationState.enabled = true;
			animationState.wrapMode = WrapMode.ClampForever;
			animation.Blend(animationName);
			list.Add(animationState);
		}
		return list.ToArray();
	}
}
