using System;
using System.Collections.Generic;
using B9PartSwitch.Utils;
using UnityEngine;

namespace B9PartSwitch.UI;

public class SwitcherSubtypeDescriptionGenerator
{
	private const string BAD_CHANGE_COLOR = "#ff3f3f";

	private const string GOOD_CHANGE_COLOR = "#3fff3f";

	private static readonly GroupedStringBuilder stringBuilder = new GroupedStringBuilder();

	private static readonly object lockObject = new object();

	private readonly ModuleB9PartSwitch module;

	private readonly float partDryMass;

	private readonly float partWetMass;

	private readonly float partDryCost;

	private readonly float partWetCost;

	private readonly float baseDryMass;

	private readonly float baseWetMass;

	private readonly float baseDryCost;

	private readonly float baseWetCost;

	private readonly bool showDryMass;

	private readonly bool showWetMass;

	private readonly bool showDryCost;

	private readonly bool showWetCost;

	private readonly bool showMaxTemp;

	private readonly bool showSkinMaxTemp;

	private readonly bool showCrashTolerance;

	private readonly float prefabMaxTemp;

	private readonly float prefabSkinMaxTemp;

	private readonly float prefabCrashTolerance;

	private readonly float currentMaxTemp;

	private readonly float currentSkinMaxTemp;

	private readonly float currentCrashTolerance;

	private readonly KeyValuePair<TankResource, float>[] parentResources;

	private readonly float baseParentVolume;

	public SwitcherSubtypeDescriptionGenerator(ModuleB9PartSwitch module)
	{
		module.ThrowIfNullArgument("module");
		this.module = module;
		Part prefab = module.part.GetPrefab();
		float mass = prefab.mass;
		partDryMass = mass + module.part.GetModuleMass(mass);
		partWetMass = partDryMass + module.part.GetResourceMassMax();
		baseDryMass = partDryMass - module.GetDryMass(module.CurrentSubtype) - module.GetParentDryMass(module.CurrentSubtype);
		baseWetMass = partWetMass - module.GetWetMass(module.CurrentSubtype) - module.GetParentWetMass(module.CurrentSubtype);
		float cost = module.part.partInfo.cost;
		partWetCost = cost + module.part.GetModuleCosts(cost);
		partDryCost = partWetCost - module.part.GetResourceCostMax();
		baseDryCost = partDryCost - module.GetDryCost(module.CurrentSubtype) - module.GetParentDryCost(module.CurrentSubtype);
		baseWetCost = partWetCost - module.GetWetCost(module.CurrentSubtype) - module.GetParentWetCost(module.CurrentSubtype);
		showWetMass = module.ChangesResourceMass;
		showWetMass |= module.Parent?.CurrentTankType.ChangesResourceMass ?? false;
		showDryMass = showWetMass;
		showDryMass |= module.ChangesDryMass;
		showDryMass |= (module.Parent?.CurrentTankType.tankMass ?? 0f) != 0f;
		showWetCost = module.ChangesResourceCost;
		showWetCost |= module.Parent?.CurrentTankType.ChangesResourceCost ?? false;
		showDryCost = showWetCost;
		showDryCost |= module.ChangesDryCost;
		showDryCost |= (module.Parent?.CurrentTankType.tankCost ?? 0f) != 0f;
		showMaxTemp = module.HasPartAspectLock("maxTemp");
		showSkinMaxTemp = module.HasPartAspectLock("skinMaxTemp");
		showCrashTolerance = module.HasPartAspectLock("crashTolerance");
		prefabMaxTemp = (float)prefab.maxTemp;
		prefabSkinMaxTemp = (float)prefab.skinMaxTemp;
		prefabCrashTolerance = prefab.crashTolerance;
		currentMaxTemp = (float)module.part.maxTemp;
		currentSkinMaxTemp = (float)module.part.skinMaxTemp;
		currentCrashTolerance = module.part.crashTolerance;
		float num = module.Parent?.GetTotalVolume(module.Parent.CurrentSubtype) ?? 0f;
		baseParentVolume = num - module.CurrentSubtype.volumeAddedToParent * module.VolumeScale;
		parentResources = new KeyValuePair<TankResource, float>[module.Parent?.CurrentTankType.resources.Count ?? 0];
		for (int i = 0; i < parentResources.Length; i++)
		{
			TankResource tankResource = module.Parent.CurrentTankType[i];
			parentResources[i] = new KeyValuePair<TankResource, float>(tankResource, tankResource.unitsPerVolume * num);
		}
	}

