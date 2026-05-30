using System;
using UnityEngine;
using ns9;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class FieldAttribute : Attribute
{
	[SerializeField]
	private string _guiName;

	public string guiName
	{
		get
		{
			return _guiName;
		}
		set
		{
			_guiName = Localizer.Format(value);
		}
	}

	public FieldAttribute()
	{
		guiName = "";
	}
}
