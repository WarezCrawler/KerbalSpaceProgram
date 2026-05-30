using FinePrint.Utilities;
using ns9;

namespace KSPAchievements;

public class PointOfInterest : ProgressNode
{
	private VesselRef firstVessel;

	private CrewRef firstCrew;

	public string body { get; private set; }

	public string name { get; private set; }

	public string text { get; private set; }

	public bool uplifting { get; private set; }

	public bool launchSite { get; private set; }

	public PointOfInterest(string body, string name, string text, bool uplifting)
		: base("POI" + body + name, startReached: false)
	{
		this.body = body;
		this.name = name;
		this.text = text;
		this.uplifting = uplifting;
		launchSite = false;
		OnDeploy = delegate
		{
			GameEvents.OnPOIRangeEntered.Add(OnAnomalyLoaded);
		};
		OnStow = delegate
		{
			GameEvents.OnPOIRangeEntered.Remove(OnAnomalyLoaded);
		};
		firstVessel = new VesselRef();
		firstCrew = new CrewRef();
	}

	public PointOfInterest(string body, string name, string text, bool uplifting, bool launchSite)
		: this(body, name, text, uplifting)
	{
		this.launchSite = launchSite;
	}

	private void OnAnomalyLoaded(CelestialBody anomalyBody, string anomalyName)
	{
		if (anomalyBody.name != body || anomalyName != name || !FlightGlobals.ready)
		{
			return;
		}
		Vessel activeVessel = FlightGlobals.ActiveVessel;
		if (activeVessel == null || !activeVessel.isCommandable || activeVessel.DiscoveryInfo.Level != DiscoveryLevels.Owned)
		{
			return;
		}
		if (!base.IsComplete)
		{
			firstVessel = VesselRef.FromVessel(activeVessel);
			firstCrew = CrewRef.FromVessel(activeVessel);
			Complete();
			CelestialBody celestialBody = null;
			int count = FlightGlobals.Bodies.Count;
			while (count-- > 0)
			{
				if (FlightGlobals.Bodies[count].name == body)
				{
					celestialBody = FlightGlobals.Bodies[count];
					break;
				}
			}
			if (!launchSite || !HighLogic.CurrentGame.Parameters.Difficulty.AllowOtherLaunchSites)
			{
				if (uplifting)
				{
					int pQSCitySeed = PQSCity.GetPQSCitySeed(anomalyName, anomalyBody.bodyName);
					string text = Localizer.Format("#autoLOC_296488", this.text, celestialBody.displayName);
					text += GetUpliftText(pQSCitySeed);
					AwardProgressRandomTech(text, pQSCitySeed);
				}
				else
				{
					AwardProgressStandard(Localizer.Format("#autoLOC_296494", this.text, celestialBody.displayName), ProgressType.POINTOFINTEREST, celestialBody);
				}
			}
		}
		Achieve();
	}

	protected override void OnLoad(ConfigNode node)
	{
		if (node.HasNode("vessel"))
		{
			firstVessel.Load(node.GetNode("vessel"));
		}
		if (node.HasNode("crew"))
		{
			firstCrew.Load(node.GetNode("crew"));
		}
	}

	protected override void OnSave(ConfigNode node)
	{
		firstVessel.Save(node.AddNode("vessel"));
		if (firstCrew.HasAny)
		{
			firstCrew.Save(node.AddNode("crew"));
		}
	}

	protected string GetUpliftText(int seed)
	{
		return new KSPRandom(seed).Next(10) switch
		{
			0 => Localizer.Format("#autoLOC_296522"), 
			1 => Localizer.Format("#autoLOC_296524"), 
			2 => Localizer.Format("#autoLOC_296526"), 
			3 => Localizer.Format("#autoLOC_296528"), 
			4 => Localizer.Format("#autoLOC_296530"), 
			5 => Localizer.Format("#autoLOC_296532"), 
			6 => Localizer.Format("#autoLOC_296534"), 
			7 => Localizer.Format("#autoLOC_296536"), 
			8 => Localizer.Format("#autoLOC_296538"), 
			_ => Localizer.Format("#autoLOC_296540"), 
		};
	}
}
