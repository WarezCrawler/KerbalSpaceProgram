using System;
using KerbalEngineer.Extensions;
using KerbalEngineer.Flight;
using KerbalEngineer.Helpers;
using KerbalEngineer.KeyBinding;
using KerbalEngineer.Settings;
using KerbalEngineer.UIControls;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Editor;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class BuildAdvanced : MonoBehaviour
{
	public static float Altitude;

	private static Rect compactModeRect = new Rect(0f, 5f, 0f, 20f);

	private static Stage stage;

	private static int stagesCount;

	private static int stagesLength;

	private static string title;

	private GUIStyle areaSettingStyle;

	private GUIStyle areaStyle;

	private float atmosphericMach;

	private GUIStyle bodiesButtonActiveStyle;

	private GUIStyle bodiesButtonStyle;

	private PopOutElement bodiesList;

	private Rect bodiesListPosition;

	private GUIStyle buttonStyle;

	private int compactCheck;

	private bool compactCollapseRight;

	private bool compactMode;

	private float compactRight;

	private bool hasChanged;

	private GUIStyle infoStyle;

	private bool isEditorLocked;

	private float maxMach;

	private int numberOfStages;

	private Rect position = new Rect(265f, 45f, 0f, 0f);

	private GUIStyle settingAtmoStyle;

	private GUIStyle settingStyle;

	private bool showAllStages;

	private bool showAtmosphericDetails;

	private bool showRCS;

	private bool showSettings;

	private Stage[] stages;

	private GUIStyle titleStyle;

	private bool visible = true;

	private GUIStyle windowStyle;

	public static BuildAdvanced Instance { get; private set; }

	public bool CompactMode
	{
		get
		{
			return compactMode;
		}
		set
		{
			compactMode = value;
		}
	}

	public bool ShowAllStages
	{
		get
		{
			return showAllStages;
		}
		set
		{
			showAllStages = value;
		}
	}

	public bool ShowAtmosphericDetails
	{
		get
		{
			return showAtmosphericDetails;
		}
		set
		{
			showAtmosphericDetails = value;
		}
	}

	public bool ShowSettings
	{
		get
		{
			return showSettings;
		}
		set
		{
			showSettings = value;
		}
	}

	public bool Visible
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

	protected void Awake()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Expected O, but got Unknown
		try
		{
			Instance = this;
			bodiesList = ((Component)this).gameObject.AddComponent<PopOutElement>();
			bodiesList.DrawCallback = new Callback(DrawBodiesList);
			Load();
			SimManager.UpdateModSettings();
			SimManager.OnReady -= GetStageInfo;
			SimManager.OnReady += GetStageInfo;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "BuildAdvanced.Awake()");
		}
	}

	protected void OnDestroy()
	{
		MyLogger.Log("BuildAdvanced->OnDestroy");
		try
		{
			SettingHandler settingHandler = new SettingHandler();
			settingHandler.Set("visible", visible);
			settingHandler.Set("windowPositionX", ((Rect)(ref position)).x);
			settingHandler.Set("windowPositionY", ((Rect)(ref position)).y);
			settingHandler.Set("compactMode", compactMode);
			settingHandler.Set("compactCollapseRight", compactCollapseRight);
			settingHandler.Set("showAllStages", showAllStages);
			settingHandler.Set("showAtmosphericDetails", showAtmosphericDetails);
			settingHandler.Set("showSettings", showSettings);
			settingHandler.Set("selectedBodyName", CelestialBodies.SelectedBody.Name);
			settingHandler.Save("BuildAdvanced.xml");
			GuiDisplaySize.OnSizeChanged -= OnSizeChanged;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "BuildAdvanced.OnDestroy()");
		}
		EditorLock(state: false);
	}

	protected void OnGUI()
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0133: Expected O, but got Unknown
		//IL_012e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0138: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (!visible || (Object)(object)EditorLogic.fetch == (Object)null || EditorLogic.fetch.ship.parts.Count == 0 || stages == null)
			{
				return;
			}
			title = ((!compactMode) ? "KERBAL ENGINEER REDUX 1.1.7.1" : "K.E.R. 1.1.7.1");
			stagesLength = stages.Length;
			if (showAllStages)
			{
				stagesCount = stagesLength;
			}
			if (!showAllStages)
			{
				stagesCount = 0;
				for (int i = 0; i < stagesLength; i++)
				{
					if (stages[i].deltaV > 0.0)
					{
						stagesCount++;
					}
				}
			}
			if (hasChanged || stagesCount != numberOfStages)
			{
				hasChanged = false;
				numberOfStages = stagesCount;
				((Rect)(ref position)).width = 0f;
				((Rect)(ref position)).height = 0f;
			}
			GUI.skin = null;
			position = GUILayout.Window(((Object)this).GetInstanceID(), position, new WindowFunction(Window), title, windowStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]).ClampToScreen();
			if (compactCheck > 0 && compactCollapseRight)
			{
				((Rect)(ref position)).x = compactRight - ((Rect)(ref position)).width;
				compactCheck--;
			}
			else if (compactCheck > 0)
			{
				compactCheck = 0;
			}
			CheckEditorLock();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "BuildAdvanced.OnGUI()");
		}
	}

	protected void Start()
	{
		try
		{
			InitialiseStyles();
			GuiDisplaySize.OnSizeChanged += OnSizeChanged;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "BuildAdvanced.Start()");
		}
	}

	protected void Update()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Input.GetKeyDown(KeyBinder.EditorShowHide))
			{
				visible = !visible;
				if (!visible)
				{
					EditorLock(state: false);
				}
			}
			if (!visible || (Object)(object)EditorLogic.fetch == (Object)null || EditorLogic.fetch.ship.parts.Count == 0)
			{
				((Behaviour)bodiesList).enabled = false;
				return;
			}
			SimManager.Gravity = CelestialBodies.SelectedBody.Gravity;
			if (showAtmosphericDetails)
			{
				SimManager.Atmosphere = CelestialBodies.SelectedBody.GetAtmospheres(Altitude);
			}
			else
			{
				SimManager.Atmosphere = 0.0;
			}
			SimManager.Mach = atmosphericMach;
			SimManager.RequestSimulation();
			SimManager.TryStartSimulation();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "BuildAdvanced.Update()");
		}
	}

	private void CheckEditorLock()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		if ((position.MouseIsOver() || bodiesList.Position.MouseIsOver()) && !isEditorLocked)
		{
			EditorLock(state: true);
		}
		else if (!position.MouseIsOver() && !bodiesList.Position.MouseIsOver() && isEditorLocked)
		{
			EditorLock(state: false);
		}
	}

	private void DrawAtmosphericDetails()
	{
		try
		{
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUILayout.Label("Altitude: " + (Altitude * 0.001f).ToString("F1") + "km", settingAtmoStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(125f * GuiDisplaySize.Offset) });
			GUI.skin = HighLogic.Skin;
			Altitude = GUILayout.HorizontalSlider(Altitude, 0f, (float)CelestialBodies.SelectedBody.CelestialBody.atmosphereDepth, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUI.skin = null;
			GUILayout.EndVertical();
			GUILayout.Space(5f);
			GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUILayout.Label("Mach: " + atmosphericMach.ToString("F2"), settingAtmoStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(125f * GuiDisplaySize.Offset) });
			GUI.skin = HighLogic.Skin;
			atmosphericMach = GUILayout.HorizontalSlider(Mathf.Clamp(atmosphericMach, 0f, maxMach), 0f, maxMach, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			GUI.skin = null;
			GUILayout.EndVertical();
			GUILayout.EndHorizontal();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "BuildAdvanced.DrawAtmosphericDetails()");
		}
	}

	private void DrawBodiesList()
	{
		if (CelestialBodies.SystemBody == CelestialBodies.SelectedBody)
		{
			DrawBody(CelestialBodies.SystemBody);
			return;
		}
		foreach (CelestialBodies.BodyInfo child in CelestialBodies.SystemBody.Children)
		{
			DrawBody(child);
		}
	}

	private void DrawBody(CelestialBodies.BodyInfo bodyInfo, int depth = 0)
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Space(20f * (float)depth);
		if (GUILayout.Button((bodyInfo.Children.Count > 0) ? (bodyInfo.Name + " [" + bodyInfo.Children.Count + "]") : bodyInfo.Name, (bodyInfo.Selected && bodyInfo.SelectedDepth == 0) ? bodiesButtonActiveStyle : bodiesButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			CelestialBodies.SetSelectedBody(bodyInfo.Name);
			Altitude = 0f;
			bodiesList.Resize = true;
		}
		GUILayout.EndHorizontal();
		if (bodyInfo.Selected)
		{
			for (int i = 0; i < bodyInfo.Children.Count; i++)
			{
				DrawBody(bodyInfo.Children[i], depth + 1);
			}
		}
	}

	private void DrawBurnTime()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(75f * GuiDisplaySize.Offset) });
		GUILayout.Label("BURN", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(TimeFormatter.ConvertToString(stage.time), infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawCost()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(110f * GuiDisplaySize.Offset) });
		GUILayout.Label("COST", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(Units.Cost(stage.cost, stage.totalCost), infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawDeltaV()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		GUILayout.Label("DELTA-V", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(stage.deltaV.ToString("N0") + " / " + stage.inverseTotalDeltaV.ToString("N0") + "m/s", infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawIsp()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(75f * GuiDisplaySize.Offset) });
		GUILayout.Label("ISP", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(stage.isp.ToString("F1") + "s", infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawMass()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(110f * GuiDisplaySize.Offset) });
		GUILayout.Label("MASS", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(Units.ToMass(stage.mass, stage.totalMass), infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawPartCount()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f * GuiDisplaySize.Offset) });
		GUILayout.Label("PARTS", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(stage.partCount + " / " + stage.totalPartCount, infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawSettings()
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Compact mode collapses to the:", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		compactCollapseRight = !GUILayout.Toggle(!compactCollapseRight, "LEFT", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		compactCollapseRight = GUILayout.Toggle(compactCollapseRight, "RIGHT", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Simulate using vectored thrust values:", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		SimManager.vectoredThrust = GUILayout.Toggle(SimManager.vectoredThrust, "ENABLED", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Verbose Simulation Log:", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		SimManager.logOutput = GUILayout.Toggle(SimManager.logOutput, "ENABLED", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Build Engineer Overlay:", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		BuildOverlay.Visible = GUILayout.Toggle(BuildOverlay.Visible, "VISIBLE", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		BuildOverlayPartInfo.NamesOnly = GUILayout.Toggle(BuildOverlayPartInfo.NamesOnly, "NAMES ONLY", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		BuildOverlayPartInfo.ClickToOpen = GUILayout.Toggle(BuildOverlayPartInfo.ClickToOpen, "CLICK TO OPEN", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Flight Engineer activation mode:", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		FlightEngineerCore.IsCareerMode = GUILayout.Toggle(FlightEngineerCore.IsCareerMode, "CAREER", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		FlightEngineerCore.IsCareerMode = !GUILayout.Toggle(!FlightEngineerCore.IsCareerMode, "PARTLESS", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Flight Engineer Career Limitations:", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		FlightEngineerCore.IsKerbalLimited = GUILayout.Toggle(FlightEngineerCore.IsKerbalLimited, "KERBAL", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		FlightEngineerCore.IsTrackingStationLimited = GUILayout.Toggle(FlightEngineerCore.IsTrackingStationLimited, "TRACKING", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Flight Engineer Toolbar Icon:", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		FlightAppLauncher.IsHoverActivated = GUILayout.Toggle(FlightAppLauncher.IsHoverActivated, "MOUSE HOVER", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(125f * GuiDisplaySize.Offset) });
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Key Bindings:", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("EDIT KEY BINDINGS", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(200f * GuiDisplaySize.Offset) }))
		{
			KeyBinder.Show();
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("GUI Size: " + GuiDisplaySize.Increment, settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("<", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) }))
		{
			GuiDisplaySize.Increment--;
		}
		if (GUILayout.Button(">", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) }))
		{
			GuiDisplaySize.Increment++;
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("VAB Overlay X Pos.: " + BuildOverlay.BuildOverlayVessel.WindowX, settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("<<", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f * GuiDisplaySize.Offset) }))
		{
			BuildOverlay.BuildOverlayVessel.WindowX -= 10f;
		}
		if (GUILayout.Button("<", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f * GuiDisplaySize.Offset) }))
		{
			BuildOverlay.BuildOverlayVessel.WindowX--;
		}
		if (GUILayout.Button(">", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f * GuiDisplaySize.Offset) }))
		{
			BuildOverlay.BuildOverlayVessel.WindowX++;
		}
		if (GUILayout.Button(">>", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f * GuiDisplaySize.Offset) }))
		{
			BuildOverlay.BuildOverlayVessel.WindowX += 10f;
		}
		GUILayout.EndHorizontal();
		GUILayout.Label("Minimum delay between simulations: " + SimManager.minSimTime.TotalMilliseconds + "ms", settingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUI.skin = HighLogic.Skin;
		SimManager.minSimTime = TimeSpan.FromMilliseconds(GUILayout.HorizontalSlider((float)SimManager.minSimTime.TotalMilliseconds, 0f, 2000f, (GUILayoutOption[])(object)new GUILayoutOption[0]));
		GUI.skin = null;
	}

	private void DrawStageNumbers()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(30f * GuiDisplaySize.Offset) });
		GUILayout.Label(string.Empty, titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label("S" + stage.number, titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawThrust()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(75f * GuiDisplaySize.Offset) });
		GUILayout.Label("THRUST", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(stage.thrust.ToForce(), infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawTorque()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(75f * GuiDisplaySize.Offset) });
		GUILayout.Label("TORQUE", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(stage.maxThrustTorque.ToTorque(), infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawTwr()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f * GuiDisplaySize.Offset) });
		GUILayout.Label("TWR (MAX)", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(stage.thrustToWeight.ToString("F2") + " (" + stage.maxThrustToWeight.ToString("F2") + ")", infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawRCS()
	{
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(85f * GuiDisplaySize.Offset) });
		GUILayout.Label("RCS ISP", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int i = 0; i < stagesLength; i++)
		{
			stage = stages[i];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(stage.RCSIsp.ToString("F2") + "s", infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(85f * GuiDisplaySize.Offset) });
		GUILayout.Label("RCS THRUST", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int j = 0; j < stagesLength; j++)
		{
			stage = stages[j];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(Units.ToForce(stage.RCSThrust), infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(105f * GuiDisplaySize.Offset) });
		GUILayout.Label("RCS TWR (MAX)", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int k = 0; k < stagesLength; k++)
		{
			stage = stages[k];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(stage.RCSTWRStart.ToString("F2") + " (" + stage.RCSTWREnd.ToString("F2") + ")", infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(120f * GuiDisplaySize.Offset) });
		GUILayout.Label("RCS DELTA-V (MAX)", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int l = 0; l < stagesLength; l++)
		{
			stage = stages[l];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(Units.ToSpeed(stage.RCSdeltaVStart, 0) + " (" + Units.ToSpeed(stage.RCSdeltaVEnd, 0) + ")", infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
		GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(105f * GuiDisplaySize.Offset) });
		GUILayout.Label("RCS BURN TIME", titleStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		for (int m = 0; m < stagesLength; m++)
		{
			stage = stages[m];
			if (showAllStages || stage.deltaV > 0.0)
			{
				GUILayout.Label(Units.ToTime(stage.RCSBurnTime), infoStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			}
		}
		GUILayout.EndVertical();
	}

	private void EditorLock(bool state)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		if (state)
		{
			InputLockManager.SetControlLock((ControlTypes)1152921504606846975L, "KER_BuildAdvanced");
			BuildOverlayPartInfo.Hidden = true;
			isEditorLocked = true;
		}
		else
		{
			InputLockManager.SetControlLock((ControlTypes)0, "KER_BuildAdvanced");
			BuildOverlayPartInfo.Hidden = false;
			isEditorLocked = false;
		}
	}

	private void GetStageInfo()
	{
		stages = SimManager.Stages;
		if (stages != null && stages.Length != 0)
		{
			maxMach = stages[stages.Length - 1].maxMach;
		}
	}

	private void InitialiseStyles()
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Expected O, but got Unknown
		//IL_0041: Expected O, but got Unknown
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_0069: Expected O, but got Unknown
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_0079: Unknown result type (might be due to invalid IL or missing references)
		//IL_007f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_009b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ae: Expected O, but got Unknown
		//IL_00b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Expected O, but got Unknown
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0142: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Expected O, but got Unknown
		//IL_0163: Unknown result type (might be due to invalid IL or missing references)
		//IL_0168: Unknown result type (might be due to invalid IL or missing references)
		//IL_0169: Unknown result type (might be due to invalid IL or missing references)
		//IL_0173: Expected O, but got Unknown
		//IL_0173: Unknown result type (might be due to invalid IL or missing references)
		//IL_0174: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Expected O, but got Unknown
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Expected O, but got Unknown
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a9: Expected O, but got Unknown
		//IL_01a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b8: Expected O, but got Unknown
		//IL_01b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01be: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ce: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0208: Expected O, but got Unknown
		//IL_020f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0214: Unknown result type (might be due to invalid IL or missing references)
		//IL_0225: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Expected O, but got Unknown
		windowStyle = new GUIStyle(HighLogic.Skin.window)
		{
			alignment = (TextAnchor)0
		};
		areaStyle = new GUIStyle(HighLogic.Skin.box)
		{
			padding = new RectOffset(0, 0, 9, 0)
		};
		areaSettingStyle = new GUIStyle(HighLogic.Skin.box)
		{
			padding = new RectOffset(10, 10, 10, 10)
		};
		GUIStyle val = new GUIStyle(HighLogic.Skin.button);
		val.normal.textColor = Color.white;
		val.fontSize = (int)(11f * GuiDisplaySize.Offset);
		val.fontStyle = (FontStyle)1;
		val.alignment = (TextAnchor)4;
		buttonStyle = val;
		GUIStyle val2 = new GUIStyle(HighLogic.Skin.label);
		val2.normal.textColor = Color.white;
		val2.fontSize = (int)(11f * GuiDisplaySize.Offset);
		val2.fontStyle = (FontStyle)1;
		val2.alignment = (TextAnchor)4;
		val2.stretchWidth = true;
		titleStyle = val2;
		infoStyle = new GUIStyle(HighLogic.Skin.label)
		{
			fontSize = (int)(11f * GuiDisplaySize.Offset),
			fontStyle = (FontStyle)1,
			alignment = (TextAnchor)4,
			stretchWidth = true
		};
		settingStyle = new GUIStyle(titleStyle)
		{
			alignment = (TextAnchor)3,
			stretchWidth = true,
			stretchHeight = true
		};
		settingAtmoStyle = new GUIStyle(titleStyle)
		{
			margin = new RectOffset(),
			padding = new RectOffset(),
			alignment = (TextAnchor)0
		};
		GUIStyle val3 = new GUIStyle(HighLogic.Skin.button)
		{
			margin = new RectOffset(0, 0, 2, 0),
			padding = new RectOffset(5, 5, 5, 5)
		};
		val3.normal.textColor = Color.white;
		val3.active.textColor = Color.white;
		val3.fontSize = (int)(11f * GuiDisplaySize.Offset);
		val3.fontStyle = (FontStyle)1;
		val3.alignment = (TextAnchor)4;
		val3.fixedHeight = 20f;
		bodiesButtonStyle = val3;
		bodiesButtonActiveStyle = new GUIStyle(bodiesButtonStyle)
		{
			normal = bodiesButtonStyle.onNormal,
			hover = bodiesButtonStyle.onHover
		};
	}

	private void Load()
	{
		try
		{
			SettingHandler settingHandler = SettingHandler.Load("BuildAdvanced.xml");
			settingHandler.Get("visible", ref visible);
			((Rect)(ref position)).x = settingHandler.Get("windowPositionX", ((Rect)(ref position)).x);
			((Rect)(ref position)).y = settingHandler.Get("windowPositionY", ((Rect)(ref position)).y);
			settingHandler.Get("compactMode", ref compactMode);
			settingHandler.Get("compactCollapseRight", ref compactCollapseRight);
			settingHandler.Get("showAllStages", ref showAllStages);
			settingHandler.Get("showAtmosphericDetails", ref showAtmosphericDetails);
			settingHandler.Get("showSettings", ref showSettings);
			CelestialBodies.SetSelectedBody(settingHandler.Get("selectedBodyName", CelestialBodies.SelectedBody.Name));
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "BuildAdvanced.Load()");
		}
	}

	private void OnSizeChanged()
	{
		InitialiseStyles();
		hasChanged = true;
	}

	private void Window(int windowId)
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00be: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Unknown result type (might be due to invalid IL or missing references)
		//IL_0188: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0245: Unknown result type (might be due to invalid IL or missing references)
		//IL_024a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			compactModeRect = new Rect(((Rect)(ref position)).width - 70f * GuiDisplaySize.Offset, 5f, 65f * GuiDisplaySize.Offset, 20f);
			if (GUI.Toggle(compactModeRect, compactMode, "COMPACT", buttonStyle) != compactMode)
			{
				hasChanged = true;
				compactCheck = 2;
				compactRight = ((Rect)(ref position)).xMax;
				compactMode = !compactMode;
			}
			if (!compactMode)
			{
				if (GUI.Toggle(new Rect(((Rect)(ref position)).width - 143f * GuiDisplaySize.Offset, 5f, 70f * GuiDisplaySize.Offset, 20f), showSettings, "SETTINGS", buttonStyle) != showSettings)
				{
					hasChanged = true;
					showSettings = !showSettings;
				}
				if (GUI.Toggle(new Rect(((Rect)(ref position)).width - 226f * GuiDisplaySize.Offset, 5f, 80f * GuiDisplaySize.Offset, 20f), showAllStages, "ALL STAGES", buttonStyle) != showAllStages)
				{
					hasChanged = true;
					showAllStages = !showAllStages;
				}
				if (GUI.Toggle(new Rect(((Rect)(ref position)).width - 324f * GuiDisplaySize.Offset, 5f, 95f * GuiDisplaySize.Offset, 20f), showAtmosphericDetails, "ATMOSPHERIC", buttonStyle) != showAtmosphericDetails)
				{
					hasChanged = true;
					showAtmosphericDetails = !showAtmosphericDetails;
				}
				bodiesListPosition = new Rect(((Rect)(ref position)).width - 452f * GuiDisplaySize.Offset, 5f, 125f * GuiDisplaySize.Offset, 20f);
				((Behaviour)bodiesList).enabled = GUI.Toggle(bodiesListPosition, ((Behaviour)bodiesList).enabled, "BODY: " + CelestialBodies.SelectedBody.Name.ToUpper(), buttonStyle);
				bodiesList.SetPosition(bodiesListPosition.Translate(position), bodiesListPosition);
				if (GUI.Toggle(new Rect(((Rect)(ref position)).width - 485f * GuiDisplaySize.Offset, 5f, 30f * GuiDisplaySize.Offset, 20f), showRCS, "RCS", buttonStyle) != showRCS)
				{
					hasChanged = true;
					showRCS = !showRCS;
				}
			}
			else if (GUI.Toggle(new Rect(((Rect)(ref position)).width - 133f * GuiDisplaySize.Offset, 5f, 60f * GuiDisplaySize.Offset, 20f), showAtmosphericDetails, "ATMO", buttonStyle) != showAtmosphericDetails)
			{
				hasChanged = true;
				showAtmosphericDetails = !showAtmosphericDetails;
			}
			if (!compactMode)
			{
				GUILayout.BeginHorizontal(areaStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				DrawStageNumbers();
				DrawPartCount();
				DrawCost();
				DrawMass();
				if (showRCS)
				{
					DrawRCS();
				}
				else
				{
					DrawIsp();
					DrawThrust();
					DrawTorque();
					DrawTwr();
					DrawDeltaV();
					DrawBurnTime();
				}
				GUILayout.EndHorizontal();
				if (showAtmosphericDetails && !compactMode)
				{
					GUILayout.BeginVertical(areaSettingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
					DrawAtmosphericDetails();
					GUILayout.EndVertical();
				}
				if (showSettings)
				{
					GUILayout.BeginVertical(areaSettingStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
					DrawSettings();
					GUILayout.EndVertical();
				}
			}
			else
			{
				GUILayout.BeginHorizontal(areaStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				DrawStageNumbers();
				DrawTwr();
				DrawDeltaV();
				GUILayout.EndHorizontal();
			}
			GUI.DragWindow();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "BuildAdvanced.Window()");
		}
	}
}
