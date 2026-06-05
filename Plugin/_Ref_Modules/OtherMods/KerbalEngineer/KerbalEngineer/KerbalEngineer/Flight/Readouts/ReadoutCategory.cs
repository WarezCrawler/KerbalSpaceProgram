using System;
using System.Collections.Generic;
using System.Linq;

namespace KerbalEngineer.Flight.Readouts;

public class ReadoutCategory
{
	public static List<ReadoutCategory> Categories { get; private set; }

	public static ReadoutCategory Selected { get; set; }

	public string Description { get; set; }

	public string Name { get; set; }

	public ReadoutCategory(string name)
	{
		Name = name;
	}

	public ReadoutCategory(string name, string description)
	{
		Name = name;
		Description = description;
	}

	static ReadoutCategory()
	{
		Categories = new List<ReadoutCategory>();
	}

	public static ReadoutCategory GetCategory(string name)
	{
		if (Categories.Any((ReadoutCategory c) => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
		{
			return Categories.Find((ReadoutCategory c) => c.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
		}
		ReadoutCategory readoutCategory = new ReadoutCategory(name);
		Categories.Add(readoutCategory);
		return readoutCategory;
	}

	public static void SetCategory(string name)
	{
		GetCategory(name).Name = name;
	}

	public static void SetCategory(string name, string description)
	{
		ReadoutCategory category = GetCategory(name);
		category.Name = name;
		category.Description = description;
	}

	public override string ToString()
	{
		return Name;
	}
}
