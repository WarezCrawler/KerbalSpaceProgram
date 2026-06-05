using System;
using System.Collections.Generic;
using KerbalEngineer.Flight.Sections;
using UnityEngine;

namespace KerbalEngineer.Flight;

public class ActionMenuGui : MonoBehaviour
{
	private int numberOfSections;

	private Rect position = new Rect((float)Screen.width, 38f, 300f, 0f);

	private GUIStyle buttonStyle;

	private GUIStyle windowStyle;

	public bool StayOpen { get; set; }

	public bool Hovering { get; set; }

	public bool Hidden { get; set; }

	private void Awake()
	{
		try
		{
			((Behaviour)this).enabled = false;
			MyLogger.Log("ActionMenuGui was created.");
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
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Expected O, but got Unknown
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0027: Expected O, but got Unknown
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Expected O, but got Unknown
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0057: Expected O, but got Unknown
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_0082: Expected O, but got Unknown
		//IL_0082: Unknown result type (might be due to invalid IL or missing references)
		//IL_0083: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Expected O, but got Unknown
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b3: Expected O, but got Unknown
		try
		{
			GUIStyle val = new GUIStyle
			{
				border = new RectOffset(10, 0, 20, 10),
				margin = new RectOffset(0, 0, 3, 0),
				padding = new RectOffset(5, 5, 26, 5)
			};
			val.normal.background = GameDatabase.Instance.GetTexture("KerbalEngineer/Textures/ToolbarBackground", false);
			windowStyle = val;
			GUIStyle val2 = new GUIStyle(HighLogic.Skin.button);
			val2.normal.textColor = Color.white;
			val2.margin = new RectOffset();
			val2.padding = new RectOffset();
			val2.alignment = (TextAnchor)4;
			val2.fontSize = 11;
			val2.fontStyle = (FontStyle)1;
			val2.fixedHeight = 20f;
			buttonStyle = val2;
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex, "ActionMenu->InitialiseStyles");
		}
	}

	private void OnGUI()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Expected O, but got Unknown
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			if (Hidden || !FlightEngineerCore.IsDisplayable)
			{
				return;
			}
			if (!((Rect)(ref position)).Contains(Event.current.mousePosition) && !StayOpen && !Hovering)
			{
				((Behaviour)this).enabled = false;
				return;
			}
			if (numberOfSections < SectionLibrary.NumberOfSections)
			{
				numberOfSections = SectionLibrary.NumberOfSections;
			}
			else if (numberOfSections > SectionLibrary.NumberOfSections)
			{
				numberOfSections = SectionLibrary.NumberOfSections;
				((Rect)(ref position)).height = 0f;
			}
			GUI.skin = null;
			((Rect)(ref position)).x = Mathf.Clamp((float)Screen.width * 0.5f + ((Component)this).transform.parent.position.x - 19f, (float)Screen.width * 0.5f, (float)Screen.width - ((Rect)(ref position)).width);
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
			GUILayout.BeginVertical((GUILayoutOption[])(object)new GUILayoutOption[0]);
			DrawControlBarButton();
			GUILayout.Space(5f);
			DrawSections(SectionLibrary.StockSections);
			DrawSections(SectionLibrary.CustomSections);
			GUILayout.Space(5f);
			if (HighLogic.LoadedSceneIsFlight)
			{
				DrawNewButton();
			}
			GUILayout.EndVertical();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void DrawControlBarButton()
	{
		try
		{
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			DisplayStack.Instance.Hidden = !GUILayout.Toggle(!DisplayStack.Instance.Hidden, "SHOW ENGINEER", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
			if (GUILayout.Toggle(DisplayStack.Instance.ShowControlBar, "CONTROL BAR", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]) != DisplayStack.Instance.ShowControlBar)
			{
				DisplayStack.Instance.ShowControlBar = !DisplayStack.Instance.ShowControlBar;
				DisplayStack.Instance.RequestResize();
			}
			GUILayout.EndHorizontal();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void DrawSections(IEnumerable<SectionModule> sections)
	{
		try
		{
			foreach (SectionModule section in sections)
			{
				GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
				section.IsVisible = GUILayout.Toggle(section.IsVisible, section.Name.ToUpper(), buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]);
				section.IsEditorVisible = GUILayout.Toggle(section.IsEditorVisible, "EDIT", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[1] { GUILayout.Width(50f) });
				GUILayout.EndHorizontal();
			}
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void DrawNewButton()
	{
		try
		{
			GUILayout.BeginHorizontal((GUILayoutOption[])(object)new GUILayoutOption[0]);
			if (GUILayout.Button("NEW CUSTOM SECTION", buttonStyle, (GUILayoutOption[])(object)new GUILayoutOption[0]))
			{
				SectionLibrary.CustomSections.Add(new SectionModule
				{
					Name = "Custom " + (SectionLibrary.CustomSections.Count + 1),
					Abbreviation = "CUST " + (SectionLibrary.CustomSections.Count + 1),
					IsVisible = true,
					IsEditorVisible = true
				});
			}
			GUILayout.EndHorizontal();
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}

	private void OnDestroy()
	{
		try
		{
			MyLogger.Log("ActionMenuGui was destroyed.");
		}
		catch (Exception ex)
		{
			MyLogger.Exception(ex);
		}
	}
}
