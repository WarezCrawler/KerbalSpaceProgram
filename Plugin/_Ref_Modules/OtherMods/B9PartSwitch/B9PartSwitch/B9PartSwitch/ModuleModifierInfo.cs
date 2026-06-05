using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using B9PartSwitch.Fishbones;
using B9PartSwitch.Fishbones.Context;
using B9PartSwitch.PartSwitch.PartModifiers;

namespace B9PartSwitch;

public class ModuleModifierInfo : IContextualNode
{
	public sealed class CannotParseFieldException : Exception
	{
		private CannotParseFieldException(string message)
			: base(message)
		{
		}

		private CannotParseFieldException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		public static CannotParseFieldException CannotFindParser(string fieldName, Type fieldType)
		{
			fieldName.ThrowIfNullArgument("fieldName");
			fieldType.ThrowIfNullArgument("fieldType");
			return new CannotParseFieldException("Could not find a suitable way to parse type " + fieldType.Name + " for field " + fieldName);
		}

		public static CannotParseFieldException ExceptionWhileParsing(string fieldName, Type fieldType, Exception innerException)
		{
			fieldName.ThrowIfNullArgument("fieldName");
			fieldType.ThrowIfNullArgument("fieldType");
			innerException.ThrowIfNullArgument("innerException");
			return new CannotParseFieldException("Exception while parsing type " + fieldType.Name + " for field " + fieldName, innerException);
		}
	}

	public static readonly ReadOnlyCollection<Type> INVALID_MODULES_FOR_DATA_LOADING = new ReadOnlyCollection<Type>(new Type[4]
	{
		typeof(ModulePartVariants),
		typeof(ModuleB9PartSwitch),
		typeof(ModuleB9PartInfo),
		typeof(ModuleB9DisableTransform)
	});

	public static readonly ReadOnlyCollection<string> INVALID_MODULES_NAMES_FOR_DATA_LOADING = new ReadOnlyCollection<string>(new string[7] { "FSfuelSwitch", "FSmeshSwitch", "FStextureSwitch", "FStextureSwitch2", "InterstellarFuelSwitch", "InterstellarMeshSwitch", "InterstellarTextureSwitch" });

	public static readonly ReadOnlyCollection<Type> INVALID_MODULES_FOR_DISABLING = new ReadOnlyCollection<Type>(new Type[4]
	{
		typeof(ModulePartVariants),
		typeof(ModuleB9PartSwitch),
		typeof(ModuleB9PartInfo),
		typeof(ModuleB9DisableTransform)
	});

	public static readonly ReadOnlyCollection<string> INVALID_MODULES_NAMES_FOR_DISABLING = new ReadOnlyCollection<string>(new string[7] { "FSfuelSwitch", "FSmeshSwitch", "FStextureSwitch", "FStextureSwitch2", "InterstellarFuelSwitch", "InterstellarMeshSwitch", "InterstellarTextureSwitch" });

	[NodeData(name = "IDENTIFIER")]
	public ConfigNode identifierNode;

	[NodeData]
	public bool moduleActive = true;

	[NodeData(name = "DATA")]
	public ConfigNode dataNode;

	public void Load(ConfigNode node, OperationContext context)
	{
		this.LoadFields(node, context);
	}

	public void Save(ConfigNode node, OperationContext context)
	{
		this.SaveFields(node, context);
	}

