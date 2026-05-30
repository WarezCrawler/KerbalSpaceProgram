using System;
using Expansions.Missions;
using UnityEngine;
using ns10;
using ns18;

[KSPScenario((ScenarioCreationOptions)1120, new GameScenes[]
{
	GameScenes.FLIGHT,
	GameScenes.TRACKSTATION,
	GameScenes.SPACECENTER,
	GameScenes.EDITOR
})]
public class Funding : ScenarioModule
{
	public static Funding Instance;

	private double funds;

	public double Funds => funds;

	public void AddFunds(double value, TransactionReasons reason)
	{
		funds += value;
		CurrencyModifierQuery data = new CurrencyModifierQuery(reason, (float)value, 0f, 0f);
		GameEvents.Modifiers.OnCurrencyModifierQuery.Fire(data);
		GameEvents.Modifiers.OnCurrencyModified.Fire(data);
		if (!HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().AllowNegativeCurrency)
		{
			funds = Math.Max(0.0, funds);
		}
		GameEvents.OnFundsChanged.Fire(funds, reason);
	}

	public void SetFunds(double value, TransactionReasons reason)
	{
		funds = value;
		if (!HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>().AllowNegativeCurrency)
		{
			funds = Math.Max(0.0, funds);
		}
		GameEvents.OnFundsChanged.Fire(funds, reason);
	}

	public override void OnAwake()
	{
		if (Instance != null)
		{
			Debug.LogError("[Funding Module]: Instance already exists!", Instance.gameObject);
		}
		Instance = this;
		if (HighLogic.CurrentGame != null)
		{
			if (HighLogic.CurrentGame.Mode != Game.Modes.MISSION)
			{
				funds = HighLogic.CurrentGame.Parameters.Career.StartingFunds;
			}
			else
			{
				funds = HighLogic.CurrentGame.Parameters.CustomParams<MissionParamsGeneral>().startingFunds;
			}
		}
		GameEvents.OnVesselRollout.Add(onVesselRollout);
		GameEvents.onVesselRecoveryProcessing.Add(onVesselRecoveryProcessing);
		GameEvents.OnPartPurchased.Add(onPartPurchased);
		GameEvents.OnPartUpgradePurchased.Add(onUpgradePurchased);
		GameEvents.OnCrewmemberHired.Add(onCrewHired);
		GameEvents.Modifiers.OnCurrencyModified.Add(OnCurrenciesModified);
	}

	public void OnDestroy()
	{
		GameEvents.OnVesselRollout.Remove(onVesselRollout);
		GameEvents.onVesselRecoveryProcessing.Remove(onVesselRecoveryProcessing);
		GameEvents.OnPartPurchased.Remove(onPartPurchased);
		GameEvents.OnPartUpgradePurchased.Remove(onUpgradePurchased);
		GameEvents.OnCrewmemberHired.Remove(onCrewHired);
		GameEvents.Modifiers.OnCurrencyModified.Remove(OnCurrenciesModified);
		if (Instance != null && Instance == this)
		{
			Instance = null;
		}
	}

	private void OnCurrenciesModified(CurrencyModifierQuery query)
	{
		float effectDelta = query.GetEffectDelta(Currency.Funds);
		funds += effectDelta;
		if (effectDelta != 0f)
		{
			GameEvents.OnFundsChanged.Fire(funds, query.reason);
		}
	}

	private void onVesselRollout(ShipConstruct ship)
	{
		AddFunds(0f - ship.GetShipCosts(out var _, out var _), TransactionReasons.VesselRollout);
	}

