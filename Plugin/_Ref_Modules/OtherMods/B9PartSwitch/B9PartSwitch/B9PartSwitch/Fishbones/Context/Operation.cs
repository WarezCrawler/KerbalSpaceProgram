using System;

namespace B9PartSwitch.Fishbones.Context;

public struct Operation
{
	private enum OperationType
	{
		LOADING,
		SAVING
	}

	public static readonly Operation LoadPrefab = new Operation("load prefab", OperationType.LOADING, useNonPersistentFields: true);

	public static readonly Operation LoadInstance = new Operation("load instance", OperationType.LOADING, useNonPersistentFields: true);

	public static readonly Operation Save = new Operation("save", OperationType.SAVING, useNonPersistentFields: false);

	public static readonly Operation Deserialize = new Operation("deserialize", OperationType.LOADING, useNonPersistentFields: true);

	public static readonly Operation Serialize = new Operation("serialize", OperationType.SAVING, useNonPersistentFields: true);

	public static readonly Operation LoadUnknown = new Operation("load (unknown)", OperationType.LOADING, useNonPersistentFields: true);

	public static readonly Operation SaveUnknown = new Operation("save (unknown)", OperationType.SAVING, useNonPersistentFields: true);

	private readonly OperationType OpType;

	public readonly string Name;

	public readonly bool UseNonPersistentFields;

	public bool Loading => OpType == OperationType.LOADING;

	public bool Saving => OpType == OperationType.SAVING;

	private Operation(string name, OperationType opType, bool useNonPersistentFields)
	{
		name.ThrowIfNullArgument("name");
		if (name == string.Empty)
		{
			throw new ArgumentException("name can't be empty");
		}
		if (!Enum.IsDefined(typeof(OperationType), opType))
		{
			throw new ArgumentException($"Invalid operation type encountered: '{opType}' (must be in the enumeration)");
		}
		Name = name;
		OpType = opType;
		UseNonPersistentFields = useNonPersistentFields;
	}

	public override int GetHashCode()
	{
		return Name.GetHashCode() ^ OpType.GetHashCode() ^ UseNonPersistentFields.GetHashCode();
	}

	public bool Equals(Operation other)
	{
		if (other.Name == Name && other.OpType == OpType)
		{
			return other.UseNonPersistentFields == UseNonPersistentFields;
		}
		return false;
	}

	public override bool Equals(object other)
	{
		if (other is Operation)
		{
			return Equals((Operation)other);
		}
		return false;
	}

	public static bool operator ==(Operation op1, Operation op2)
	{
		return op1.Equals(op2);
	}

	public static bool operator !=(Operation op1, Operation op2)
	{
		return !op1.Equals(op2);
	}
}
