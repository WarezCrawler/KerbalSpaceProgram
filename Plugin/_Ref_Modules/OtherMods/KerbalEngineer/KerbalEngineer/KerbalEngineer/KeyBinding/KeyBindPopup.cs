using System;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.KeyBinding;

public class KeyBindPopup : MonoBehaviour
{
	private const string LOCK_ID = "KER_KeyBindPopup";

	private static Rect position = new Rect((float)Screen.width, (float)Screen.height, 250f, 0f);

	private static bool hasCentred;

	private static KeyBindPopup instance;

	private readonly Array availableBindings = Enum.GetValues(typeof(KeyCode));

	public Action<KeyCode> AcceptClicked { get; private set; }

	public string Name { get; private set; }

	public KeyCode Binding { get; private set; }

	public static bool IsOpen => (Object)(object)instance != (Object)null;

	public bool InputLock
	{
		get
		{
			//IL_0005: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Invalid comparison between Unknown and I8
			return (long)InputLockManager.GetControlLock("KER_KeyBindPopup") > 0L;
		}
		set
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0011: Unknown result type (might be due to invalid IL or missing references)
			if (value)
			{
				InputLockManager.SetControlLock((ControlTypes)1152921504606846975L, "KER_KeyBindPopup");
			}
			else
			{
				InputLockManager.SetControlLock((ControlTypes)0, "KER_KeyBindPopup");
			}
		}
	}

	public static void Show(string name, KeyCode currentBinding, Action<KeyCode> acceptClicked)
	{
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)instance == (Object)null)
		{
			instance = new GameObject("SelectKeyBind").AddComponent<KeyBindPopup>();
		}
		instance.Name = name;
		instance.Binding = currentBinding;
		instance.AcceptClicked = acceptClicked;
	}

	public void OnAccept()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		if (AcceptClicked != null)
		{
			AcceptClicked(Binding);
		}
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	public void OnCancel()
	{
		Object.Destroy((Object)(object)((Component)this).gameObject);
	}

	protected virtual void Awake()
	{
		if ((Object)(object)instance == (Object)null)
		{
			instance = this;
		}
		else if ((Object)(object)instance != (Object)(object)this)
		{
			OnCancel();
		}
	}

	protected virtual void OnDestroy()
	{
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
		position = GUILayout.Window(((Object)this).GetInstanceID(), position, new WindowFunction(RenderWindow), "Select Key Bind", HighLogic.Skin.window, (GUILayoutOption[])(object)new GUILayoutOption[0]).ClampToScreen();
		CentreWindow();
	}

	protected virtual void Update()
	{
		CentreWindow();
		UpdateBinding();
		UpdateInputLock();
	}

	private static void CentreWindow()
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
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		GUILayout.Label("Press the desired key to change it.", (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.BeginVertical(HighLogic.Skin.textArea, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Key Bind: " + Name, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		GUILayout.Label("Selected: " + Binding, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("Clear", HighLogic.Skin.button, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			Binding = (KeyCode)0;
		}
		GUILayout.EndVertical();
		GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
		if (GUILayout.Button("Cancel", HighLogic.Skin.button, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			OnCancel();
		}
		if (GUILayout.Button("Accept", HighLogic.Skin.button, (GUILayoutOption[])(object)new GUILayoutOption[0]))
		{
			OnAccept();
		}
		GUILayout.EndHorizontal();
		GUI.DragWindow();
	}

	private void UpdateBinding()
	{
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Invalid comparison between Unknown and I4
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		for (int i = 0; i < availableBindings.Length; i++)
		{
			KeyCode val = (KeyCode)availableBindings.GetValue(i);
			if ((int)val != 323 && Input.GetKeyDown(val) && Input.GetKeyDown(val))
			{
				Binding = val;
			}
		}
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
