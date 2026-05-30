using System;
using System.Collections.Generic;
using ns9;

public class ModuleAsteroidInfo : PartModule
{
	protected class ResourceData
	{
		public string Name;

		public float Weight;
	}

	[KSPField(guiActive = true, guiActiveEditor = false, guiName = "#autoLOC_462423")]
	public string displayMass = "???";

	[KSPField(isPersistant = true)]
	public string massThreshold = "0";

	[KSPField(isPersistant = true)]
	public string currentMass = "0";

	[KSPField(guiActive = true, guiActiveEditor = false, guiName = "#autoLOC_6001746")]
	public string resources = "???";

	protected ModuleAsteroid baseMod;

	public virtual double currentMassVal
	{
		get
		{
			double result = 0.0;
			double.TryParse(currentMass, out result);
			return result;
		}
		set
		{
			currentMass = value.ToString("G17");
		}
	}

	public virtual double massThresholdVal
	{
		get
		{
			double result = 0.0;
			double.TryParse(massThreshold, out result);
			return result;
		}
		set
		{
			massThreshold = value.ToString("G17");
		}
	}

	public override void OnStart(StartState state)
	{
		baseMod = base.part.Modules.GetModule<ModuleAsteroid>();
		if ((object)baseMod != null)
		{
			baseMod.OnStart(state);
			if (currentMassVal <= 1E-09)
			{
				currentMassVal = base.part.mass;
			}
			if (massThresholdVal <= 1E-09)
			{
				SetupAsteroidResources();
			}
			base.part.force_activate();
			baseMod.SetAsteroidMass((float)currentMassVal);
			base.part.mass = (float)currentMassVal;
		}
	}

	protected virtual void SetupAsteroidResources()
	{
		if (!HighLogic.LoadedSceneIsFlight)
		{
			return;
		}
		KSPRandom kSPRandom = new KSPRandom();
		double num = (double)kSPRandom.Next(5, 20) / 100.0;
		double num2 = 1.0 - num;
		massThresholdVal = currentMassVal * num;
		List<ResourceData> list = new List<ResourceData>();
		List<ModuleAsteroidResource> list2 = base.part.FindModulesImplementing<ModuleAsteroidResource>();
		int count = list2.Count;
		float num3 = 0f;
		for (int i = 0; i < count; i++)
		{
			ModuleAsteroidResource moduleAsteroidResource = list2[i];
			if (kSPRandom.Next(100) < moduleAsteroidResource.presenceChance)
			{
				ResourceData resourceData = new ResourceData
				{
					Name = moduleAsteroidResource.resourceName,
					Weight = kSPRandom.Next(moduleAsteroidResource.lowRange, moduleAsteroidResource.highRange)
				};
				list.Add(resourceData);
				num3 += resourceData.Weight;
			}
		}
		int count2 = list.Count;
		for (int j = 0; j < count; j++)
		{
			ModuleAsteroidResource moduleAsteroidResource2 = list2[j];
			for (int k = 0; k < count2; k++)
			{
				ResourceData resourceData2 = list[k];
				if (resourceData2.Name == moduleAsteroidResource2.resourceName)
				{
					float val = resourceData2.Weight / num3;
					moduleAsteroidResource2.abundance = Math.Max(0.01f, val);
					moduleAsteroidResource2.displayAbundance = moduleAsteroidResource2.abundance * (float)num2;
				}
			}
		}
	}

	public virtual void Update()
	{
		if (HighLogic.LoadedSceneIsFlight)
		{
			displayMass = $"{currentMassVal:0.00000}t";
			if ((object)baseMod != null)
			{
				baseMod.SetAsteroidMass((float)currentMassVal);
			}
			resources = Localizer.Format("#autoLOC_6001049", (currentMassVal - massThresholdVal).ToString("0.00000"), ((currentMassVal - massThresholdVal) / currentMassVal * 100.0).ToString("0.000"));
		}
	}
}
