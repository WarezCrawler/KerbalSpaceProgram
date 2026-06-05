using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace KerbalEngineer.Settings;

public class SettingHandler
{
	private static string settingsDirectory;

	public static string SettingsDirectory => settingsDirectory;

	public List<SettingItem> Items { get; set; }

	public SettingHandler()
	{
		if (settingsDirectory == null)
		{
			settingsDirectory = Path.Combine(EngineerGlobals.AssemblyPath, "Settings");
		}
		Items = new List<SettingItem>();
	}

	static SettingHandler()
	{
		if (settingsDirectory == null)
		{
			settingsDirectory = Path.Combine(EngineerGlobals.AssemblyPath, "Settings");
		}
	}

	public T Get<T>(string name, T defaultObject)
	{
		foreach (SettingItem item in Items)
		{
			if (item.Name == name)
			{
				try
				{
					return (T)Convert.ChangeType(item.Value, typeof(T));
				}
				catch
				{
					item.Value = defaultObject;
					return (T)Convert.ChangeType(item.Value, typeof(T));
				}
			}
		}
		return defaultObject;
	}

	public bool Get<T>(string name, ref T outputObject)
	{
		foreach (SettingItem item in Items)
		{
			if (item.Name == name)
			{
				outputObject = (T)Convert.ChangeType(item.Value, typeof(T));
				return true;
			}
		}
		return false;
	}

	public void Set(string name, object value)
	{
		int num = -1;
		foreach (SettingItem item in Items)
		{
			if (item.Name == name)
			{
				num = Items.IndexOf(item);
			}
		}
		if (num >= 0)
		{
			Items.RemoveAt(num);
		}
		Items.Insert((num >= 0) ? num : Items.Count, new SettingItem(name, value));
	}

	public T GetSet<T>(string name, T defaultObject)
	{
		foreach (SettingItem item in Items)
		{
			if (item.Name == name)
			{
				try
				{
					return (T)Convert.ChangeType(item.Value, typeof(T));
				}
				catch
				{
					item.Value = defaultObject;
				}
			}
		}
		if (defaultObject != null)
		{
			Items.Add(new SettingItem(name, defaultObject));
		}
		return defaultObject;
	}

	public void Save(string fileName)
	{
		fileName = Path.Combine(settingsDirectory, fileName);
		Serialise(fileName);
	}

	private void CreateDirectory(string fileName)
	{
		string directoryName = new FileInfo(fileName).DirectoryName;
		if (!Directory.Exists(directoryName))
		{
			Directory.CreateDirectory(directoryName);
		}
	}

	private void Serialise(string fileName)
	{
		CreateDirectory(fileName);
		XmlWriterSettings settings = new XmlWriterSettings
		{
			Encoding = Encoding.UTF8,
			Indent = true
		};
		using XmlWriter xmlWriter = XmlWriter.Create(fileName, settings);
		new XmlSerializer(typeof(List<SettingItem>), Items.Select((SettingItem s) => s.Value.GetType()).ToArray()).Serialize(xmlWriter, Items);
		xmlWriter.Close();
	}

	public static void DeleteSettings()
	{
		try
		{
			string[] files = Directory.GetFiles(settingsDirectory);
			for (int i = 0; i < files.Length; i++)
			{
				File.Delete(files[i]);
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	public static bool Exists(string fileName)
	{
		return File.Exists(Path.Combine(settingsDirectory, fileName));
	}

	public static SettingHandler Load(string fileName, Type[] extraTypes = null)
	{
		fileName = Path.Combine(settingsDirectory, fileName);
		SettingHandler settingHandler = Deserialise(fileName, extraTypes);
		for (int num = settingHandler.Items.Count - 1; num >= 0; num--)
		{
			if (settingHandler.Items[num].Value is XmlNode[])
			{
				MyLogger.Log("fixed old or invalid setting: " + settingHandler.Items[num].Name);
				settingHandler.Items[num].Value = settingHandler.Items[num].Value.ToString();
			}
		}
		return settingHandler;
	}

	private static SettingHandler Deserialise(string fileName, Type[] extraTypes)
	{
		if (!File.Exists(fileName))
		{
			return new SettingHandler();
		}
		SettingHandler settingHandler = new SettingHandler();
		using FileStream fileStream = new FileStream(fileName, FileMode.Open);
		settingHandler.Items = new XmlSerializer(typeof(List<SettingItem>), extraTypes).Deserialize(fileStream) as List<SettingItem>;
		fileStream.Close();
		return settingHandler;
	}
}