	private void onVesselRecoveryProcessing(ProtoVessel pv, MissionRecoveryDialog mrDialog, float recoveryScore)
	{
		if (pv == null)
		{
			return;
		}
		bool flag = mrDialog != null;
		double num = 0.0;
		for (int i = 0; i < pv.protoPartSnapshots.Count; i++)
		{
			ProtoPartSnapshot protoPartSnapshot = pv.protoPartSnapshots[i];
			AvailablePart partInfoByName = PartLoader.getPartInfoByName(protoPartSnapshot.partInfo.name);
			if (partInfoByName != null)
			{
				bool flag2 = false;
				ConfigNode configNode = null;
				for (int j = 0; j < protoPartSnapshot.modules.Count; j++)
				{
					if (string.Equals(protoPartSnapshot.modules[j].moduleName, "ModuleInventoryPart"))
					{
						flag2 = true;
						configNode = protoPartSnapshot.modules[j].moduleValues;
					}
				}
				float dryCost;
				float fuelCost;
				if (flag2)
				{
					ShipConstruction.GetPartCosts(protoPartSnapshot, includeModuleCosts: false, partInfoByName, out dryCost, out fuelCost);
				}
				else
				{
					ShipConstruction.GetPartCosts(protoPartSnapshot, partInfoByName, out dryCost, out fuelCost);
				}
				dryCost *= recoveryScore;
				fuelCost *= recoveryScore;
				num += (double)(dryCost + fuelCost);
				if (!flag)
				{
					continue;
				}
				if (!string.Equals(partInfoByName.name, "kerbalEVA"))
				{
					mrDialog.AddPartWidget(PartWidget.Create(partInfoByName, dryCost, fuelCost, mrDialog));
				}
				if (flag2)
				{
					string value = "";
					if (configNode.TryGetValue("inventory", ref value))
					{
						string[] array = value.Split(',');
						for (int k = 0; k < array.Length; k++)
						{
							if (!string.IsNullOrEmpty(array[k]))
							{
								AvailablePart partInfoByName2 = PartLoader.getPartInfoByName(array[k]);
								ShipConstruction.GetPartCosts(protoPartSnapshot, includeModuleCosts: false, partInfoByName2, out var dryCost2, out var fuelCost2);
								dryCost2 *= recoveryScore;
								fuelCost2 *= recoveryScore;
								num += (double)(dryCost2 + fuelCost2);
								mrDialog.AddPartWidget(PartWidget.Create(partInfoByName2, dryCost2, fuelCost2, mrDialog));
							}
						}
					}
				}
				for (int l = 0; l < protoPartSnapshot.resources.Count; l++)
				{
					ProtoPartResourceSnapshot protoPartResourceSnapshot = protoPartSnapshot.resources[l];
					PartResourceDefinition definition = PartResourceLibrary.Instance.GetDefinition(protoPartResourceSnapshot.resourceName);
					if (definition != null)
					{
						mrDialog.AddResourceWidget(ResourceWidget.Create(definition, (float)protoPartResourceSnapshot.amount, definition.unitCost * recoveryScore, mrDialog));
					}
					else
					{
						Debug.LogError("[ShipTemplate]: No Resource definition found for " + protoPartResourceSnapshot.resourceName);
					}
				}
			}
			else if (flag)
			{
				Debug.Log("[Funding]: Cannot recover " + protoPartSnapshot.partName + ". Part has no entry in PartLoader catalog. That is only OK if the part is an EVA.");
			}
		}
		if (flag)
		{
			mrDialog.fundsEarned = num;
		}
		AddFunds(num, TransactionReasons.VesselRecovery);
	}

	private void onPartPurchased(AvailablePart aP)
	{
		if (!HighLogic.CurrentGame.Parameters.Difficulty.BypassEntryPurchaseAfterResearch && aP.costsFunds)
		{
			AddFunds(-aP.entryCost, TransactionReasons.RnDPartPurchase);
		}
	}

	private void onUpgradePurchased(PartUpgradeHandler.Upgrade upgrade)
	{
		if (!HighLogic.CurrentGame.Parameters.Difficulty.BypassEntryPurchaseAfterResearch && upgrade.entryCost > 0f)
		{
			AddFunds(0f - upgrade.entryCost, TransactionReasons.RnDPartPurchase);
		}
	}

	private void onCrewHired(ProtoCrewMember crew, int crewCount)
	{
		AddFunds(0f - GameVariables.Instance.GetRecruitHireCost(crewCount), TransactionReasons.CrewRecruited);
	}

	public override void OnLoad(ConfigNode node)
	{
		if (node.HasValue("funds"))
		{
			funds = double.Parse(node.GetValue("funds"));
		}
	}

	public override void OnSave(ConfigNode node)
	{
		node.AddValue("funds", funds);
	}

	public static bool CanAfford(float cost)
	{
		if (Instance == null)
		{
			return true;
		}
		return (double)cost <= Instance.Funds;
	}
}
