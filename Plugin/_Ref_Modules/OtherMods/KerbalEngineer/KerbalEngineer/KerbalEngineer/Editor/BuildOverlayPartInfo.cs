using System;
using System.Collections.Generic;
using KSP.UI.Screens;
using KerbalEngineer.Extensions;
using KerbalEngineer.Helpers;
using KerbalEngineer.KeyBinding;
using KerbalEngineer.Unity;
using UnityEngine;

namespace KerbalEngineer.Editor;

public class BuildOverlayPartInfo : MonoBehaviour
{
	private static bool clickToOpen = true;

	private static bool namesOnly;

	private static bool visible = true;

	private readonly List<PartInfoItem> infoItems = new List<PartInfoItem>();

	private Rect position;

	private Part selectedPart;

	private bool showInfo;

	private bool skipFrame;

	private PointerHoverDetector stageUiPointerHoverDetector;

	public static bool ClickToOpen
	{
		get
		{
			return clickToOpen;
		}
		set
		{
			clickToOpen = value;
		}
	}

	public static bool Hidden { get; set; }

	public static bool NamesOnly
	{
		get
		{
			return namesOnly;
		}
		set
		{
			namesOnly = value;
		}
	}

	public static bool Visible
	{
		get
		{
			return visible;
		}
		set
		{
			visible = value;
		}
	}

