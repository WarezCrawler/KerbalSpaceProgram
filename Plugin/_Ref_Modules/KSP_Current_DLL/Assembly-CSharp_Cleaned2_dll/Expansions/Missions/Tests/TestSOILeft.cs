using System;
using Expansions.Missions.Editor;
using ns9;

namespace Expansions.Missions.Tests;

[MEScoreModule(new Type[]
{
	typeof(ScoreModule_Resource),
	typeof(ScoreModule_Time)
})]
internal class TestSOILeft : TestVessel, INodeBody, IScoreableObjective
{
	[MEGUI_CelestialBody(order = 10, showAnySOIoption = true, resetValue = "0", gapDisplay = true, guiName = "#autoLOC_8000263", Tooltip = "#autoLOC_8000157")]
	public MissionCelestialBody missionBody;

	private bool eventFound;

	public override void Awake()
	{
		base.Awake();
		title = Localizer.Format("#autoLOC_8000155");
		missionBody = new MissionCelestialBody(FlightGlobals.GetHomeBody());
		useActiveVessel = true;
	}

	public override void Initialized()
	{
		eventFound = false;
		GameEvents.onVesselSOIChanged.Add(OnEscapedSOI);
	}

	public override void Cleared()
	{
		GameEvents.onVesselSOIChanged.Remove(OnEscapedSOI);
	}

	private void OnEscapedSOI(GameEvents.HostedFromToAction<Vessel, CelestialBody> fromTo)
	{
		base.Test();
		if (vessel == fromTo.host && missionBody.IsValid(fromTo.from))
		{
			eventFound = true;
		}
	}

	public override bool Test()
	{
		return eventFound;
	}

	public bool HasNodeBody()
	{
		return true;
	}

	public CelestialBody GetNodeBody()
	{
		return missionBody.Body;
	}

	public override string GetInfo()
	{
		return Localizer.Format("#autoLOC_8004013");
	}

	public override void Save(ConfigNode node)
	{
		base.Save(node);
		missionBody.Save(node);
	}

	public override void Load(ConfigNode node)
	{
		base.Load(node);
		missionBody.Load(node);
	}

	public object GetScoreModifier(Type scoreModule)
	{
		if (scoreModule == typeof(ScoreModule_Resource))
		{
			if (vesselID != 0)
			{
				return FlightGlobals.PersistentVesselIds[vesselID];
			}
			return FlightGlobals.ActiveVessel;
		}
		return null;
	}
}
