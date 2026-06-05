using System;
using System.Collections.Generic;
using System.Linq;
using KerbalEngineer.Extensions;
using KerbalEngineer.Flight.Readouts;
using KerbalEngineer.Flight.Readouts.Surface;
using KerbalEngineer.Flight.Sections;
using KerbalEngineer.Settings;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Flight;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public sealed class FlightEngineerCore : MonoBehaviour
{
	private static bool isCareerMode;

	private static bool isKerbalLimited;

	private static bool isTrackingStationLimited;

	private static bool switchVesselOnUpdate;

	private static Vessel switchVesselTarget;

	private static ITargetable switchVesselTargetTarget;

	public static int markerDeadman;

	public static bool gamePaused;

	public static FlightEngineerCore Instance { get; private set; }

	public static bool IsCareerMode
	{
		get
		{
			return isCareerMode;
		}
		set
		{
			try
			{
				if (isCareerMode != value)
				{
					SettingHandler settingHandler = SettingHandler.Load("FlightEngineerCore.xml");
					settingHandler.Set("isCareerMode", value);
					settingHandler.Save("FlightEngineerCore.xml");
				}
				isCareerMode = value;
			}
			catch (Exception ex)
			{
				MyLogger.Exception(ex);
			}
		}
	}

	public static bool IsDisplayable
	{
		get
		{
			if (!((Behaviour)MainCanvasUtil.MainCanvas).enabled)
			{
				return false;
			}
			if (isCareerMode && (Object)(object)FlightGlobals.ActiveVessel != (Object)null)
			{
				if (isKerbalLimited && FlightGlobals.ActiveVessel.GetVesselCrew().Exists((ProtoCrewMember c) => c.experienceTrait.TypeName == "Engineer"))
				{
					return true;
				}
				if (isTrackingStationLimited && ScenarioUpgradeableFacilities.GetFacilityLevel((SpaceCenterFacility)6) == 1f)
				{
					return true;
				}
				return FlightGlobals.ActiveVessel.parts.Any((Part p) => p.HasModule<FlightEngineerModule>());
			}
			return true;
		}
	}

	public static bool IsKerbalLimited
	{
		get
		{
			return isKerbalLimited;
		}
		set
		{
			try
			{
				if (isKerbalLimited != value)
				{
					SettingHandler settingHandler = SettingHandler.Load("FlightEngineerCore.xml");
					settingHandler.Set("isKerbalLimited", value);
					settingHandler.Save("FlightEngineerCore.xml");
				}
				isKerbalLimited = value;
			}
			catch (Exception ex)
			{
				MyLogger.Exception(ex);
			}
		}
	}

	public static bool IsTrackingStationLimited
	{
		get
		{
			return isTrackingStationLimited;
		}
		set
		{
			try
			{
				if (isTrackingStationLimited != value)
				{
					SettingHandler settingHandler = SettingHandler.Load("FlightEngineerCore.xml");
					settingHandler.Set("isTrackingStationLimited", value);
					settingHandler.Save("FlightEngineerCore.xml");
				}
				isTrackingStationLimited = value;
			}
			catch (Exception ex)
			{
				MyLogger.Exception(ex);
			}
		}
	}

	public List<SectionEditor> SectionEditors { get; private set; }

	public List<SectionWindow> SectionWindows { get; private set; }

	public List<IUpdatable> UpdatableModules { get; private set; }

	static FlightEngineerCore()
	{
		isCareerMode = true;
		isKerbalLimited = true;
		isTrackingStationLimited = true;
		switchVesselOnUpdate = false;
		switchVesselTarget = null;
		switchVesselTargetTarget = null;
		markerDeadman = 0;
		try
		{
			SettingHandler settingHandler = SettingHandler.Load("FlightEngineerCore.xml");
			settingHandler.Get("isCareerMode", ref isCareerMode);
			settingHandler.Get("isKerbalLimited", ref isKerbalLimited);
			settingHandler.Get("isTrackingStationLimited", ref isTrackingStationLimited);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	public static void SwitchToVessel(Vessel vessel, ITargetable target = null)
	{
		switchVesselTarget = vessel;
		switchVesselOnUpdate = true;
		switchVesselTargetTarget = target;
	}

	public SectionEditor AddSectionEditor(SectionModule section)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			SectionEditor sectionEditor = ((Component)this).gameObject.AddComponent<SectionEditor>();
			sectionEditor.ParentSection = section;
			sectionEditor.Position = new Rect(section.EditorPositionX, section.EditorPositionY, 500f, 500f);
			SectionEditors.Add(sectionEditor);
			ReadoutCategory.Selected = ReadoutCategory.GetCategory("Orbital");
			return sectionEditor;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
			return null;
		}
	}

	public SectionWindow AddSectionWindow(SectionModule section)
	{
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if ((Object)(object)((Component)this).gameObject == (Object)null)
			{
				return null;
			}
			SectionWindow sectionWindow = ((Component)this).gameObject.AddComponent<SectionWindow>();
			sectionWindow.ParentSection = section;
			sectionWindow.WindowPosition = new Rect(section.FloatingPositionX, section.FloatingPositionY, 0f, 0f);
			SectionWindows.Add(sectionWindow);
			return sectionWindow;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
			return null;
		}
	}

	public void AddUpdatable(IUpdatable updatable)
	{
		try
		{
			if (!UpdatableModules.Contains(updatable))
			{
				UpdatableModules.Add(updatable);
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void Awake()
	{
		try
		{
			Instance = this;
			SectionWindows = new List<SectionWindow>();
			SectionEditors = new List<SectionEditor>();
			UpdatableModules = new List<IUpdatable>();
			SimManager.UpdateModSettings();
			MyLogger.Log("FlightEngineerCore->Awake");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void FixedUpdate()
	{
		if ((Object)(object)FlightGlobals.ActiveVessel == (Object)null)
		{
			return;
		}
		try
		{
			SectionLibrary.FixedUpdate();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnDestroy()
	{
		try
		{
			SectionLibrary.Save();
			foreach (SectionWindow sectionWindow in SectionWindows)
			{
				MonoBehaviour.print((object)("[FlightEngineer]: Destroying Floating Window for " + sectionWindow.ParentSection.Name));
				Object.Destroy((Object)(object)sectionWindow);
			}
			foreach (SectionEditor sectionEditor in SectionEditors)
			{
				MonoBehaviour.print((object)("[FlightEngineer]: Destroying Editor Window for " + sectionEditor.ParentSection.Name));
				Object.Destroy((Object)(object)sectionEditor);
			}
			MyLogger.Log("FlightEngineerCore->OnDestroy");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnRenderObject()
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		if (((Object)Camera.current).name.Contains("00"))
		{
			ImpactProcessor.drawImpact(Color.red);
		}
		if (((Object)Camera.current).name.Contains("01"))
		{
			ImpactProcessor.drawImpact(Color.red);
		}
		if (MapView.MapIsEnabled && ((Object)Camera.current).name.Contains("UIVec"))
		{
			ImpactProcessor.drawImpact(Color.red);
		}
	}

	private void Start()
	{
		try
		{
			SectionLibrary.Load();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
		ReadoutLibrary.Reset();
		MyLogger.Log("FlightEngineerCore->Start");
	}

	private void Update()
	{
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Expected O, but got Unknown
		if (switchVesselOnUpdate)
		{
			Vessel val = switchVesselTarget;
			switchVesselTarget = null;
			switchVesselOnUpdate = false;
			if (switchVesselTargetTarget != null)
			{
				val.protoVessel.targetInfo = new ProtoTargetInfo(switchVesselTargetTarget);
				val.pTI = val.protoVessel.targetInfo;
				val.targetObject = switchVesselTargetTarget;
			}
			FlightGlobals.SetActiveVessel(val);
		}
		if ((Object)(object)FlightGlobals.ActiveVessel == (Object)null)
		{
			return;
		}
		try
		{
			markerDeadman--;
			if (markerDeadman <= 0)
			{
				markerDeadman = 0;
				ImpactProcessor.ShowMarker = false;
			}
			else
			{
				ImpactProcessor.ShowMarker = IsDisplayable;
			}
			SectionLibrary.Update();
			UpdateModules();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void UpdateModules()
	{
		try
		{
			foreach (IUpdatable updatableModule in UpdatableModules)
			{
				if (updatableModule is IUpdateRequest)
				{
					IUpdateRequest updateRequest = updatableModule as IUpdateRequest;
					if (updateRequest.UpdateRequested)
					{
						updatableModule.Update();
						updateRequest.UpdateRequested = false;
					}
				}
				else
				{
					updatableModule.Update();
				}
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void onGamePause()
	{
		gamePaused = true;
	}

	private void onGameUnpause()
	{
		gamePaused = false;
	}
}
