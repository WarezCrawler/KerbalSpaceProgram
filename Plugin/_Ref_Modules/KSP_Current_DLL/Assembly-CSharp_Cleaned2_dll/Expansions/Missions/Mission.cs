using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Expansions.Missions.Actions;
using Expansions.Missions.Editor;
using Expansions.Missions.Flow;
using Expansions.Missions.Runtime;
using UnityEngine;
using ns9;

namespace Expansions.Missions;

[Serializable]
public class Mission : MonoBehaviour, IConfigNode
{
	public string expansionVersion = "";

	public static string lastCompatibleVersion = "1.0.0";

	[SerializeField]
	private string _title = "";

	[SerializeField]
	private string activeNodeName;

	public string briefing = "";

	public string author = "";

	public string modsBriefing = "";

	public string packName;

	public int order;

	public bool hardIcon;

	public MissionDifficulty difficulty = MissionDifficulty.Intermediate;

	[SerializeField]
	internal bool isBriefingSet;

	[SerializeField]
	private int seed;

	public string flagURL = "";

	private MEBannerEntry bannerMenu;

	private MEBannerEntry bannerSuccess;

	private MEBannerEntry bannerFail;

	public bool isScoreEnabled = true;

	public float maxScore;

	public DictionaryValueList<string, MissionCraft> craftFileList;

	[SerializeField]
	private MissionFileInfo _missionInfo;

	public MissionSituation situation;

	public MissionFlow flow;

	public DictionaryValueList<Guid, MENode> nodes;

	public List<string> tags;

	public MissionScore globalScore;

	public MissionAwards awards;

	private Guid loadActiveNodeID;

	public double startedUT;

	public float currentScore;

	public string exportName = "";

	internal string missionNameAtLastExport = "";

	public ulong steamPublishedFileId;

	public bool briefingNodeActive;

	[SerializeField]
	private List<MENode> orphanNodes;

	private List<MENode> nextObjectivesNodes;

	private List<MENode> inactiveEventNodes;

	private bool switchActiveVessel;

	private Vessel vesselToSwitchActive;

	internal bool saveRevertOnSwitchActiveVessel;

	public MissionCameraModeOptions cameraLockMode = MissionCameraModeOptions.Flight;

	public MissionCameraLockOptions cameraLockOptions = MissionCameraLockOptions.Unlock;

	public Guid historyId;

	private List<string> blockedMessages;

	private Callback<Mission> generateLaunchSitesCallback;

	internal List<MissionValidationTestResult> validationResults;

	internal bool hasBeenValidated;

	public Guid id { get; private set; }

	internal string idName { get; private set; }

	public string title
	{
		get
		{
			return _title;
		}
		set
		{
			_title = value;
			base.gameObject.name = GetGameObjectName(this);
		}
	}

	public string ActiveNodeName => activeNodeName;

	internal int Seed => seed;

	public string PersistentSaveName
	{
		get
		{
			if (MissionLoadedFromSFS)
			{
				return HighLogic.SaveFolder.Replace(MissionsUtils.SavesPath, "");
			}
			return MissionInfo.folderName;
		}
	}

	public bool MissionLoadedFromSFS
	{
		get
		{
			if (!HighLogic.LoadedSceneIsMissionBuilder && HighLogic.CurrentGame != null)
			{
				if (HighLogic.CurrentGame.Mode != Game.Modes.MISSION)
				{
					if (HighLogic.CurrentGame.Mode == Game.Modes.MISSION_BUILDER)
					{
						return HighLogic.LoadedScene == GameScenes.TRACKSTATION;
					}
					return false;
				}
				return true;
			}
			return false;
		}
	}

	public string ShipsPath
	{
		get
		{
			if (MissionLoadedFromSFS)
			{
				return MissionsUtils.SavesRootPath + PersistentSaveName + "/Ships/";
			}
			if (MissionInfo == null)
			{
				return string.Empty;
			}
			return MissionInfo.ShipFolderPath;
		}
	}

	public string BannersPath
	{
		get
		{
			if (MissionLoadedFromSFS)
			{
				return MissionsUtils.SavesRootPath + PersistentSaveName + "/Banners/";
			}
			if (MissionInfo == null)
			{
				return string.Empty;
			}
			return MissionInfo.BannerPath;
		}
	}

	public MissionFileInfo MissionInfo
	{
		get
		{
			return _missionInfo;
		}
		private set
		{
			_missionInfo = value;
		}
	}

	public MENode activeNode { get; private set; }

	public MENode startNode { get; private set; }

	public bool isInitialized { get; private set; }

	public bool isStarted { get; private set; }

	public bool isEnded { get; private set; }

	public bool isSuccesful { get; private set; }

	public List<MENode> InactiveEventNodes => inactiveEventNodes;

	public bool IsTutorialMission => packName == "squad_MakingHistory_Tutorial";

	public List<MissionValidationTestResult> ValidationResults => validationResults;

	public bool HasBeenValidated => hasBeenValidated;

	private void Awake()
	{
		if (!ExpansionsLoader.IsExpansionInstalled("MakingHistory"))
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		InitSeed();
		id = Guid.NewGuid();
		situation = new MissionSituation(this);
		nodes = new DictionaryValueList<Guid, MENode>();
		tags = new List<string>();
		craftFileList = new DictionaryValueList<string, MissionCraft>();
		orphanNodes = new List<MENode>();
		flow = new MissionFlow(this);
		currentScore = 0f;
		isSuccesful = false;
		isEnded = false;
		isStarted = false;
		isInitialized = false;
		hasBeenValidated = false;
		globalScore = new MissionScore(null);
		awards = new MissionAwards(null);
		flagURL = "Squad/Flags/default";
		bannerMenu = new MEBannerEntry(MEBannerType.Menu);
		bannerSuccess = new MEBannerEntry(MEBannerType.Success);
		bannerFail = new MEBannerEntry(MEBannerType.Fail);
		order = int.MaxValue;
		GameEvents.onGameSceneSwitchRequested.Add(OnGameSceneSwitchRequested);
	}

