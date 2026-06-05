using System.Collections;
using System.Collections.Generic;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.Utils;
using UniLinq;
using UnityEngine;

namespace B9PartSwitch;

public class TankType : IContextualNode, IEnumerable<TankResource>, IEnumerable
{
	[NodeData(name = "name")]
	public string tankName;

	[NodeData]
	public Color? primaryColor;

	[NodeData]
	public Color? secondaryColor;

	[NodeData]
	public float tankMass;

	[NodeData]
	public float tankCost;

	[NodeData]
	public float? percentFilled;

	[NodeData]
	public bool? resourcesTweakable;

	[NodeData(name = "RESOURCE")]
	public List<TankResource> resources = new List<TankResource>();

	public TankResource this[int index] => resources[index];

	public TankResource this[string name] => resources.FirstOrDefault((TankResource r) => r.ResourceName == name);

	public int ResourcesCount => resources.Count;

	public IEnumerable<string> ResourceNames => resources.Select((TankResource r) => r.ResourceName);

	public bool IsStructuralTankType
	{
		get
		{
			if (tankName == "Structural" && tankMass == 0f && tankCost == 0f)
			{
				return resources.Count == 0;
			}
			return false;
		}
	}

	public float ResourceUnitMass => resources.Sum((TankResource r) => r.unitsPerVolume * r.resourceDefinition.density);

	public float ResourceUnitCost => resources.Sum((TankResource r) => r.unitsPerVolume * r.resourceDefinition.unitCost);

	public float TotalUnitMass => ResourceUnitMass + tankMass;

	public float TotalUnitCost => ResourceUnitCost + tankCost;

	public bool ChangesResourceMass => resources.Any((TankResource r) => r.resourceDefinition.density != 0f);

	public bool ChangesMass
	{
		get
		{
			if (tankMass == 0f)
			{
				return ChangesResourceMass;
			}
			return true;
		}
	}

	public bool ChangesResourceCost => resources.Any((TankResource r) => r.resourceDefinition.unitCost != 0f);

	public bool ChangesCost
	{
		get
		{
			if (tankCost == 0f)
			{
				return ChangesResourceCost;
			}
			return true;
		}
	}

	public bool ContainsResource(string resourceName)
	{
		return resources.Any((TankResource r) => r.ResourceName == resourceName);
	}

	public void Load(ConfigNode node, OperationContext context)
	{
		this.LoadFields(node, context);
		OnLoad();
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		this.SaveFields(node, context);
	}

	public List<TankResource>.Enumerator GetEnumerator()
	{
		return resources.GetEnumerator();
	}

	IEnumerator<TankResource> IEnumerable<TankResource>.GetEnumerator()
	{
		return GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return GetEnumerator();
	}

	public override string ToString()
	{
		string text = $"TankType: {tankName}, mass = {tankMass}, cost = {tankCost}";
		foreach (TankResource resource in resources)
		{
			text += "\n\t ";
			text = ((resource == null) ? (text + "Null Tank Resource") : (text + resource.ToString()));
		}
		return text;
	}

	private void OnLoad()
	{
		SetDefaultColors();
	}

	private void SetDefaultColors()
	{
		if (!primaryColor.IsNotNull() && !secondaryColor.IsNotNull())
		{
			if (resources.Count == 1 && resources[0].ResourceName == "LiquidFuel")
			{
				primaryColor = ResourceColors.LiquidFuel;
			}
			else if (resources.Count == 2 && resources[0].ResourceName == "LiquidFuel" && resources[1].ResourceName == "Oxidizer")
			{
				primaryColor = ResourceColors.LiquidFuel;
				secondaryColor = ResourceColors.Oxidizer;
			}
			else if (resources.Count == 1 && resources[0].ResourceName == "MonoPropellant")
			{
				primaryColor = ResourceColors.MonoPropellant;
			}
			else if (resources.Count == 1 && resources[0].ResourceName == "ElectricCharge")
			{
				primaryColor = ResourceColors.ElectricChargePrimary;
				secondaryColor = ResourceColors.ElectricChargeSecondary;
			}
			else if (resources.Count == 1 && resources[0].ResourceName == "LqdHydrogen")
			{
				primaryColor = ResourceColors.LqdHydrogen;
			}
			else if (resources.Count == 2 && resources[0].ResourceName == "LqdHydrogen" && resources[1].ResourceName == "Oxidizer")
			{
				primaryColor = ResourceColors.LqdHydrogen;
				secondaryColor = ResourceColors.Oxidizer;
			}
			else if (resources.Count == 1 && resources[0].ResourceName == "LqdMethane")
			{
				primaryColor = ResourceColors.LqdMethane;
			}
			else if (resources.Count == 2 && resources[0].ResourceName == "LqdMethane" && resources[1].ResourceName == "Oxidizer")
			{
				primaryColor = ResourceColors.LqdMethane;
				secondaryColor = ResourceColors.Oxidizer;
			}
			else if (resources.Count == 1 && resources[0].ResourceName == "Oxidizer")
			{
				primaryColor = ResourceColors.Oxidizer;
			}
			else if (resources.Count == 1 && resources[0].ResourceName == "XenonGas")
			{
				primaryColor = ResourceColors.XenonGas;
			}
			else if (resources.Count == 1 && resources[0].ResourceName == "Ore")
			{
				primaryColor = ResourceColors.Ore;
			}
		}
	}
}
