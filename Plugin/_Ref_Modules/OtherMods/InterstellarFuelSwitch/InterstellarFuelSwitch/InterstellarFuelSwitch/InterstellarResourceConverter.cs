using System;
using System.Collections.Generic;
using System.Linq;

namespace InterstellarFuelSwitch;

internal class InterstellarResourceConverter : PartModule
{
	[KSPField]
	public bool showPowerUsageFloatRange;

	[KSPField]
	public bool showControlToggle;

	[KSPField]
	public string sliderText = string.Empty;

	[KSPField]
	public float percentageMaxValue = 100f;

	[KSPField]
	public float percentageMinValue = -100f;

	[KSPField]
	public float percentageStepIncrement = 1f;

	[KSPField]
	public bool percentageSymetry = true;

	[KSPField]
	public string primaryResourceNames = string.Empty;

	[KSPField]
	public string secondaryResourceNames = string.Empty;

	[KSPField]
	public double primaryConversionEnergyCost = 0.001;

	[KSPField]
	public double secondaryConversionEnergyCost = 0.001;

	[KSPField]
	public string primaryConversionEnergyResource = "ElectricCharge";

	[KSPField]
	public string secondaryConversionEnergResource = "ElectricCharge";

	[KSPField]
	public float primaryConversionEnergyMult = 1f;

	[KSPField]
	public float secondaryConversionEnergMult = 1f;

	[KSPField(guiActive = false, guiUnits = " U/s")]
	public float primaryChange;

	[KSPField(guiActive = false, guiUnits = " U/s")]
	public float secondaryChange;

	[KSPField(guiActive = false, guiActiveEditor = false)]
	public double primaryconversionRatio;

	[KSPField(guiActive = false, guiActiveEditor = false)]
	public double secondaryconversionRatio;

	[KSPField(guiActive = false)]
	public double neededAmount;

	[KSPField(guiActive = false)]
	public double availableSpaceInTarget;

	[KSPField(guiActive = false)]
	private bool retreivePrimary;

	[KSPField(guiActive = false)]
	private bool retrieveSecondary;

	[KSPField(guiActive = false)]
	public double transferRate;

	[KSPField(guiActive = false)]
	public double conversionRatio;

	[KSPField]
	public double maxPowerPrimary = 10.0;

	[KSPField]
	public double maxPowerSecondary = 10.0;

	[KSPField]
	public bool requiresPrimaryLocalInEditor = true;

	[KSPField]
	public bool requiresPrimaryLocalInFlight = true;

	[KSPField]
	public bool primaryConversionCostPower = true;

	[KSPField]
	public bool secondaryConversionCostPower = true;

	[KSPField]
	public double primaryNormalizedDensity = 0.001;

	[KSPField]
	public double secondaryNormalizedDensity = 0.001;

	[KSPField]
	public double requestedPower;

	[UI_FloatRange]
	[KSPField(isPersistant = true, guiActive = true, guiActiveEditor = true, guiName = "#LOC_IFS_ResourceConverter_ConvertPercentage", guiUnits = "%")]
	public float convertPercentage;

	[KSPField(isPersistant = true, guiActive = false, guiActiveEditor = false, guiName = "#LOC_IFS_ResourceConverter_PowerPercentage", guiUnits = "%")]
	[UI_FloatRange(minValue = 0f, maxValue = 100f, stepIncrement = 1f)]
	public float powerUsagePercentage;

	private PartResourceDefinition definitionPrimaryPowerResource;

	private PartResourceDefinition definitionSecondaryPowerResource;

	private BaseField primaryChangeField;

	private BaseField secondaryChangeField;

	private BaseField convertPercentageField;

	private List<ResourceStats> primaryResources;

	private List<ResourceStats> secondaryResources;

	private UI_FloatRange convertPecentageEditorFloatRange;

	private UI_FloatRange convertPecentageFlightFloatRange;

	private int changedFieldCounter;

	private bool hasNullDefinitions;

	public float PowerUsagePercentageRatio => 1f - (showPowerUsageFloatRange ? (powerUsagePercentage / 100f) : 1f);

