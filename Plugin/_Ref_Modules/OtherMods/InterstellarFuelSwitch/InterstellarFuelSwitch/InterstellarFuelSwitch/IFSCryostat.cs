using System;

namespace InterstellarFuelSwitch;

[KSPModule("Cryostat")]
internal class IFSCryostat : PartModule
{
	public const string STOCK_RESOURCE_ELECTRICCHARGE = "ElectricCharge";

	public const string FNRESOURCE_MEGAJOULES = "Megajoules";

	public const int KEBRIN_HOURS_DAY = 8;

	public const int SECONDS_IN_HOUR = 3600;

	public const int KEBRIN_DAY_SECONDS = 28800;

	[UI_Toggle(disabledText = "On", enabledText = "Off")]
	[KSPField(isPersistant = true, guiActive = true, guiName = "Cooling")]
	public bool isDisabled;

	[KSPField(isPersistant = true)]
	public double storedTemp;

	[KSPField]
	public string resourceName = "";

	[KSPField]
	public string resourceGUIName = "";

	[KSPField]
	public double boilOffRate;

	[KSPField]
	public double powerReqKW;

	[KSPField]
	public double powerReqMult = 1.0;

	[KSPField]
	public double boilOffMultiplier;

	[KSPField]
	public double boilOffBase = 10000.0;

	[KSPField]
	public double boilOffAddition;

	[KSPField]
	public double boilOffTemp = 20.271;

	[KSPField]
	public double convectionMod = 1.0;

	[KSPField]
	public bool showPower = true;

	[KSPField]
	public bool showBoiloff = true;

	[KSPField]
	public bool showTemp = true;

	[KSPField]
	public bool warningShown;

	[KSPField]
	public int initializationCountdown = 10;

	[KSPField(isPersistant = false, guiActive = false, guiName = "Power")]
	public string powerStatusStr = string.Empty;

	[KSPField(isPersistant = false, guiActive = false, guiName = "Boiloff")]
	public string boiloffStr;

	[KSPField(isPersistant = false, guiActive = false, guiName = "Temperature", guiFormat = "F3", guiUnits = " K")]
	public double externalTemperature;

	[KSPField(isPersistant = false, guiActive = false, guiName = "internal boiloff")]
	public double boiloff;

	private BaseField isDisabledField;

	private BaseField boiloffStrField;

	private BaseField powerStatusStrField;

	private BaseField externalTemperatureField;

	private double environmentBoiloff;

	private double environmentFactor;

	private double recievedPowerKW;

	private double previousRecievedPowerKW;

	private double currentPowerReq;

	private double previousPowerReq;

	private double previousPowerUsage;

	private bool requiresPower;

	private float previousDeltaTime;

	public override void OnStart(StartState state)
	{
		base.enabled = true;
		base.part.temperature = storedTemp;
		requiresPower = powerReqKW > 0.0;
		isDisabledField = base.Fields["isDisabled"];
		boiloffStrField = base.Fields["boiloffStr"];
		powerStatusStrField = base.Fields["powerStatusStr"];
		externalTemperatureField = base.Fields["externalTemperature"];
		if (state != StartState.Editor)
		{
			base.part.temperature = storedTemp;
			base.part.skinTemperature = storedTemp;
			if (!base.part.Resources.Contains("ElectricCharge"))
			{
				ConfigNode configNode = new ConfigNode("RESOURCE");
				configNode.AddValue("name", "ElectricCharge");
				configNode.AddValue("maxAmount", (powerReqKW > 0.0) ? (powerReqKW / 50.0) : 1.0);
				configNode.AddValue("amount", (powerReqKW > 0.0) ? (powerReqKW / 50.0) : 1.0);
				base.part.AddResource(configNode);
			}
		}
	}

	private void UpdateElectricChargeBuffer(double currentPowerUsage)
	{
		PartResource partResource = base.part.Resources["ElectricCharge"];
		if (partResource != null && (TimeWarp.fixedDeltaTime != previousDeltaTime || previousPowerUsage != currentPowerUsage))
		{
			double num = 2.0 * currentPowerUsage * (double)TimeWarp.fixedDeltaTime;
			double num2 = ((partResource.maxAmount > 0.0) ? (partResource.amount / partResource.maxAmount) : 0.0);
			partResource.maxAmount = num;
			partResource.amount = num2 * num;
		}
		previousPowerUsage = currentPowerUsage;
		previousDeltaTime = TimeWarp.fixedDeltaTime;
	}

