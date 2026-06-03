using System.Collections.Generic;
using System.Linq;

namespace B9PartSwitch;

public class BestSubtypeDeterminator
{
	public PartSubtype FindBestSubtype(IEnumerable<PartSubtype> subtypes, IEnumerable<string> resourceNamesOnPart)
	{
		if (!subtypes.Any((PartSubtype s) => s.HasTank))
		{
			return subtypes.MaxBy((PartSubtype subtype) => subtype.defaultSubtypePriority);
		}
		string[] array = subtypes.SelectMany((PartSubtype s) => s.ResourceNames).Distinct().Intersect(resourceNamesOnPart)
			.ToArray();
		if (array.Any())
		{
			PartSubtype partSubtype = null;
			foreach (PartSubtype subtype in subtypes)
			{
				if (subtype.HasTank && subtype.ResourceNames.SameElementsAs(array) && (!partSubtype.IsNotNull() || !(subtype.defaultSubtypePriority <= partSubtype.defaultSubtypePriority)))
				{
					partSubtype = subtype;
				}
			}
			if (partSubtype.IsNotNull())
			{
				return partSubtype;
			}
		}
		else
		{
			PartSubtype partSubtype2 = null;
			foreach (PartSubtype subtype2 in subtypes)
			{
				if (!subtype2.HasTank && (!partSubtype2.IsNotNull() || !(subtype2.defaultSubtypePriority <= partSubtype2.defaultSubtypePriority)))
				{
					partSubtype2 = subtype2;
				}
			}
			if (partSubtype2.IsNotNull())
			{
				return partSubtype2;
			}
		}
		return subtypes.MaxBy((PartSubtype subtype) => subtype.defaultSubtypePriority);
	}
}