	public string GetFullSubtypeDescription(PartSubtype subtype)
	{
		lock (lockObject)
		{
			try
			{
				GetFullSubtypeDescriptionInternal(subtype);
				string text = stringBuilder.ToString();
				int num = text.Length - text.TrimStart().Length;
				int num2 = text.Length - text.TrimEnd().Length;
				if (num != 0)
				{
					Debug.LogError("[SwitcherSubtypeDescriptionGenerator] decription has leading whitespace: " + num);
				}
				if (num2 != 0)
				{
					Debug.LogError("[SwitcherSubtypeDescriptionGenerator] decription has trailing whitespace: " + num2);
				}
				return text;
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return "<color=orange><b>error generating description</b></color>\n\nhere's some placeholder text instead";
			}
			finally
			{
				stringBuilder.Clear();
			}
		}
	}

	private void GetFullSubtypeDescriptionInternal(PartSubtype subtype)
	{
		if (!subtype.descriptionSummary.IsNullOrEmpty())
		{
			stringBuilder.AppendLine(subtype.descriptionSummary);
		}
		stringBuilder.BeginGroup();
		if (subtype.tankType.resources.Count > 0 || parentResources.Length != 0)
		{
			stringBuilder.AppendLine("<b>{0}:</b>", Localization.SwitcherSubtypeDescriptionGenerator_Resources);
			foreach (TankResource item in subtype.tankType)
			{
				stringBuilder.AppendLine("  <color=#bfff3f>- {0}</color>: {1:0.#}", item.resourceDefinition.displayName, item.unitsPerVolume * module.GetTotalVolume(subtype));
			}
			float num = baseParentVolume + subtype.volumeAddedToParent * module.VolumeScale;
			KeyValuePair<TankResource, float>[] array = parentResources;
			for (int i = 0; i < array.Length; i++)
			{
				KeyValuePair<TankResource, float> keyValuePair = array[i];
				float num2 = keyValuePair.Key.unitsPerVolume * num;
				float num3 = num2 - keyValuePair.Value;
				stringBuilder.Append("  <color=#bfff3f>- {0}</color>: {1:0.#}", keyValuePair.Key.resourceDefinition.displayName, num2);
				if (!ApproximatelyZero(num3))
				{
					stringBuilder.Append(FormatResourceDifference(num3));
				}
				stringBuilder.AppendLine();
			}
		}
		float num4 = baseDryMass + module.GetDryMass(subtype) + module.GetParentDryMass(subtype);
		float num5 = baseWetMass + module.GetWetMass(subtype) + module.GetParentWetMass(subtype);
		float num6 = num4 - partDryMass;
		float num7 = num5 - partWetMass;
		float num8 = baseDryCost + module.GetDryCost(subtype) + module.GetParentDryCost(subtype);
		float num9 = baseWetCost + module.GetWetCost(subtype) + module.GetParentWetCost(subtype);
		float num10 = num8 - partDryCost;
		float num11 = num9 - partWetCost;
		stringBuilder.BeginGroup();
		if (showWetMass)
		{
			stringBuilder.Append("<b>{0}:</b> {1} {2}", Localization.SwitcherSubtypeDescriptionGenerator_Mass, Localization.SwitcherSubypeDescriptionGenerator_MassTons(num4, "0.###"), Localization.SwitcherSubtypeDescriptionGenerator_TankEmpty);
			if (!ApproximatelyZero(num6) && !ApproximatelyEqual(num6, num7))
			{
				stringBuilder.Append(FormatMassDifference(num6));
			}
			stringBuilder.Append(" / {0} {1}", Localization.SwitcherSubypeDescriptionGenerator_MassTons(num5, "0.###"), Localization.SwitcherSubtypeDescriptionGenerator_TankFull);
			if (!ApproximatelyZero(num7))
			{
				stringBuilder.Append(FormatMassDifference(num7));
			}
			stringBuilder.AppendLine();
		}
		else if (showDryMass)
		{
			stringBuilder.Append("<b>{0}:</b> {1}", Localization.SwitcherSubtypeDescriptionGenerator_Mass, Localization.SwitcherSubypeDescriptionGenerator_MassTons(num4, "0.###"));
			if (!ApproximatelyZero(num6))
			{
				stringBuilder.Append(FormatMassDifference(num6));
			}
			stringBuilder.AppendLine();
		}
		if (showWetCost)
		{
			stringBuilder.Append("<b>{0}:</b> <sprite=\"CurrencySpriteAsset\" name=\"Funds\" tint=1> {1:0.#} {2}", Localization.SwitcherSubtypeDescriptionGenerator_Cost, num8, Localization.SwitcherSubtypeDescriptionGenerator_TankEmpty);
			if (!ApproximatelyZero(num10) && !ApproximatelyEqual(num10, num11))
			{
				stringBuilder.Append(FormatCostDifference(num10));
			}
			stringBuilder.Append(" / <sprite=\"CurrencySpriteAsset\" name=\"Funds\" tint=1> {0:0.#} {1}", num9, Localization.SwitcherSubtypeDescriptionGenerator_TankFull);
			if (!ApproximatelyZero(num11))
			{
				stringBuilder.Append(FormatCostDifference(num11));
			}
			stringBuilder.AppendLine();
		}
		else if (showDryCost)
		{
			stringBuilder.Append("<b>{0}:</b> <sprite=\"CurrencySpriteAsset\" name=\"Funds\" tint=1> {1:#.#}", Localization.SwitcherSubtypeDescriptionGenerator_Cost, num8);
			if (!ApproximatelyZero(num10))
			{
				stringBuilder.Append(FormatCostDifference(num10));
			}
			stringBuilder.AppendLine();
		}
		if (showMaxTemp)
		{
			float num12 = ((subtype.maxTemp > 0f) ? subtype.maxTemp : prefabMaxTemp);
			stringBuilder.Append("<b>{0}:</b> {1}", Localization.SwitcherSubtypeDescriptionGenerator_MaxTemp, Localization.SwitcherSubypeDescriptionGenerator_TemperatureKelvins(num12, "#"));
			if (!ApproximatelyEqual(num12, currentMaxTemp))
			{
				stringBuilder.Append(FormatTemperatureDifference(num12 - currentMaxTemp));
			}
			stringBuilder.AppendLine();
		}
		if (showSkinMaxTemp)
		{
			float num13 = ((subtype.skinMaxTemp > 0f) ? subtype.skinMaxTemp : prefabSkinMaxTemp);
			stringBuilder.Append("<b>{0}:</b> {1}", Localization.SwitcherSubtypeDescriptionGenerator_MaxSkinTemp, Localization.SwitcherSubypeDescriptionGenerator_TemperatureKelvins(num13, "#"));
			if (!ApproximatelyEqual(num13, currentSkinMaxTemp))
			{
				stringBuilder.Append(FormatTemperatureDifference(num13 - currentSkinMaxTemp));
			}
			stringBuilder.AppendLine();
		}
		if (showCrashTolerance)
		{
			float num14 = ((subtype.crashTolerance > 0f) ? subtype.crashTolerance : prefabCrashTolerance);
			stringBuilder.Append("<b>{0}:</b> {1}", Localization.SwitcherSubtypeDescriptionGenerator_CrashTolerance, Localization.SwitcherSubypeDescriptionGenerator_SpeedMetersPerSecond(num14, "#"));
			if (!ApproximatelyEqual(num14, currentCrashTolerance))
			{
				stringBuilder.Append(FormatSpeedDifference(num14 - currentCrashTolerance));
			}
			stringBuilder.AppendLine();
		}
		stringBuilder.BeginGroup();
		if (!subtype.descriptionDetail.IsNullOrEmpty())
		{
			stringBuilder.AppendLine(subtype.descriptionDetail);
		}
	}