	public void Update()
	{
		storedTemp = base.part.temperature;
		if (initializationCountdown > 0)
		{
			initializationCountdown--;
		}
		PartResource partResource = base.part.Resources[resourceName];
		if (partResource != null)
		{
			if (HighLogic.LoadedSceneIsEditor)
			{
				isDisabledField.guiActiveEditor = true;
				return;
			}
			isDisabledField.guiActive = powerReqKW > 0.0;
			bool flag = partResource.amount > 1E-07 && (boilOffRate > 0.0 || requiresPower);
			powerStatusStrField.guiActive = showPower && requiresPower && flag;
			boiloffStrField.guiActive = showBoiloff && boiloff > 1E-05;
			externalTemperatureField.guiActive = showTemp && flag;
			if (!flag)
			{
				currentPowerReq = 0.0;
				return;
			}
			double num = ((convectionMod == -1.0) ? 0.0 : (convectionMod + base.part.atmDensity / (convectionMod + 1.0)));
			externalTemperature = base.part.temperature;
			if (double.IsNaN(externalTemperature) || double.IsInfinity(externalTemperature))
			{
				base.part.temperature = base.part.skinTemperature;
				externalTemperature = base.part.skinTemperature;
			}
			double num2 = Math.Max(0.0, externalTemperature - boilOffTemp) / 300.0;
			environmentFactor = num * num2;
			if (powerReqKW > 0.0)
			{
				currentPowerReq = powerReqKW * 0.2 * environmentFactor * powerReqMult;
				UpdatePowerStatusSting();
			}
			else
			{
				currentPowerReq = 0.0;
			}
			environmentBoiloff = environmentFactor * boilOffMultiplier * boilOffBase;
		}
		else
		{
			boiloffStrField.guiActive = false;
			powerStatusStrField.guiActive = false;
			if (HighLogic.LoadedSceneIsEditor)
			{
				isDisabledField.guiActiveEditor = false;
			}
			else
			{
				isDisabledField.guiActive = false;
			}
		}
	}

	private void UpdatePowerStatusSting()
	{
		powerStatusStr = ((currentPowerReq < 1000.0) ? (recievedPowerKW.ToString("0.00") + " KW / " + currentPowerReq.ToString("0.00") + " KW") : ((currentPowerReq < 1000000.0) ? ((recievedPowerKW / 1000.0).ToString("0.000") + " MW / " + (currentPowerReq / 1000.0).ToString("0.000") + " MW") : ((recievedPowerKW / 1000000.0).ToString("0.000") + " GW / " + (currentPowerReq / 1000000.0).ToString("0.000") + " GW")));
	}

	public void FixedUpdate()
	{
		PartResource partResource = base.part.Resources[resourceName];
		if (partResource == null || double.IsPositiveInfinity(currentPowerReq))
		{
			boiloff = 0.0;
			return;
		}
		double num = (double)(decimal)Math.Round(TimeWarp.fixedDeltaTime, 7);
		if (!isDisabled && currentPowerReq > 0.0)
		{
			UpdateElectricChargeBuffer(Math.Max(currentPowerReq, 0.1 * powerReqKW));
			double num2 = currentPowerReq * num;
			double num3 = (CheatOptions.InfiniteElectricity ? num2 : 0.0);
			if (num3 <= num2)
			{
				num3 += base.part.RequestResource("Megajoules", (num2 - num3) / 1000.0) * 1000.0;
			}
			if (currentPowerReq < 1000.0 && num3 <= num2)
			{
				num3 += base.part.RequestResource("ElectricCharge", num2 - num3);
			}
			recievedPowerKW = num3 / num;
		}
		else
		{
			recievedPowerKW = 0.0;
		}
		bool flag = initializationCountdown == 0 && powerReqKW > 0.0 && currentPowerReq > 0.0 && recievedPowerKW < currentPowerReq && previousRecievedPowerKW < previousPowerReq;
		double num4 = ((!flag) ? boilOffRate : (boilOffRate + boilOffAddition * (1.0 - recievedPowerKW / currentPowerReq)));
		boiloff = ((CheatOptions.IgnoreMaxTemperature || num4 <= 0.0) ? 0.0 : (num4 * environmentBoiloff));
		if (boiloff > 1E-10)
		{
			partResource.amount = Math.Max(0.0, partResource.amount - boiloff * num);
			boiloffStr = boiloff.ToString("0.0000000") + " L/s " + partResource.resourceName;
			if (flag && base.part.vessel.isActiveVessel && !warningShown)
			{
				warningShown = true;
				ScreenMessages.PostScreenMessage("Warning: " + boiloffStr + " Boiloff", 5f, ScreenMessageStyle.UPPER_CENTER);
			}
		}
		else
		{
			warningShown = false;
			boiloffStr = "0.0000000 L/s " + partResource.resourceName;
		}
		previousPowerReq = currentPowerReq;
		previousRecievedPowerKW = recievedPowerKW;
	}

	public override string GetInfo()
	{
		return "<size=10>" + resourceName + " @ " + boilOffTemp + " K</size>";
	}
}
