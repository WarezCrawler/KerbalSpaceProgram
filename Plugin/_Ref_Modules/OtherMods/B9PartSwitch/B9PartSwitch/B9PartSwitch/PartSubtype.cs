using System;
using System.Collections.Generic;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;
using B9PartSwitch.Utils;
using UniLinq;
using UnityEngine;

namespace B9PartSwitch;

public class PartSubtype : IContextualNode
{
	[NodeData(name = "name")]
	public string subtypeName;

	[NodeData]
	public string title;

	[NodeData]
	public string descriptionSummary;

	[NodeData]
	public string descriptionDetail;

	[NodeData]
	public Color? primaryColor;

	[NodeData]
	public Color? secondaryColor;

	[NodeData]
	public string upgradeRequired;

	[NodeData]
	public float defaultSubtypePriority;

	[NodeData(name = "transform")]
	public List<IStringMatcher> transformNames = new List<IStringMatcher>();

	[NodeData(name = "node")]
	public List<IStringMatcher> nodeNames = new List<IStringMatcher>();

	[NodeData(name = "TEXTURE")]
	public List<TextureSwitchInfo> textureSwitches = new List<TextureSwitchInfo>();

	[NodeData(name = "MATERIAL")]
	public List<MaterialModifierInfo> materialModifierInfos = new List<MaterialModifierInfo>();

	[NodeData(name = "NODE")]
	public List<AttachNodeModifierInfo> attachNodeModifierInfos = new List<AttachNodeModifierInfo>();

	[NodeData(name = "TRANSFORM")]
	public List<TransformModifierInfo> transformModifierInfos = new List<TransformModifierInfo>();

	[NodeData(name = "MODULE")]
	public List<ModuleModifierInfo> moduleModifierInfos = new List<ModuleModifierInfo>();

	[NodeData]
	public float addedMass;

	[NodeData]
	public float addedCost;

	[UseParser(typeof(TankTypeValueParser))]
	[NodeData]
	public TankType tankType;

	[NodeData]
	public float volumeMultiplier = 1f;

	[NodeData]
	public float volumeAdded;

	[NodeData]
	public float volumeAddedToParent;

	[NodeData]
	public float? percentFilled;

	[NodeData]
	public bool? resourcesTweakable;

	[NodeData]
	public float maxTemp;

	[NodeData]
	public float skinMaxTemp;

	[NodeData]
	public AttachNode attachNode;

	[NodeData]
	public float crashTolerance;

	[NodeData]
	public Vector3 CoMOffset = Vector3Extensions.NaN();

	[NodeData]
	public Vector3 CoPOffset = Vector3Extensions.NaN();

	[NodeData]
	public Vector3 CoLOffset = Vector3Extensions.NaN();

	[NodeData]
	public Vector3 CenterOfBuoyancy = Vector3Extensions.NaN();

	[NodeData]
	public Vector3 CenterOfDisplacement = Vector3Extensions.NaN();

	[NodeData]
	public int stackSymmetry = -1;

	[NodeData]
	public bool allowSwitchInFlight = true;

	[NodeData]
	public string mirrorSymmetrySubtype;

	private ModuleB9PartSwitch parent;

	private readonly List<Transform> transforms = new List<Transform>();

	private readonly List<AttachNode> nodes = new List<AttachNode>();

	private readonly List<IPartModifier> partModifiers = new List<IPartModifier>();

	private readonly List<object> aspectLocks = new List<object>();

	public string Name => subtypeName;

	public bool HasTank
	{
		get
		{
			if (tankType != null)
			{
				return tankType.ResourcesCount > 0;
			}
			return false;
		}
	}

	public bool HasUpgradeRequired => !upgradeRequired.IsNullOrEmpty();

	public IEnumerable<string> ResourceNames => tankType.ResourceNames;

	public bool ChangesGeometry => partModifiers.Any((IPartModifier modifier) => modifier.ChangesGeometry);

	public bool ChangesDryMass
	{
		get
		{
			if (addedMass == 0f)
			{
				return tankType.tankMass != 0f;
			}
			return true;
		}
	}

	public bool ChangesMass
	{
		get
		{
			if (addedMass == 0f)
			{
				return tankType.ChangesMass;
			}
			return true;
		}
	}

	public bool ChangesDryCost
	{
		get
		{
			if (addedCost == 0f)
			{
				return tankType.tankCost != 0f;
			}
			return true;
		}
	}

	public bool ChangesCost
	{
		get
		{
			if (addedCost == 0f)
			{
				return tankType.ChangesCost;
			}
			return true;
		}
	}

	public IEnumerable<object> PartAspectLocks => aspectLocks.All();

	public Color PrimaryColor => primaryColor ?? tankType.primaryColor ?? Color.white;

