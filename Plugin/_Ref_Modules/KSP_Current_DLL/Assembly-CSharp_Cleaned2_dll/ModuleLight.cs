using System.Collections.Generic;
using Expansions.Missions.Adjusters;
using UnityEngine;
using ns9;

public class ModuleLight : PartModule, IResourceConsumer, IScalarModule
{
	public List<Light> lights;

	public List<float> brightnessLevels;

	[KSPField]
	public string lightName = "toggleableLight";

	[KSPField(isPersistant = true)]
	public bool isOn;

	[KSPField(isPersistant = true)]
	public bool uiWriteLock;

	[KSPField]
	public bool useResources;

	[KSPField]
	public string resourceName = "ElectricCharge";

	[KSPField]
	public string animationName = "LightAnimation";

	[KSPField]
	public float resourceAmount = 0.01f;

	[KSPField]
	public bool useAnimationDim;

	[KSPField]
	public bool useAutoDim;

	[KSPField]
	public float lightBrightenSpeed = 0.3f;

	[KSPField]
	public float lightDimSpeed = 0.8f;

	[KSPField(guiActive = true, guiName = "#autoLOC_6001401")]
	public string displayStatus = Localizer.Format("#autoLOC_219034");

	[KSPField]
	public string status = "Nominal";

	[UI_FloatRange(stepIncrement = 0.05f, maxValue = 1f, minValue = 0f)]
	[KSPAxisField(incrementalSpeed = 0.5f, isPersistant = true, axisMode = KSPAxisMode.Incremental, guiActive = false, guiActiveEditor = true, guiName = "#autoLOC_6001402")]
	public float lightR = 1f;

	[KSPAxisField(incrementalSpeed = 0.5f, isPersistant = true, axisMode = KSPAxisMode.Incremental, guiActive = false, guiActiveEditor = true, guiName = "#autoLOC_6001403")]
	[UI_FloatRange(stepIncrement = 0.05f, maxValue = 1f, minValue = 0f)]
	public float lightG = 1f;

	[KSPAxisField(incrementalSpeed = 0.5f, isPersistant = true, axisMode = KSPAxisMode.Incremental, guiActive = false, guiActiveEditor = true, guiName = "#autoLOC_6001404")]
	[UI_FloatRange(stepIncrement = 0.05f, maxValue = 1f, minValue = 0f)]
	public float lightB = 1f;

	private double recieved;

	private float currentLight;

	private float targetLight;

	private float resourceFraction;

	private Animation anim;

	private AnimationState animState;

	private bool isStarted;

	private Color lightColor;

	private Color lastColor;

	private List<PartResourceDefinition> consumedResources;

	[KSPField]
	public string moduleID = "lightModule";

	private EventData<float, float> OnMove = new EventData<float, float>("OnMove");

	private EventData<float> OnStopped = new EventData<float>("OnStop");

	private List<AdjusterLightBase> adjusterCache = new List<AdjusterLightBase>();

	private static string cacheAutoLOC_220477;

	private static string cacheAutoLOC_219034;

	private static string cacheAutoLOC_6003046;

	public string ScalarModuleID => moduleID;

	public float GetScalar
	{
		get
		{
			if (!isOn)
			{
				return 0f;
			}
			return 1f;
		}
	}

	public bool CanMove
	{
		get
		{
			if (!useResources)
			{
				return true;
			}
			return resourceFraction > 0.5f;
		}
	}

	public EventData<float, float> OnMoving => OnMove;

	public EventData<float> OnStop => OnStopped;

	[KSPAction("#autoLOC_6001405", KSPActionGroup.Light)]
	public void ToggleLightAction(KSPActionParam param)
	{
		ToggleLightAction(param.type);
	}

	public void ToggleLightAction(KSPActionType action)
	{
		if ((action == KSPActionType.Activate && !uiWriteLock) || (action == KSPActionType.Toggle && !isOn && !uiWriteLock))
		{
			SetLightState(state: true);
		}
		else
		{
			SetLightState(state: false);
		}
	}

	[KSPAction("#autoLOC_6001406")]
	public void LightOnAction(KSPActionParam param)
	{
		if (!uiWriteLock)
		{
			SetLightState(state: true);
		}
	}

	[KSPAction("#autoLOC_6001407")]
	public void LightOffAction(KSPActionParam param)
	{
		SetLightState(state: false);
	}