	public override void OnStart(StartState state)
	{
		definitionPrimaryPowerResource = PartResourceLibrary.Instance.GetDefinition(primaryConversionEnergyResource);
		definitionSecondaryPowerResource = PartResourceLibrary.Instance.GetDefinition(secondaryConversionEnergResource);
		primaryChangeField = base.Fields["primaryChange"];
		secondaryChangeField = base.Fields["secondaryChange"];
		base.Fields["powerUsagePercentage"].guiActiveEditor = showPowerUsageFloatRange;
		base.Fields["powerUsagePercentage"].guiActive = showPowerUsageFloatRange;
		convertPercentageField = base.Fields["convertPercentage"];
		primaryResources = (from m in primaryResourceNames.Split(';')
			select new ResourceStats
			{
				definition = PartResourceLibrary.Instance.GetDefinition(m.Trim())
			}).ToList();
		secondaryResources = (from m in secondaryResourceNames.Split(';')
			select new ResourceStats
			{
				definition = PartResourceLibrary.Instance.GetDefinition(m.Trim())
			}).ToList();
		primaryChangeField.guiName = primaryResources.First().definition.name;
		secondaryChangeField.guiName = secondaryResources.First().definition.name;
		hasNullDefinitions = primaryResources.Any((ResourceStats m) => m.definition == null) || secondaryResources.Any((ResourceStats m) => m.definition == null);
		if (hasNullDefinitions)
		{
			convertPercentageField.guiActiveEditor = false;
			convertPercentageField.guiActive = false;
			return;
		}
		foreach (ResourceStats primaryResource in primaryResources)
		{
			if (primaryResource.definition.density > 0f)
			{
				primaryResource.normalizedDensity = primaryResource.definition.density;
			}
		}
		foreach (ResourceStats secondaryResource in secondaryResources)
		{
			if (secondaryResource.definition.density > 0f)
			{
				secondaryResource.normalizedDensity = secondaryResource.definition.density;
			}
		}
		if (primaryResources.Count == 1 && secondaryResources.Count == 1)
		{
			ResourceStats resourceStats = primaryResources.First();
			ResourceStats resourceStats2 = secondaryResources.First();
			if (resourceStats.normalizedDensity > 0.0 && resourceStats2.normalizedDensity > 0.0)
			{
				resourceStats.conversionRatio = resourceStats2.normalizedDensity / resourceStats.normalizedDensity;
				resourceStats2.conversionRatio = resourceStats.normalizedDensity / resourceStats2.normalizedDensity;
			}
			else if (resourceStats.definition.unitCost > 0f && resourceStats2.definition.unitCost > 0f)
			{
				resourceStats.conversionRatio = (double)(decimal)resourceStats2.definition.unitCost / (double)(decimal)resourceStats.definition.unitCost;
				resourceStats2.conversionRatio = (double)(decimal)resourceStats.definition.unitCost / (double)(decimal)resourceStats2.definition.unitCost;
			}
			if (resourceStats.normalizedDensity == 0.0)
			{
				resourceStats.normalizedDensity = primaryNormalizedDensity;
			}
			if (resourceStats2.normalizedDensity == 0.0)
			{
				resourceStats2.normalizedDensity = secondaryNormalizedDensity;
			}
			if (resourceStats2.conversionRatio == 0.0 && resourceStats2.conversionRatio == 0.0)
			{
				if (resourceStats.normalizedDensity > 0.0 && resourceStats2.normalizedDensity > 0.0)
				{
					resourceStats.conversionRatio = resourceStats2.normalizedDensity / resourceStats.normalizedDensity;
					resourceStats2.conversionRatio = resourceStats.normalizedDensity / resourceStats2.normalizedDensity;
				}
				else
				{
					resourceStats.conversionRatio = 1.0;
					resourceStats2.conversionRatio = 1.0;
				}
			}
		}
		primaryconversionRatio = primaryResources.First().conversionRatio;
		secondaryconversionRatio = secondaryResources.First().conversionRatio;
		primaryResources.ForEach(delegate(ResourceStats m)
		{
			m.transferRate = maxPowerPrimary / primaryConversionEnergyCost / 1000.0 / m.normalizedDensity;
		});
		secondaryResources.ForEach(delegate(ResourceStats m)
		{
			m.transferRate = maxPowerSecondary / secondaryConversionEnergyCost / 1000.0 / m.normalizedDensity;
		});
		if (string.IsNullOrEmpty(sliderText))
		{
			convertPercentageField.guiName = string.Join("+", primaryResources.Select((ResourceStats m) => m.definition.name).ToArray()) + "<->" + string.Join("+", secondaryResources.Select((ResourceStats m) => m.definition.name).ToArray());
		}
		else
		{
			convertPercentageField.guiName = sliderText;
		}
		convertPecentageFlightFloatRange = convertPercentageField.uiControlFlight as UI_FloatRange;
		convertPecentageFlightFloatRange.maxValue = percentageMaxValue;
		convertPecentageFlightFloatRange.minValue = percentageMinValue;
		convertPecentageFlightFloatRange.stepIncrement = percentageStepIncrement;
		convertPecentageFlightFloatRange.affectSymCounterparts = (percentageSymetry ? UI_Scene.All : UI_Scene.None);
		convertPecentageEditorFloatRange = convertPercentageField.uiControlEditor as UI_FloatRange;
		convertPecentageEditorFloatRange.maxValue = percentageMaxValue;
		convertPecentageEditorFloatRange.minValue = percentageMinValue;
		convertPecentageEditorFloatRange.stepIncrement = percentageStepIncrement;
		convertPecentageEditorFloatRange.affectSymCounterparts = (percentageSymetry ? UI_Scene.All : UI_Scene.None);
	}

