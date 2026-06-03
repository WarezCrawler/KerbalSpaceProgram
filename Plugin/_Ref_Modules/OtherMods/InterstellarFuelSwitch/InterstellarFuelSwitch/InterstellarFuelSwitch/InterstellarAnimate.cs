using System;
using UnityEngine;

namespace InterstellarFuelSwitch;

public class InterstellarAnimate : PartModule
{
	public enum SSTUAnimState
	{
		EXTENDED,
		EXTENDING,
		RETRACTED,
		RETRACTING
	}

	[KSPField]
	public string animationName;

	[KSPField]
	public string actionDeployName = "Deploy";

	[KSPField]
	public string actionRetractName = "Retract";

	[KSPField]
	public string actionToggleName = "Toggle Status";

	[KSPField]
	public string animStatusName = "AnimState";

	public bool showAnimState = true;

	[KSPField]
	public bool editorEnabled = true;

	[KSPField]
	public bool flightEnabled = true;

	[KSPField]
	public bool toggleShielded;

	[KSPField]
	public int animationLayer;

	[KSPField]
	public int animationModule;

	[KSPField(isPersistant = true, guiName = "AnimState", guiActive = true)]
	public string deployedStatus = "RETRACTED";

	private SSTUAnimState animationState = SSTUAnimState.RETRACTED;

	private Animation[] deployAnimation;

	[KSPAction("Deploy")]
	public void deployAction(KSPActionParam param)
	{
		toggle();
	}

	[KSPAction("Retract")]
	public void retractAction(KSPActionParam param)
	{
		toggle();
	}

	[KSPAction("Toggle")]
	public void toggleAction(KSPActionParam param)
	{
		toggle();
	}

	[KSPEvent(name = "deployEvent", guiName = "Deploy", guiActiveUnfocused = true, externalToEVAOnly = true, guiActive = true, unfocusedRange = 4f, guiActiveEditor = true)]
	public void deployEvent()
	{
		toggle();
	}

	[KSPEvent(name = "retractEvent", guiName = "Retract", guiActiveUnfocused = true, externalToEVAOnly = true, guiActive = true, unfocusedRange = 4f, guiActiveEditor = true)]
	public void retractEvent()
	{
		toggle();
	}

	public override void OnStart(StartState state)
	{
		base.OnStart(state);
		MonoBehaviour.print("SSTUAnimate OnStart");
		initializeAnimation();
		if (animationState != SSTUAnimState.RETRACTED)
		{
			if (animationState == SSTUAnimState.EXTENDED || animationState == SSTUAnimState.EXTENDING)
			{
				playAnimationForward();
				setAnimationNormTime(1f);
				setAnimationState(SSTUAnimState.EXTENDED);
			}
			else if (animationState == SSTUAnimState.RETRACTING)
			{
				playAnimationReverse();
				setAnimationNormTime(0f);
				setAnimationState(SSTUAnimState.RETRACTED);
			}
		}
		initializeGuiLabels();
		updateGuiLabels();
	}

	public override void OnLoad(ConfigNode node)
	{
		base.OnLoad(node);
		MonoBehaviour.print("SSTUAnimate OnLoad");
		try
		{
			animationState = (SSTUAnimState)Enum.Parse(typeof(SSTUAnimState), deployedStatus);
		}
		catch (Exception ex)
		{
			Debug.LogError("[KSPI]: InterstellarAnimate in OnLoad: " + ex.Message);
			animationState = SSTUAnimState.RETRACTED;
		}
		updateGuiLabels();
	}

	public override void OnUpdate()
	{
		if (animationState == SSTUAnimState.EXTENDING && !isAnimationPlaying())
		{
			MonoBehaviour.print("Animation finished playing forwards");
			setAnimationState(SSTUAnimState.EXTENDED);
			updateGuiLabels();
		}
		else if (animationState == SSTUAnimState.RETRACTING && !isAnimationPlaying())
		{
			MonoBehaviour.print("Animation finished playing reverse");
			setAnimationState(SSTUAnimState.RETRACTED);
			updateGuiLabels();
		}
	}

	public SSTUAnimState getAnimationState()
	{
		return animationState;
	}

	private void toggle()
	{
		MonoBehaviour.print("SSTUAnimate toggle -- state: " + animationState);
		if (animationState == SSTUAnimState.EXTENDED || animationState == SSTUAnimState.EXTENDING)
		{
			playAnimationReverse();
			if (animationState == SSTUAnimState.EXTENDED)
			{
				setAnimationNormTime(1f);
			}
			setAnimationState(SSTUAnimState.RETRACTING);
			updateGuiLabels();
		}
		else
		{
			if (animationState != SSTUAnimState.RETRACTED && animationState != SSTUAnimState.RETRACTING)
			{
				return;
			}
			if (toggleShielded && base.part != null && base.part.ShieldedFromAirstream)
			{
				MonoBehaviour.print("Cannot deploy while shielded from airstream");
				return;
			}
			if (animationState == SSTUAnimState.RETRACTED)
			{
				setAnimationNormTime(0f);
			}
			playAnimationForward();
			setAnimationState(SSTUAnimState.EXTENDING);
			updateGuiLabels();
		}
	}

