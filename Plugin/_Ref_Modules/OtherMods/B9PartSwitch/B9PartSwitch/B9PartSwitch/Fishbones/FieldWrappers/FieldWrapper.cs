using System;
using System.Reflection;

namespace B9PartSwitch.Fishbones.FieldWrappers;

public class FieldWrapper : IFieldWrapper
{
	private readonly FieldInfo field;

	public string Name => field.Name;

	public Type FieldType => field.FieldType;

	public MemberInfo MemberInfo => field;

	public FieldWrapper(FieldInfo field)
	{
		field.ThrowIfNullArgument("field");
		this.field = field;
	}

	public object GetValue(object subject)
	{
		subject.ThrowIfNullArgument("subject");
		return field.GetValue(subject);
	}

	public void SetValue(object subject, object value)
	{
		subject.ThrowIfNullArgument("subject");
		field.SetValue(subject, value);
	}
}
