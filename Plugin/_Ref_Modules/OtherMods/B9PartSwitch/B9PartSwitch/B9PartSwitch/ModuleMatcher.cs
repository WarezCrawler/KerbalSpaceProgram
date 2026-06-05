using System;
using B9PartSwitch.Fishbones.Parsers;
using B9PartSwitch.Utils;

namespace B9PartSwitch;

public class ModuleMatcher
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

	private readonly ConfigNode identifierNode;

	private readonly IStringMatcher moduleName;

	public ModuleMatcher(ConfigNode identifierNode)
	{
		this.identifierNode = identifierNode ?? throw new ArgumentNullException("identifierNode");
		string value = identifierNode.GetValue("name");
		if (value == null)
		{
			throw new ArgumentException("node has no name", "identifierNode");
		}
		if (value == "")
		{
			throw new ArgumentException("node has empty name", "identifierNode");
		}
		moduleName = StringMatcher.Parse(value);
	}

	public PartModule FindModule(Part part)
	{
		PartModule partModule = null;
		foreach (PartModule module in part.Modules)
		{
			if (IsMatch(module))
			{
				if (partModule.IsNotNull())
				{
					throw new Exception("Found more than one matching module");
				}
				partModule = module;
			}
		}
		if (partModule.IsNull())
		{
			throw new Exception("Could not find matching module");
		}
		return partModule;
	}

	public ConfigNode FindPrefabNode(PartModule module)
	{
		AvailablePart partInfo = module.part.partInfo;
		if (partInfo == null)
		{
			throw new InvalidOperationException("partInfo is null on part " + module.part.name);
		}
		ConfigNode obj = partInfo.partConfig ?? throw new InvalidOperationException("partInfo.partConfig is null on part " + partInfo.name);
		ConfigNode configNode = null;
		foreach (ConfigNode node in obj.nodes)
		{
			if (!(node.name != "MODULE") && NodeMatchesModule(module, node))
			{
				if (configNode.IsNotNull())
				{
					throw new Exception("Found more than one matching module node");
				}
				configNode = node;
			}
		}
		if (configNode.IsNull())
		{
			throw new Exception("Could not find matching module node");
		}
		return configNode;
	}

	private bool IsMatch(PartModule module)
	{
		if (!moduleName.Match(module.GetType().Name))
		{
			return false;
		}
		foreach (ConfigNode.Value value in identifierNode.values)
		{
			if (value.name == "name")
			{
				continue;
			}
			BaseField baseField = module.Fields[value.name];
			if (baseField != null)
			{
				IValueParser parser;
				try
				{
					parser = DefaultValueParseMap.Instance.GetParser(baseField.FieldInfo.FieldType);
				}
				catch (ParseTypeNotRegisteredException)
				{
					throw CannotParseFieldException.CannotFindParser(baseField.name, baseField.FieldInfo.FieldType);
				}
				object objA;
				try
				{
					objA = parser.Parse(value.value);
				}
				catch (Exception innerException)
				{
					throw CannotParseFieldException.ExceptionWhileParsing(baseField.name, baseField.FieldInfo.FieldType, innerException);
				}
				if (!object.Equals(objA, baseField.GetValue(module)))
				{
					return false;
				}
			}
			else
			{
				if (!(module is CustomPartModule customPartModule) || !(value.name == "moduleID"))
				{
					return false;
				}
				if (customPartModule.moduleID != value.value)
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool NodeMatchesModule(PartModule module, ConfigNode node)
	{
		string value = node.GetValue("name");
		if (value.IsNullOrEmpty())
		{
			throw new ArgumentException("Cannot match a module node without a name!");
		}
		if (!moduleName.Match(value))
		{
			return false;
		}
		foreach (ConfigNode.Value value4 in identifierNode.values)
		{
			if (value4.name == "name")
			{
				continue;
			}
			string value3 = node.GetValue(value4.name);
			if (value3 == null)
			{
				return false;
			}
			BaseField baseField = module.Fields[value4.name];
			if (baseField != null)
			{
				object objA;
				object objB;
				try
				{
					IValueParser parser = DefaultValueParseMap.Instance.GetParser(baseField.FieldInfo.FieldType);
					objA = parser.Parse(value4.value);
					objB = parser.Parse(value3);
				}
				catch (ParseTypeNotRegisteredException)
				{
					throw CannotParseFieldException.CannotFindParser(baseField.name, baseField.FieldInfo.FieldType);
				}
				catch (Exception innerException)
				{
					throw CannotParseFieldException.ExceptionWhileParsing(baseField.name, baseField.FieldInfo.FieldType, innerException);
				}
				if (!object.Equals(objA, objB))
				{
					return false;
				}
			}
			else
			{
				if (!(module is CustomPartModule) || !(value4.name == "moduleID"))
				{
					return false;
				}
				if (node.GetValue("moduleID") != value4.value)
				{
					return false;
				}
			}
		}
		return true;
	}
}
