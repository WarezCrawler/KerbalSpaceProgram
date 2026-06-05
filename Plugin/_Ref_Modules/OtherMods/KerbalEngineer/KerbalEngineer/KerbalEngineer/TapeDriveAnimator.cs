using System;
using UnityEngine;

namespace KerbalEngineer;

public class TapeDriveAnimator : PartModule
{
	[KSPField]
	public string Lights1 = string.Empty;

	[KSPField]
	public float Lights1Speed;

	[KSPField]
	public string Lights2 = string.Empty;

	[KSPField]
	public float Lights2Speed;

	[KSPField]
	public string Lights3 = string.Empty;

	[KSPField]
	public float Lights3Speed;

	[KSPField]
	public string Lights4 = string.Empty;

	[KSPField]
	public float Lights4Speed;

	[KSPField]
	public string Lights5 = string.Empty;

	[KSPField]
	public float Lights5Speed;

	[KSPField]
	public string Lights6 = string.Empty;

	[KSPField]
	public float Lights6Speed;

	[KSPField]
	public int MaxReelSpeed;

	[KSPField]
	public int MaxRepeatTime;

	[KSPField]
	public int MinReelSpeed;

	[KSPField]
	public int MinRepeatTime;

	[KSPField]
	public string Reel1 = string.Empty;

	[KSPField]
	public float Reel1SpeedRatio = 1f;

	[KSPField]
	public string Reel2 = string.Empty;

	[KSPField]
	public float Reel2SpeedRatio = 1f;

	[KSPField]
	public float RepeatTimeDenominator = 1f;

	[KSPField]
	public float SpeedChangeAmount;

	[KSPField]
	public float SpeedDeadZone;

	[KSPField]
	public float SpeedStopZone;

	[KSPField]
	public bool UseBakedAnimation;

	private Shader buttonLightOffShader;

	private Shader buttonLightOnShader;

	private Material buttonSet1Material;

	private Material buttonSet2Material;

	private Material buttonSet3Material;

	private Material buttonSet4Material;

	private Material buttonSet5Material;

	private Material buttonSet6Material;

	private float currentTime;

	private float deltaTime;

	private bool isRunning;

	private Random random;

	private Transform reel1Transform;

	private Transform reel2Transform;

	private float repeatTime;

	private bool sceneIsEditor;

	private float speed;

	private float targetSpeed;

	public bool IsRunning
	{
		get
		{
			return isRunning;
		}
		set
		{
			isRunning = value;
			if (UseBakedAnimation)
			{
				if (isRunning)
				{
					StartBakedAnimation();
				}
				else
				{
					StopBakedAnimation();
				}
			}
		}
	}

	public override void OnStart(StartState state)
	{
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Expected O, but got Unknown
		random = new Random();
		StopBakedAnimation();
		IsRunning = false;
		if (HighLogic.LoadedSceneIsEditor)
		{
			Part part = ((PartModule)this).part;
			part.OnEditorAttach = (Callback)Delegate.Combine((Delegate)(object)part.OnEditorAttach, (Delegate)new Callback(OnEditorAttach));
			Part part2 = ((PartModule)this).part;
			part2.OnEditorDetach = (Callback)Delegate.Combine((Delegate)(object)part2.OnEditorDetach, (Delegate)new Callback(OnEditorDetach));
			sceneIsEditor = true;
			if ((Object)(object)((PartModule)this).part.parent != (Object)null)
			{
				IsRunning = true;
			}
		}
		else if (HighLogic.LoadedSceneIsFlight)
		{
			IsRunning = true;
		}
		if (!UseBakedAnimation)
		{
			InitialiseReels();
			InitialiseLights();
		}
	}