	public IEnumerable<IPartModifier> CreatePartModifiers(Part part, PartModule parentModule, BaseEventDetails moduleDataChangedEventDetails)
	{
		part.ThrowIfNullArgument("part");
		parentModule.ThrowIfNullArgument("parentModule");
		moduleDataChangedEventDetails.ThrowIfNullArgument("moduleDataChangedEventDetails");
		if (identifierNode.IsNull())
		{
			throw new Exception("module modifier must have an IDENTIFIER node");
		}
		ModuleMatcher moduleMatcher = new ModuleMatcher(identifierNode);
		PartModule module = moduleMatcher.FindModule(part);
		if (module == parentModule)
		{
			throw new Exception("Cannot use parent module!");
		}
		if (dataNode.IsNotNull())
		{
			AvailablePart partInfo = module.part.partInfo;
			if (partInfo == null)
			{
				throw new InvalidOperationException("partInfo is null on part " + part.name);
			}
			if (partInfo.partConfig == null)
			{
				throw new InvalidOperationException("partInfo.partConfig is null on part " + partInfo.name);
			}
			ConfigNode originalNode = moduleMatcher.FindPrefabNode(module);
			if (INVALID_MODULES_FOR_DATA_LOADING.Any((Type type) => module.GetType().Implements(type)))
			{
				throw new InvalidOperationException($"Cannot modify data on {module.GetType()}");
			}
			if (INVALID_MODULES_NAMES_FOR_DATA_LOADING.Any((string moduleTypeName) => module.GetType().Name == moduleTypeName))
			{
				throw new InvalidOperationException($"Cannot modify data on {module.GetType()}");
			}
			if (module is ModuleEnginesFX moduleEnginesFX)
			{
				yield return new ModuleDataHandlerBasic(module, originalNode, dataNode, moduleDataChangedEventDetails);
				string value = dataNode.GetValue("flameoutEffectName");
				if (value != null)
				{
					yield return new EffectDeactivator(part, moduleEnginesFX.flameoutEffectName, value);
				}
				string value2 = dataNode.GetValue("runningEffectName");
				if (value2 != null)
				{
					yield return new EffectDeactivator(part, moduleEnginesFX.runningEffectName, value2);
				}
				string value3 = dataNode.GetValue("powerEffectName");
				if (value3 != null)
				{
					yield return new EffectDeactivator(part, moduleEnginesFX.powerEffectName, value3);
				}
				string value4 = dataNode.GetValue("engageEffectName");
				if (value4 != null)
				{
					yield return new EffectDeactivator(part, moduleEnginesFX.engageEffectName, value4);
				}
				string value5 = dataNode.GetValue("disengageEffectName");
				if (value5 != null)
				{
					yield return new EffectDeactivator(part, moduleEnginesFX.disengageEffectName, value5);
				}
				string value6 = dataNode.GetValue("directThrottleEffectName");
				if (value6 != null)
				{
					yield return new EffectDeactivator(part, moduleEnginesFX.directThrottleEffectName, value6);
				}
				string value7 = dataNode.GetValue("spoolEffectName");
				if (value7 != null)
				{
					yield return new EffectDeactivator(part, moduleEnginesFX.spoolEffectName, value7);
				}
			}
			else if (module is ModuleRCSFX moduleRCSFX)
			{
				yield return new ModuleDataHandlerBasic(module, originalNode, dataNode, moduleDataChangedEventDetails);
				string value8 = dataNode.GetValue("runningEffectName");
				if (value8 != null)
				{
					yield return new EffectDeactivator(part, moduleRCSFX.runningEffectName, value8);
				}
			}
			else if (module is ModuleDeployableSolarPanel && dataNode.HasValue("chargeRate"))
			{
				yield return new ModuleOutputResourceResetter(module);
				yield return new ModuleDataHandlerBasic(module, originalNode, dataNode, moduleDataChangedEventDetails);
			}
			else
			{
				yield return new ModuleDataHandlerBasic(module, originalNode, dataNode, moduleDataChangedEventDetails);
			}
		}
		if (!moduleActive)
		{
			if (INVALID_MODULES_FOR_DISABLING.Any((Type type) => module.GetType().Implements(type)))
			{
				throw new InvalidOperationException($"Cannot disable {module.GetType()}");
			}
			if (INVALID_MODULES_NAMES_FOR_DISABLING.Any((string moduleTypeName) => module.GetType().Name == moduleTypeName))
			{
				throw new InvalidOperationException($"Cannot disable {module.GetType()}");
			}
			yield return new ModuleDeactivator(module, parentModule);
		}
	}
}