	public Color SecondaryColor => secondaryColor ?? tankType.secondaryColor ?? primaryColor ?? tankType.primaryColor ?? Color.gray;

	public void Load(ConfigNode node, OperationContext context)
	{
		OperationContext context2;
		try
		{
			context2 = this.LoadFields(node, context);
		}
		catch (Exception innerException)
		{
			throw new Exception($"Exception while loading fields on subtype {this}", innerException);
		}
		OnLoad(node, context2);
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		try
		{
			this.SaveFields(node, context);
		}
		catch (Exception innerException)
		{
			throw new Exception($"Exception while loading fields on subtype {this}", innerException);
		}
	}

	private void OnLoad(ConfigNode node, OperationContext context)
	{
		if (Name.IsNullOrEmpty())
		{
			SeriousWarningHandler.DisplaySeriousWarning($"Subtype has no name: {this}");
			LogError("Subtype has no name");
		}
		if (HasUpgradeRequired && PartUpgradeManager.Handler.GetUpgrade(upgradeRequired).IsNull())
		{
			SeriousWarningHandler.DisplaySeriousWarning($"Upgrade does not exist: {upgradeRequired} on: {this}");
			LogError("Upgrade does not exist: " + upgradeRequired);
			upgradeRequired = null;
		}
		if (tankType == null)
		{
			tankType = B9TankSettings.StructuralTankType;
		}
		if (mirrorSymmetrySubtype == null)
		{
			mirrorSymmetrySubtype = Name;
		}
		if (context.Operation == Operation.LoadPrefab)
		{
			if (title.IsNullOrEmpty())
			{
				title = subtypeName;
			}
			ConfigNode[] array = node.GetNodes("RESOURCE");
			if (array.Length != 0)
			{
				LoadAdditionalResources(array, context);
			}
		}
	}

	public void OnBeforeReinitializeInactiveSubtype()
	{
		foreach (IPartModifier partModifier in partModifiers)
		{
			partModifier.OnBeforeReinitializeInactiveSubtype();
		}
	}

	public void OnBeforeReinitializeActiveSubtype()
	{
		foreach (IPartModifier partModifier in partModifiers)
		{
			partModifier.OnBeforeReinitializeActiveSubtype();
		}
	}

	public void OnAfterReinitializeInactiveSubtype()
	{
		foreach (IPartModifier partModifier in partModifiers)
		{
			partModifier.OnAfterReinitializeInactiveSubtype();
		}
	}

	public void OnAfterReinitializeActiveSubtype()
	{
		foreach (IPartModifier partModifier in partModifiers)
		{
			partModifier.OnAfterReinitializeActiveSubtype();
		}
	}