	public void Update()
	{
		if (hasNullDefinitions)
		{
			convertPercentageField.guiActive = false;
			primaryChangeField.guiActive = false;
			secondaryChangeField.guiActive = false;
		}
		else if (requiresPrimaryLocalInEditor && HighLogic.LoadedSceneIsEditor)
		{
			convertPercentageField.guiActiveEditor = primaryResources.All((ResourceStats m) => base.part.Resources.Contains(m.definition.id));
		}
		else if (requiresPrimaryLocalInFlight && HighLogic.LoadedSceneIsFlight && !primaryResources.All((ResourceStats m) => base.part.Resources.Contains(m.definition.id)))
		{
			convertPercentageField.guiActive = false;
			primaryChangeField.guiActive = false;
			secondaryChangeField.guiActive = false;
		}
		else
		{
			if (!HighLogic.LoadedSceneIsFlight)
			{
				return;
			}
			for (int i = 0; i < primaryResources.Count; i++)
			{
				ResourceStats resourceStats = primaryResources[i];
				base.part.GetConnectedResourceTotals(resourceStats.definition.id, out var amount, out var maxAmount);
				if (maxAmount == 0.0)
				{
					convertPercentageField.guiActive = false;
					primaryChangeField.guiActive = false;
					secondaryChangeField.guiActive = false;
					return;
				}
				resourceStats.currentAmount = amount;
				resourceStats.maxAmount = maxAmount;
			}
			for (int j = 0; j < secondaryResources.Count; j++)
			{
				ResourceStats resourceStats2 = secondaryResources[j];
				base.part.GetConnectedResourceTotals(resourceStats2.definition.id, out var amount2, out var maxAmount2);
				if (maxAmount2 == 0.0)
				{
					convertPercentageField.guiActive = false;
					primaryChangeField.guiActive = false;
					secondaryChangeField.guiActive = false;
					return;
				}
				resourceStats2.currentAmount = amount2;
				resourceStats2.maxAmount = maxAmount2;
			}
			convertPercentageField.guiActive = true;
		}
	}