	protected void OnGUI()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Expected O, but got Unknown
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Visible && !Hidden && !((Object)(object)selectedPart == (Object)null) && (!EditorPanels.Instance.IsMouseOver() || IsPointerOverStaging()))
			{
				position = GUILayout.Window(((Object)this).GetInstanceID(), position, new WindowFunction(Window), string.Empty, BuildOverlay.WindowStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void Update()
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_0161: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_0139: Unknown result type (might be due to invalid IL or missing references)
		//IL_020a: Unknown result type (might be due to invalid IL or missing references)
		//IL_022e: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!Visible || Hidden || (Object)(object)EditorLogic.RootPart == (Object)null || (EditorPanels.Instance.IsMouseOver() && !IsPointerOverStaging()))
			{
				return;
			}
			((Rect)(ref position)).x = Mathf.Clamp(Input.mousePosition.x + 16f, 0f, (float)Screen.width - ((Rect)(ref position)).width);
			((Rect)(ref position)).y = Mathf.Clamp((float)Screen.height - Input.mousePosition.y, 0f, (float)Screen.height - ((Rect)(ref position)).height);
			if (((Rect)(ref position)).x < Input.mousePosition.x + 20f)
			{
				((Rect)(ref position)).y = Mathf.Clamp(((Rect)(ref position)).y + 20f, 0f, (float)Screen.height - ((Rect)(ref position)).height);
			}
			if (((Rect)(ref position)).x < Input.mousePosition.x + 16f && ((Rect)(ref position)).y < (float)Screen.height - Input.mousePosition.y)
			{
				((Rect)(ref position)).x = Input.mousePosition.x - 3f - ((Rect)(ref position)).width;
			}
			Part val = null;
			RaycastHit val2 = default(RaycastHit);
			val = ((!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), ref val2)) ? (EditorLogic.fetch.ship.parts.Find((Part p) => p.HighlightActive) ?? EditorLogic.SelectedPart) : ((Component)((RaycastHit)(ref val2)).transform).GetComponent<Part>());
			if ((Object)(object)val != (Object)null)
			{
				if (!((object)val).Equals((object)selectedPart))
				{
					selectedPart = val;
					ResetInfo();
				}
				if (NamesOnly || skipFrame)
				{
					skipFrame = false;
					return;
				}
				if (!showInfo && Input.GetKeyDown(KeyBinder.PartInfoShowHide))
				{
					showInfo = true;
				}
				else if (ClickToOpen && showInfo && Input.GetKeyDown(KeyBinder.PartInfoShowHide))
				{
					ResetInfo();
				}
				if (showInfo)
				{
					PartInfoItem.Release(infoItems);
					infoItems.Clear();
					SetCostInfo();
					SetMassItems();
					SetResourceItems();
					SetEngineInfo();
					SetAlternatorInfo();
					SetGimbalInfo();
					SetRcsInfo();
					SetParachuteInfo();
					SetSasInfo();
					SetReactionWheelInfo();
					SetSolarPanelInfo();
					SetGeneratorInfo();
					SetDecouplerInfo();
					SetTransmitterInfo();
					SetScienceExperimentInfo();
					SetScienceContainerInfo();
					SetSingleActivationInfo();
				}
			}
			else
			{
				selectedPart = null;
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private bool IsPointerOverStaging()
	{
		if ((Object)(object)stageUiPointerHoverDetector == (Object)null)
		{
			stageUiPointerHoverDetector = GameObjectExtension.AddOrGetComponent<PointerHoverDetector>(((Component)StageManager.Instance.scrollRect).gameObject);
		}
		if ((Object)(object)stageUiPointerHoverDetector != (Object)null)
		{
			return stageUiPointerHoverDetector.IsPointerHovering;
		}
		return false;
	}

	private void ResetInfo()
	{
		showInfo = !clickToOpen;
		skipFrame = true;
		((Rect)(ref position)).width = ((namesOnly || clickToOpen) ? 0f : 200f);
		((Rect)(ref position)).height = 0f;
	}

	private void SetAlternatorInfo()
	{
		ModuleAlternator module = selectedPart.GetModule<ModuleAlternator>();
		if ((Object)(object)module != (Object)null)
		{
			infoItems.Add(PartInfoItem.Create("Alternator"));
			for (int i = 0; i < ((PartModule)module).resHandler.outputResources.Count; i++)
			{
				ModuleResource val = ((PartModule)module).resHandler.outputResources[i];
				infoItems.Add(PartInfoItem.Create("\t" + val.name, val.rate.ToRate()));
			}
		}
	}

	private void SetCostInfo()
	{
		infoItems.Add(PartInfoItem.Create("Cost", Units.ConcatF(selectedPart.GetCostDry(), selectedPart.GetCostWet())));
	}

	private void SetDecouplerInfo()
	{
		PartExtensions.ProtoModuleDecoupler protoModuleDecoupler = selectedPart.GetProtoModuleDecoupler();
		if (protoModuleDecoupler != null)
		{
			infoItems.Add(PartInfoItem.Create("Ejection Force", protoModuleDecoupler.EjectionForce.ToForce()));
			if (protoModuleDecoupler.IsOmniDecoupler)
			{
				infoItems.Add(PartInfoItem.Create("Omni-directional"));
			}
		}
	}

	private void SetEngineInfo()
	{
		PartExtensions.ProtoModuleEngine protoModuleEngine = selectedPart.GetProtoModuleEngine();
		if (protoModuleEngine == null)
		{
			return;
		}
		infoItems.Add(PartInfoItem.Create("Thrust", Units.ToForce(protoModuleEngine.MinimumThrust, protoModuleEngine.MaximumThrust)));
		infoItems.Add(PartInfoItem.Create("Isp", Units.ConcatF(protoModuleEngine.GetSpecificImpulse(1f), protoModuleEngine.GetSpecificImpulse(0f)) + "s"));
		if (protoModuleEngine.Propellants.Count > 0)
		{
			infoItems.Add(PartInfoItem.Create("Propellants"));
			float num = 0f;
			for (int i = 0; i < protoModuleEngine.Propellants.Count; i++)
			{
				num += protoModuleEngine.Propellants[i].ratio;
			}
			for (int j = 0; j < protoModuleEngine.Propellants.Count; j++)
			{
				Propellant val = protoModuleEngine.Propellants[j];
				infoItems.Add(PartInfoItem.Create("\t" + val.name, (val.ratio / num).ToPercent()));
			}
		}
	}

	private void SetGeneratorInfo()
	{
		ModuleGenerator module = selectedPart.GetModule<ModuleGenerator>();
		if (!((Object)(object)module != (Object)null))
		{
			return;
		}
		if (((PartModule)module).resHandler.inputResources.Count > 0)
		{
			infoItems.Add(PartInfoItem.Create("Generator Input"));
			for (int i = 0; i < ((PartModule)module).resHandler.inputResources.Count; i++)
			{
				ModuleResource val = ((PartModule)module).resHandler.inputResources[i];
				infoItems.Add(PartInfoItem.Create("\t" + val.name, val.rate.ToRate()));
			}
		}
		if (((PartModule)module).resHandler.outputResources.Count > 0)
		{
			infoItems.Add(PartInfoItem.Create("Generator Output"));
			for (int j = 0; j < ((PartModule)module).resHandler.outputResources.Count; j++)
			{
				ModuleResource val2 = ((PartModule)module).resHandler.outputResources[j];
				infoItems.Add(PartInfoItem.Create("\t" + val2.name, val2.rate.ToRate()));
			}
		}
		if (module.isAlwaysActive)
		{
			infoItems.Add(PartInfoItem.Create("Generator is Always Active"));
		}
	}

	private void SetGimbalInfo()
	{
		ModuleGimbal module = selectedPart.GetModule<ModuleGimbal>();
		if ((Object)(object)module != (Object)null)
		{
			infoItems.Add(PartInfoItem.Create("Thrust Vectoring", module.gimbalRange.ToString("F2")));
		}
	}

	private void SetMassItems()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		if ((int)selectedPart.physicalSignificance == 0)
		{
			infoItems.Add(PartInfoItem.Create("Mass", Units.ToMass(selectedPart.GetDryMass(), selectedPart.GetWetMass())));
		}
	}

	private void SetParachuteInfo()
	{
		ModuleParachute module = selectedPart.GetModule<ModuleParachute>();
		if ((Object)(object)module != (Object)null)
		{
			infoItems.Add(PartInfoItem.Create("Deployed Drag", Units.ConcatF(module.semiDeployedDrag, module.fullyDeployedDrag)));
			infoItems.Add(PartInfoItem.Create("Deployment Altitude", module.deployAltitude.ToDistance()));
			infoItems.Add(PartInfoItem.Create("Deployment Pressure", module.minAirPressureToOpen.ToString("F2")));
		}
	}

	private void SetRcsInfo()
	{
		ModuleRCS module = selectedPart.GetModule<ModuleRCS>();
		if ((Object)(object)module != (Object)null)
		{
			infoItems.Add(PartInfoItem.Create("Thruster Power", module.thrusterPower.ToForce()));
			infoItems.Add(PartInfoItem.Create("Specific Impulse", Units.ConcatF(module.atmosphereCurve.Evaluate(1f), module.atmosphereCurve.Evaluate(0f)) + "s"));
		}
	}

	private void SetReactionWheelInfo()
	{
		ModuleReactionWheel module = selectedPart.GetModule<ModuleReactionWheel>();
		if ((Object)(object)module != (Object)null)
		{
			infoItems.Add(PartInfoItem.Create("Reaction Wheel Torque"));
			infoItems.Add(PartInfoItem.Create("\tPitch", module.PitchTorque.ToTorque()));
			infoItems.Add(PartInfoItem.Create("\tRoll", module.RollTorque.ToTorque()));
			infoItems.Add(PartInfoItem.Create("\tYaw", module.YawTorque.ToTorque()));
			for (int i = 0; i < ((PartModule)module).resHandler.inputResources.Count; i++)
			{
				ModuleResource val = ((PartModule)module).resHandler.inputResources[i];
				infoItems.Add(PartInfoItem.Create("\t" + val.name, val.rate.ToRate()));
			}
		}
	}

	private void SetResourceItems()
	{
		bool flag = false;
		for (int i = 0; i < selectedPart.Resources.dict.Count; i++)
		{
			if (!selectedPart.Resources.dict.At(i).hideFlow)
			{
				flag = true;
				break;
			}
		}
		if (!flag)
		{
			return;
		}
		infoItems.Add(PartInfoItem.Create("Resources"));
		for (int j = 0; j < selectedPart.Resources.dict.Count; j++)
		{
			PartResource val = selectedPart.Resources.dict.At(j);
			if (!val.hideFlow)
			{
				infoItems.Add((val.GetDensity() > 0.0) ? PartInfoItem.Create("\t" + val.info.name, "(" + val.GetMass().ToMass() + ") " + val.amount.ToString("F1")) : PartInfoItem.Create("\t" + val.info.name, val.amount.ToString("F1")));
			}
		}
	}

	private void SetSasInfo()
	{
		if (selectedPart.HasModule<ModuleSAS>())
		{
			infoItems.Add(PartInfoItem.Create("SAS Equiped"));
		}
	}

	private void SetScienceContainerInfo()
	{
		if (selectedPart.HasModule<ModuleScienceContainer>())
		{
			infoItems.Add(PartInfoItem.Create("Science Container"));
		}
	}

	private void SetScienceExperimentInfo()
	{
		ModuleScienceExperiment module = selectedPart.GetModule<ModuleScienceExperiment>();
		if ((Object)(object)module != (Object)null)
		{
			infoItems.Add(PartInfoItem.Create("Science Experiment", module.experimentActionName));
			infoItems.Add(PartInfoItem.Create("\tTransmit Efficiency", module.xmitDataScalar.ToPercent()));
			if (!module.rerunnable)
			{
				infoItems.Add(PartInfoItem.Create("\tSingle Usage"));
			}
		}
	}

	private void SetSingleActivationInfo()
	{
		if (selectedPart.HasModule<ModuleAnimateGeneric>((Func<ModuleAnimateGeneric, bool>)((ModuleAnimateGeneric m) => m.isOneShot)))
		{
			infoItems.Add(PartInfoItem.Create("Single Activation"));
		}
	}

	private void SetSolarPanelInfo()
	{
		ModuleDeployableSolarPanel module = selectedPart.GetModule<ModuleDeployableSolarPanel>();
		if ((Object)(object)module != (Object)null)
		{
			infoItems.Add(PartInfoItem.Create("Charge Rate", module.chargeRate.ToRate()));
			if (((ModuleDeployablePart)module).isBreakable)
			{
				infoItems.Add(PartInfoItem.Create("Breakable"));
			}
			if ((Object)(object)((ModuleDeployablePart)module).trackingBody == (Object)(object)Sun.Instance)
			{
				infoItems.Add(PartInfoItem.Create("Sun Tracking"));
			}
		}
	}

	private void SetTransmitterInfo()
	{
		ModuleDataTransmitter module = selectedPart.GetModule<ModuleDataTransmitter>();
		if ((Object)(object)module != (Object)null)
		{
			infoItems.Add(PartInfoItem.Create("Packet Size", module.packetSize.ToString("F2") + " Mits"));
			infoItems.Add(PartInfoItem.Create("Bandwidth", (module.packetInterval * module.packetSize).ToString("F2") + "Mits/sec"));
			infoItems.Add(PartInfoItem.Create(module.GetConsumedResources()[0].name, module.packetResourceCost.ToString("F2") + "/Packet"));
		}
	}

	private void Window(int windowId)
	{
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			GUILayout.Label(selectedPart.partInfo.title, BuildOverlay.TitleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			if (showInfo)
			{
				for (int i = 0; i < infoItems.Count; i++)
				{
					PartInfoItem partInfoItem = infoItems[i];
					GUILayout.Space(2f);
					GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
					if (partInfoItem.Value != null)
					{
						GUILayout.Label(partInfoItem.Name + ":", BuildOverlay.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
						GUILayout.Space(25f);
						GUILayout.Label(partInfoItem.Value, BuildOverlay.ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
					}
					else
					{
						GUILayout.Label(partInfoItem.Name, BuildOverlay.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
					}
					GUILayout.EndHorizontal();
				}
			}
			else if (clickToOpen && !namesOnly)
			{
				GUILayout.Space(2f);
				GUILayout.Label(string.Concat("Click [", KeyBinder.PartInfoShowHide, "] to show more info..."), BuildOverlay.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