	public void Setup(ModuleB9PartSwitch parent, bool displayWarnings = true)
	{
		if (parent == null)
		{
			throw new ArgumentNullException("parent cannot be null");
		}
		if (parent.part == null)
		{
			throw new ArgumentNullException("parent.part cannot be null");
		}
		this.parent = parent;
		aspectLocks.Clear();
		Part part = parent.part;
		Part part2 = part.GetPrefab() ?? part;
		partModifiers.Clear();
		IEnumerable<object> aspectLocksOnOtherModules = parent.PartAspectLocksOnOtherModules;
		string errorString = null;
		if (maxTemp > 0f)
		{
			MaybeAddModifier(new PartMaxTempModifier(part, part2.maxTemp, maxTemp));
		}
		if (skinMaxTemp > 0f)
		{
			MaybeAddModifier(new PartSkinMaxTempModifier(part, part2.skinMaxTemp, skinMaxTemp));
		}
		if (crashTolerance > 0f)
		{
			MaybeAddModifier(new PartCrashToleranceModifier(part, part2.crashTolerance, crashTolerance));
		}
		if (attachNode.IsNotNull())
		{
			if (part.attachRules.srfAttach)
			{
				if (part.srfAttachNode.IsNotNull())
				{
					MaybeAddModifier(new PartAttachNodeModifier(part.srfAttachNode, part2.srfAttachNode, attachNode, parent));
				}
				else
				{
					OnInitializationError("attachNode specified but part does not have a surface attach node");
				}
			}
			else
			{
				OnInitializationError("attachNode specified but part does not allow surface attach");
			}
		}
		if (CoMOffset.IsFinite())
		{
			MaybeAddModifier(new PartCoMOffsetModifier(part, part2.CoMOffset, CoMOffset));
		}
		if (CoPOffset.IsFinite())
		{
			MaybeAddModifier(new PartCoPOffsetModifier(part, part2.CoPOffset, CoPOffset));
		}
		if (CoLOffset.IsFinite())
		{
			MaybeAddModifier(new PartCoLOffsetModifier(part, part2.CoLOffset, CoLOffset));
		}
		if (CenterOfBuoyancy.IsFinite())
		{
			MaybeAddModifier(new PartCenterOfBuoyancyModifier(part, part2.CenterOfBuoyancy, CenterOfBuoyancy));
		}
		if (CenterOfDisplacement.IsFinite())
		{
			MaybeAddModifier(new PartCenterOfDisplacementModifier(part, part2.CenterOfDisplacement, CenterOfDisplacement));
		}
		if (stackSymmetry >= 0)
		{
			MaybeAddModifier(new PartStackSymmetryModifier(part, part2.stackSymmetry, stackSymmetry));
		}
		foreach (AttachNodeModifierInfo attachNodeModifierInfo in attachNodeModifierInfos)
		{
			foreach (IPartModifier item in attachNodeModifierInfo.CreatePartModifiers(part, parent, OnInitializationError))
			{
				MaybeAddModifier(item);
			}
		}
		foreach (TextureSwitchInfo textureSwitch in textureSwitches)
		{
			foreach (TextureReplacement item2 in textureSwitch.CreateTextureReplacements(part, OnInitializationError))
			{
				MaybeAddModifier(item2);
			}
		}
		foreach (MaterialModifierInfo materialModifierInfo in materialModifierInfos)
		{
			foreach (IPartModifier item3 in materialModifierInfo.CreateModifiers(part.GetModelRoot(), OnInitializationError))
			{
				MaybeAddModifier(item3);
			}
		}
		nodes.Clear();
		foreach (IStringMatcher nodeName in nodeNames)
		{
			bool flag = false;
			foreach (AttachNode attachNode in part.attachNodes)
			{
				if (nodeName.Match(attachNode.id))
				{
					flag = true;
					if (attachNode.nodeType != 0)
					{
						OnInitializationError("Node " + attachNode.id + " is not a stack node, and thus cannot be managed by ModuleB9PartSwitch");
						continue;
					}
					nodes.Add(attachNode);
					partModifiers.Add(new AttachNodeToggler(attachNode));
				}
			}
			if (!flag)
			{
				OnInitializationError($"No attach nodes matching '{nodeName}' found");
			}
		}
		if (HasTank)
		{
			foreach (TankResource item4 in tankType)
			{
				ResourceModifier modifier2 = new ResourceModifier(filledProportion: (item4.percentFilled ?? percentFilled ?? tankType.percentFilled ?? 100f) * 0.01f, tweakable: resourcesTweakable ?? tankType.resourcesTweakable, tankResource: item4, getVolumeDelegate: () => parent.GetTotalVolume(this), part: part);
				MaybeAddModifier(modifier2);
			}
		}
		transforms.Clear();
		foreach (IStringMatcher transformName in transformNames)
		{
			bool flag2 = false;
			foreach (Transform item5 in from t in part.GetModelRoot().TraverseHierarchy()
				where transformName.Match(t.name)
				select t)
			{
				flag2 = true;
				partModifiers.Add(new TransformToggler(item5, part));
				transforms.Add(item5);
			}
			if (!flag2)
			{
				OnInitializationError($"No transforms matching '{transformName}' found");
			}
		}
		foreach (TransformModifierInfo transformModifierInfo in transformModifierInfos)
		{
			foreach (IPartModifier item6 in transformModifierInfo.CreatePartModifiers(part, OnInitializationError))
			{
				MaybeAddModifier(item6);
			}
		}
		if ((part.partInfo?.partConfig).IsNotNull())
		{
			foreach (ModuleModifierInfo moduleModifierInfo in moduleModifierInfos)
			{
				try
				{
					foreach (IPartModifier item7 in moduleModifierInfo.CreatePartModifiers(part, parent, parent.CreateModuleDataChangedEventDetails()))
					{
						MaybeAddModifier(item7);
					}
				}
				catch (Exception ex)
				{
					OnInitializationError(ex.Message);
					Debug.LogException(ex);
				}
			}
		}
		if (!parent.subtypes.Any((PartSubtype subtype) => subtype.Name == mirrorSymmetrySubtype))
		{
			OnInitializationError("Cannot find subtype '" + mirrorSymmetrySubtype + "' for mirror symmetry subtype");
			mirrorSymmetrySubtype = Name;
		}
		if (errorString.IsNotNull())
		{
			SeriousWarningHandler.DisplaySeriousWarning(errorString);
		}
		void MaybeAddModifier(IPartModifier modifier)
		{
			if (modifier != null)
			{
				if (modifier is IPartAspectLock partAspectLock)
				{
					object obj = partAspectLock;
					if (aspectLocksOnOtherModules.Contains(obj))
					{
						OnInitializationError("More than one module can't manage " + modifier.Description);
						return;
					}
					aspectLocks.Add(obj);
				}
				partModifiers.Add(modifier);
			}
		}
		void OnInitializationError(string message)
		{
			LogError(message);
			if (displayWarnings)
			{
				if (errorString == null)
				{
					errorString = $"Initialization errors on {parent} subtype '{Name}'";
				}
				errorString = errorString + "\n  " + message;
			}
		}
	}

