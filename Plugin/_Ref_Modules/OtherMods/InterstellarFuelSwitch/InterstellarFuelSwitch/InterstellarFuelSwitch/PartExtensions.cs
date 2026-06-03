using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace InterstellarFuelSwitch;

public static class PartExtensions
{
	private static FieldInfo windowListField;

	public static UIPartActionWindow FindActionWindow(this Part part)
	{
		if (part == null)
		{
			return null;
		}
		UIPartActionController instance = UIPartActionController.Instance;
		if (instance == null)
		{
			return null;
		}
		if (windowListField == null)
		{
			Type typeFromHandle = typeof(UIPartActionController);
			FieldInfo[] fields = typeFromHandle.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
			int num = 0;
			FieldInfo fieldInfo;
			while (true)
			{
				if (num < fields.Length)
				{
					fieldInfo = fields[num];
					if (fieldInfo.FieldType == typeof(List<UIPartActionWindow>))
					{
						break;
					}
					num++;
					continue;
				}
				Debug.LogWarning("*PartUtils* Unable to find UIPartActionWindow list");
				return null;
			}
			windowListField = fieldInfo;
		}
		return ((List<UIPartActionWindow>)windowListField.GetValue(instance))?.FirstOrDefault((UIPartActionWindow window) => window != null && window.part == part);
	}
}
