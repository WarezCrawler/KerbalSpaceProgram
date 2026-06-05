using System.Collections.Generic;
using System.Linq;

namespace InterstellarFuelSwitch;

public class IFSmodularTank
{
	public bool hasTech;

	public string GuiName = string.Empty;

	public string SwitchName = string.Empty;

	public string Composition = string.Empty;

	public string techReq;

	public double tankCost;

	public double tankMass;

	public double resourceMassDivider;

	public double resourceMassDividerAddition;

	public double habitatVolume;

	public double habitatSurface;

	public int crewCapacity;

	public List<IFSresource> Resources = new List<IFSresource>();

	public double FullResourceMass => Resources.Sum((IFSresource m) => m.FullMass);
}
