using System.Collections.Generic;
using UnityEngine;

public class DatabaseGameObject : MonoBehaviour
{
	private static List<string> databases = new List<string>();

	public string databaseName = string.Empty;

	private void Awake()
	{
		if (databaseName != string.Empty)
		{
			if (databases.Contains(databaseName))
			{
				Object.DestroyImmediate(base.gameObject);
				return;
			}
			databases.Add(databaseName);
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}
}
