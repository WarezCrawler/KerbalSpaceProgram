using System;
using System.Collections;
using System.Collections.Generic;
using Expansions;
using FinePrint;
using FinePrint.Contracts;
using FinePrint.Utilities;
using UnityEngine;

namespace Contracts;

[KSPScenario((ScenarioCreationOptions)96, new GameScenes[]
{
	GameScenes.FLIGHT,
	GameScenes.TRACKSTATION,
	GameScenes.SPACECENTER,
	GameScenes.EDITOR
})]
public class ContractSystem : ScenarioModule
{
	public static bool loaded;

	public static List<Type> ContractTypes = null;

	public static List<Type> MandatoryTypes = null;

	public static List<Type> ParameterTypes = null;

	public static List<Type> PredicateTypes = null;

	[SerializeField]
	private int finishedContractIDCheck = 5;

	[SerializeField]
	private List<Contract> contracts = new List<Contract>();

	[SerializeField]
	private List<Contract> contractsFinished = new List<Contract>();

	[SerializeField]
	private double lastUpdate = -1000.0;

	[SerializeField]
	private string version = "-1";

	public static int generateContractIterations = 50;

	private static float maxWarpFactorForUpdate = 100f;

	private static double updateInterval = 10.0;

	private bool updateDaemonRunning;

	private bool requireRefresh;

	public static Dictionary<string, int> ContractWeights = null;

	private static int WeightDefault = 30;

	private static int WeightMinimum = 10;

	private static int WeightMaximum = 90;

	private static int WeightAcceptDelta = 12;

	private static int WeightDeclineDelta = -8;

	private static int WeightWithdrawReadDelta = -2;

	private static int WeightWithdrawSeenDelta = -1;

	public static ContractSystem Instance { get; private set; }

	public List<Contract> Contracts => contracts;

	public List<Contract> ContractsFinished => contractsFinished;

	private static void GenerateTypes()
	{
		GenerateContractTypes();
		GenerateParameterTypes();
		GeneratePredicateTypes();
		GenerateMandatoryTypes();
	}

	private static void GenerateContractTypes()
	{
		if (ContractTypes != null)
		{
			return;
		}
		ContractTypes = new List<Type>();
		AssemblyLoader.loadedAssemblies.TypeOperation(delegate(Type t)
		{
			if (t.IsSubclassOf(typeof(Contract)) && !(t == typeof(Contract)) && (string.IsNullOrEmpty(t.Namespace) || !t.Namespace.Contains("Expansions.Serenity.Contracts") || ExpansionsLoader.IsExpansionInstalled("Serenity")))
			{
				ContractTypes.Add(t);
			}
		});
		Debug.Log("[ContractSystem]: Found " + ContractTypes.Count + " contract types");
	}