	public void FixedUpdate()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			return;
		}
		double num = (double)(decimal)TimeWarp.fixedDeltaTime;
		if (!convertPercentageField.guiActive)
		{
			return;
		}
		changedFieldCounter++;
		for (int i = 0; i < primaryResources.Count; i++)
		{
			ResourceStats resourceStats = primaryResources[i];
			base.part.GetConnectedResourceTotals(resourceStats.definition.id, out var amount, out var maxAmount);
			if (maxAmount == 0.0)
			{
				return;
			}
			resourceStats.currentAmount = amount;
			resourceStats.maxAmount = maxAmount;
		}
		for (int j = 0; j < secondaryResources.Count; j++)
		{
			ResourceStats resourceStats2 = secondaryResources[j];
			base.part.GetConnectedResourceTotals(resourceStats2.definition.id, out var amount2, out var maxAmount2);
			if (maxAmount2 == 0.0)
			{
				return;
			}
			resourceStats2.currentAmount = amount2;
			resourceStats2.maxAmount = maxAmount2;
		}
		primaryResources.ForEach(delegate(ResourceStats m)
		{
			m.amountRatio = m.currentAmount / m.maxAmount;
		});
		secondaryResources.ForEach(delegate(ResourceStats m)
		{
			m.amountRatio = m.currentAmount / m.maxAmount;
		});
		double percentageRatio = (double)Math.Abs(convertPercentage) / 100.0;
		retreivePrimary = false;
		retrieveSecondary = false;
		if (convertPercentage > 0f)
		{
			if (secondaryResources.Any((ResourceStats m) => percentageRatio > m.amountRatio))
			{
				retreivePrimary = true;
				neededAmount = secondaryResources.Min((ResourceStats m) => Math.Max(percentageRatio - m.amountRatio, 0.0) * m.maxAmount / m.conversionRatio);
				primaryResources.ForEach(delegate(ResourceStats m)
				{
					m.retrieveAmount = neededAmount;
				});
			}
			else if (percentageMinValue < 0f)
			{
				retrieveSecondary = true;
				availableSpaceInTarget = primaryResources.Min((ResourceStats m) => (m.maxAmount - m.currentAmount) / m.conversionRatio);
				secondaryResources.ForEach(delegate(ResourceStats m)
				{
					m.retrieveAmount = Math.Min(Math.Max(m.amountRatio - percentageRatio, 0.0) * m.maxAmount, availableSpaceInTarget);
				});
			}
		}
		else if (convertPercentage < 0f)
		{
			if (primaryResources.Any((ResourceStats m) => percentageRatio > m.amountRatio))
			{
				retrieveSecondary = true;
				neededAmount = primaryResources.Min((ResourceStats m) => Math.Max(percentageRatio - m.amountRatio, 0.0) * m.maxAmount / m.conversionRatio);
				secondaryResources.ForEach(delegate(ResourceStats m)
				{
					m.retrieveAmount = neededAmount;
				});
			}
			else if (percentageMaxValue > 0f)
			{
				retreivePrimary = true;
				availableSpaceInTarget = secondaryResources.Min((ResourceStats m) => (m.maxAmount - m.currentAmount) / m.conversionRatio);
				primaryResources.ForEach(delegate(ResourceStats m)
				{
					m.retrieveAmount = Math.Min(Math.Max(m.amountRatio - percentageRatio, 0.0) * m.maxAmount, availableSpaceInTarget);
				});
			}
		}
		transferRate = 0.0;
		conversionRatio = 0.0;
		if (retreivePrimary && primaryResources.Any((ResourceStats r) => r.retrieveAmount > 0.0))
		{
			for (int k = 0; k < primaryResources.Count; k++)
			{
				ResourceStats resourceStats3 = primaryResources[k];
				transferRate = resourceStats3.transferRate;
				double num2 = transferRate * num;
				if (num2 != 0.0)
				{
					double num3 = 1.0;
					if (primaryConversionCostPower)
					{
						base.part.GetConnectedResourceTotals(definitionPrimaryPowerResource.id, out var amount3, out var maxAmount3);
						double num4 = ((maxAmount3 > 0.0) ? (amount3 / maxAmount3) : 0.0);
						double val = maxAmount3 * Math.Max(0.0, num4 - (double)PowerUsagePercentageRatio);
						double num5 = ((resourceStats3.retrieveAmount >= num2) ? 1.0 : (resourceStats3.retrieveAmount / num2));
						double val2 = num5 * maxPowerPrimary * num * (double)primaryConversionEnergyMult;
						requestedPower = Math.Min(val, val2);
						double num6 = base.part.RequestResource(definitionPrimaryPowerResource.id, requestedPower);
						num3 = ((requestedPower > 0.0) ? (num6 / requestedPower) : 0.0);
					}
					double num7 = Math.Min(num2, resourceStats3.retrieveAmount);
					double demand = num7 * num3;
					double num8 = base.part.RequestResource(resourceStats3.definition.id, demand);
					primaryChange = 0f - (float)(num8 / num);
					if (primaryChange != 0f)
					{
						changedFieldCounter = 0;
					}
					primaryChangeField.guiActive = changedFieldCounter < 50;
					double num9 = 0.0;
					for (int l = 0; l < secondaryResources.Count; l++)
					{
						ResourceStats resourceStats4 = secondaryResources[l];
						conversionRatio = resourceStats4.conversionRatio;
						double demand2 = (0.0 - num8) * conversionRatio;
						double num10 = base.part.RequestResource(resourceStats4.definition.id, demand2);
						secondaryChange = 0f - (float)(num10 / num);
						secondaryChangeField.guiActive = secondaryChange != 0f;
						double num11 = num10 / conversionRatio;
						num9 += num11;
					}
					double num12 = base.part.RequestResource(resourceStats3.definition.id, num9 + num8);
					resourceStats3.retrieveAmount = resourceStats3.retrieveAmount - num8 - num12;
				}
			}
		}
		else if (retrieveSecondary && secondaryResources.Any((ResourceStats r) => r.retrieveAmount > 0.0))
		{
			for (int n = 0; n < secondaryResources.Count; n++)
			{
				ResourceStats resourceStats5 = secondaryResources[n];
				transferRate = resourceStats5.transferRate;
				double num13 = transferRate * num;
				if (num13 == 0.0)
				{
					continue;
				}
				double num14 = 1.0;
				if (secondaryConversionCostPower)
				{
					base.part.GetConnectedResourceTotals(definitionSecondaryPowerResource.id, out var amount4, out var maxAmount4);
					double num15 = ((maxAmount4 > 0.0) ? (amount4 / maxAmount4) : 0.0);
					double val3 = maxAmount4 * Math.Max(0.0, num15 - (double)PowerUsagePercentageRatio);
					double num16 = ((resourceStats5.retrieveAmount >= num13) ? 1.0 : (resourceStats5.retrieveAmount / num13));
					double val4 = num16 * maxPowerPrimary * num * (double)secondaryConversionEnergMult;
					requestedPower = Math.Min(val3, val4);
					double num17 = base.part.RequestResource(definitionSecondaryPowerResource.id, requestedPower);
					num14 = ((requestedPower > 0.0) ? (num17 / requestedPower) : 0.0);
				}
				double num18 = Math.Min(num13, resourceStats5.retrieveAmount);
				double num19 = base.part.RequestResource(resourceStats5.definition.id, num18 * num14);
				secondaryChange = 0f - (float)(num19 / num);
				secondaryChangeField.guiActive = secondaryChange != 0f;
				double num20 = 0.0;
				for (int num21 = 0; num21 < primaryResources.Count; num21++)
				{
					ResourceStats resourceStats6 = primaryResources[num21];
					conversionRatio = resourceStats6.conversionRatio;
					double demand3 = (0.0 - num19) * conversionRatio;
					double num22 = base.part.RequestResource(resourceStats6.definition.id, demand3);
					primaryChange = 0f - (float)(num22 / num);
					if (primaryChange != 0f)
					{
						changedFieldCounter = 0;
					}
					primaryChangeField.guiActive = changedFieldCounter < 50;
					double num23 = num22 / conversionRatio;
					num20 += num23;
				}
				double num24 = base.part.RequestResource(resourceStats5.definition.id, num20 + num19);
				resourceStats5.retrieveAmount = resourceStats5.retrieveAmount - num19 - num24;
			}
		}
		else
		{
			secondaryChangeField.guiActive = changedFieldCounter < 50;
			primaryChangeField.guiActive = changedFieldCounter < 50;
		}
	}

	public override string GetInfo()
	{
		return "Primary: " + primaryResourceNames + "\nSecondary: " + secondaryResourceNames;
	}

	public static void UpdateResourceConverterOffline(Vessel vessel)
	{
		foreach (ProtoPartSnapshot protoPartSnapshot in vessel.protoVessel.protoPartSnapshots)
		{
			ProtoPartModuleSnapshot protoPartModuleSnapshot = protoPartSnapshot.modules.FirstOrDefault((ProtoPartModuleSnapshot m) => m.moduleName == "InterstellarResourceConverter");
			if (protoPartModuleSnapshot == null)
			{
				continue;
			}
			string value = protoPartModuleSnapshot.moduleValues.GetValue("primaryConversionEnergyResource");
			string value2 = protoPartModuleSnapshot.moduleValues.GetValue("secondaryConversionEnergResource");
			int num = 0;
			foreach (ProtoPartResourceSnapshot resource in protoPartSnapshot.resources)
			{
				if (resource.resourceName == value)
				{
					resource.amount = resource.maxAmount;
					num++;
				}
				else if (resource.resourceName == value2)
				{
					resource.amount = resource.maxAmount;
					num++;
				}
				if (num == 2)
				{
					break;
				}
			}
		}
	}
}
