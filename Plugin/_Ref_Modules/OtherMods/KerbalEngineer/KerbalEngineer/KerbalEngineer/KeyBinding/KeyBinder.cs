using System;
using System.IO;
using KerbalEngineer.Extensions;
using KerbalEngineer.Helpers;
using UnityEngine;

namespace KerbalEngineer.KeyBinding;

public class KeyBinder : MonoBehaviour
{
	private const string LOCK_ID = "KER_KeyBinder";

	private static readonly string filePath;

	private static KeyBindingsObject bindings;

	private static Rect position;

	private static bool hasCentred;

	public static bool IsOpen { get; private set; }

	public static KeyBindingsObject Bindings
	{
		get
		{
			if (bindings == null)
			{
				bindings = new KeyBindingsObject();
			}
			return bindings;
		}
		private set
		{
			if (value != null)
			{
				bindings = value;
			}
		}
	}

	public static KeyCode EditorShowHide
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return Bindings.EditorShowHide;
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Bindings.EditorShowHide = value;
			Save();
		}
	}

	public static KeyCode FlightShowHide
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return Bindings.FlightShowHide;
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Bindings.FlightShowHide = value;
			Save();
		}
	}

	public static KeyCode PartInfoShowHide
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			return Bindings.PartInfoShowHide;
		}
		set
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			Bindings.PartInfoShowHide = value;
			Save();
		}
	}

	public bool InputLock
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I8
			return (long)InputLockManager.GetControlLock("KER_KeyBinder") > 0L;
		}
		set
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (value)
			{
				InputLockManager.SetControlLock((ControlTypes)1152921504606846975L, "KER_KeyBinder");
			}
			else
			{
				InputLockManager.SetControlLock((ControlTypes)0, "KER_KeyBinder");
			}
		}
	}

	static KeyBinder()
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		filePath = Path.Combine(EngineerGlobals.SettingsPath, "KeyBinds.xml");
		position = new Rect((float)Screen.width, (float)Screen.height, 500f, 0f);
		Load();
	}

	public static void Load()
	{
		Bindings = XmlHelper.LoadObject<KeyBindingsObject>(filePath);
	}

	public static void Save()
	{
		XmlHelper.SaveObject(filePath, Bindings);
	}

	public static void Show()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		if (!IsOpen)
		{
			new GameObject("KeyBinder").AddComponent<KeyBinder>();
		}
	}

	protected virtual void Awake()
	{
		if (IsOpen)
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
			return;
		}
		IsOpen = true;
		((Rect)(ref position)).height = 0f;
	}

	protected virtual void OnDestroy()
	{
		IsOpen = false;
		InputLock = false;
	}

	protected virtual void OnGUI()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		position = GUILayout.Window(((Object)this).GetInstanceID(), position, new WindowFunction(RenderWindow), "Kerbal Engineer Redux - Key Bindings", HighLogic.Skin.window, (GUILayoutOption[])(object)new GUILayoutOption[0]).ClampToScreen();
		CentreWindow();
	}

	protected virtual void Update()
	{
		UpdateInputLock();
	}

	private static void RenderKeyBind(string name, KeyCode currentBinding, Action<KeyCode> acceptClicked)
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label(name, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button(((object)(KeyCode)(ref currentBinding)).ToString(), HighLogic.Skin.button, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(100f) }))
		{
			KeyBindPopup.Show(name, currentBinding, acceptClicked);
		}
		GUILayout.EndHorizontal();
	}

	private void CentreWindow()
	{
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		if (!hasCentred && ((Rect)(ref position)).width > 0f && ((Rect)(ref position)).height > 0f)
		{
			hasCentred = true;
			((Rect)(ref position)).center = new Vector2((float)Screen.width * 0.5f, (float)Screen.height * 0.5f);
		}
	}

	private void RenderWindow(int id)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.BeginVertical(HighLogic.Skin.textArea, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		RenderKeyBind("Editor Show/Hide", EditorShowHide, delegate(KeyCode binding)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			EditorShowHide = binding;
		});
		RenderKeyBind("Flight Show/Hide", FlightShowHide, delegate(KeyCode binding)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			FlightShowHide = binding;
		});
		RenderKeyBind("Part Info Show/Hide", PartInfoShowHide, delegate(KeyCode binding)
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			PartInfoShowHide = binding;
		});
		GUILayout.EndVertical();
		if (GUILayout.Button("Close", HighLogic.Skin.button, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			Object.Destroy((Object)(object)((Component)this).gameObject);
		}
		GUI.DragWindow();
	}

	private void UpdateInputLock()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		bool flag = position.MouseIsOver();
		bool inputLock = InputLock;
		if (flag && !inputLock)
		{
			InputLock = true;
		}
		else if (!flag && inputLock)
		{
			InputLock = false;
		}
	}
}
