using System;
using KerbalEngineer.Extensions;
using UnityEngine;

namespace KerbalEngineer.UIControls;

public class PopOutElement : MonoBehaviour
{
	private Rect button;

	private Rect position;

	private GUIStyle windowStyle;

	public bool Resize { get; set; }

	public Callback DrawCallback { get; set; }

	public Callback ClosedCallback { get; set; }

	public Rect Position => position;

	private void Awake()
	{
		try
		{
			((Behaviour)this).enabled = false;
			Resize = true;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void Start()
	{
		try
		{
			InitialiseStyles();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void InitialiseStyles()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Expected O, but got Unknown
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Expected O, but got Unknown
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_004a: Expected O, but got Unknown
		//IL_004f: Expected O, but got Unknown
		try
		{
			GUIStyle val = new GUIStyle();
			val.normal.background = GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/DropDownBackground", false);
			val.border = new RectOffset(8, 8, 1, 8);
			val.margin = new RectOffset();
			val.padding = new RectOffset(5, 5, 5, 5);
			windowStyle = val;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void Update()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2)) && !position.MouseIsOver() && !button.MouseIsOver())
			{
				((Behaviour)this).enabled = false;
				ClosedCallback.Invoke();
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnGUI()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Expected O, but got Unknown
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Resize)
			{
				((Rect)(ref position)).height = 0f;
				Resize = false;
			}
			GUI.skin = null;
			position = GUILayout.Window(((Object)this).GetInstanceID(), position, new WindowFunction(Window), string.Empty, windowStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void Window(int windowId)
	{
		try
		{
			GUI.BringWindowToFront(windowId);
			DrawCallback.Invoke();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	public void SetPosition(Rect button, Rect size)
	{
		//IL_003f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			((Rect)(ref position)).x = ((Rect)(ref button)).x;
			((Rect)(ref position)).y = ((Rect)(ref button)).y + ((Rect)(ref button)).height;
			((Rect)(ref position)).width = ((Rect)(ref size)).width;
			this.button = button;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
