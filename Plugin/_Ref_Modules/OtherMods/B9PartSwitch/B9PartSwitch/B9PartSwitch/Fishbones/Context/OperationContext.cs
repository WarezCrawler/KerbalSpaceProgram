namespace B9PartSwitch.Fishbones.Context;

public class OperationContext
{
	public readonly Operation Operation;

	public readonly OperationContext ParentOperation;

	public readonly object Subject;

	public object Parent => ParentOperation?.Subject;

	public object Root => ParentOperation?.Root ?? Subject;

	public OperationContext(Operation operation, object subject)
	{
		subject.ThrowIfNullArgument("subject");
		Operation = operation;
		Subject = subject;
	}

	public OperationContext(OperationContext parentOperation, object subject)
	{
		parentOperation.ThrowIfNullArgument("parentOperation");
		subject.ThrowIfNullArgument("subject");
		ParentOperation = parentOperation;
		Operation = ParentOperation.Operation;
		Subject = subject;
	}
}