	public void DeactivateOnStart()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.DeactivateOnStartEditor();
			});
		}
		else
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.DeactivateOnStartFlight();
			});
		}
	}

	public void ActivateOnStart()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.ActivateOnStartEditor();
			});
		}
		else
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.ActivateOnStartFlight();
			});
		}
	}

	public void ActivateOnStartFinished()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.ActivateOnStartFinishedEditor();
			});
		}
		else
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.ActivateOnStartFinishedFlight();
			});
		}
	}

	public void DeactivateOnStartFinished()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.DeactivateOnStartFinishedEditor();
			});
		}
		else
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.DeactivateOnStartFinishedFlight();
			});
		}
	}

	public void DeactivateOnSwitch()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.DeactivateOnSwitchEditor();
			});
		}
		else
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.DeactivateOnSwitchFlight();
			});
		}
	}

	public void ActivateOnSwitch()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.ActivateOnSwitchEditor();
			});
		}
		else
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.ActivateOnSwitchFlight();
			});
		}
	}

	public void DeactivateForIcon()
	{
		partModifiers.ForEach(delegate(IPartModifier modifier)
		{
			modifier.OnIconCreateInactiveSubtype();
		});
	}

	public void ActivateForIcon()
	{
		partModifiers.ForEach(delegate(IPartModifier modifier)
		{
			modifier.OnIconCreateActiveSubtype();
		});
	}

	public void UpdateVolume()
	{
		if (HighLogic.LoadedSceneIsEditor)
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.UpdateVolumeEditor();
			});
		}
		else
		{
			partModifiers.ForEach(delegate(IPartModifier modifier)
			{
				modifier.UpdateVolumeFlight();
			});
		}
	}

	public void OnWillBeCopiedActiveSubtype()
	{
		partModifiers.ForEach(delegate(IPartModifier modifier)
		{
			modifier.OnWillBeCopiedActiveSubtype();
		});
	}

	public void OnWillBeCopiedInactiveSubtype()
	{
		partModifiers.ForEach(delegate(IPartModifier modifier)
		{
			modifier.OnWillBeCopiedInactiveSubtype();
		});
	}

	public void OnWasCopiedActiveSubtype()
	{
		partModifiers.ForEach(delegate(IPartModifier modifier)
		{
			modifier.OnWasCopiedActiveSubtype();
		});
	}

	public void OnWasCopiedInactiveSubtype()
	{
		partModifiers.ForEach(delegate(IPartModifier modifier)
		{
			modifier.OnWasCopiedInactiveSubtype();
		});
	}

	public bool TransformIsManaged(Transform transform)
	{
		return transforms.Contains(transform);
	}

	public bool NodeManaged(AttachNode node)
	{
		return nodes.Contains(node);
	}

	public bool ModuleShouldBeEnabled(PartModule module)
	{
		foreach (IPartModifier partModifier in partModifiers)
		{
			if (partModifier is ModuleDeactivator moduleDeactivator && moduleDeactivator.module == module)
			{
				return false;
			}
		}
		return true;
	}

	public void AssignStructuralTankType()
	{
		if (!tankType.IsStructuralTankType)
		{
			tankType = B9TankSettings.StructuralTankType;
		}
	}

	public bool IsUnlocked()
	{
		if (!HasUpgradeRequired)
		{
			return true;
		}
		if (HighLogic.CurrentGame.IsNull())
		{
			return true;
		}
		if (HighLogic.CurrentGame.Mode == Game.Modes.SANDBOX)
		{
			return true;
		}
		return PartUpgradeManager.Handler.IsUnlocked(upgradeRequired);
	}

	public override string ToString()
	{
		string text = "PartSubtype";
		if (!Name.IsNullOrEmpty())
		{
			text = text + " " + Name;
		}
		if (parent != null)
		{
			text += $" on module {parent}";
		}
		return text;
	}

	private void LoadAdditionalResources(ConfigNode[] resourceNodes, OperationContext context)
	{
		OperationContext context2 = new OperationContext(context, this);
		foreach (ConfigNode configNode in resourceNodes)
		{
			string value = configNode.GetValue("name");
			if (value.IsNullOrEmpty())
			{
				LogError("Cannot load a RESOURCE node without a name");
				continue;
			}
			TankResource tankResource = tankType[value];
			if (tankResource.IsNull())
			{
				tankResource = new TankResource();
				tankType.resources.Add(tankResource);
			}
			tankResource.Load(configNode, context2);
		}
	}

	private void LogWarning(string message)
	{
		Debug.LogWarning($"Warning on {this}: {message}");
	}

	private void LogError(string message)
	{
		Debug.LogWarning($"Warning on {this}: {message}");
	}
}
