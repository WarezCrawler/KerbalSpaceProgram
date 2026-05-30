using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static class ReflectionUtil
{
	public class AttributedType<Tatt>
	{
		public Type Type { get; private set; }

		public Tatt Attribute { get; private set; }

		public AttributedType(Type t, Tatt attrib)
		{
			Type = t;
			Attribute = attrib;
		}
	}

	public static List<AttributedType<Tatt>> GetAttributedTypesInAssemblies<Tbase, Tatt>(List<Assembly> assemblies) where Tbase : class where Tatt : Attribute
	{
		List<AttributedType<Tatt>> list = new List<AttributedType<Tatt>>();
		int i = 0;
		for (int count = assemblies.Count; i < count; i++)
		{
			Type[] types = assemblies[i].GetTypes();
			int j = 0;
			for (int num = types.Length; j < num; j++)
			{
				if (types[j].IsSubclassOf(typeof(Tbase)) || types[j] == typeof(Tbase))
				{
					Tatt[] array = (Tatt[])types[j].GetCustomAttributes(typeof(Tatt), inherit: true);
					if (array.Length != 0)
					{
						list.Add(new AttributedType<Tatt>(types[j], array[0]));
					}
				}
			}
		}
		Debug.Log("[ReflectionUtil]: Found " + list.Count + " types with " + typeof(Tatt).Name + " attribute in " + assemblies.Count + " assemblies.");
		return list;
	}
}
