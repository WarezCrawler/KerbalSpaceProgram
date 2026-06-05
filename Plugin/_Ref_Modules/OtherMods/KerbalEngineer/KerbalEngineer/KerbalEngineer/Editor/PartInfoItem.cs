using System.Collections.Generic;

namespace KerbalEngineer.Editor;

public class PartInfoItem
{
	private static readonly Pool<PartInfoItem> pool = new Pool<PartInfoItem>(Create, Reset);

	public string Name { get; set; }

	public string Value { get; set; }

	private static PartInfoItem Create()
	{
		return new PartInfoItem();
	}

	public void Release()
	{
		pool.Release(this);
	}

	public static void Release(List<PartInfoItem> objList)
	{
		for (int i = 0; i < objList.Count; i++)
		{
			objList[i].Release();
		}
	}

	private static void Reset(PartInfoItem obj)
	{
		obj.Name = string.Empty;
		obj.Value = string.Empty;
	}

	public static PartInfoItem Create(string name)
	{
		return New(name);
	}

	public static PartInfoItem Create(string name, string value)
	{
		return New(name, value);
	}

	public static PartInfoItem New(string name)
	{
		PartInfoItem partInfoItem = pool.Borrow();
		partInfoItem.Name = name;
		partInfoItem.Value = string.Empty;
		return partInfoItem;
	}

	public static PartInfoItem New(string name, string value)
	{
		PartInfoItem partInfoItem = pool.Borrow();
		partInfoItem.Name = name;
		partInfoItem.Value = value;
		return partInfoItem;
	}
}
