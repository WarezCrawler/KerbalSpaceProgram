using System;
using System.Collections;
using KSPAchievements;
using UnityEngine;

[KSPScenario(ScenarioCreationOptions.AddToAllGames, new GameScenes[]
{
	GameScenes.FLIGHT,
	GameScenes.TRACKSTATION,
	GameScenes.SPACECENTER
})]
public class ScenarioDiscoverableObjects : ScenarioModule
{
	public float spawnInterval = 15f;

	public float maxUntrackedLifetime = 20f;

	public float minUntrackedLifetime = 1f;

	public int spawnOddsAgainst = 2;

	public int spawnGroupMinLimit = 3;

	public int spawnGroupMaxLimit = 8;

	[KSPField(isPersistant = true)]
	public FloatCurve sizeCurve;

	[KSPField(isPersistant = true)]
	public int lastSeed;

	private int currentSize;

	private int rndGroupSize;

	private int newGroupSize;

	private bool discoveryUnlocked;

	public override void OnLoad(ConfigNode node)
	{
	}

	public override void OnSave(ConfigNode node)
	{
	}

	public override void OnAwake()
	{
		if (sizeCurve == null)
		{
			sizeCurve = new FloatCurve();
			sizeCurve.Add(0f, 0f);
			sizeCurve.Add(0.3f, 0.45f);
			sizeCurve.Add(0.7f, 0.55f);
			sizeCurve.Add(1f, 1f);
		}
	}

	public void Start()
	{
		UnityEngine.Random.InitState(lastSeed);
		discoveryUnlocked = GameVariables.Instance.UnlockedSpaceObjectDiscovery(ScenarioUpgradeableFacilities.GetFacilityLevel(SpaceCenterFacility.TrackingStation));
		StartCoroutine(SpawnDaemon());
	}

	private IEnumerator SpawnDaemon()
	{
		while ((bool)this)
		{
			UpdateAsteroids(Planetarium.GetUniversalTime());
			yield return new WaitForSeconds(Mathf.Max(0.1f, spawnInterval / TimeWarp.CurrentRate));
		}
	}

	private void UpdateAsteroids(double double_0)
	{
		bool flag = false;
		int num = 0;
		currentSize = 0;
		for (int num2 = FlightGlobals.Vessels.Count - 1; num2 >= 0; num2--)
		{
			Vessel vessel = FlightGlobals.Vessels[num2];
			if (vessel != null && !vessel.DiscoveryInfo.HaveKnowledgeAbout(DiscoveryLevels.StateVectors))
			{
				if (vessel.DiscoveryInfo.GetSignalLife(double_0) == 0.0)
				{
					if (!flag)
					{
						Debug.Log("[AsteroidSpawner]: " + vessel.vesselName + " has been untracked for too long and is now lost.", base.gameObject);
						vessel.Die();
						flag = true;
					}
					num++;
				}
				else
				{
					currentSize++;
				}
			}
		}
		if (flag || !discoveryUnlocked)
		{
			return;
		}
		rndGroupSize = UnityEngine.Random.Range(spawnGroupMinLimit, spawnGroupMaxLimit);
		newGroupSize = Mathf.Max(currentSize, rndGroupSize);
		if (newGroupSize > currentSize)
		{
			if (UnityEngine.Random.Range(0, spawnOddsAgainst) == 0)
			{
				SpawnAsteroid();
			}
			else
			{
				Debug.Log("[AsteroidSpawner]: No new objects this time. (Odds are 1:" + spawnOddsAgainst + ")", base.gameObject);
			}
		}
	}

	[ContextMenu("Spawn An Asteroid")]
	public void SpawnAsteroid()
	{
		int num = UnityEngine.Random.Range(0, int.MaxValue);
		UnityEngine.Random.InitState(num);
		lastSeed = num;
		if (ReachedBody("Dres") && Mathf.Abs(num % 3) == 0)
		{
			SpawnDresAsteroid(num);
		}
		else
		{
			SpawnHomeAsteroid(num);
		}
	}