	private void OnDestroy()
	{
		bannerMenu.DestroyTexture();
		bannerSuccess.DestroyTexture();
		bannerFail.DestroyTexture();
		ClearCraftFiles();
		if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Mode == Game.Modes.MISSION && (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER))
		{
			awards.StopTracking();
		}
		GameEvents.onGameSceneSwitchRequested.Remove(OnGameSceneSwitchRequested);
		GameEvents.onEditorStarted.Remove(OnEditorStarted);
	}

	public static Mission Spawn()
	{
		GameObject obj = new GameObject("Mission");
		obj.transform.SetParent(MissionSystem.MissionsGameObject.transform);
		Mission mission = obj.gameObject.AddComponent<Mission>();
		mission.isBriefingSet = false;
		return mission;
	}

	public static Mission Spawn(MissionFileInfo missionInfo)
	{
		Mission mission = Spawn();
		mission.MissionInfo = missionInfo;
		mission.gameObject.name = GetGameObjectName(missionInfo);
		return mission;
	}

	public static Mission SpawnAndLoad(MissionFileInfo missionInfo, ConfigNode missionNode)
	{
		Mission mission = Spawn();
		mission._missionInfo = missionInfo;
		mission.gameObject.name = GetGameObjectName(missionInfo);
		mission.Load(missionNode, simple: false);
		mission.InitMission();
		MEFlowParser.ParseMission(mission);
		return mission;
	}

	private void InitSeed()
	{
		seed = (int)(DateTime.Now.Ticks & 0x7FFFFFFFL);
	}

	internal static string GetGameObjectName(MissionFileInfo missionInfo)
	{
		string text = "";
		text = (string.IsNullOrEmpty(missionInfo.idName) ? Localizer.Format(missionInfo.title) : missionInfo.idName);
		return "Mission (" + text + ")";
	}

	internal static string GetGameObjectName(Mission mission)
	{
		string text = "";
		text = (string.IsNullOrEmpty(mission.idName) ? Localizer.Format(mission.title) : mission.idName);
		return "Mission (" + text + ")";
	}

	internal void RegenerateMissionID()
	{
		id = Guid.NewGuid();
		packName = "";
	}

	private void OnGameSceneSwitchRequested(GameEvents.FromToAction<GameScenes, GameScenes> scenes)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			nodes.ValuesList[i].ClearTestGroups();
		}
	}

	public void InitMission()
	{
		orphanNodes.Clear();
		SetStartNode();
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		listEnumerator = nodes.GetListEnumerator();
		try
		{
			while (listEnumerator.MoveNext())
			{
				MENode current = listEnumerator.Current;
				current.fromNodes.Clear();
				if (current.fromNodeIDs.Count > 0)
				{
					for (int i = 0; i < current.fromNodeIDs.Count; i++)
					{
						if (nodes.ContainsKey(current.fromNodeIDs[i]))
						{
							current.fromNodes.Add(nodes[current.fromNodeIDs[i]]);
						}
					}
				}
				else if (current.IsOrphanNode)
				{
					orphanNodes.Add(current);
				}
			}
		}
		finally
		{
			listEnumerator.Dispose();
		}
		listEnumerator = nodes.GetListEnumerator();
		try
		{
			while (listEnumerator.MoveNext())
			{
				MENode current2 = listEnumerator.Current;
				if (current2.toNodeIDs.Count <= 0)
				{
					continue;
				}
				for (int j = 0; j < current2.toNodeIDs.Count; j++)
				{
					if (nodes.ContainsKey(current2.toNodeIDs[j]) && nodes[current2.toNodeIDs[j]].fromNodes.Contains(current2))
					{
						current2.toNodes.AddUnique(nodes[current2.toNodeIDs[j]]);
					}
				}
				current2.toNodeIDs.Clear();
			}
		}
		finally
		{
			listEnumerator.Dispose();
		}
		listEnumerator = nodes.GetListEnumerator();
		try
		{
			while (listEnumerator.MoveNext())
			{
				MENode current3 = listEnumerator.Current;
				for (int k = 0; k < current3.fromNodes.Count; k++)
				{
					current3.fromNodes[k].toNodes.AddUnique(current3);
				}
				if (!current3.isEndNode)
				{
					current3.toNodes.AddRange(orphanNodes);
					if (current3.IsOrphanNode)
					{
						current3.toNodes.Remove(current3);
					}
				}
			}
		}
		finally
		{
			listEnumerator.Dispose();
		}
		listEnumerator = nodes.GetListEnumerator();
		try
		{
			while (listEnumerator.MoveNext())
			{
				MENode current4 = listEnumerator.Current;
				if (current4.toNodes.Contains(current4))
				{
					current4.toNodes.Remove(current4);
				}
			}
		}
		finally
		{
			listEnumerator.Dispose();
		}
		if (!isInitialized)
		{
			isInitialized = true;
			situation.InitSituation();
		}
		DictionaryValueList<VesselSituation, Guid> dictionaryValueList = new DictionaryValueList<VesselSituation, Guid>();
		for (int l = 0; l < situation.VesselSituationList.Count; l++)
		{
			if (!string.IsNullOrEmpty(situation.VesselSituationList.KeyAt(l).craftFile) && GetMissionCraftByFileName(situation.VesselSituationList.KeyAt(l).craftFile) == null)
			{
				dictionaryValueList.Add(situation.VesselSituationList.KeyAt(l), situation.VesselSituationList.At(l));
			}
		}
		for (int m = 0; m < dictionaryValueList.Count; m++)
		{
			Debug.LogWarning("[Mission]: Vessel " + dictionaryValueList.KeyAt(m).vesselName + " removed from mission as filename:" + dictionaryValueList.KeyAt(m).craftFile + " not found.");
			situation.VesselSituationList.Remove(dictionaryValueList.KeyAt(m));
		}
		if (dictionaryValueList.Count > 0)
		{
			ScreenMessages.PostScreenMessage("#autoLOC_8006009", 10f);
		}
		if (HighLogic.LoadedScene != GameScenes.EDITOR)
		{
			GameEvents.onEditorStarted.Add(OnEditorStarted);
		}
		else
		{
			OnEditorStarted();
		}
		if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Mode == Game.Modes.MISSION && (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER))
		{
			awards.StartTracking();
		}
	}

	private void SetStartNode()
	{
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		listEnumerator = nodes.GetListEnumerator();
		try
		{
			while (listEnumerator.MoveNext())
			{
				MENode current = listEnumerator.Current;
				if (current.isStartNode)
				{
					startNode = current;
					break;
				}
			}
		}
		finally
		{
			listEnumerator.Dispose();
		}
	}

	private void SetActiveNode()
	{
		if (nodes.Contains(loadActiveNodeID))
		{
			activeNode = nodes[loadActiveNodeID];
		}
		else
		{
			activeNode = startNode;
		}
		activeNodeName = activeNode.Title;
		loadActiveNodeID = Guid.Empty;
	}

	private void SetDockedNodes()
	{
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		try
		{
			while (listEnumerator.MoveNext())
			{
				MENode current = listEnumerator.Current;
				if (nodes.ContainsKey(current.dockParentNodeID))
				{
					current.dockParentNode = nodes[current.dockParentNodeID];
				}
				if (current.dockedNodesIDsOnLoad.Count <= 0)
				{
					continue;
				}
				for (int i = 0; i < current.dockedNodesIDsOnLoad.Count; i++)
				{
					if (nodes.ContainsKey(current.dockedNodesIDsOnLoad[i]))
					{
						current.dockedNodes.AddUnique(nodes[current.dockedNodesIDsOnLoad[i]]);
						continue;
					}
					Debug.LogFormat("[Mission]: ({0}) SetDockedNodes found invalid docked Node reference from {1} contains reference to {2}, ignoring.", title, string.Concat(current.Title, "(", current.id, ")"), current.dockedNodesIDsOnLoad[i]);
				}
			}
		}
		finally
		{
			listEnumerator.Dispose();
		}
	}

	public bool BlockPlayMission(bool showDialog = false)
	{
		if (blockedMessages == null)
		{
			blockedMessages = new List<string>();
		}
		else
		{
			blockedMessages.Clear();
		}
		bool flag = false;
		List<VesselSituation> allVesselSituations = GetAllVesselSituations();
		for (int i = 0; i < allVesselSituations.Count; i++)
		{
			if (!allVesselSituations[i].playerCreated && string.IsNullOrEmpty(allVesselSituations[i].craftFile))
			{
				string text = Localizer.Format("#autoLOC_8003065");
				if (allVesselSituations[i].node != null)
				{
					text += Localizer.Format("#autoLOC_8003066", allVesselSituations[i].node.Title);
				}
				blockedMessages.Add(text);
				flag = true;
			}
		}
		string text2 = Localizer.Format("#autoLOC_8003067");
		for (int j = 0; j < blockedMessages.Count; j++)
		{
			text2 = text2 + blockedMessages[j] + "\n";
		}
		text2 += Localizer.Format("#autoLOC_8003068");
		if (flag)
		{
			Debug.LogError("[Mission] Unable to be started due to blocking errors - " + text2.Replace("\n", "-").Replace("\t", " "));
			if (showDialog)
			{
				MissionEditorLogic.Instance.unableToStartMission = PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("MissionStartBlocked", text2, Localizer.Format("#autoLOC_8003069"), null, 350f, new DialogGUIButton("#autoLOC_417274", null, dismissOnSelect: true)), persistAcrossScenes: false, null);
			}
		}
		return flag;
	}

	internal void MissionCriticalError(string errorString = "")
	{
		FlightDriver.SetPause(pauseState: true);
		PopupDialog.SpawnPopupDialog(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new MultiOptionDialog("MissionStartBlocked", string.IsNullOrEmpty(errorString) ? Localizer.Format("#autoLOC_8003069") : errorString, Localizer.Format("#autoLOC_8003069"), null, 350f, new DialogGUIButton("#autoLOC_417274", ContinueCriticalError, dismissOnSelect: true)), persistAcrossScenes: false, null).OnDismiss = ContinueCriticalError;
	}

	private void ContinueCriticalError()
	{
		FlightGlobals.ClearAllVessels();
		if (FlightGlobals.fetch != null && FlightGlobals.ActiveVessel != null)
		{
			FlightGlobals.fetch.activeVessel = null;
		}
		HighLogic.CurrentGame = null;
		FlightDriver.SetPause(pauseState: false);
		InputLockManager.ClearControlLocks();
		if (MissionSystem.IsTestMode)
		{
			MissionEditorLogic.StartUpMissionEditor((MissionInfo != null) ? MissionInfo.FilePath : "");
			return;
		}
		if (MissionInfo != null && Directory.Exists(MissionInfo.SaveFolderPath))
		{
			FileInfo[] files = new DirectoryInfo(MissionInfo.SaveFolderPath).GetFiles();
			for (int i = 0; i < files.Length; i++)
			{
				files[i].Delete();
			}
		}
		if (HighLogic.LoadedScene != GameScenes.MAINMENU)
		{
			HighLogic.LoadScene(GameScenes.MAINMENU);
		}
	}

	internal IEnumerator GenerateMissionLaunchSites(Callback<Mission> onComplete, bool createPQSObject = true)
	{
		generateLaunchSitesCallback = onComplete;
		if ((bool)LoadingBufferMask.Instance)
		{
			LoadingBufferMask.Instance.textInfo.text = Localizer.Format("#autoLOC_8002119");
		}
		List<string> missionLaunchSites = new List<string>();
		nodes.GetListEnumerator();
		List<MENode>.Enumerator nodeList = nodes.GetListEnumerator();
		try
		{
			while (nodeList.MoveNext())
			{
				MENode current = nodeList.Current;
				if (!current.IsLaunchPadNode || !current.IsDockedToStartNode)
				{
					continue;
				}
				ActionCreateLaunchSite actionLaunchSite = current.actionModules[0] as ActionCreateLaunchSite;
				if (!(actionLaunchSite != null))
				{
					continue;
				}
				missionLaunchSites.Add(actionLaunchSite.launchSiteSituation.LaunchSiteObjectName);
				LaunchSite launchSite = PSystemSetup.Instance.GetLaunchSite(actionLaunchSite.launchSiteSituation.LaunchSiteObjectName);
				if (launchSite != null && launchSite.IsSetup)
				{
					continue;
				}
				if (launchSite != null)
				{
					PSystemSetup.Instance.RemoveLaunchSite(actionLaunchSite.launchSiteSituation.LaunchSiteObjectName);
				}
				GClass4 sphere = null;
				Transform sphereTarget = null;
				GameObject tempObject = new GameObject("TempLocation_" + actionLaunchSite.launchSiteSituation.LaunchSiteObjectName);
				if (createPQSObject)
				{
					for (int i = 0; i < PSystemSetup.Instance.pqsArray.Length; i++)
					{
						if (PSystemSetup.Instance.pqsArray[i].gameObject.name == actionLaunchSite.launchSiteSituation.launchSiteGroundLocation.targetBody.bodyName)
						{
							sphere = PSystemSetup.Instance.pqsArray[i];
							sphereTarget = sphere.target;
							sphere.SetTarget(null);
							FloatingOrigin.SetOffset(sphere.PrecisePosition);
							Planetarium.CelestialFrame cf = default(Planetarium.CelestialFrame);
							Planetarium.CelestialFrame.SetFrame(0.0, 0.0, 0.0, ref cf);
							Vector3d vector3d = LatLon.GetSurfaceNVector(cf, actionLaunchSite.launchSiteSituation.launchSiteGroundLocation.latitude, actionLaunchSite.launchSiteSituation.launchSiteGroundLocation.longitude) * (sphere.radius + actionLaunchSite.launchSiteSituation.launchSiteGroundLocation.altitude);
							tempObject.transform.SetParent(sphere.transform);
							tempObject.transform.localPosition = vector3d;
							FloatingOrigin.SetOffset(tempObject.transform.position);
							yield return new WaitForFixedUpdate();
							break;
						}
					}
				}
				yield return StartCoroutine(actionLaunchSite.launchSiteSituation.AddLaunchSite(createPQSObject));
				if (createPQSObject)
				{
					sphere.SetTarget(sphereTarget);
				}
				UnityEngine.Object.Destroy(tempObject);
			}
		}
		finally
		{
			nodeList.Dispose();
		}
		LaunchSite[] nonStockLaunchSites = PSystemSetup.Instance.NonStockLaunchSites;
		for (int num = nonStockLaunchSites.Length - 1; num >= 0; num--)
		{
			if (!missionLaunchSites.Contains(nonStockLaunchSites[num].name))
			{
				PSystemSetup.Instance.RemoveLaunchSite(nonStockLaunchSites[num].name);
			}
		}
		if ((bool)LoadingBufferMask.Instance)
		{
			LoadingBufferMask.Instance.textInfo.text = "";
		}
		if (createPQSObject)
		{
			PSystemSetup.Instance.OnSceneChange(GameScenes.MAINMENU);
			PSystemSetup.Instance.OnLevelLoaded(GameScenes.MAINMENU);
		}
		yield return null;
		if (generateLaunchSitesCallback != null)
		{
			generateLaunchSitesCallback(this);
		}
	}

	public void StartMission()
	{
		for (int i = 0; i < orphanNodes.Count; i++)
		{
			orphanNodes[i].InitializeTestGroups();
		}
		if (activeNode == null)
		{
			InitMission();
		}
		activeNode.ActivateNode(Loading: true);
		if (!isStarted && HighLogic.CurrentGame != null)
		{
			isStarted = true;
			startedUT = HighLogic.CurrentGame.UniversalTime;
			startNode.activatedUT = startedUT;
			AnalyticsUtil.LogMissionStart(this);
			GameEvents.Mission.onStarted.Fire(this);
		}
	}

	public void ResumeMission()
	{
		if (!isStarted || isEnded || !isInitialized)
		{
			return;
		}
		for (int i = 0; i < orphanNodes.Count; i++)
		{
			orphanNodes[i].InitializeTestGroups();
		}
		if (briefingNodeActive)
		{
			MissionSystem.Instance.CheckFireBriefingDialog(this, chckMissionStarted: false);
		}
		activeNode.ActivateNode(Loading: true);
		if (HighLogic.CurrentGame == null || HighLogic.CurrentGame.Mode != Game.Modes.MISSION || !HighLogic.LoadedSceneHasPlanetarium)
		{
			return;
		}
		for (int j = 0; j < nodes.ValuesList.Count; j++)
		{
			for (int k = 0; k < nodes.ValuesList[j].actionModules.Count; k++)
			{
				ActionModule actionModule = nodes.ValuesList[j].actionModules[k] as ActionModule;
				if (actionModule != null && actionModule.isRunning && actionModule.restartOnSceneLoad)
				{
					MissionSystem.Instance.StartCoroutine(actionModule.Fire());
				}
			}
		}
	}

	public void SetStartNode(MENode newStart)
	{
		startNode = newStart;
		newStart.isStartNode = true;
	}

	public void UpdateMission()
	{
		if (activeNode == null || !isStarted || !MissionSystem.IsActive || activeNode.isPaused || briefingNodeActive)
		{
			return;
		}
		if (activeNode.isEndNode)
		{
			DoEndMissionActions();
			return;
		}
		if (switchActiveVessel)
		{
			switchActiveVessel = false;
			if (HighLogic.LoadedSceneIsFlight)
			{
				FlightGlobals.SetActiveVessel(vesselToSwitchActive);
			}
			return;
		}
		if (saveRevertOnSwitchActiveVessel)
		{
			bool flag = false;
			if (briefingNodeActive)
			{
				flag = true;
				briefingNodeActive = false;
			}
			saveRevertOnSwitchActiveVessel = false;
			string value = GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.OVERWRITE, HighLogic.LoadedScene);
			if (File.Exists(KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/lastcreatevesselspawn.missionsfs"))
			{
				File.Delete(KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/lastcreatevesselspawn.missionsfs");
			}
			if (!string.IsNullOrEmpty(value))
			{
				File.Copy(KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/persistent.sfs", KSPUtil.ApplicationRootPath + "saves/" + HighLogic.SaveFolder + "/lastcreatevesselspawn.missionsfs", overwrite: true);
			}
			else
			{
				Debug.Log("[Mission]: Unable to save revert to vessel spawn for mission as autosave is not available.");
			}
			if (flag)
			{
				briefingNodeActive = true;
			}
		}
		if (PendingVesselLaunch(activeNode))
		{
			return;
		}
		for (int i = 0; i < activeNode.toNodes.Count; i++)
		{
			MENode mENode = activeNode.toNodes[i];
			if (!mENode.RunTests())
			{
				continue;
			}
			SwitchActiveNodes(mENode);
			i = activeNode.toNodes.Count;
			for (int j = 0; j < activeNode.dockedNodes.Count; j++)
			{
				MENode mENode2 = activeNode.dockedNodes[j];
				if (mENode2.HasTestModules)
				{
					if (mENode2.RunTests())
					{
						SwitchActiveNodes(mENode2);
						i = activeNode.toNodes.Count;
					}
				}
				else
				{
					mENode2.ActivateNode();
				}
			}
			if (mENode.isEndNode && !mENode.isPaused)
			{
				DoEndMissionActions();
			}
		}
	}

	private void DoEndMissionActions()
	{
		isSuccesful = activeNode.missionEndOptions == MissionEndOptions.Success;
		if (isScoreEnabled)
		{
			globalScore.AwardScore(this);
		}
		if (isSuccesful)
		{
			if (isScoreEnabled)
			{
				awards.EvaluateAwards(this);
			}
			AnalyticsUtil.LogMissionCompleted(this);
			GameEvents.Mission.onCompleted.Fire(this);
		}
		else
		{
			AnalyticsUtil.LogMissionFailed(this);
			GameEvents.Mission.onFailed.Fire(this);
		}
		GameEvents.Mission.onFinished.Fire(this);
		isEnded = true;
		GameEvents.onEditorStarted.Remove(OnEditorStarted);
	}

	private void SwitchActiveNodes(MENode newNode)
	{
		MENode mENode = activeNode;
		GameEvents.Mission.onActiveNodeChanging.Fire(this, new GameEvents.FromToAction<MENode, MENode>(mENode, newNode));
		mENode.DeactivateNode();
		activeNode = newNode.ActivateNode();
		GameEvents.Mission.onActiveNodeChanged.Fire(this, new GameEvents.FromToAction<MENode, MENode>(mENode, newNode));
	}

	internal void SetNextObjectives(MENode node)
	{
		if (nextObjectivesNodes == null)
		{
			nextObjectivesNodes = new List<MENode>();
		}
		else
		{
			nextObjectivesNodes.Clear();
		}
		if (!flow.NodePaths.ContainsKey(node))
		{
			return;
		}
		MENodePathInfo mENodePathInfo = flow.NodePaths[node];
		for (int i = 0; i < mENodePathInfo.paths.Count; i++)
		{
			MEPath mEPath = mENodePathInfo.paths[i];
			for (int j = 0; j < mEPath.Nodes.Count && (!mEPath.Nodes[j].isEvent || mEPath.Nodes[j].HasBeenActivated); j++)
			{
				if (mEPath.Nodes[j].isObjective)
				{
					nextObjectivesNodes.AddUnique(mEPath.Nodes[j]);
					break;
				}
			}
		}
	}

	public bool IsNextObjective(MENode node)
	{
		if (nextObjectivesNodes != null && nextObjectivesNodes.Contains(node))
		{
			return true;
		}
		return false;
	}

	internal void RemoveInactiveEvent(MENode node)
	{
		if (inactiveEventNodes != null && inactiveEventNodes.Contains(node))
		{
			inactiveEventNodes.Remove(node);
		}
	}

	internal void UpdateOrphanNodeState(MENode node, bool makeOrphan)
	{
		if (makeOrphan && node.IsOrphanNode)
		{
			for (int i = 0; i < nodes.Count; i++)
			{
				if (nodes.ValuesList[i] != node)
				{
					nodes.ValuesList[i].toNodes.AddUnique(node);
				}
			}
			orphanNodes.AddUnique(node);
			return;
		}
		for (int j = 0; j < nodes.Count; j++)
		{
			if (!node.fromNodes.Contains(nodes.ValuesList[j]))
			{
				nodes.ValuesList[j].toNodes.Remove(node);
			}
		}
		orphanNodes.Remove(node);
	}

	public string PrintObjectives(bool onlyPrintActivatedNodes, bool startWithActiveNode)
	{
		return PrintObjectives(onlyPrintActivatedNodes, startWithActiveNode, onlyPrintScore: false, onlyAwardedScores: false);
	}

	public string PrintScoreObjectives(bool onlyPrintActivatedNodes, bool startWithActiveNode, bool onlyAwardedScores)
	{
		return PrintObjectives(onlyPrintActivatedNodes, startWithActiveNode, onlyPrintScore: true, onlyAwardedScores);
	}

	private string PrintObjectives(bool onlyPrintActivatedNodes, bool startWithActiveNode, bool onlyPrintScore, bool onlyAwardedScores)
	{
		string objectiveString = "";
		List<MENode> visitedNodesList = new List<MENode>();
		if (startWithActiveNode)
		{
			visitedNodesList.Add(activeNode);
			for (int i = 0; i < activeNode.toNodes.Count; i++)
			{
				if (onlyPrintScore)
				{
					RecursivelyCreateObjectiveScoreString(activeNode.toNodes[i], ref objectiveString, onlyPrintActivatedNodes, onlyAwardedScores, ref visitedNodesList, rejectOrphanNodes: true);
				}
				else
				{
					RecursivelyCreateObjectiveString(activeNode.toNodes[i], ref objectiveString, "", onlyPrintActivatedNodes, ref visitedNodesList, rejectOrphanNodes: true);
				}
			}
		}
		else if (onlyPrintScore)
		{
			RecursivelyCreateObjectiveScoreString(startNode, ref objectiveString, onlyPrintActivatedNodes, onlyAwardedScores, ref visitedNodesList, rejectOrphanNodes: true);
		}
		else
		{
			RecursivelyCreateObjectiveString(startNode, ref objectiveString, "", onlyPrintActivatedNodes, ref visitedNodesList, rejectOrphanNodes: true);
		}
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		while (listEnumerator.MoveNext())
		{
			MENode current = listEnumerator.Current;
			if (current.fromNodeIDs.Count == 0 && !current.isStartNode)
			{
				if (onlyPrintScore)
				{
					RecursivelyCreateObjectiveScoreString(current, ref objectiveString, onlyPrintActivatedNodes, onlyAwardedScores, ref visitedNodesList, rejectOrphanNodes: false);
				}
				else
				{
					RecursivelyCreateObjectiveString(current, ref objectiveString, "", onlyPrintActivatedNodes, ref visitedNodesList, rejectOrphanNodes: false);
				}
			}
		}
		if (objectiveString == "")
		{
			objectiveString = "#autoLOC_8100310";
		}
		return objectiveString;
	}

	private void RecursivelyCreateObjectiveString(MENode currentNode, ref string objectiveString, string indent, bool onlyPrintActivatedNodes, ref List<MENode> visitedNodesList, bool rejectOrphanNodes)
	{
		if (currentNode == null || (onlyPrintActivatedNodes && !currentNode.isStartNode && !currentNode.HasBeenActivated) || (rejectOrphanNodes && currentNode.IsOrphanNode))
		{
			return;
		}
		if (currentNode.isObjective)
		{
			StringBuilder stringBuilder = StringBuilderCache.Acquire();
			stringBuilder.Append(indent).Append("<color=#EDFEAAFF>• ").Append(Localizer.Format(currentNode.ObjectiveString))
				.Append("</color> ");
			if (currentNode.HasBeenActivated)
			{
				stringBuilder.Append(Localizer.Format("#autoLOC_8001017", KSPUtil.PrintTimeCompact(currentNode.activatedUT, explicitPositive: false)));
			}
			stringBuilder.Append("\n");
			objectiveString += stringBuilder.ToStringAndRelease();
			indent += "\t";
		}
		visitedNodesList.Add(currentNode);
		for (int i = 0; i < currentNode.toNodes.Count; i++)
		{
			if (!visitedNodesList.Contains(currentNode.toNodes[i]))
			{
				RecursivelyCreateObjectiveString(currentNode.toNodes[i], ref objectiveString, indent, onlyPrintActivatedNodes, ref visitedNodesList, rejectOrphanNodes: true);
			}
		}
	}

	private void RecursivelyCreateObjectiveScoreString(MENode currentNode, ref string objectiveString, bool onlyPrintActivatedNodes, bool onlyAwardedScores, ref List<MENode> visitedNodesList, bool rejectOrphanNodes)
	{
		if (currentNode == null || (onlyPrintActivatedNodes && !currentNode.isStartNode && !currentNode.HasBeenActivated) || (rejectOrphanNodes && currentNode.IsOrphanNode))
		{
			return;
		}
		bool flag = false;
		if (currentNode.isObjective)
		{
			foreach (MENode dockedNode in currentNode.dockedNodes)
			{
				if (dockedNode.toNodes.Count < 1 || dockedNode.toNodes[0].actionModules.Count <= 0 || (!(dockedNode.toNodes[0].actionModules[0].GetType() == typeof(ActionMissionScore)) && !dockedNode.toNodes[0].actionModules[0].GetType().IsSubclassOf(typeof(ActionMissionScore))))
				{
					continue;
				}
				StringBuilder stringBuilder = StringBuilderCache.Acquire();
				if (!flag)
				{
					stringBuilder.Append("<color=#EDFEAAFF>• ").Append(Localizer.Format(currentNode.ObjectiveString)).Append("</color> ");
					if (currentNode.HasBeenActivated)
					{
						stringBuilder.Append(Localizer.Format("#autoLOC_8001017", KSPUtil.PrintTimeCompact(currentNode.activatedUT, explicitPositive: false)));
					}
				}
				if (onlyAwardedScores && dockedNode.HasBeenActivated)
				{
					stringBuilder.Append("\n" + (dockedNode.toNodes[0].actionModules[0] as ActionMissionScore).GetAwardedScoreDescription());
				}
				else if (!onlyAwardedScores)
				{
					stringBuilder.Append((dockedNode.toNodes[0].actionModules[0] as ActionMissionScore).GetScoreDescription());
				}
				visitedNodesList.Add(dockedNode.toNodes[0]);
				objectiveString += stringBuilder.ToStringAndRelease();
				flag = true;
			}
			if (flag)
			{
				objectiveString += "\n\n";
			}
			if (!flag && onlyAwardedScores && (currentNode.actionModules.Count <= 0 || (!(currentNode.actionModules[0].GetType() == typeof(ActionMissionScore)) && !currentNode.actionModules[0].GetType().IsSubclassOf(typeof(ActionMissionScore)))))
			{
				StringBuilder stringBuilder2 = StringBuilderCache.Acquire();
				stringBuilder2.Append("<color=#EDFEAAFF>• ").Append(Localizer.Format(currentNode.ObjectiveString)).Append("</color> ");
				if (currentNode.HasBeenActivated)
				{
					stringBuilder2.Append(Localizer.Format("#autoLOC_8001017", KSPUtil.PrintTimeCompact(currentNode.activatedUT, explicitPositive: false)));
				}
				stringBuilder2.Append("\n\n");
				objectiveString += stringBuilder2.ToStringAndRelease();
			}
		}
		if (!flag && currentNode.dockParentNode == null && currentNode.actionModules.Count > 0 && (currentNode.actionModules[0].GetType() == typeof(ActionMissionScore) || currentNode.actionModules[0].GetType().IsSubclassOf(typeof(ActionMissionScore))))
		{
			StringBuilder stringBuilder3 = StringBuilderCache.Acquire();
			if (onlyAwardedScores && currentNode.HasBeenActivated)
			{
				stringBuilder3.Append("<color=#EDFEAAFF>• ").Append(Localizer.Format(currentNode.ObjectiveString)).Append("</color>");
				stringBuilder3.Append("\n" + (currentNode.actionModules[0] as ActionMissionScore).GetAwardedScoreDescription());
				stringBuilder3.Append("\n\n");
			}
			else if (!onlyAwardedScores)
			{
				stringBuilder3.Append("<color=#EDFEAAFF>• ").Append(Localizer.Format(currentNode.ObjectiveString)).Append("</color>");
				stringBuilder3.Append((currentNode.actionModules[0] as ActionMissionScore).GetScoreDescription());
				stringBuilder3.Append("\n\n");
			}
			objectiveString += stringBuilder3.ToStringAndRelease();
		}
		visitedNodesList.Add(currentNode);
		for (int i = 0; i < currentNode.toNodes.Count; i++)
		{
			if (!visitedNodesList.Contains(currentNode.toNodes[i]))
			{
				RecursivelyCreateObjectiveScoreString(currentNode.toNodes[i], ref objectiveString, onlyPrintActivatedNodes, onlyAwardedScores, ref visitedNodesList, rejectOrphanNodes: true);
			}
		}
	}

	private void OnEditorStarted()
	{
		if ((bool)ResearchAndDevelopment.Instance && HighLogic.CurrentGame != null && (HighLogic.CurrentGame.Mode == Game.Modes.MISSION || HighLogic.CurrentGame.Mode == Game.Modes.MISSION_BUILDER))
		{
			ResearchAndDevelopment.Instance.SetScience(1337f, TransactionReasons.Cheating);
			ResearchAndDevelopment.Instance.CheatTechnology();
		}
	}

	internal MissionFileInfo UpdateMissionFileInfo(string MissionFilePath)
	{
		MissionInfo = MissionFileInfo.CreateFromPath(MissionFilePath);
		return MissionInfo;
	}

	public void RebuildCraftFileList()
	{
		ClearCraftFiles();
		processcraftFileFolder(ShipsPath + "VAB/");
		processcraftFileFolder(ShipsPath + "SPH/");
	}

	private void ClearCraftFiles()
	{
		if (craftFileList != null && craftFileList.Count > 0)
		{
			for (int i = 0; i < craftFileList.Count; i++)
			{
				craftFileList.At(i).Clear();
			}
			craftFileList.Clear();
		}
	}

	private void processcraftFileFolder(string saveShipFolderPath)
	{
		if (!Directory.Exists(saveShipFolderPath))
		{
			return;
		}
		string[] files = Directory.GetFiles(saveShipFolderPath, "*.craft");
		for (int i = 0; i < files.Length; i++)
		{
			string fileName = Path.GetFileName(files[i]);
			MissionCraft val = new MissionCraft(saveShipFolderPath, fileName);
			if (!craftFileList.ContainsKey(fileName))
			{
				craftFileList.Add(fileName, val);
			}
		}
	}

	public MissionCraft GetCraftBySituationsVesselID(uint PersistentId)
	{
		DictionaryValueList<VesselSituation, Guid> allVesselSituationsGuid = GetAllVesselSituationsGuid();
		int num = 0;
		while (true)
		{
			if (num < allVesselSituationsGuid.Count)
			{
				if (allVesselSituationsGuid.KeyAt(num).persistentId == PersistentId)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		if (craftFileList.ContainsKey(allVesselSituationsGuid.KeyAt(num).craftFile))
		{
			return craftFileList[allVesselSituationsGuid.KeyAt(num).craftFile];
		}
		return null;
	}

	public MissionCraft GetMissionCraftByFileName(string craftFileName)
	{
		MissionCraft val = null;
		craftFileList.TryGetValue(craftFileName, out val);
		return val;
	}

	public MissionCraft GetMissionCraftByName(string vesselName)
	{
		int num = 0;
		while (true)
		{
			if (num < craftFileList.Count)
			{
				if (craftFileList.At(num).VesselName == vesselName)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return craftFileList.At(num);
	}

	public int GetSituationsIndexByVessel(uint persistentId)
	{
		List<VesselSituation> allVesselSituations = GetAllVesselSituations();
		int num = 0;
		while (true)
		{
			if (num < allVesselSituations.Count)
			{
				if (allVesselSituations[num].persistentId == persistentId)
				{
					break;
				}
				num++;
				continue;
			}
			return -1;
		}
		return num;
	}

	public int GetSituationsIndexByPart(uint persistentId)
	{
		List<VesselSituation> allVesselSituations = GetAllVesselSituations();
		for (int i = 0; i < allVesselSituations.Count; i++)
		{
			MissionCraft missionCraftByFileName = GetMissionCraftByFileName(allVesselSituations[i].craftFile);
			if (missionCraftByFileName == null)
			{
				continue;
			}
			ConfigNode[] array = missionCraftByFileName.CraftNode.GetNodes("PART");
			int j = 0;
			for (int num = array.Length; j < num; j++)
			{
				if (missionCraftByFileName.GetPartPersistentId(array[j]) == persistentId)
				{
					return i;
				}
			}
		}
		return -1;
	}

	public int GetCraftFileIndexByPart(uint persistentId)
	{
		int i = 0;
		for (int count = craftFileList.Count; i < count; i++)
		{
			if (craftFileList.At(i).CraftNode == null)
			{
				continue;
			}
			ConfigNode[] array = craftFileList.At(i).CraftNode.GetNodes("PART");
			int j = 0;
			for (int num = array.Length; j < num; j++)
			{
				if (craftFileList.At(i).GetPartPersistentId(array[j]) == persistentId)
				{
					return i;
				}
			}
		}
		return 0;
	}

	public void SwitchActiveVessel(Vessel vessel, bool saveRevertOnSwitch = false, bool switchImmediately = false)
	{
		if (HighLogic.LoadedSceneIsFlight && vessel != null)
		{
			if (switchImmediately)
			{
				FlightGlobals.ForceSetActiveVessel(MissionSystem.Instance.returnVessel);
				saveRevertOnSwitchActiveVessel = saveRevertOnSwitch;
			}
			else
			{
				vesselToSwitchActive = vessel;
				switchActiveVessel = true;
				saveRevertOnSwitchActiveVessel = saveRevertOnSwitch;
			}
		}
	}

	public void SetLockedCamera(MissionCameraModeOptions newCameraMode, MissionCameraLockOptions newCameraLock)
	{
		cameraLockMode = newCameraMode;
		cameraLockOptions = newCameraLock;
		MissionSystem.SetLockedCamera(this, cameraLockMode, cameraLockOptions);
	}

	public bool Export(string exportFileName, bool overwrite = false)
	{
		if (MissionInfo == null)
		{
			Debug.LogError("Unable to Export Mission (" + Localizer.Format(title) + "). It hasn't been saved yet!");
			return false;
		}
		if (!Directory.Exists(MissionsUtils.MissionExportsPath))
		{
			Directory.CreateDirectory(MissionsUtils.MissionExportsPath);
		}
		KSPCompression.CompressDirectory(MissionInfo.FolderPath, MissionsUtils.MissionExportsPath + exportFileName, includeTopLevelFolder: true, overwrite);
		missionNameAtLastExport = base.name;
		return true;
	}

	public void Load(ConfigNode node)
	{
		Load(node, simple: false);
	}

	public void Load(ConfigNode node, bool simple = false)
	{
		if (node == null)
		{
			return;
		}
		hasBeenValidated = false;
		ConfigNode configNode = new ConfigNode();
		if (!(node.name == "MISSION") && !(node.name == "MISSIONTOSTART"))
		{
			if (node.HasNode("MISSION"))
			{
				configNode = node.GetNode("MISSION");
			}
		}
		else
		{
			configNode = node;
		}
		if (configNode.HasValue("id"))
		{
			id = new Guid(configNode.GetValue("id"));
		}
		if (configNode.HasValue("idName"))
		{
			idName = configNode.GetValue("idName");
		}
		configNode.TryGetValue("expansionVersion", ref expansionVersion);
		string value = "";
		configNode.TryGetValue("title", ref value);
		title = value;
		configNode.TryGetValue("briefing", ref briefing);
		configNode.TryGetValue("author", ref author);
		configNode.TryGetValue("modsBriefing", ref modsBriefing);
		configNode.TryGetValue("isBriefingSet", ref isBriefingSet);
		configNode.TryGetValue("packName", ref packName);
		configNode.TryGetValue("order", ref order);
		configNode.TryGetValue("hardIcon", ref hardIcon);
		configNode.TryGetEnum("difficulty", ref difficulty, hardIcon ? MissionDifficulty.Advanced : MissionDifficulty.Beginner);
		if (!configNode.TryGetValue("seed", ref seed))
		{
			InitSeed();
		}
		configNode.TryGetValue("historyId", ref historyId);
		configNode.TryGetValue("steamPublishedFileId", ref steamPublishedFileId);
		configNode.TryGetValue("flag", ref flagURL);
		string value2 = string.Empty;
		configNode.TryGetValue("bannerMenu", ref value2);
		bannerMenu.LoadFromMissionFolder(value2, MEBannerType.Menu, this);
		string value3 = string.Empty;
		configNode.TryGetValue("bannerSuccess", ref value3);
		bannerSuccess.LoadFromMissionFolder(value3, MEBannerType.Success, this);
		string value4 = string.Empty;
		configNode.TryGetValue("bannerFail", ref value4);
		configNode.TryGetValue("isScoreEnabled", ref isScoreEnabled);
		configNode.TryGetValue("maxScore", ref maxScore);
		bannerFail.LoadFromMissionFolder(value4, MEBannerType.Fail, this);
		configNode.TryGetValue("activeNodeID", ref loadActiveNodeID);
		configNode.TryGetValue("currentScore", ref currentScore);
		configNode.TryGetValue("exportName", ref exportName);
		configNode.TryGetValue("missionNameAtLastExport", ref missionNameAtLastExport);
		if (configNode.HasValue("isStarted"))
		{
			isStarted = Convert.ToBoolean(configNode.GetValue("isStarted"));
		}
		if (configNode.HasValue("isEnded"))
		{
			isEnded = Convert.ToBoolean(configNode.GetValue("isEnded"));
		}
		if (configNode.HasValue("isSuccesful"))
		{
			isSuccesful = Convert.ToBoolean(configNode.GetValue("isSuccesful"));
		}
		configNode.TryGetEnum("cameraLockMode", ref cameraLockMode, MissionCameraModeOptions.NoChange);
		configNode.TryGetEnum("cameraLockOptions", ref cameraLockOptions, MissionCameraLockOptions.NoChange);
		configNode.TryGetValue("saveRevertOnSwitchActiveVessel", ref saveRevertOnSwitchActiveVessel);
		ConfigNode node2 = new ConfigNode();
		if (configNode.TryGetNode("TAGS", ref node2))
		{
			ConfigNode[] array = node2.GetNodes("TAG");
			for (int i = 0; i < array.Length; i++)
			{
				string value5 = "";
				array[i].TryGetValue("name", ref value5);
				tags.Add(value5);
			}
		}
		configNode.TryGetValue("briefingNodeActive", ref briefingNodeActive);
		globalScore.Load(configNode);
		awards.Load(configNode);
		if (configNode.HasNode("NODES"))
		{
			ConfigNode node3 = new ConfigNode();
			if (configNode.TryGetNode("SITUATION", ref node3))
			{
				situation.Load(node3);
				if (expansionVersion == "" && node3.HasValue("version"))
				{
					expansionVersion = "1.2.0";
				}
			}
			ConfigNode node4 = new ConfigNode();
			if (configNode.TryGetNode("NODES", ref node4))
			{
				inactiveEventNodes = new List<MENode>();
				ConfigNode[] array2 = node4.GetNodes("NODE");
				foreach (ConfigNode node5 in array2)
				{
					MENode mENode = MENode.Spawn(this);
					mENode.Load(node5);
					if (mENode.isEvent && !mENode.HasBeenActivated)
					{
						inactiveEventNodes.AddUnique(mENode);
					}
					nodes.Add(mENode.id, mENode);
				}
			}
			if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Mode == Game.Modes.MISSION)
			{
				ConfigNode[] array2 = situation.vesselsToBuildNode.GetNodes("VESSELTOBUILD");
				foreach (ConfigNode obj in array2)
				{
					Guid value6 = Guid.Empty;
					obj.TryGetValue("NodeGuid", ref value6);
					if (!(value6 != Guid.Empty))
					{
						continue;
					}
					MENode nodeById = GetNodeById(value6);
					if (nodeById != null)
					{
						ActionCreateVessel actionCreateVessel = GetActionCreateVessel(nodeById);
						if (actionCreateVessel != null)
						{
							situation.vesselSituationList.Add(actionCreateVessel.vesselSituation, value6);
						}
					}
				}
				array2 = situation.startingActionsNode.GetNodes("STARTINGACTION");
				foreach (ConfigNode obj2 in array2)
				{
					Guid value7 = Guid.Empty;
					obj2.TryGetValue("NodeGuid", ref value7);
					if (!(value7 != Guid.Empty))
					{
						continue;
					}
					MENode nodeById2 = GetNodeById(value7);
					if (!(nodeById2 != null))
					{
						continue;
					}
					ActionCreateKerbal actionCreateKerbal = GetActionCreateKerbal(nodeById2);
					if (actionCreateKerbal != null)
					{
						situation.startingActions.Add(actionCreateKerbal);
						continue;
					}
					ActionCreateAsteroid actionCreateAsteroid = GetActionCreateAsteroid(nodeById2);
					if (actionCreateAsteroid != null)
					{
						situation.startingActions.Add(actionCreateAsteroid);
						continue;
					}
					ActionCreateFlag actionCreateFlag = GetActionCreateFlag(nodeById2);
					if (actionCreateFlag != null)
					{
						situation.startingActions.Add(actionCreateFlag);
					}
				}
			}
			if (!simple)
			{
				RebuildCraftFileList();
			}
			SetStartNode();
			SetActiveNode();
			SetDockedNodes();
			if (HighLogic.CurrentGame != null && HighLogic.CurrentGame.Mode == Game.Modes.MISSION && MissionInfo == null)
			{
				string text = "";
				if (MissionSystem.IsTestMode && !string.IsNullOrEmpty(MissionEditorLogic.MissionToTest))
				{
					text = MissionEditorLogic.MissionToTest;
				}
				else
				{
					string text2 = HighLogic.SaveFolder.Substring(MissionsUtils.SavesPath.Length);
					text = ((packName == null || !packName.StartsWith("squad_")) ? MissionsUtils.UsersMissionsPath : MissionsUtils.StockMissionsPath) + text2 + "/persistent.mission";
					if (!File.Exists(text))
					{
						text = ((packName == null || !packName.StartsWith("squad_")) ? MissionsUtils.StockMissionsPath : MissionsUtils.UsersMissionsPath) + text2 + "/persistent.mission";
					}
					if (!File.Exists(text))
					{
						text = SteamManager.KSPSteamWorkshopFolder + text2 + "/persistent.mission";
					}
				}
				if (File.Exists(text))
				{
					UpdateMissionFileInfo(text);
				}
				else
				{
					Debug.LogError("Failed to load mission. Unable to find Mission folder");
				}
			}
			if (configNode.HasNode("FLOW"))
			{
				try
				{
					flow = new MissionFlow(this);
					flow.Load(configNode.GetNode("FLOW"));
					SetNextObjectives(activeNode);
				}
				catch (Exception)
				{
					Debug.LogWarning("Unable to load mission flow from ConfigNode");
				}
			}
		}
		else
		{
			Debug.LogError("Unable to load mission from ConfigNode");
		}
	}

	public void Save(ConfigNode node)
	{
		node.AddValue("id", id.ToString());
		if (!string.IsNullOrEmpty(idName))
		{
			node.AddValue("idName", idName);
		}
		node.AddValue("expansionVersion", ExpansionsLoader.GetExpansionVersion("MakingHistory"));
		node.AddValue("title", title);
		string text = briefing.Replace("\n", "\\n");
		text = text.Replace("\t", "\\t");
		node.AddValue("briefing", text);
		node.AddValue("author", author);
		string text2 = modsBriefing.Replace("\n", "\\n");
		text2 = text2.Replace("\t", "\\t");
		node.AddValue("modsBriefing", text2);
		if (!string.IsNullOrEmpty(packName))
		{
			node.AddValue("packName", packName);
		}
		if (order < int.MaxValue)
		{
			node.AddValue("order", order);
		}
		node.AddValue("hardIcon", hardIcon);
		node.AddValue("difficulty", difficulty);
		node.AddValue("isBriefingSet", isBriefingSet);
		node.AddValue("seed", seed);
		node.AddValue("flag", flagURL);
		node.AddValue("bannerMenu", bannerMenu.fileName);
		node.AddValue("bannerSuccess", bannerSuccess.fileName);
		node.AddValue("bannerFail", bannerFail.fileName);
		node.AddValue("isScoreEnabled", isScoreEnabled);
		node.AddValue("maxScore", maxScore);
		node.AddValue("steamPublishedFileId", steamPublishedFileId);
		node.AddValue("historyId", historyId);
		if (activeNode != null)
		{
			node.AddValue("activeNodeID", activeNode.id);
		}
		node.AddValue("currentScore", currentScore);
		node.AddValue("exportName", exportName);
		node.AddValue("missionNameAtLastExport", missionNameAtLastExport);
		node.AddValue("isStarted", isStarted);
		node.AddValue("isEnded", isEnded);
		node.AddValue("isSuccesful", isSuccesful);
		node.AddValue("briefingNodeActive", briefingNodeActive);
		node.AddValue("cameraLockMode", cameraLockMode);
		node.AddValue("cameraLockOptions", cameraLockOptions);
		if (saveRevertOnSwitchActiveVessel)
		{
			node.AddValue("saveRevertOnSwitchActiveVessel", saveRevertOnSwitchActiveVessel);
		}
		ConfigNode node2 = node.AddNode("SITUATION");
		situation.Save(node2);
		globalScore.Save(node);
		awards.Save(node);
		ConfigNode configNode = node.AddNode("NODES");
		foreach (MENode value in nodes.Values)
		{
			ConfigNode node3 = configNode.AddNode("NODE");
			value.Save(node3);
		}
		ConfigNode configNode2 = node.AddNode("TAGS");
		for (int i = 0; i < tags.Count; i++)
		{
			configNode2.AddNode("TAG").AddValue("name", tags[i]);
		}
		ConfigNode node4 = node.AddNode("FLOW");
		flow.Save(node4);
	}

	public MENode GetNodeById(Guid guid)
	{
		int num = 0;
		while (true)
		{
			if (num < nodes.Count)
			{
				if (nodes.KeyAt(num) == guid)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return nodes.At(num);
	}

	public Guid GetNodeGuidByVesselID(uint persistentId)
	{
		DictionaryValueList<VesselSituation, Guid> allVesselSituationsGuid = GetAllVesselSituationsGuid();
		int num = 0;
		while (true)
		{
			if (num < allVesselSituationsGuid.Count)
			{
				if (allVesselSituationsGuid.KeyAt(num).persistentId == persistentId)
				{
					break;
				}
				num++;
				continue;
			}
			return Guid.Empty;
		}
		return allVesselSituationsGuid.At(num);
	}

	public bool PendingVesselLaunch(MENode node)
	{
		return node.HasPendingVesselLaunch;
	}

	public VesselSituation GetVesselSituationByVesselID(uint PersistentId)
	{
		Guid nodeGuidByVesselID = GetNodeGuidByVesselID(PersistentId);
		if (nodeGuidByVesselID != Guid.Empty)
		{
			MENode nodeById = GetNodeById(nodeGuidByVesselID);
			if (nodeById != null)
			{
				ActionCreateVessel actionCreateVessel = GetActionCreateVessel(nodeById);
				if (actionCreateVessel != null)
				{
					return actionCreateVessel.vesselSituation;
				}
			}
		}
		return null;
	}

	public Asteroid GetAsteroidByPersistentID(uint PersistentId)
	{
		List<ActionCreateAsteroid> allActionModules = GetAllActionModules<ActionCreateAsteroid>();
		int num = 0;
		while (true)
		{
			if (num < allActionModules.Count)
			{
				if (allActionModules[num].asteroid.persistentId == PersistentId)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return allActionModules[num].asteroid;
	}

	public ActionCreateFlag GetActionCreateFlagByPersistentID(uint PersistentId)
	{
		List<ActionCreateFlag> allActionModules = GetAllActionModules<ActionCreateFlag>();
		int num = 0;
		while (true)
		{
			if (num < allActionModules.Count)
			{
				if (allActionModules[num].persistentID == PersistentId)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return allActionModules[num];
	}

	public ActionCreateVessel GetActionCreateVessel(MENode node)
	{
		List<IActionModule> actionModules = node.actionModules;
		int num = 0;
		while (true)
		{
			if (num < actionModules.Count)
			{
				if (actionModules[num] is ActionCreateVessel)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return actionModules[num] as ActionCreateVessel;
	}

	public ActionCreateKerbal GetActionCreateKerbal(MENode node)
	{
		List<IActionModule> actionModules = node.actionModules;
		int num = 0;
		while (true)
		{
			if (num < actionModules.Count)
			{
				if (actionModules[num] is ActionCreateKerbal)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return actionModules[num] as ActionCreateKerbal;
	}

	public ActionCreateAsteroid GetActionCreateAsteroid(MENode node)
	{
		List<IActionModule> actionModules = node.actionModules;
		int num = 0;
		while (true)
		{
			if (num < actionModules.Count)
			{
				if (actionModules[num] is ActionCreateAsteroid)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return actionModules[num] as ActionCreateAsteroid;
	}

	public ActionCreateFlag GetActionCreateFlag(MENode node)
	{
		List<IActionModule> actionModules = node.actionModules;
		int num = 0;
		while (true)
		{
			if (num < actionModules.Count)
			{
				if (actionModules[num] is ActionCreateFlag)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return actionModules[num] as ActionCreateFlag;
	}

	public List<VesselSituation> GetAllVesselSituations()
	{
		List<VesselSituation> list = new List<VesselSituation>();
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		while (listEnumerator.MoveNext())
		{
			List<IActionModule> actionModules = listEnumerator.Current.actionModules;
			if (actionModules == null)
			{
				continue;
			}
			for (int i = 0; i < actionModules.Count; i++)
			{
				if (actionModules[i].GetType() == typeof(ActionCreateVessel))
				{
					ActionCreateVessel actionCreateVessel = actionModules[i] as ActionCreateVessel;
					if (actionCreateVessel != null)
					{
						list.Add(actionCreateVessel.vesselSituation);
					}
				}
			}
		}
		return list;
	}

	public DictionaryValueList<VesselSituation, Guid> GetAllVesselSituationsGuid()
	{
		DictionaryValueList<VesselSituation, Guid> dictionaryValueList = new DictionaryValueList<VesselSituation, Guid>();
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		while (listEnumerator.MoveNext())
		{
			List<IActionModule> actionModules = listEnumerator.Current.actionModules;
			if (actionModules == null)
			{
				continue;
			}
			for (int i = 0; i < actionModules.Count; i++)
			{
				if (actionModules[i].GetType() == typeof(ActionCreateVessel))
				{
					ActionCreateVessel actionCreateVessel = actionModules[i] as ActionCreateVessel;
					if (actionCreateVessel != null)
					{
						dictionaryValueList.Add(actionCreateVessel.vesselSituation, listEnumerator.Current.id);
					}
				}
			}
		}
		return dictionaryValueList;
	}

	public List<T> GetAllActionModules<T>() where T : ActionModule
	{
		List<T> list = new List<T>();
		Type typeFromHandle = typeof(T);
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		while (listEnumerator.MoveNext())
		{
			List<IActionModule> actionModules = listEnumerator.Current.actionModules;
			if (actionModules == null)
			{
				continue;
			}
			for (int i = 0; i < actionModules.Count; i++)
			{
				if (actionModules[i].GetType() == typeFromHandle)
				{
					T val = actionModules[i] as T;
					if (val != null)
					{
						list.Add(val);
					}
				}
			}
		}
		return list;
	}

	public List<T> GetAllTestModules<T>() where T : TestModule
	{
		List<T> list = new List<T>();
		Type typeFromHandle = typeof(T);
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		while (listEnumerator.MoveNext())
		{
			List<TestGroup> testGroups = listEnumerator.Current.testGroups;
			if (testGroups == null)
			{
				continue;
			}
			for (int i = 0; i < testGroups.Count; i++)
			{
				List<ITestModule> testModules = listEnumerator.Current.testGroups[i].testModules;
				if (testModules == null)
				{
					continue;
				}
				for (int j = 0; j < testModules.Count; j++)
				{
					if (testModules[j].GetType() == typeFromHandle)
					{
						T val = testModules[j] as T;
						if (val != null)
						{
							list.Add(val);
						}
					}
				}
			}
		}
		return list;
	}

	public bool MissionHasLaunchSite(string name)
	{
		List<MENode>.Enumerator listEnumerator = nodes.GetListEnumerator();
		while (true)
		{
			if (listEnumerator.MoveNext())
			{
				if (listEnumerator.Current.IsDockedToStartNode && listEnumerator.Current.IsLaunchPadNode)
				{
					ActionCreateLaunchSite actionCreateLaunchSite = listEnumerator.Current.actionModules[0] as ActionCreateLaunchSite;
					if (actionCreateLaunchSite != null && actionCreateLaunchSite.launchSiteSituation.launchSiteName == name)
					{
						break;
					}
				}
				continue;
			}
			return false;
		}
		return true;
	}

	public MEBannerEntry GetBanner(MEBannerType bannerType)
	{
		MEBannerEntry result = null;
		switch (bannerType)
		{
		case MEBannerType.Menu:
			result = bannerMenu;
			break;
		case MEBannerType.Success:
			result = bannerSuccess;
			break;
		case MEBannerType.Fail:
			result = bannerFail;
			break;
		}
		return result;
	}

	public void SetBanner(MEBannerEntry newBanner, MEBannerType bannerType)
	{
		if (newBanner != null)
		{
			switch (bannerType)
			{
			case MEBannerType.Menu:
				bannerMenu = newBanner;
				break;
			case MEBannerType.Success:
				bannerSuccess = newBanner;
				break;
			case MEBannerType.Fail:
				bannerFail = newBanner;
				break;
			}
		}
	}
}