	private void updateGuiLabels()
	{
		MonoBehaviour.print("SSTUAnimate updateGuiLabels");
		if (animationState == SSTUAnimState.EXTENDED || animationState == SSTUAnimState.EXTENDING)
		{
			base.Events["deployEvent"].guiActiveEditor = false;
			base.Events["deployEvent"].guiActive = false;
			base.Events["retractEvent"].guiActiveEditor = editorEnabled;
			base.Events["retractEvent"].guiActive = flightEnabled;
		}
		else
		{
			base.Events["deployEvent"].guiActiveEditor = editorEnabled;
			base.Events["deployEvent"].guiActive = flightEnabled;
			base.Events["retractEvent"].guiActiveEditor = false;
			base.Events["retractEvent"].guiActive = false;
		}
	}

	private void playAnimationForward()
	{
		MonoBehaviour.print("SSTUAnimate playAnimationForward");
		setAnimationSpeed(1f);
		setAnimationEnabled(enabled: true);
		startAnimation();
	}

	private void playAnimationReverse()
	{
		MonoBehaviour.print("SSTUAnimate playAnimationReverse");
		setAnimationSpeed(-1f);
		setAnimationEnabled(enabled: true);
		startAnimation();
	}

	private void setAnimationState(SSTUAnimState newState)
	{
		MonoBehaviour.print("SSTUAnimate setAnimationState");
		animationState = newState;
		deployedStatus = animationState.ToString();
	}

	private void setAnimationSpeed(float speed)
	{
		MonoBehaviour.print("SSTUAnimate setAnimationSpeed");
		Animation[] array = deployAnimation;
		foreach (Animation animation in array)
		{
			animation[animationName].speed = speed;
		}
	}

	private void setAnimationNormTime(float time)
	{
		MonoBehaviour.print("SSTUAnimate setAnimationNormTime");
		Animation[] array = deployAnimation;
		foreach (Animation animation in array)
		{
			animation[animationName].normalizedTime = time;
		}
	}

	private void setAnimationEnabled(bool enabled)
	{
		MonoBehaviour.print("SSTUAnimate setAnimationEnabled");
		Animation[] array = deployAnimation;
		foreach (Animation animation in array)
		{
			animation[animationName].enabled = enabled;
		}
	}

	private void startAnimation()
	{
		MonoBehaviour.print("SSTUAnimate startAnimation");
		setAnimationEnabled(enabled: true);
		Animation[] array = deployAnimation;
		foreach (Animation animation in array)
		{
			animation.Play(animationName);
		}
	}

	private void stopAnimation()
	{
		MonoBehaviour.print("SSTUAnimate stopAnimation");
		setAnimationEnabled(enabled: false);
		Animation[] array = deployAnimation;
		foreach (Animation animation in array)
		{
			animation.Stop(animationName);
		}
	}

	private bool isAnimationPlaying()
	{
		bool result = false;
		Animation[] array = deployAnimation;
		foreach (Animation animation in array)
		{
			if (animation[animationName].enabled)
			{
				result = true;
			}
		}
		return result;
	}

	private void initializeAnimation()
	{
		MonoBehaviour.print("SSTUAnimate initializeAnimation");
		deployAnimation = base.part.FindModelAnimators(animationName);
		if (deployAnimation == null || deployAnimation.Length <= 0)
		{
			MonoBehaviour.print("Could not find or load animation for name: " + animationName);
			return;
		}
		setAnimationSpeed(1f);
		setAnimationNormTime(0f);
		Animation[] array = deployAnimation;
		foreach (Animation animation in array)
		{
			animation[animationName].layer = animationLayer;
			animation[animationName].blendMode = AnimationBlendMode.Blend;
			animation[animationName].wrapMode = WrapMode.Once;
		}
	}

	private void initializeGuiLabels()
	{
		MonoBehaviour.print("SSTUAnimate initializeGuiLabels");
		base.Actions["deployAction"].guiName = actionDeployName;
		base.Actions["retractAction"].guiName = actionRetractName;
		base.Actions["toggleAction"].guiName = actionToggleName;
		base.Events["deployEvent"].guiName = actionDeployName;
		base.Events["deployEvent"].guiActiveEditor = editorEnabled;
		base.Events["deployEvent"].guiActive = flightEnabled;
		base.Events["retractEvent"].guiName = actionRetractName;
		base.Events["retractEvent"].guiActiveEditor = editorEnabled;
		base.Events["retractEvent"].guiActive = flightEnabled;
		base.Fields["deployedStatus"].guiName = animStatusName;
		base.Fields["deployedStatus"].guiActive = showAnimState;
	}
}
