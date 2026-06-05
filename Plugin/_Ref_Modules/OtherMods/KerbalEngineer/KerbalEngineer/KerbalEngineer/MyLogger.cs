using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace KerbalEngineer;

[KSPAddon(/*Could not decode attribute arguments.*/)]
public class MyLogger : MonoBehaviour
{
	private static readonly List<string[]> messages;

	private static readonly string fileName;

	private static readonly AssemblyName assemblyName;

	static MyLogger()
	{
		messages = new List<string[]>();
		assemblyName = Assembly.GetExecutingAssembly().GetName();
		fileName = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, "log");
		File.Delete(fileName);
		lock (messages)
		{
			messages.Add(new string[1] { "Executing: " + assemblyName.Name + " - " + assemblyName.Version });
			messages.Add(new string[1] { "Assembly: " + Assembly.GetExecutingAssembly().Location });
		}
		Blank();
	}

	private void Awake()
	{
		Object.DontDestroyOnLoad((Object)(object)this);
	}

	public static void Blank()
	{
		lock (messages)
		{
			messages.Add(new string[0]);
		}
	}

	public static void Log(object obj)
	{
		lock (messages)
		{
			try
			{
				messages.Add(new string[2]
				{
					"Log " + DateTime.Now.TimeOfDay,
					GetObjString(obj)
				});
			}
			catch (Exception ex)
			{
				Exception(ex);
			}
		}
	}

	public static void Log(string name, object obj)
	{
		lock (messages)
		{
			try
			{
				messages.Add(new string[2]
				{
					"Log " + DateTime.Now.TimeOfDay,
					name + "\n" + GetObjString(obj)
				});
			}
			catch (Exception ex)
			{
				Exception(ex);
			}
		}
	}

	private static string GetObjString(object obj, int tabs = 0)
	{
		string text = string.Empty;
		for (int i = 0; i < tabs; i++)
		{
			text += " ";
		}
		string text2;
		if (obj != null)
		{
			text2 = text + obj;
			if (obj is IEnumerable enumerable)
			{
				foreach (object item in enumerable)
				{
					text2 = text2 + "\n" + GetObjString(item, tabs + 1);
				}
			}
		}
		else
		{
			text2 = "Null";
		}
		return text2;
	}

	public static void Log(string message)
	{
		lock (messages)
		{
			messages.Add(new string[2]
			{
				"Log " + DateTime.Now.TimeOfDay,
				message
			});
		}
	}

	public static void Warning(string message)
	{
		lock (messages)
		{
			messages.Add(new string[2]
			{
				"Warning " + DateTime.Now.TimeOfDay,
				message
			});
		}
	}

	public static void Error(string message)
	{
		lock (messages)
		{
			messages.Add(new string[2]
			{
				"Error " + DateTime.Now.TimeOfDay,
				message
			});
		}
	}

	public static void Exception(Exception ex)
	{
		Exception(ex, "");
	}

	public static void Exception(Exception ex, string location)
	{
		lock (messages)
		{
			messages.Add(new string[2]
			{
				DateTime.Now.TimeOfDay.ToString(),
				"Exception " + location + " // " + ex.ToString()
			});
			messages.Add(new string[2]
			{
				string.Empty,
				ex.StackTrace
			});
			for (ex = ex.InnerException; ex != null; ex = ex.InnerException)
			{
				messages.Add(new string[2]
				{
					DateTime.Now.TimeOfDay.ToString(),
					"Inner Exception " + location + " // " + ex.ToString()
				});
				messages.Add(new string[2]
				{
					string.Empty,
					ex.StackTrace
				});
			}
			Blank();
		}
	}

	public static void Flush()
	{
		lock (messages)
		{
			if (messages.Count <= 0)
			{
				return;
			}
			using (StreamWriter streamWriter = File.AppendText(fileName))
			{
				foreach (string[] message in messages)
				{
					streamWriter.WriteLine((message.Length == 0) ? string.Empty : ((message.Length > 1) ? ("[" + message[0] + "]: " + message[1]) : message[0]));
					if (message.Length != 0)
					{
						MonoBehaviour.print((object)((message.Length > 1) ? (assemblyName.Name + " -> " + message[1]) : (assemblyName.Name + " -> " + message[0])));
					}
				}
			}
			messages.Clear();
		}
	}

	private void LateUpdate()
	{
		Flush();
	}

	private void OnDestroy()
	{
		Flush();
	}

	~MyLogger()
	{
		try
		{
			Flush();
		}
		finally
		{
			((object)this).Finalize();
		}
	}
}