	[KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001408")]
	public void LightsOff()
	{
		SetLightState(state: false);
		int count = base.part.symmetryCounterparts.Count;
		while (count-- > 0)
		{
			if (base.part.symmetryCounterparts[count] != base.part)
			{
				base.part.symmetryCounterparts[count].Modules.GetModule<ModuleLight>().SetLightState(state: false);
			}
		}
	}

	[KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "#autoLOC_6001409")]
	public void LightsOn()
	{
		SetLightState(state: true);
		int count = base.part.symmetryCounterparts.Count;
		while (count-- > 0)
		{
			if (base.part.symmetryCounterparts[count] != base.part)
			{
				base.part.symmetryCounterparts[count].Modules.GetModule<ModuleLight>().SetLightState(state: true);
			}
		}
	}

	public void SetLightState(bool state)
	{
		if (state)
		{
			isOn = true;
			status = "Nominal";
			displayStatus = cacheAutoLOC_219034;
		}
		else
		{
			isOn = false;
			status = "Off";
			displayStatus = cacheAutoLOC_220477;
		}
		if (!useAnimationDim)
		{
			int i = 0;
			for (int count = lights.Count; i < count; i++)
			{
				Light light = lights[i];
				if (light != null)
				{
					light.enabled = state;
				}
			}
		}
		else
		{
			if (animState == null)
			{
				GetAnims();
			}
			if (HighLogic.LoadedSceneIsEditor && animState != null)
			{
				animState.normalizedTime = (state ? 1f : 0f);
				anim.Play(animationName);
				anim.Sample();
			}
		}
		base.Events["LightsOn"].active = !state;
		base.Events["LightsOff"].active = state;
	}

	public void UpdateLightColors()
	{
		int i = 0;
		for (int count = lights.Count; i < count; i++)
		{
			lights[i].color = lightColor;
		}
	}

	public List<PartResourceDefinition> GetConsumedResources()
	{
		return consumedResources;
	}

	public override void OnAwake()
	{
		if (useResources)
		{
			if (consumedResources == null)
			{
				consumedResources = new List<PartResourceDefinition>();
			}
			else
			{
				consumedResources.Clear();
			}
			int i = 0;
			for (int count = resHandler.inputResources.Count; i < count; i++)
			{
				consumedResources.Add(PartResourceLibrary.Instance.GetDefinition(resHandler.inputResources[i].name));
			}
		}
		else if (consumedResources == null)
		{
			consumedResources = new List<PartResourceDefinition>();
		}
		else
		{
			consumedResources.Clear();
		}
	}

	public override void OnStart(StartState state)
	{
		lights = new List<Light>(base.part.FindModelComponents<Light>(lightName));
		brightnessLevels = new List<float>();
		lightColor = new Color(lightR, lightG, lightB);
		if (useAnimationDim)
		{
			GetAnims();
		}
		int i = 0;
		for (int count = lights.Count; i < count; i++)
		{
			Light light = lights[i];
			brightnessLevels.Add(light.intensity);
			light.color = lightColor;
		}
		if (isOn)
		{
			LightsOn();
		}
		else
		{
			LightsOff();
		}
		if (HighLogic.LoadedSceneIsFlight)
		{
			isStarted = true;
			if (base.vessel.situation == Vessel.Situations.PRELAUNCH && isOn)
			{
				base.vessel.ActionGroups[KSPActionGroup.Light] = true;
			}
		}
	}

	public override void OnLoad(ConfigNode node)
	{
		if (resHandler.inputResources.Count == 0)
		{
			ModuleResource moduleResource = new ModuleResource();
			moduleResource.name = resourceName;
			moduleResource.title = KSPUtil.PrintModuleName(resourceName);
			moduleResource.id = resourceName.GetHashCode();
			moduleResource.rate = resourceAmount;
			resHandler.inputResources.Add(moduleResource);
		}
	}

	private void GetAnims()
	{
		List<Animation> list = new List<Animation>(base.part.FindModelComponents<Animation>());
		int i = 0;
		for (int count = list.Count; i < count; i++)
		{
			Animation animation = list[i];
			if (animation[animationName] != null)
			{
				animState = animation[animationName];
				anim = animation;
				break;
			}
		}
		if (animState != null)
		{
			animState.wrapMode = WrapMode.ClampForever;
			animState.normalizedSpeed = 0f;
			animState.normalizedTime = 0f;
			if (isOn)
			{
				animState.normalizedTime = 0f;
			}
			anim.Play(animationName);
		}
	}