	private static bool ApproximatelyEqual(float a, float b)
	{
		if (a != b)
		{
			return (double)Mathf.Abs(a - b) < 0.0001;
		}
		return true;
	}

	private static bool ApproximatelyZero(float a)
	{
		return ApproximatelyEqual(a, 0f);
	}

	private static string FormatMassDifference(float massDifference)
	{
		string text = ((massDifference > 0f) ? "#ff3f3f" : "#3fff3f");
		string text2 = Localization.SwitcherSubypeDescriptionGenerator_MassTons(massDifference, "+0.###;-0.###");
		return " (<color=" + text + ">" + text2 + "</color>)";
	}

	private static string FormatCostDifference(float costDifference)
	{
		string text = ((costDifference > 0f) ? "#ff3f3f" : "#3fff3f");
		string text2 = costDifference.ToString("+0.#;-0.#");
		return " (<color=" + text + ">" + text2 + "</color>)";
	}

	private static string FormatTemperatureDifference(float temperatureDifference)
	{
		string text = ((temperatureDifference > 0f) ? "#3fff3f" : "#ff3f3f");
		string text2 = Localization.SwitcherSubypeDescriptionGenerator_TemperatureKelvins(temperatureDifference, "+#;-#");
		return " (<color=" + text + ">" + text2 + "</color>)";
	}

	private static string FormatSpeedDifference(float speedDifference)
	{
		string text = ((speedDifference > 0f) ? "#3fff3f" : "#ff3f3f");
		string text2 = Localization.SwitcherSubypeDescriptionGenerator_SpeedMetersPerSecond(speedDifference, "+#;-#");
		return " (<color=" + text + ">" + text2 + "</color>)";
	}

	private static string FormatResourceDifference(float resourceDifference)
	{
		string arg = ((resourceDifference > 0f) ? "#3fff3f" : "#ff3f3f");
		return $" (<color={arg}>{resourceDifference:+0.#;-0.#}</color>)";
	}
}
