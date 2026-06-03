using System;
using System.Reflection;

namespace B9PartSwitch.Fishbones.FieldWrappers;

public class PropertyWrapper : IFieldWrapper
{
	private readonly PropertyInfo property;

	private readonly MethodInfo setMethod;

	private readonly MethodInfo getMethod;

	public string Name => property.Name;

	public Type FieldType => property.PropertyType;

	public MemberInfo MemberInfo => property;

	public PropertyWrapper(PropertyInfo property)
	{
		property.ThrowIfNullArgument("property");
		if (!property.CanRead || !property.CanWrite)
		{
			throw new ArgumentException($"Property must have read and write accessors, however property {property.Name} of class {property.DeclaringType} does not", "property");
		}
		this.property = property;
		getMethod = property.GetGetMethod(nonPublic: true);
		setMethod = property.GetSetMethod(nonPublic: true);
	}

	public object GetValue(object subject)
	{
		subject.ThrowIfNullArgument("subject");
		return getMethod.Invoke(subject, null);
	}

	public void SetValue(object subject, object value)
	{
		subject.ThrowIfNullArgument("subject");
		setMethod.Invoke(subject, new object[1] { value });
	}
}
