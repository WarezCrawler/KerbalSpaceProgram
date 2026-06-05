using System;
using System.Collections.Generic;
using KerbalEngineer.Helpers;
using KerbalEngineer.VesselSimulator;
using UnityEngine;

namespace KerbalEngineer.Editor;

public class BuildOverlayVessel : MonoBehaviour
{
	private const float Width = 175f;

	private static bool visible = true;

	private readonly List<PartInfoItem> infoItems = new List<PartInfoItem>();

	private Stage lastStage;

	private bool open = true;

	private float openPercent;

	private GUIContent tabContent;

	private Rect tabPosition;

	private Vector2 tabSize;

	private Rect windowPosition = new Rect(330f, 0f, 175f, 0f);

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

	public bool Open
	{
		get
		{
			return open;
		}
		set
		{
			open = value;
		}
	}

	public Rect WindowPosition => windowPosition;

	public float WindowX
	{
		get
		{
			return ((Rect)(ref windowPosition)).x;
		}
		set
		{
			((Rect)(ref windowPosition)).x = value;
		}
	}

	protected void Awake()
	{
		try
		{
			SimManager.OnReady -= GetStageInfo;
			SimManager.OnReady += GetStageInfo;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void OnGUI()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Expected O, but got Unknown
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Visible && !((Object)(object)EditorLogic.RootPart == (Object)null))
			{
				open = GUI.Toggle(tabPosition, open, tabContent, BuildOverlay.TabStyle);
				if ((double)openPercent > 0.0)
				{
					windowPosition = GUILayout.Window(((Object)this).GetInstanceID(), windowPosition, new WindowFunction(VesselWindow), string.Empty, BuildOverlay.WindowStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				}
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void Start()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Expected O, but got Unknown
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			tabContent = new GUIContent("VESSEL");
			tabSize = BuildOverlay.TabStyle.CalcSize(tabContent);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	protected void Update()
	{
		try
		{
			if (Visible && !((Object)(object)EditorLogic.RootPart == (Object)null))
			{
				if ((double)openPercent > 0.0)
				{
					SetVesselInfo();
				}
				SetSlidePosition();
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void GetStageInfo()
	{
		lastStage = SimManager.LastStage;
	}

	private void SetSlidePosition()
	{
		if (open && openPercent < 1f)
		{
			openPercent = Mathf.Clamp(openPercent + Time.deltaTime * BuildOverlay.TabSpeed, 0f, 1f);
		}
		else if (!open && openPercent > 0f)
		{
			openPercent = Mathf.Clamp(openPercent - Time.deltaTime * BuildOverlay.TabSpeed, 0f, 1f);
		}
		((Rect)(ref windowPosition)).y = Mathf.Lerp((float)Screen.height, (float)Screen.height - ((Rect)(ref windowPosition)).height, openPercent);
		if (((Rect)(ref windowPosition)).width < 175f)
		{
			((Rect)(ref windowPosition)).width = 175f;
		}
		((Rect)(ref tabPosition)).width = tabSize.x;
		((Rect)(ref tabPosition)).height = tabSize.y;
		((Rect)(ref tabPosition)).x = ((Rect)(ref windowPosition)).x;
		((Rect)(ref tabPosition)).y = ((Rect)(ref windowPosition)).y - ((Rect)(ref tabPosition)).height;
	}

	private void SetVesselInfo()
	{
		SimManager.Gravity = CelestialBodies.SelectedBody.Gravity;
		if (BuildAdvanced.Instance.ShowAtmosphericDetails)
		{
			SimManager.Atmosphere = CelestialBodies.SelectedBody.GetAtmospheres(BuildAdvanced.Altitude);
		}
		else
		{
			SimManager.Atmosphere = 0.0;
		}
		SimManager.RequestSimulation();
		SimManager.TryStartSimulation();
		if (lastStage != null)
		{
			PartInfoItem.Release(infoItems);
			infoItems.Clear();
			infoItems.Add(PartInfoItem.Create("Delta-V", lastStage.deltaV.ToString("N0") + " / " + lastStage.totalDeltaV.ToString("N0") + "m/s"));
			infoItems.Add(PartInfoItem.Create("Mass", Units.ToMass(lastStage.mass, lastStage.totalMass)));
			infoItems.Add(PartInfoItem.Create("TWR", lastStage.thrustToWeight.ToString("F2") + " (" + lastStage.maxThrustToWeight.ToString("F2") + ")"));
			infoItems.Add(PartInfoItem.Create("Parts", lastStage.partCount + " / " + lastStage.totalPartCount));
		}
	}

	private void VesselWindow(int windowId)
	{
		try
		{
			bool flag = true;
			foreach (PartInfoItem infoItem in infoItems)
			{
				if (!flag)
				{
					GUILayout.Space(2f);
				}
				flag = false;
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				if (infoItem.Value != null)
				{
					GUILayout.Label(infoItem.Name + ":", BuildOverlay.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
					GUILayout.FlexibleSpace();
					GUILayout.Label(infoItem.Value, BuildOverlay.ValueStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				}
				else
				{
					GUILayout.Label(infoItem.Name, BuildOverlay.NameStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				}
				GUILayout.EndHorizontal();
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