	[ContextMenu("Spawn Last Asteroid")]
	public void SpawnLastAsteroid()
	{
		UnityEngine.Random.InitState(lastSeed);
		if (ReachedBody("Dres") && Mathf.Abs(lastSeed % 3) == 0)
		{
			SpawnDresAsteroid(lastSeed);
		}
		else
		{
			SpawnHomeAsteroid(lastSeed);
		}
	}

	public void SpawnHomeAsteroid(int asteroidSeed)
	{
		CelestialBody homeBody = FlightGlobals.GetHomeBody();
		if (!(homeBody == null))
		{
			double randomDuration = GetRandomDuration();
			string text = DiscoverableObjectsUtil.GenerateAsteroidName();
			UntrackedObjectClass objClass = (UntrackedObjectClass)(sizeCurve.Evaluate(UnityEngine.Random.Range(0f, 1f)) * (float)Enum.GetNames(typeof(UntrackedObjectClass)).Length);
			double lifeTime = (double)UnityEngine.Random.Range(minUntrackedLifetime, maxUntrackedLifetime) * 24.0 * 60.0 * 60.0;
			double lifeTimeMax = (double)maxUntrackedLifetime * 24.0 * 60.0 * 60.0;
			DiscoverableObjectsUtil.SpawnAsteroid(text, Orbit.CreateRandomOrbitFlyBy(homeBody, randomDuration), (uint)asteroidSeed, objClass, lifeTime, lifeTimeMax);
			Debug.Log("[AsteroidSpawner]: New object found near " + homeBody.name + ": " + text + "!", base.gameObject);
		}
	}

	public void SpawnDresAsteroid(int asteroidSeed)
	{
		CelestialBody bodyByName = FlightGlobals.GetBodyByName("Dres");
		if (!(bodyByName == null))
		{
			string text = DiscoverableObjectsUtil.GenerateAsteroidName();
			UntrackedObjectClass objClass = (UntrackedObjectClass)(sizeCurve.Evaluate(UnityEngine.Random.Range(0.5f, 1f)) * (float)Enum.GetNames(typeof(UntrackedObjectClass)).Length);
			double lifeTime = (double)UnityEngine.Random.Range(minUntrackedLifetime, maxUntrackedLifetime) * 24.0 * 60.0 * 60.0;
			double lifeTimeMax = (double)maxUntrackedLifetime * 24.0 * 60.0 * 60.0;
			double num = (bodyByName.sphereOfInfluence - bodyByName.Radius) / 2.0;
			Orbit orbit = Orbit.CreateRandomOrbitAround(bodyByName, bodyByName.Radius + num * 1.100000023841858, bodyByName.Radius + num * 1.25);
			orbit.meanAnomalyAtEpoch = (double)(UnityEngine.Random.value * 2f) * Math.PI;
			DiscoverableObjectsUtil.SpawnAsteroid(text, orbit, (uint)asteroidSeed, objClass, lifeTime, lifeTimeMax);
			Debug.Log("[AsteroidSpawner]: New object found near Dres: " + text + "!", base.gameObject);
		}
	}

	[ContextMenu("Check Spawn Probability")]
	public void DebugSpawnProbability()
	{
		float num = 1f / (float)spawnOddsAgainst;
		float num2 = spawnInterval / 60f / 60f;
		float num3 = 1f / num2 * num;
		if (num2 > 1f)
		{
			Debug.Log("For a " + num.ToString("0.####") + " chance every " + num2.ToString("0.0##") + " hours, the average spawn rate is a spawn every " + 1f / num3 + " hours");
		}
		else
		{
			Debug.Log("For a " + num.ToString("0.####") + " chance every " + (num2 * 60f).ToString("0.0##") + " minutes, the average spawn rate is a spawn every " + 1f / num3 * 60f + " minutes");
		}
	}

	private double GetRandomDuration()
	{
		return UnityEngine.Random.Range(15f, 60f);
	}

	private bool ReachedBody(string bodyName)
	{
		if (ProgressTracking.Instance != null)
		{
			CelestialBodySubtree bodyTree = ProgressTracking.Instance.GetBodyTree(bodyName);
			if (bodyTree != null && bodyTree.IsReached)
			{
				return true;
			}
		}
		return false;
	}
}