	public override void OnUpdate()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Invalid comparison between Unknown and I4
		if (UseBakedAnimation)
		{
			return;
		}
		deltaTime = (sceneIsEditor ? Time.deltaTime : TimeWarp.deltaTime);
		if (TimeWarp.CurrentRate != 1f && (int)TimeWarp.WarpMode != 1)
		{
			return;
		}
		if (IsRunning)
		{
			UpdateTimerCycle();
			UpdateSpeed();
			UpdateReels();
			UpdateLights();
			return;
		}
		targetSpeed = 0f;
		if (speed != 0f)
		{
			UpdateSpeed();
			UpdateReels();
			UpdateLights();
		}
	}

	private static void SetShaderOnMaterial(Material material, Shader shader)
	{
		if ((Object)(object)material != (Object)null && (Object)(object)shader != (Object)null)
		{
			material.shader = shader;
		}
	}

	private Material GetMaterialOnModelTransform(string transformName)
	{
		Transform modelTransform = GetModelTransform(transformName);
		if ((Object)(object)modelTransform != (Object)null)
		{
			Renderer component = ((Component)modelTransform).GetComponent<Renderer>();
			if ((Object)(object)component != (Object)null)
			{
				return component.material;
			}
		}
		return null;
	}

	private Transform GetModelTransform(string transformName)
	{
		if (!string.IsNullOrEmpty(transformName))
		{
			return ((PartModule)this).part.FindModelTransform(transformName);
		}
		return null;
	}

	private void InitialiseLights()
	{
		buttonSet1Material = GetMaterialOnModelTransform(Lights1);
		buttonSet2Material = GetMaterialOnModelTransform(Lights2);
		buttonSet3Material = GetMaterialOnModelTransform(Lights3);
		buttonSet4Material = GetMaterialOnModelTransform(Lights4);
		buttonSet5Material = GetMaterialOnModelTransform(Lights5);
		buttonSet6Material = GetMaterialOnModelTransform(Lights6);
		buttonLightOffShader = Shader.Find("KSP/Specular");
		buttonLightOnShader = Shader.Find("KSP/Unlit");
	}

	private void InitialiseReels()
	{
		if (!string.IsNullOrEmpty(Reel1))
		{
			reel1Transform = ((PartModule)this).part.FindModelTransform(Reel1);
		}
		if (!string.IsNullOrEmpty(Reel2))
		{
			reel2Transform = ((PartModule)this).part.FindModelTransform(Reel2);
		}
	}

	private void OnEditorAttach()
	{
		IsRunning = true;
	}

	private void OnEditorDetach()
	{
		IsRunning = false;
	}

	private void StartBakedAnimation()
	{
		Animation[] array = ((PartModule)this).part.FindModelAnimators();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Play();
		}
	}

	private void StopBakedAnimation()
	{
		Animation[] array = ((PartModule)this).part.FindModelAnimators();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Stop();
		}
	}

	private void Update()
	{
		if (sceneIsEditor)
		{
			((PartModule)this).OnUpdate();
		}
	}

	private void UpdateButtonMaterial(Material material, float targetSpeed)
	{
		if (!((Object)(object)material == (Object)null))
		{
			bool flag = ((targetSpeed > 0f) ? (speed > targetSpeed) : ((!(targetSpeed < 0f)) ? (speed == 0f) : (speed < targetSpeed)));
			SetShaderOnMaterial(material, flag ? buttonLightOnShader : buttonLightOffShader);
		}
	}

	private void UpdateLights()
	{
		UpdateButtonMaterial(buttonSet1Material, Lights1Speed);
		UpdateButtonMaterial(buttonSet2Material, Lights2Speed);
		UpdateButtonMaterial(buttonSet3Material, Lights3Speed);
		UpdateButtonMaterial(buttonSet4Material, Lights4Speed);
		UpdateButtonMaterial(buttonSet5Material, Lights5Speed);
		UpdateButtonMaterial(buttonSet6Material, Lights6Speed);
	}

	private void UpdateReels()
	{
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)reel1Transform != (Object)null && speed != 0f)
		{
			((Component)reel1Transform).transform.Rotate(Vector3.right, speed * Reel1SpeedRatio);
		}
		if ((Object)(object)reel2Transform != (Object)null && speed != 0f)
		{
			((Component)reel2Transform).transform.Rotate(Vector3.right, speed * Reel2SpeedRatio);
		}
	}

	private void UpdateSpeed()
	{
		if (speed < targetSpeed)
		{
			if (speed < targetSpeed - SpeedDeadZone)
			{
				speed += SpeedChangeAmount * deltaTime;
			}
			else
			{
				speed = targetSpeed;
			}
		}
		else if (speed > targetSpeed)
		{
			if (speed > targetSpeed + SpeedDeadZone)
			{
				speed -= SpeedChangeAmount * deltaTime;
			}
			else
			{
				speed = targetSpeed;
			}
		}
	}

	private void UpdateTimerCycle()
	{
		currentTime += deltaTime;
		if (currentTime >= repeatTime)
		{
			targetSpeed = random.Next(MinReelSpeed, MaxReelSpeed);
			if (targetSpeed > 0f - SpeedStopZone && targetSpeed < SpeedStopZone)
			{
				targetSpeed = 0f;
			}
			repeatTime = random.Next(MinRepeatTime, MaxRepeatTime);
			if (RepeatTimeDenominator != 0f)
			{
				repeatTime /= RepeatTimeDenominator;
			}
			currentTime -= repeatTime;
		}
	}
}
