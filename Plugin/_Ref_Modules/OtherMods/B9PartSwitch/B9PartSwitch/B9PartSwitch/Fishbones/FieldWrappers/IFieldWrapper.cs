using System;
using System.Reflection;

namespace B9PartSwitch.Fishbones.FieldWrappers;

public interface IFieldWrapper
{
	string Name { get; }

	Type FieldType { get; }

	MemberInfo MemberInfo { get; }

	object GetValue(object subject);

	void SetValue(object subject, object value);
}
