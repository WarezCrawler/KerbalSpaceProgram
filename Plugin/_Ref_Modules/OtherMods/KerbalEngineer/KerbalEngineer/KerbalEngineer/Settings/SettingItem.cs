namespace KerbalEngineer.Settings;

public class SettingItem
{
	public string Name { get; set; }

	public object Value { get; set; }

	public SettingItem()
	{
	}

	public SettingItem(string name, object value)
	{
		Name = name;
		Value = value;
	}
}
