using System;
using KerbalEngineer.Unity.Flight;
using UnityEngine;

namespace KerbalEngineer.Flight.Readouts.Rendezvous;

public class TargetSelector : ReadoutModule
{
	private string searchQuery = string.Empty;

	private string searchText = string.Empty;

	private int targetCount;

	private ITargetable targetObject;

	private float typeButtonWidth;

	private bool typeIsBody;

	private bool usingSearch;

	private VesselType vesselType = (VesselType)2;

	private bool wasMapview;

	public TargetSelector()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		base.Name = "Target Selector";
		base.Category = ReadoutCategory.GetCategory("Rendezvous");
		base.HelpString = "A tool to allow easy browsing, searching and selection of targets.";
		base.IsDefault = true;
	}

	public override void Draw(ISectionModule section)
	{
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Invalid comparison between Unknown and I4
		if (!HighLogic.LoadedSceneIsFlight)
		{
			DrawTarget(section);
			return;
		}
		if (FlightGlobals.fetch.VesselTarget == null)
		{
			if ((int)vesselType == 2 && !typeIsBody)
			{
				DrawSearch();
				if (searchQuery.Length == 0)
				{
					DrawTypes();
				}
				else
				{
					DrawTargetList();
				}
			}
			else
			{
				DrawBackToTypes();
				DrawTargetList();
			}
		}
		else
		{
			DrawTarget(section);
		}
		if (targetObject != FlightGlobals.fetch.VesselTarget)
		{
			targetObject = FlightGlobals.fetch.VesselTarget;
			base.ResizeRequested = true;
		}
	}

	private void DrawBackToTypes()
	{
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		if (GUILayout.Button("Go Back to Type Selection", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
		{
			typeIsBody = false;
			vesselType = (VesselType)2;
			base.ResizeRequested = true;
		}
		GUILayout.Space(3f);
	}

	private int DrawMoons()
	{
		int num = 0;
		foreach (CelestialBody body in FlightGlobals.Bodies)
		{
			if (!((Object)(object)FlightGlobals.ActiveVessel.mainBody != (Object)(object)body.referenceBody) && !((Object)(object)body == (Object)(object)Planetarium.fetch.Sun) && (searchQuery.Length <= 0 || body.bodyName.ToLower().Contains(searchQuery)))
			{
				num++;
				if (GUILayout.Button(body.bodyName, base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
				{
					SetTargetAs((ITargetable)(object)body);
				}
			}
		}
		return num;
	}

	private int DrawPlanets()
	{
		int num = 0;
		foreach (CelestialBody body in FlightGlobals.Bodies)
		{
			if (!((Object)(object)FlightGlobals.ActiveVessel.mainBody == (Object)(object)Planetarium.fetch.Sun) && !((Object)(object)FlightGlobals.ActiveVessel.mainBody.referenceBody != (Object)(object)body.referenceBody) && !((Object)(object)body == (Object)(object)Planetarium.fetch.Sun) && !((Object)(object)body == (Object)(object)FlightGlobals.ActiveVessel.mainBody) && (searchQuery.Length <= 0 || body.bodyName.ToLower().Contains(searchQuery)))
			{
				num++;
				if (GUILayout.Button(body.GetName(), base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
				{
					SetTargetAs((ITargetable)(object)body);
				}
			}
		}
		return num;
	}

	private void DrawSearch()
	{
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("SEARCH:", base.FlexiLabelStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(60f * GuiDisplaySize.Offset) });
		searchText = GUILayout.TextField(searchText, base.TextFieldStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (searchText.Length > 0 || searchQuery.Length > 0)
		{
			searchQuery = searchText.ToLower();
			if (!usingSearch)
			{
				usingSearch = true;
				base.ResizeRequested = true;
			}
		}
		else if (usingSearch)
		{
			usingSearch = false;
			base.ResizeRequested = true;
		}
		GUILayout.EndHorizontal();
	}

	private void DrawTarget(ISectionModule section)
	{
		//IL_01f5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fb: Invalid comparison between Unknown and I4
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0250: Unknown result type (might be due to invalid IL or missing references)
		//IL_0299: Unknown result type (might be due to invalid IL or missing references)
		//IL_029e: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02af: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0283: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_025d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0262: Unknown result type (might be due to invalid IL or missing references)
		//IL_0267: Unknown result type (might be due to invalid IL or missing references)
		//IL_026c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d0: Unknown result type (might be due to invalid IL or missing references)
		ITargetable activeTarget = RendezvousProcessor.activeTarget;
		base.ResizeRequested = true;
		if (activeTarget == null)
		{
			return;
		}
		if (HighLogic.LoadedSceneIsFlight)
		{
			if (GUILayout.Button("Go Back to Target Selection", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
			{
				FlightGlobals.fetch.SetVesselTarget((ITargetable)null, false);
			}
		}
		else if (RendezvousProcessor.TrackingStationSource != activeTarget && GUILayout.Button("Use " + RendezvousProcessor.nameForTargetable(activeTarget) + " As Reference", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
		{
			RendezvousProcessor.TrackingStationSource = activeTarget;
		}
		if (HighLogic.LoadedSceneIsFlight)
		{
			Vessel activeVessel = FlightGlobals.ActiveVessel;
			if ((Object)(object)activeVessel == (Object)null)
			{
				return;
			}
			if (!(activeTarget is CelestialBody) && GUILayout.Button("Switch to Target", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
			{
				FlightEngineerCore.SwitchToVessel(activeTarget.GetVessel(), (ITargetable)(object)activeVessel);
			}
			if (activeTarget is CelestialBody || activeTarget is Vessel)
			{
				MapObject val = null;
				val = ((!(activeTarget is Vessel)) ? ((CelestialBody)activeTarget).MapObject : ((Vessel)activeTarget).mapObject);
				if ((Object)(object)val != (Object)null && ((Object)(object)val != (Object)(object)PlanetariumCamera.fetch.target || !MapView.MapIsEnabled) && GUILayout.Button("Focus Target", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
				{
					wasMapview = MapView.MapIsEnabled;
					MapView.EnterMapView();
					PlanetariumCamera.fetch.SetTarget(val);
				}
			}
			if ((Object)(object)PlanetariumCamera.fetch.target != (Object)(object)activeVessel.mapObject && MapView.MapIsEnabled && GUILayout.Button("Focus Vessel", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
			{
				PlanetariumCamera.fetch.SetTarget(activeVessel.mapObject);
				if (!wasMapview)
				{
					MapView.ExitMapView();
				}
			}
			if ((int)FlightCamera.fetch.mode != 4 && !MapView.MapIsEnabled && GUILayout.Button("Look at Target", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
			{
				_ = PlanetariumCamera.fetch;
				FlightCamera fetch = FlightCamera.fetch;
				Vector3 val2 = default(Vector3);
				if (activeTarget is Vessel && ((Vessel)activeTarget).LandedOrSplashed)
				{
					val2 = Vector3d.op_Implicit(((Vessel)activeTarget).GetWorldPos3D());
				}
				else if (activeTarget.GetOrbit() != null)
				{
					val2 = Vector3d.op_Implicit(activeTarget.GetOrbit().getTruePositionAtUT(Planetarium.GetUniversalTime()));
				}
				Vector3 val3 = Vector3d.op_Implicit(FlightGlobals.fetch.activeVessel.GetWorldPos3D());
				float distance = fetch.Distance;
				Vector3 val4 = val2 - val3;
				Vector3 normalized = ((Vector3)(ref val4)).normalized;
				if (!VectorExtensions.IsInvalid(normalized))
				{
					fetch.SetCamCoordsFromPosition(normalized * (0f - distance));
				}
			}
		}
		GUILayout.Space(3f);
		DrawLine("Selected Target", RendezvousProcessor.nameForTargetable(activeTarget), section.IsHud);
		try
		{
			if (RendezvousProcessor.sourceDisplay != null)
			{
				if (RendezvousProcessor.landedSamePlanet || RendezvousProcessor.overrideANDN)
				{
					DrawLine("Ref Orbit", "Landed on " + RendezvousProcessor.activeVessel.GetOrbit().referenceBody.GetName(), section.IsHud);
				}
				else
				{
					DrawLine("Ref Orbit", RendezvousProcessor.sourceDisplay, section.IsHud);
				}
			}
			if (RendezvousProcessor.targetDisplay != null)
			{
				if (RendezvousProcessor.landedSamePlanet || RendezvousProcessor.overrideANDNRev)
				{
					DrawLine("Target Orbit", "Landed on " + activeTarget.GetOrbit().referenceBody.GetName(), section.IsHud);
				}
				else
				{
					DrawLine("Target Orbit", RendezvousProcessor.targetDisplay, section.IsHud);
				}
			}
		}
		catch (Exception)
		{
			Debug.Log((object)string.Concat(" target ", activeTarget, " ", RendezvousProcessor.activeVessel, " ", activeTarget.GetOrbit(), " ", RendezvousProcessor.overrideANDN.ToString(), " ", RendezvousProcessor.overrideANDNRev.ToString(), " ", RendezvousProcessor.landedSamePlanet.ToString()));
		}
	}

	private void DrawTargetList()
	{
		int num = 0;
		if (searchQuery.Length == 0)
		{
			if (typeIsBody)
			{
				GUILayout.Label("Local Bodies", base.FlexiLabelStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) });
				num += DrawMoons();
				GUILayout.Label("Remote Bodies", base.FlexiLabelStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) });
				num += DrawPlanets();
			}
			else
			{
				GUILayout.Label(((object)(VesselType)(ref vesselType)).ToString(), base.FlexiLabelStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) });
				num += DrawVessels();
			}
		}
		else
		{
			GUILayout.Label("Search Results", base.FlexiLabelStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) });
			num += DrawVessels();
			num += DrawMoons();
			num += DrawPlanets();
		}
		if (num == 0)
		{
			DrawMessageLine("No targets found!");
		}
		if (num != targetCount)
		{
			targetCount = num;
			base.ResizeRequested = true;
		}
	}

	private void DrawTypes()
	{
		typeButtonWidth = Mathf.Round(base.ContentWidth * 0.5f);
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("Celestial Bodies", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAsBody();
		}
		if (GUILayout.Button("Debris", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)0);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("Probes", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)3);
		}
		if (GUILayout.Button("Relays", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)4);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("Rovers", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)5);
		}
		if (GUILayout.Button("Landers", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)6);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("Ships", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)7);
		}
		if (GUILayout.Button("Planes", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)8);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("Stations", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)9);
		}
		if (GUILayout.Button("Bases", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)10);
		}
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("EVAs", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)11);
		}
		if (GUILayout.Button("Flags", base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(typeButtonWidth) }))
		{
			SetTypeAs((VesselType)12);
		}
		GUILayout.EndHorizontal();
	}

	private int DrawVessels()
	{
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		int num = 0;
		foreach (Vessel vessel in FlightGlobals.Vessels)
		{
			if ((Object)(object)vessel == (Object)(object)FlightGlobals.ActiveVessel || (searchQuery.Length == 0 && vessel.vesselType != vesselType))
			{
				continue;
			}
			if (searchQuery.Length == 0)
			{
				num++;
				if (GUILayout.Button(vessel.GetName(), base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
				{
					SetTargetAs((ITargetable)(object)vessel);
				}
			}
			else if (vessel.vesselName.ToLower().Contains(searchQuery))
			{
				num++;
				if (GUILayout.Button(vessel.GetName(), base.ButtonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(base.ContentWidth) }))
				{
					SetTargetAs((ITargetable)(object)vessel);
				}
			}
		}
		return num;
	}

	private void SetTargetAs(ITargetable target)
	{
		FlightGlobals.fetch.SetVesselTarget(target, false);
		base.ResizeRequested = true;
	}

	private void SetTypeAs(VesselType vesselType)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		this.vesselType = vesselType;
		base.ResizeRequested = true;
	}

	private void SetTypeAsBody()
	{
		typeIsBody = true;
		base.ResizeRequested = true;
	}

	public override void Update()
	{
		RendezvousProcessor.RequestUpdate();
	}
}