	public override string GetInfo()
	{
		string text = "";
		if (useResources)
		{
			text += resHandler.PrintModuleResources();
		}
		return text;
	}

	public void FixedUpdate()
	{
		lightColor.r = lightR;
		lightColor.g = lightG;
		lightColor.b = lightB;
		if (lastColor != lightColor)
		{
			UpdateLightColors();
		}
		lastColor = lightColor;
		if (HighLogic.LoadedSceneIsEditor || !isStarted)
		{
			return;
		}
		float fixedDeltaTime = TimeWarp.fixedDeltaTime;
		if (isOn && useResources)
		{
			resourceFraction = (float)resHandler.UpdateModuleResourceInputs(ref status, 1.0, 0.99, returnOnFirstLack: false, average: false, stringOps: true);
			if (resourceFraction >= 0.99f)
			{
				if (status != "Nominal")
				{
					status = "Nominal";
					displayStatus = cacheAutoLOC_219034;
				}
			}
			else
			{
				SetLightState(state: false);
			}
			resourceFraction = ApplyIntensityAdjustments(resourceFraction);
			if (useAutoDim)
			{
				int i = 0;
				for (int count = lights.Count; i < count; i++)
				{
					targetLight = brightnessLevels[i] * resourceFraction;
					if (currentLight < targetLight)
					{
						currentLight = Mathf.Lerp(currentLight, targetLight, lightBrightenSpeed * fixedDeltaTime);
					}
					else
					{
						currentLight = Mathf.Lerp(currentLight, targetLight, lightDimSpeed * fixedDeltaTime);
					}
					lights[i].intensity = currentLight;
				}
			}
			if (useAnimationDim)
			{
				targetLight = resourceFraction;
				currentLight = Mathf.Lerp(currentLight, targetLight, lightBrightenSpeed * fixedDeltaTime);
				if (animState != null)
				{
					animState.normalizedTime = currentLight;
				}
			}
		}
		else if (useAnimationDim)
		{
			targetLight = 0f;
			currentLight = Mathf.Lerp(currentLight, targetLight, lightDimSpeed * fixedDeltaTime);
			if (animState != null)
			{
				animState.normalizedTime = currentLight;
			}
		}
	}

	public void SetScalar(float t)
	{
		if (t > 0.5f)
		{
			if (!isOn)
			{
				LightsOn();
			}
		}
		else if (t <= 0.5f && isOn)
		{
			LightsOff();
		}
	}

	public void SetUIRead(bool state)
	{
	}

	public void SetUIWrite(bool state)
	{
		uiWriteLock = !state;
		if (state)
		{
			base.Events["LightsOn"].active = !isOn;
			base.Events["LightsOff"].active = isOn;
		}
		else
		{
			base.Events["LightsOn"].active = false;
			base.Events["LightsOff"].active = false;
		}
	}

	public bool IsMoving()
	{
		return false;
	}

	protected override void OnModuleAdjusterAdded(AdjusterPartModuleBase adjuster)
	{
		if (adjuster is AdjusterLightBase item)
		{
			adjusterCache.Add(item);
		}
		base.OnModuleAdjusterAdded(adjuster);
	}

	public override void OnModuleAdjusterRemoved(AdjusterPartModuleBase adjuster)
	{
		AdjusterLightBase item = adjuster as AdjusterLightBase;
		adjusterCache.Remove(item);
		base.OnModuleAdjusterRemoved(adjuster);
	}

	protected float ApplyIntensityAdjustments(float intensity)
	{
		for (int i = 0; i < adjusterCache.Count; i++)
		{
			intensity = adjusterCache[i].ApplyIntensityAdjustment(intensity);
		}
		return intensity;
	}

	public override string GetModuleDisplayName()
	{
		return cacheAutoLOC_6003046;
	}

	internal static void CacheLocalStrings()
	{
		cacheAutoLOC_220477 = Localizer.Format("#autoLOC_220477");
		cacheAutoLOC_219034 = Localizer.Format("#autoLOC_219034");
		cacheAutoLOC_6003046 = Localizer.Format("#autoLoc_6003046");
	}
}
