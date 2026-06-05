using System;

namespace InterstellarFuelSwitch;

[KSPModule("Radioactive Decay")]
internal class IFSRadioactiveDecay : PartModule
{
	[KSPField(isPersistant = false)]
	public double decayConstant;

	[KSPField(isPersistant = false)]
	public string resourceName = "";

	[KSPField(isPersistant = false)]
	public string decayProduct = "";

	[KSPField(isPersistant = false)]
	public double convFactor = 1.0;

	[KSPField(isPersistant = true)]
	public double lastActiveTime = 1.0;

	protected double density_rat = 1.0;

	private bool resourceDefinitionsContainDecayProduct;

	public override void OnStart(StartState state)
	{
		double num = lastActiveTime - Planetarium.GetUniversalTime();
		if (state == StartState.Editor)
		{
			return;
		}
		PartResource partResource = base.part.Resources[resourceName];
		if (partResource == null)
		{
			return;
		}
		resourceDefinitionsContainDecayProduct = PartResourceLibrary.Instance.resourceDefinitions.Contains(decayProduct);
		if (resourceDefinitionsContainDecayProduct)
		{
			float density = PartResourceLibrary.Instance.GetDefinition(decayProduct).density;
			if (density > 0f && partResource.info.density > 0f)
			{
				density_rat = partResource.info.density / PartResourceLibrary.Instance.GetDefinition(decayProduct).density;
			}
		}
		if (!CheatOptions.UnbreakableJoints && partResource != null && num > 0.0)
		{
			double amount = partResource.amount;
			partResource.amount = amount * Math.Exp((0.0 - decayConstant) * num);
			double num2 = amount - partResource.amount;
			if (resourceDefinitionsContainDecayProduct && num2 > 0.0)
			{
				base.part.RequestResource(decayProduct, (0.0 - num2) * density_rat);
			}
		}
	}

	public void FixedUpdate()
	{
		if (!HighLogic.LoadedSceneIsFlight)
		{
			return;
		}
		PartResource partResource = base.part.Resources[resourceName];
		if (partResource == null)
		{
			return;
		}
		lastActiveTime = Planetarium.GetUniversalTime();
		if (!CheatOptions.UnbreakableJoints)
		{
			double num = decayConstant * partResource.amount * (double)TimeWarp.fixedDeltaTime;
			partResource.amount -= num;
			if (resourceDefinitionsContainDecayProduct && num > 0.0)
			{
				base.part.RequestResource(decayProduct, (0.0 - num) * density_rat);
			}
		}
	}

	public override string GetInfo()
	{
		return "Radioactive Decay";
	}
}