	public static Type GetContractType(string typeName)
	{
		int num = 0;
		int count = ContractTypes.Count;
		while (true)
		{
			if (num < count)
			{
				if (ContractTypes[num].Name == typeName)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return ContractTypes[num];
	}

	private static void GenerateParameterTypes()
	{
		if (ParameterTypes != null)
		{
			return;
		}
		ParameterTypes = new List<Type>();
		AssemblyLoader.loadedAssemblies.TypeOperation(delegate(Type t)
		{
			if (t.IsSubclassOf(typeof(ContractParameter)) && !(t == typeof(ContractParameter)) && (string.IsNullOrEmpty(t.Namespace) || !t.Namespace.Contains("Expansions.Serenity.Contracts") || ExpansionsLoader.IsExpansionInstalled("Serenity")))
			{
				ParameterTypes.Add(t);
			}
		});
		Debug.Log("[ContractSystem]: Found " + ParameterTypes.Count + " parameter types");
	}

	public static Type GetParameterType(string typeName)
	{
		int num = 0;
		int count = ParameterTypes.Count;
		while (true)
		{
			if (num < count)
			{
				if (ParameterTypes[num].Name == typeName)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return ParameterTypes[num];
	}

	private static void GeneratePredicateTypes()
	{
		if (PredicateTypes != null)
		{
			return;
		}
		PredicateTypes = new List<Type>();
		AssemblyLoader.loadedAssemblies.TypeOperation(delegate(Type t)
		{
			if (t.IsSubclassOf(typeof(ContractPredicate)) && !(t == typeof(ContractPredicate)))
			{
				PredicateTypes.Add(t);
			}
		});
		Debug.Log("[ContractSystem]: Found " + PredicateTypes.Count + " predicate types");
	}

	public static Type GetPredicateType(string typeName)
	{
		int num = 0;
		int count = PredicateTypes.Count;
		while (true)
		{
			if (num < count)
			{
				if (PredicateTypes[num].Name == typeName)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return PredicateTypes[num];
	}

	private static void GenerateMandatoryTypes()
	{
		if (MandatoryTypes == null)
		{
			MandatoryTypes = new List<Type> { typeof(ExplorationContract) };
		}
	}

	public override void OnAwake()
	{
		loaded = false;
		Instance = this;
		GenerateTypes();
		if (!HighLogic.LoadedSceneIsEditor)
		{
			StartCoroutine(UpdateDaemon());
		}
	}

	private void OnDestroy()
	{
		int i = 0;
		for (int count = contracts.Count; i < count; i++)
		{
			contracts[i].Unregister();
			contracts[i] = null;
		}
		int j = 0;
		for (int count2 = contractsFinished.Count; j < count2; j++)
		{
			contractsFinished[j] = null;
		}
		contracts.Clear();
		contractsFinished.Clear();
		GameEvents.onGameSceneLoadRequested.Remove(OnSceneLoadRequested);
		GameEvents.onFlightReady.Remove(OnFlightReady);
		GameEvents.OnFlightGlobalsReady.Remove(OnFlightGlobalsReady);
		GameEvents.onVesselChange.Remove(OnVesselChange);
		GameEvents.OnProgressReached.Remove(OnNodeReached);
		GameEvents.OnReputationChanged.Remove(OnReputationChanged);
		GameEvents.onGUIMissionControlSpawn.Remove(OnMissionControlSpawned);
		if (Instance != null && Instance == this)
		{
			Instance = null;
		}
	}

	public override void OnLoad(ConfigNode gameNode)
	{
		StartCoroutine(OnLoadRoutine(gameNode));
	}

	private IEnumerator OnLoadRoutine(ConfigNode gameNode)
	{
		yield return null;
		SystemUtilities.LoadNode(gameNode, "ContractSystem", "version", ref version, "-1", logging: false);
		if (CompatibilityUtilities.OldCareerSave(Game.Modes.CAREER, version))
		{
			CompatibilityUtilities.UpdateCareerSave(gameNode);
			version = VersioningBase.GetVersionString();
		}
		LoadContractWeights(gameNode);
		contracts.Clear();
		ConfigNode node = gameNode.GetNode("CONTRACTS");
		if (node == null)
		{
			yield break;
		}
		ConfigNode[] nodes = node.GetNodes("CONTRACT");
		int i = 0;
		for (int num = nodes.Length; i < num; i++)
		{
			ConfigNode cNode = nodes[i];
			Contract contract = LoadContract(cNode);
			if (contract != null)
			{
				contracts.Add(contract);
			}
		}
		nodes = node.GetNodes("CONTRACT_FINISHED");
		int j = 0;
		for (int num2 = nodes.Length; j < num2; j++)
		{
			ConfigNode cNode = nodes[j];
			Contract contract2 = LoadContract(cNode);
			if (contract2 != null)
			{
				contractsFinished.Add(contract2);
			}
			else
			{
				Debug.LogError("[ContractSystem]: Contract " + j + " is invalid");
			}
		}
		if (node.HasValue("update"))
		{
			lastUpdate = double.Parse(node.GetValue("update"));
		}
		RegisterContracts();
		GameEvents.Contract.onContractsLoaded.Fire();
		ResetContracts();
		GameEvents.onGameSceneLoadRequested.Add(OnSceneLoadRequested);
		GameEvents.onFlightReady.Add(OnFlightReady);
		GameEvents.OnFlightGlobalsReady.Add(OnFlightGlobalsReady);
		GameEvents.onVesselChange.Add(OnVesselChange);
		GameEvents.OnProgressReached.Add(OnNodeReached);
		GameEvents.OnReputationChanged.Add(OnReputationChanged);
		GameEvents.onGUIMissionControlSpawn.Add(OnMissionControlSpawned);
		loaded = true;
	}

	private Contract LoadContract(ConfigNode cNode)
	{
		string value = cNode.GetValue("type");
		if (value == null)
		{
			Debug.LogError("[ContractSystem]: Contract config is invalid");
			return null;
		}
		cNode.RemoveValues("type");
		Type contractType = GetContractType(value);
		if (contractType == null)
		{
			Debug.LogError("[ContractSystem]: Contract type '" + value + "' not found");
			return null;
		}
		return Contract.Load((Contract)Activator.CreateInstance(contractType), cNode);
	}

	private void RegisterContracts()
	{
		int i = 0;
		for (int count = contracts.Count; i < count; i++)
		{
			if (contracts[i].ContractState == Contract.State.Active)
			{
				contracts[i].Register();
			}
		}
	}

	public override void OnSave(ConfigNode gameNode)
	{
		gameNode.AddNode(SaveContractWeights());
		ConfigNode configNode = gameNode.AddNode("CONTRACTS");
		int i = 0;
		for (int count = contracts.Count; i < count; i++)
		{
			Contract contract = contracts[i];
			ConfigNode node = configNode.AddNode("CONTRACT");
			contract.Save(node);
		}
		int j = 0;
		for (int count2 = contractsFinished.Count; j < count2; j++)
		{
			Contract contract2 = contractsFinished[j];
			ConfigNode node = configNode.AddNode("CONTRACT_FINISHED");
			contract2.Save(node);
		}
		gameNode.AddValue("update", lastUpdate.ToString("G17"));
		gameNode.AddValue("version", version);
	}

	private void OnNodeReached(ProgressNode node)
	{
		RefreshContracts();
	}

	private void OnReputationChanged(float newRep, TransactionReasons reason)
	{
		RefreshContracts();
	}

	[ContextMenu("Refresh Contracts")]
	private void RefreshContracts()
	{
		long ticks = DateTime.Now.Ticks;
		int seed = (int)(ticks & 0x7FFFFFFFL) ^ (int)((ticks & 0x7FFFFFFF00000000L) >> 8);
		GetContractCounts(Reputation.UnitRep, ContractDefs.AverageAvailableContracts, out var tier, out var tier2, out var tier3);
		bool flag = false;
		if (WithdrawSurplusContracts(Contract.ContractPrestige.Trivial, tier))
		{
			flag = true;
		}
		if (WithdrawSurplusContracts(Contract.ContractPrestige.Significant, tier2))
		{
			flag = true;
		}
		if (WithdrawSurplusContracts(Contract.ContractPrestige.Exceptional, tier3))
		{
			flag = true;
		}
		if (GenerateContracts(ref seed, Contract.ContractPrestige.Trivial, tier - CountContracts(Contract.ContractPrestige.Trivial)))
		{
			flag = true;
		}
		if (GenerateContracts(ref seed, Contract.ContractPrestige.Significant, tier2 - CountContracts(Contract.ContractPrestige.Significant)))
		{
			flag = true;
		}
		if (GenerateContracts(ref seed, Contract.ContractPrestige.Exceptional, tier3 - CountContracts(Contract.ContractPrestige.Exceptional)))
		{
			flag = true;
		}
		if (flag)
		{
			GameEvents.Contract.onContractsListChanged.Fire();
		}
		requireRefresh = false;
		lastUpdate = Planetarium.fetch.time;
	}

	public static void GetContractCounts(float rep, int avgContracts, out int tier1, out int tier2, out int tier3)
	{
		float num = (float)avgContracts * Mathf.Lerp(0.5f, 1.5f, Mathf.InverseLerp(-1f, 1f, rep));
		float num2 = Mathf.InverseLerp(-1f, 1f, rep);
		float num3 = Mathf.InverseLerp(-0.33f, 0.6f, rep);
		float num4 = Mathf.InverseLerp(0.1f, 1f, rep);
		float num5 = Mathf.Floor(Mathf.Lerp(num, 2f, num2 - num3 * 0.5f));
		float num6 = Mathf.Floor(Mathf.Lerp(0f, num, num3 - num4 * 0.5f));
		float num7 = Mathf.Floor(Mathf.Lerp(0f, num, num4));
		float num8 = 1f / ((num5 + num6 + num7) / num);
		tier1 = Mathf.RoundToInt(num5 * num8);
		tier2 = Mathf.RoundToInt(num6 * num8);
		tier3 = Mathf.RoundToInt(num7 * num8);
	}

	public int CountContracts(Contract.ContractPrestige difficulty)
	{
		int num = 0;
		int i = 0;
		for (int count = contracts.Count; i < count; i++)
		{
			if (contracts[i].Prestige == difficulty)
			{
				num++;
			}
		}
		return num;
	}

	public bool GenerateContracts(ref int seed, Contract.ContractPrestige difficulty, int count)
	{
		bool result = false;
		int num = 0;
		while (true)
		{
			if (num < count)
			{
				Contract contract = GenerateContract(ref seed, difficulty);
				if (contract == null)
				{
					break;
				}
				contract.Offer();
				contracts.Add(contract);
				if (contract.AutoAccept)
				{
					contract.Accept();
				}
				result = true;
				num++;
				continue;
			}
			return result;
		}
		return result;
	}

	public Contract GenerateContract(ref int seed, Contract.ContractPrestige difficulty)
	{
		Contract contract = null;
		int num = 0;
		while (contract == null && ++num < generateContractIterations)
		{
			if (seed == int.MaxValue)
			{
				seed = int.MinValue;
			}
			else
			{
				seed++;
			}
			contract = GenerateContract(seed, difficulty);
			if (contract == null)
			{
				continue;
			}
			int i = 0;
			for (int count = contracts.Count; i < count; i++)
			{
				if (contract.ContractID == contracts[i].ContractID)
				{
					contract.GenerateFailed();
					contract = null;
					break;
				}
			}
			if (contract == null)
			{
				continue;
			}
			for (int j = Mathf.Max(0, contractsFinished.Count - finishedContractIDCheck); j < contractsFinished.Count; j++)
			{
				if (contract.ContractID == contractsFinished[j].ContractID)
				{
					contract.GenerateFailed();
					contract = null;
					break;
				}
			}
		}
		return contract;
	}

	public Contract GenerateContract(int seed, Contract.ContractPrestige difficulty, Type contractType = null)
	{
		UnityEngine.Random.InitState(seed);
		if (contractType == null)
		{
			contractType = WeightedContractChoice();
		}
		return Contract.Generate(contractType, difficulty, seed, Contract.State.Generated);
	}

	public bool WithdrawSurplusContracts(Contract.ContractPrestige level, int maxAllowed)
	{
		bool result = false;
		int num = 0;
		int count = contracts.Count;
		while (count-- > 0)
		{
			Contract contract = contracts[count];
			if (contract.Prestige == level && contract.ContractState == Contract.State.Offered)
			{
				num++;
			}
		}
		int num2 = num - maxAllowed;
		int num3 = 0;
		while (num2 > 0 && num3 < contracts.Count)
		{
			Contract contract2 = contracts[num3];
			if (contract2.CanBeCancelled() && contract2.Prestige == level && contract2.ContractState == Contract.State.Offered)
			{
				contract2.Withdraw();
				result = true;
				num2--;
				Debug.Log("[ContractSystem]: Contract " + contract2.Title + " is no longer being offered by " + contract2.Agent.Name);
			}
			else
			{
				num3++;
			}
		}
		return result;
	}

	private IEnumerator UpdateDaemon()
	{
		yield return null;
		yield return null;
		if (updateDaemonRunning)
		{
			yield break;
		}
		updateDaemonRunning = true;
		while ((bool)this)
		{
			UpdateContracts();
			if (TimeWarp.CurrentRate < maxWarpFactorForUpdate && (Planetarium.fetch.time > lastUpdate + updateInterval || requireRefresh))
			{
				RefreshContracts();
			}
			yield return null;
		}
	}

	private void UpdateContracts()
	{
		int count = contracts.Count;
		while (count-- > 0)
		{
			Contract contract = contracts[count];
			if (contract.IsFinished())
			{
				contract.Unregister();
				if (contract.ContractState == Contract.State.Completed || contract.ContractState == Contract.State.DeadlineExpired || contract.ContractState == Contract.State.Failed || contract.ContractState == Contract.State.Cancelled)
				{
					contractsFinished.Add(contract);
				}
				contracts.RemoveAt(count);
				requireRefresh = true;
			}
			else
			{
				contract.Update();
			}
		}
		if (contracts.Count == 0)
		{
			requireRefresh = true;
		}
	}

	private void OnSceneLoadRequested(GameScenes scene)
	{
		ResetContracts();
	}

	private void OnFlightReady()
	{
		ResetContracts();
	}

	private void OnFlightGlobalsReady(bool ready)
	{
		ResetContracts();
	}

	private void OnVesselChange(Vessel vessel)
	{
		ResetContracts();
	}

	private void ResetContracts()
	{
		int count = contracts.Count;
		while (count-- > 0)
		{
			Contract contract = contracts[count];
			if (contract.ContractState == Contract.State.Active)
			{
				contract.Reset();
			}
		}
	}

	public Contract GetContractByGuid(Guid guid)
	{
		int count = contracts.Count;
		int num = 0;
		while (true)
		{
			if (num < count)
			{
				if (contracts[num].ContractGuid == guid)
				{
					break;
				}
				num++;
				continue;
			}
			return null;
		}
		return contracts[num];
	}

	public T[] GetCurrentContracts<T>(Func<T, bool> where = null) where T : Contract
	{
		List<T> list = new List<T>();
		int count = contracts.Count;
		for (int i = 0; i < count; i++)
		{
			if (contracts[i] is T val && (where == null || where(val)))
			{
				list.Add(val);
			}
		}
		return list.ToArray();
	}

	public bool AnyCurrentContracts<T>(Func<T, bool> where = null) where T : Contract
	{
		int count = contracts.Count;
		do
		{
			if (count-- <= 0)
			{
				return false;
			}
		}
		while (!(contracts[count] is T arg) || (where != null && !where(arg)));
		return true;
	}

	public T[] GetCurrentActiveContracts<T>(Func<T, bool> where = null) where T : Contract
	{
		List<T> list = new List<T>();
		int count = contracts.Count;
		for (int i = 0; i < count; i++)
		{
			if (contracts[i] is T val && val.ContractState == Contract.State.Active && (where == null || where(val)))
			{
				list.Add(val);
			}
		}
		return list.ToArray();
	}

	public bool AnyCurrentActiveContracts<T>(Func<T, bool> where = null) where T : Contract
	{
		int count = contracts.Count;
		do
		{
			if (count-- <= 0)
			{
				return false;
			}
		}
		while (!(contracts[count] is T val) || val.ContractState != Contract.State.Active || (where != null && !where(val)));
		return true;
	}

	public bool HasCompletedContract(Type type)
	{
		int num = 0;
		int count = contractsFinished.Count;
		while (true)
		{
			if (num < count)
			{
				Contract contract = contractsFinished[num];
				if (contract.GetType() == type && contract.ContractState == Contract.State.Completed)
				{
					break;
				}
				num++;
				continue;
			}
			return false;
		}
		return true;
	}

	public T[] GetCompletedContracts<T>(Func<T, bool> where = null) where T : Contract
	{
		List<T> list = new List<T>();
		int count = contractsFinished.Count;
		for (int i = 0; i < count; i++)
		{
			if (contractsFinished[i] is T val && val.ContractState == Contract.State.Completed && (where == null || where(val)))
			{
				list.Add(val);
			}
		}
		return list.ToArray();
	}

	public bool AnyCompletedContracts<T>(Func<T, bool> where = null) where T : Contract
	{
		int count = contractsFinished.Count;
		do
		{
			if (count-- <= 0)
			{
				return false;
			}
		}
		while (!(contractsFinished[count] is T val) || val.ContractState != Contract.State.Completed || (where != null && !where(val)));
		return true;
	}

	public int GetActiveContractCount()
	{
		int num = 0;
		int count = contracts.Count;
		while (count-- > 0)
		{
			Contract contract = contracts[count];
			if (contract.ContractState == Contract.State.Active && !contract.AutoAccept)
			{
				num++;
			}
		}
		return num;
	}

	public void RebuildContracts()
	{
		List<Type> list = new List<Type>();
		List<int> list2 = new List<int>();
		List<Contract.ContractPrestige> list3 = new List<Contract.ContractPrestige>();
		int i = 0;
		for (int count = contracts.Count; i < count; i++)
		{
			list.Add(contracts[i].GetType());
			list2.Add(contracts[i].MissionSeed);
			list3.Add(contracts[i].Prestige);
			if (contracts[i].ContractState == Contract.State.Active)
			{
				contracts[i].Cancel();
			}
		}
		contracts.Clear();
		int j = 0;
		for (int count2 = list.Count; j < count2; j++)
		{
			Contract contract = GenerateContract(list2[j], list3[j], list[j]);
			if (contract != null)
			{
				contracts.Add(contract);
				contract.Offer();
			}
		}
	}

	public void ClearContractsCurrent()
	{
		int i = 0;
		for (int count = contracts.Count; i < count; i++)
		{
			contracts[i].Kill();
		}
		contracts.Clear();
	}

	public void ClearContractsFinished()
	{
		contractsFinished.Clear();
	}

	private static int ClampedWeight(int weight)
	{
		return Math.Min(Math.Max(weight, WeightMinimum), WeightMaximum);
	}

	private static int TryGetWeight(string name)
	{
		int value = WeightDefault;
		if (ContractWeights != null)
		{
			ContractWeights.TryGetValue(name, out value);
		}
		return value;
	}

	private static int TryGetWeight(string name, out bool success)
	{
		int value = WeightDefault;
		success = ContractWeights != null && ContractWeights.TryGetValue(name, out value);
		return value;
	}

	private void LoadContractWeights(ConfigNode gameNode)
	{
		WeightAcceptDelta = ContractDefs.WeightAcceptDelta;
		WeightDeclineDelta = ContractDefs.WeightDeclineDelta;
		WeightWithdrawReadDelta = ContractDefs.WeightWithdrawReadDelta;
		WeightWithdrawSeenDelta = ContractDefs.WeightWithdrawSeenDelta;
		WeightMinimum = ContractDefs.WeightMinimum;
		WeightMinimum = ((WeightMinimum >= 0) ? WeightMinimum : 0);
		WeightMaximum = ContractDefs.WeightMaximum;
		WeightMaximum = ((WeightMaximum >= 0) ? WeightMaximum : 0);
		WeightDefault = ContractDefs.WeightDefault;
		WeightDefault = ((WeightDefault < WeightMinimum || WeightDefault > WeightMaximum) ? ((WeightMinimum + WeightMaximum) / 2) : WeightDefault);
		WeightDefault = ContractDefs.WeightDefault;
		WeightDefault = ((WeightDefault < WeightMinimum || WeightDefault > WeightMaximum) ? ((WeightMinimum + WeightMaximum) / 2) : WeightDefault);
		if (WeightMinimum > WeightMaximum)
		{
			int weightMinimum = WeightMinimum;
			WeightMinimum = WeightMaximum;
			WeightMaximum = weightMinimum;
		}
		if (WeightDefault > 0 && WeightMaximum > 0)
		{
			if (WeightMinimum <= 0 && (WeightAcceptDelta < 0 || WeightDeclineDelta < 0))
			{
				Debug.LogWarning("[ContractSystem]: Contract weight configuration settings can render contract types permanently inaccessible");
			}
		}
		else
		{
			Debug.LogWarning("[ContractSystem]: Contract weight configuration settings will not ever allow contracts to generate");
		}
		ContractWeights = new Dictionary<string, int>();
		if (ContractTypes == null)
		{
			return;
		}
		ConfigNode node = gameNode.GetNode("WEIGHTS");
		int count = ContractTypes.Count;
		while (count-- > 0)
		{
			string key = ContractTypes[count].Name;
			int result = WeightDefault;
			if (node != null && node.HasValue(key))
			{
				int.TryParse(node.GetValue(key), out result);
			}
			ContractWeights[key] = result;
		}
		if (FlightGlobals.Bodies == null)
		{
			return;
		}
		int count2 = FlightGlobals.Bodies.Count;
		while (count2-- > 0)
		{
			string key2 = FlightGlobals.Bodies[count2].name;
			int result2 = WeightDefault;
			if (node != null && node.HasValue(key2))
			{
				int.TryParse(node.GetValue(key2), out result2);
			}
			ContractWeights[key2] = result2;
		}
	}

	private ConfigNode SaveContractWeights()
	{
		ConfigNode configNode = new ConfigNode("WEIGHTS");
		if (ContractTypes == null)
		{
			return configNode;
		}
		int count = ContractTypes.Count;
		while (count-- > 0)
		{
			string text = ContractTypes[count].Name;
			int value = TryGetWeight(text);
			configNode.AddValue(text, value);
		}
		if (FlightGlobals.Bodies == null)
		{
			return configNode;
		}
		int count2 = FlightGlobals.Bodies.Count;
		while (count2-- > 0)
		{
			string text2 = FlightGlobals.Bodies[count2].name;
			int value2 = TryGetWeight(text2);
			configNode.AddValue(text2, value2);
		}
		return configNode;
	}

	public static void ResetWeights()
	{
		if (ContractTypes == null || ContractWeights == null)
		{
			return;
		}
		Debug.LogError("[ContractSystem]: Contract weights have been reset to default");
		int count = ContractTypes.Count;
		while (count-- > 0)
		{
			ContractWeights[ContractTypes[count].Name] = WeightDefault;
		}
		if (FlightGlobals.Bodies != null)
		{
			int count2 = FlightGlobals.Bodies.Count;
			while (count2-- > 0)
			{
				ContractWeights[FlightGlobals.Bodies[count2].name] = WeightDefault;
			}
		}
	}

	public static Type WeightedContractChoice()
	{
		if (ContractTypes != null && ContractTypes.Count > 0)
		{
			int num = 0;
			int count = ContractTypes.Count;
			while (count-- > 0)
			{
				num += TryGetWeight(ContractTypes[count].Name);
			}
			int num2 = UnityEngine.Random.Range(0, num);
			int count2 = ContractTypes.Count;
			while (count2-- > 0)
			{
				int num3 = TryGetWeight(ContractTypes[count2].Name);
				if (num2 >= num3)
				{
					num2 -= num3;
					continue;
				}
				num2 = count2;
				break;
			}
			return ContractTypes[num2];
		}
		Debug.LogError("[ContractSystem]: Attempted to generate contract with no contract types available");
		return null;
	}

	public static CelestialBody WeightedBodyChoice(IList<CelestialBody> bodies, System.Random generator = null)
	{
		KSPRandom kSPRandom = null;
		if (generator != null)
		{
			kSPRandom = generator as KSPRandom;
		}
		if (bodies != null && bodies.Count > 0)
		{
			int num = 0;
			int count = bodies.Count;
			while (count-- > 0)
			{
				num += TryGetWeight(bodies[count].name);
			}
			int num2 = kSPRandom?.Next(0, num) ?? UnityEngine.Random.Range(0, num);
			int count2 = bodies.Count;
			while (count2-- > 0)
			{
				int num3 = TryGetWeight(bodies[count2].name);
				if (num2 >= num3)
				{
					num2 -= num3;
					continue;
				}
				num2 = count2;
				break;
			}
			return bodies[num2];
		}
		Debug.LogError("[ContractSystem]: Attempted to generate contract with no celestial bodies available");
		return null;
	}

	public static void WeightAssignment(string name, int amount, bool ignoreLimits = false)
	{
		if (ContractWeights != null && ContractWeights.ContainsKey(name))
		{
			ContractWeights[name] = (ignoreLimits ? amount : ClampedWeight(amount));
		}
	}

	public static void WeightAdjustment(string name, int delta, bool ignoreLimits = false)
	{
		bool success;
		int num = TryGetWeight(name, out success);
		if (success)
		{
			int num2 = num + delta;
			ContractWeights[name] = (ignoreLimits ? num2 : ClampedWeight(num2));
		}
	}

	public static void AdjustWeight(string name, Contract contract)
	{
		switch (contract.ContractState)
		{
		case Contract.State.OfferExpired:
		case Contract.State.Withdrawn:
			if (contract.ContractViewed == Contract.Viewed.Read)
			{
				WeightAdjustment(name, WeightWithdrawReadDelta);
			}
			else if (contract.ContractViewed == Contract.Viewed.Seen)
			{
				WeightAdjustment(name, WeightWithdrawSeenDelta);
			}
			break;
		case Contract.State.Declined:
			WeightAdjustment(name, WeightDeclineDelta);
			break;
		case Contract.State.Active:
			WeightAdjustment(name, WeightAcceptDelta);
			break;
		}
	}

	private void OnMissionControlSpawned()
	{
		GenerateMandatoryContracts();
	}

	public void GenerateMandatoryContracts()
	{
		bool flag = false;
		for (int num = MandatoryTypes.Count - 1; num >= 0; num--)
		{
			Type type = MandatoryTypes[num];
			bool flag2 = true;
			int num2 = Contracts.Count - 1;
			while (num2 >= 0)
			{
				if (!(Contracts[num2].GetType() == type))
				{
					num2--;
					continue;
				}
				flag2 = false;
				break;
			}
			if (flag2)
			{
				Array values = Enum.GetValues(typeof(Contract.ContractPrestige));
				Contract.ContractPrestige difficulty = (Contract.ContractPrestige)values.GetValue(UnityEngine.Random.Range(0, values.Length));
				Contract contract = GenerateContract(UnityEngine.Random.Range(int.MinValue, int.MaxValue), difficulty, type);
				if (contract != null)
				{
					contract.Offer();
					contracts.Add(contract);
					if (contract.AutoAccept)
					{
						contract.Accept();
					}
					flag = true;
				}
			}
		}
		if (flag)
		{
			GameEvents.Contract.onContractsListChanged.Fire();
		}
	}
}
