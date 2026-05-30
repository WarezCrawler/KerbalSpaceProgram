using UnityEngine;

namespace ns2;

public class UIMainCamera : UICameraBase
{
	public static UIMainCamera Instance;

	public static Camera Camera
	{
		get
		{
			if (!Instance)
			{
				return Camera.main;
			}
			return Instance.cam;
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void OnDestroy()
	{
		if (Instance != null && Instance == this)
		{
			Instance = null;
		}
	}
}
