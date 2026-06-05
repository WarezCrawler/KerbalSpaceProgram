using System;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace KerbalEngineer.Helpers;

public static class XmlHelper
{
	public static T LoadObject<T>(string path)
	{
		T result = default(T);
		if (File.Exists(path))
		{
			try
			{
				using StreamReader textReader = new StreamReader(path, Encoding.UTF8);
				result = (T)new XmlSerializer(typeof(T)).Deserialize(textReader);
				return result;
			}
			catch (Exception ex)
			{
				MyLogger.Exception(ex);
			}
		}
		return result;
	}

	public static bool LoadObject<T>(string path, out T obj)
	{
		obj = LoadObject<T>(path);
		return obj != null;
	}

	public static void SaveObject<T>(string path, T obj)
	{
		if (obj == null || string.IsNullOrEmpty(path))
		{
			return;
		}
		try
		{
			using StreamWriter textWriter = new StreamWriter(path, append: false, Encoding.UTF8);
			new XmlSerializer(typeof(T)).Serialize(textWriter, obj);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
